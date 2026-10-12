using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartBiz.Application.Common;
using SmartBiz.Application.DTOs.Sales;
using SmartBiz.Application.Exceptions;
using SmartBiz.Application.Interfaces;
using SmartBiz.Domain.Entities;
using SmartBiz.Domain.Enums;

namespace SmartBiz.Application.Services;

public class SaleService : ISaleService
{
    private readonly IApplicationDbContext _db;
    private readonly ILogger<SaleService> _logger;

    public SaleService(IApplicationDbContext db, ILogger<SaleService> logger)
    {
        _db = db;
        _logger = logger;
    }

    // -------------------------------------------------------------------
    // LIST
    // -------------------------------------------------------------------
    public async Task<PagedResult<SaleListItemDto>> GetSalesAsync(
        Guid businessId,
        int page,
        int pageSize,
        string? search,
        string? paymentStatus,
        Guid? customerId,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken ct = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var query = _db.Sales
            .IgnoreQueryFilters()
            .Where(s => s.BusinessId == businessId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(s =>
                s.InvoiceNumber.ToLower().Contains(term) ||
                (s.Customer != null && s.Customer.Name.ToLower().Contains(term)));
        }

        if (!string.IsNullOrWhiteSpace(paymentStatus) &&
            Enum.TryParse<PaymentStatus>(paymentStatus, true, out var ps))
        {
            query = query.Where(s => s.PaymentStatus == ps);
        }

        if (customerId.HasValue)
            query = query.Where(s => s.CustomerId == customerId.Value);

        if (fromDate.HasValue)
            query = query.Where(s => s.InvoiceDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(s => s.InvoiceDate <= toDate.Value);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(s => s.InvoiceDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new SaleListItemDto
            {
                Id = s.Id,
                InvoiceNumber = s.InvoiceNumber,
                InvoiceDate = s.InvoiceDate,
                CustomerId = s.CustomerId,
                CustomerName = s.Customer != null ? s.Customer.Name : "Walk-in Customer",
                GrandTotal = s.GrandTotal,
                PaidAmount = s.PaidAmount,
                DueAmount = s.DueAmount,
                Status = s.Status.ToString(),
                PaymentStatus = s.PaymentStatus.ToString(),
                ItemCount = s.Items.Count
            })
            .AsNoTracking()
            .ToListAsync(ct);

        return new PagedResult<SaleListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }

    // -------------------------------------------------------------------
    // GET BY ID
    // -------------------------------------------------------------------
    public async Task<SaleDto> GetSaleAsync(Guid businessId, Guid saleId, CancellationToken ct = default)
    {
        var sale = await _db.Sales
            .IgnoreQueryFilters()
            .Include(s => s.Customer)
            .Include(s => s.CreatedByUser)
            .Include(s => s.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == saleId && s.BusinessId == businessId, ct);

        if (sale is null)
            throw new AuthException("Sale not found.", 404);

        return MapToDto(sale);
    }

    // -------------------------------------------------------------------
    // CREATE — THE TRANSACTIONAL LOGIC
    // -------------------------------------------------------------------
    public async Task<SaleDto> CreateSaleAsync(
        Guid businessId, Guid userId, CreateSaleRequest request, CancellationToken ct = default)
    {
        // ---- 1. Validate request ----
        if (request.Items is null || request.Items.Count == 0)
            throw new AuthException("A sale must have at least one item.", 400);

        // ---- 2. Load customer (if specified) ----
        Customer? customer = null;
        if (request.CustomerId.HasValue)
        {
            customer = await _db.Customers
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.Id == request.CustomerId.Value
                                          && c.BusinessId == businessId, ct);

            if (customer is null)
                throw new AuthException("Customer not found.", 404);
        }

        // ---- 3. Load business settings (for invoice number) ----
        var settings = await _db.BusinessSettings
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(s => s.BusinessId == businessId, ct);

        if (settings is null)
        {
            settings = new BusinessSettings
            {
                BusinessId = businessId,
                InvoicePrefix = "INV",
                InvoiceNextNumber = 1,
                InvoiceNumberFormat = "{PREFIX}-{YEAR}-{NUMBER:000000}",
                Currency = "INR",
                CurrencySymbol = "₹"
            };
            _db.BusinessSettings.Add(settings);
        }

        // ---- 4. Load all products ----
        var productIds = request.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await _db.Products
            .IgnoreQueryFilters()
            .Where(p => p.BusinessId == businessId && productIds.Contains(p.Id))
            .ToListAsync(ct);

        if (products.Count != productIds.Count)
            throw new AuthException("One or more products were not found.", 400);

        // ---- 5. Validate stock + build line items ----
        var saleItems = new List<SaleItem>();
        decimal subtotal = 0m;
        decimal totalTax = 0m;
        decimal totalDiscount = 0m;

        foreach (var reqItem in request.Items)
        {
            var product = products.First(p => p.Id == reqItem.ProductId);

            if (reqItem.Quantity <= 0)
                throw new AuthException($"Quantity must be positive for {product.Name}.", 400);

            if (product.TrackInventory && product.StockQuantity < reqItem.Quantity)
                throw new AuthException(
                    $"Insufficient stock for '{product.Name}'. Available: {product.StockQuantity}, Requested: {reqItem.Quantity}.",
                    400);

            var lineSubtotal = reqItem.Quantity * reqItem.UnitPrice;
            var lineAfterDiscount = lineSubtotal - reqItem.DiscountAmount;
            var lineTax = lineAfterDiscount * (reqItem.TaxRate / 100m);
            var lineTotal = lineAfterDiscount + lineTax;

            subtotal += lineSubtotal;
            totalDiscount += reqItem.DiscountAmount;
            totalTax += lineTax;

            saleItems.Add(new SaleItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                ProductSku = product.Sku,
                Quantity = reqItem.Quantity,
                UnitPrice = reqItem.UnitPrice,
                DiscountAmount = reqItem.DiscountAmount,
                TaxRate = reqItem.TaxRate,
                TaxAmount = lineTax,
                LineTotal = lineTotal
            });
        }

        var grandTotal = subtotal - totalDiscount + totalTax;
        var roundOff = Math.Round(grandTotal) - grandTotal;
        grandTotal = Math.Round(grandTotal);

        var paidAmount = Math.Min(request.PaidAmount, grandTotal);
        var dueAmount = grandTotal - paidAmount;

        // ---- 6. Determine payment status ----
        var paymentStatus = paidAmount <= 0
            ? PaymentStatus.Unpaid
            : (paidAmount >= grandTotal ? PaymentStatus.Paid : PaymentStatus.Partial);

        // ---- 7. Generate invoice number (SAFE) ----
        var invoiceNumber = BuildInvoiceNumber(settings);
        settings.InvoiceNextNumber++;

        // ---- 8. Create Sale ----
        var sale = new Sale
        {
            BusinessId = businessId,
            CustomerId = customer?.Id,
            InvoiceNumber = invoiceNumber,
            InvoiceDate = request.InvoiceDate ?? DateTime.UtcNow,
            Subtotal = subtotal,
            DiscountAmount = totalDiscount,
            TaxAmount = totalTax,
            RoundOff = roundOff,
            GrandTotal = grandTotal,
            PaidAmount = paidAmount,
            DueAmount = dueAmount,
            Status = SaleStatus.Completed,
            PaymentStatus = paymentStatus,
            Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim(),
            CreatedByUserId = userId,
            Items = saleItems
        };

        _db.Sales.Add(sale);

        // ---- 9. Create Invoice ----
        var invoice = new Invoice
        {
            BusinessId = businessId,
            SaleId = sale.Id,
            InvoiceNumber = invoiceNumber,
            IssuedAt = DateTime.UtcNow,
            TemplateCode = "default"
        };
        _db.Invoices.Add(invoice);

        // ---- 10. Decrement inventory + create InventoryTransactions ----
        foreach (var item in saleItems)
        {
            var product = products.First(p => p.Id == item.ProductId);

            if (!product.TrackInventory)
                continue;

            var previousStock = product.StockQuantity;
            var newStock = previousStock - item.Quantity;
            product.StockQuantity = newStock;

            _db.InventoryTransactions.Add(new InventoryTransaction
            {
                BusinessId = businessId,
                ProductId = product.Id,
                Type = InventoryTransactionType.Sale,
                Quantity = -item.Quantity,
                BalanceAfter = newStock,
                ReferenceType = "Sale",
                ReferenceId = sale.Id,
                Notes = $"Sold via invoice {invoiceNumber}",
                CreatedByUserId = userId
            });
        }

        // ---- 11. Record payment if paid > 0 ----
        if (paidAmount > 0)
        {
            var paymentMethod = ParsePaymentMethod(request.PaymentMethod);

            _db.Payments.Add(new Payment
            {
                BusinessId = businessId,
                Type = PaymentType.CustomerReceipt,
                CustomerId = customer?.Id,
                SaleId = sale.Id,
                Amount = paidAmount,
                Method = paymentMethod,
                Reference = string.IsNullOrWhiteSpace(request.PaymentReference) ? null : request.PaymentReference.Trim(),
                PaymentDate = DateTime.UtcNow,
                Notes = $"Payment for invoice {invoiceNumber}",
                CreatedByUserId = userId
            });
        }

        // ---- 12. Commit everything in ONE transaction ----
        await _db.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Sale created: {InvoiceNumber} for {GrandTotal} ({Items} items) by user {UserId}",
            invoiceNumber, grandTotal, saleItems.Count, userId);

        // ---- 13. Return DTO with joined data ----
        return await GetSaleAsync(businessId, sale.Id, ct);
    }

    // -------------------------------------------------------------------
    // CANCEL
    // -------------------------------------------------------------------
    public async Task<SaleDto> CancelSaleAsync(
        Guid businessId, Guid saleId, string reason, CancellationToken ct = default)
    {
        var sale = await _db.Sales
            .IgnoreQueryFilters()
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == saleId && s.BusinessId == businessId, ct);

        if (sale is null)
            throw new AuthException("Sale not found.", 404);

        if (sale.Status == SaleStatus.Cancelled)
            throw new AuthException("This sale is already cancelled.", 400);

        // ---- 1. Mark cancelled ----
        sale.Status = SaleStatus.Cancelled;
        sale.Notes = string.IsNullOrWhiteSpace(sale.Notes)
            ? $"Cancelled: {reason}"
            : $"{sale.Notes} | Cancelled: {reason}";

        // ---- 2. Restore inventory ----
        foreach (var item in sale.Items)
        {
            var product = await _db.Products
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == item.ProductId && p.BusinessId == businessId, ct);

            if (product is null || !product.TrackInventory)
                continue;

            var previousStock = product.StockQuantity;
            var newStock = previousStock + item.Quantity;
            product.StockQuantity = newStock;

            _db.InventoryTransactions.Add(new InventoryTransaction
            {
                BusinessId = businessId,
                ProductId = product.Id,
                Type = InventoryTransactionType.Return,
                Quantity = item.Quantity,
                BalanceAfter = newStock,
                ReferenceType = "SaleCancel",
                ReferenceId = sale.Id,
                Notes = $"Restored from cancelled sale {sale.InvoiceNumber}: {reason}",
                CreatedByUserId = sale.CreatedByUserId
            });
        }

        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("Sale cancelled: {InvoiceNumber} — {Reason}", sale.InvoiceNumber, reason);

        return await GetSaleAsync(businessId, sale.Id, ct);
    }

    // -------------------------------------------------------------------
    // NEXT INVOICE NUMBER (preview)
    // -------------------------------------------------------------------
    public async Task<string> GetNextInvoiceNumberAsync(Guid businessId, CancellationToken ct = default)
    {
        var settings = await _db.BusinessSettings
            .IgnoreQueryFilters()
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.BusinessId == businessId, ct);

        if (settings is null)
            return "INV-" + DateTime.UtcNow.Year + "-000001";

        return BuildInvoiceNumber(settings);
    }

    // -------------------------------------------------------------------
    // HELPERS
    // -------------------------------------------------------------------

    private static string BuildInvoiceNumber(BusinessSettings settings)
    {
        var format = string.IsNullOrWhiteSpace(settings.InvoiceNumberFormat)
            ? "{PREFIX}-{YEAR}-{NUMBER:000000}"
            : settings.InvoiceNumberFormat;

        var number = settings.InvoiceNextNumber.ToString("D6");

        return format
            .Replace("{PREFIX}", settings.InvoicePrefix)
            .Replace("{YEAR}", DateTime.UtcNow.Year.ToString())
            .Replace("{NUMBER:000000}", number)
            .Replace("{NUMBER}", settings.InvoiceNextNumber.ToString());
    }

    private static PaymentMethod ParsePaymentMethod(string? method)
    {
        if (string.IsNullOrWhiteSpace(method))
            return PaymentMethod.Cash;

        return Enum.TryParse<PaymentMethod>(method, true, out var pm) ? pm : PaymentMethod.Other;
    }

    private static SaleDto MapToDto(Sale s) => new()
    {
        Id = s.Id,
        InvoiceNumber = s.InvoiceNumber,
        InvoiceDate = s.InvoiceDate,
        CustomerId = s.CustomerId,
        CustomerName = s.Customer?.Name,
        CustomerPhone = s.Customer?.Phone,
        Subtotal = s.Subtotal,
        DiscountAmount = s.DiscountAmount,
        TaxAmount = s.TaxAmount,
        RoundOff = s.RoundOff,
        GrandTotal = s.GrandTotal,
        PaidAmount = s.PaidAmount,
        DueAmount = s.DueAmount,
        Status = s.Status.ToString(),
        PaymentStatus = s.PaymentStatus.ToString(),
        Notes = s.Notes,
        CreatedByUserId = s.CreatedByUserId,
        CreatedByUserName = s.CreatedByUser?.FullName,
        CreatedAt = s.CreatedAt,
        UpdatedAt = s.UpdatedAt,
        Items = s.Items.Select(i => new SaleItemDto
        {
            Id = i.Id,
            ProductId = i.ProductId,
            ProductName = i.ProductName,
            ProductSku = i.ProductSku,
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice,
            DiscountAmount = i.DiscountAmount,
            TaxRate = i.TaxRate,
            TaxAmount = i.TaxAmount,
            LineTotal = i.LineTotal
        }).ToList()
    };
}