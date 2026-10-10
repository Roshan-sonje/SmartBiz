using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartBiz.Application.Common;
using SmartBiz.Application.DTOs.Inventory;
using SmartBiz.Application.Exceptions;
using SmartBiz.Application.Interfaces;
using SmartBiz.Domain.Entities;
using SmartBiz.Domain.Enums;

namespace SmartBiz.Application.Services;

public class InventoryService : IInventoryService
{
    private readonly IApplicationDbContext _db;
    private readonly ILogger<InventoryService> _logger;

    public InventoryService(IApplicationDbContext db, ILogger<InventoryService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<PagedResult<InventoryTransactionDto>> GetTransactionsAsync(
        Guid businessId, int page, int pageSize, Guid? productId, CancellationToken ct = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var query = _db.InventoryTransactions
            .IgnoreQueryFilters()
            .Where(t => t.BusinessId == businessId);

        if (productId.HasValue)
            query = query.Where(t => t.ProductId == productId.Value);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new InventoryTransactionDto
            {
                Id = t.Id,
                ProductId = t.ProductId,
                ProductName = t.Product != null ? t.Product.Name : "—",
                ProductSku = t.Product != null ? t.Product.Sku : null,
                Type = t.Type.ToString(),
                Quantity = t.Quantity,
                BalanceAfter = t.BalanceAfter,
                ReferenceType = t.ReferenceType,
                ReferenceId = t.ReferenceId,
                Notes = t.Notes,
                CreatedByUserName = t.CreatedByUser != null ? t.CreatedByUser.FullName : null,
                CreatedAt = t.CreatedAt
            })
            .AsNoTracking()
            .ToListAsync(ct);

        return new PagedResult<InventoryTransactionDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }

    public async Task<PagedResult<InventoryTransactionDto>> GetProductHistoryAsync(
        Guid businessId, Guid productId, int page, int pageSize, CancellationToken ct = default)
    {
        // Verify product belongs to this business
        var productExists = await _db.Products
            .IgnoreQueryFilters()
            .AnyAsync(p => p.Id == productId && p.BusinessId == businessId, ct);

        if (!productExists)
            throw new AuthException("Product not found.", 404);

        return await GetTransactionsAsync(businessId, page, pageSize, productId, ct);
    }

    public async Task<InventoryTransactionDto> AdjustStockAsync(
        Guid businessId, Guid userId, AdjustStockRequest request, CancellationToken ct = default)
    {
        if (request.Adjustment == 0)
            throw new AuthException("Adjustment cannot be zero.", 400);

        var product = await _db.Products
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == request.ProductId && p.BusinessId == businessId, ct);

        if (product is null)
            throw new AuthException("Product not found.", 404);

        if (!product.TrackInventory)
            throw new AuthException("This product does not track inventory.", 400);

        var previous = product.StockQuantity;
        var newBalance = previous + request.Adjustment;

        if (newBalance < 0)
            throw new AuthException(
                $"Adjustment would result in negative stock ({newBalance}). Current stock: {previous}.",
                400);

        var type = request.Adjustment > 0
            ? InventoryTransactionType.Adjustment
            : InventoryTransactionType.Adjustment;

        var transaction = new InventoryTransaction
        {
            BusinessId = businessId,
            ProductId = product.Id,
            Type = type,
            Quantity = request.Adjustment,
            BalanceAfter = newBalance,
            ReferenceType = "ManualAdjustment",
            Notes = request.Reason.Trim(),
            CreatedByUserId = userId
        };

        _db.InventoryTransactions.Add(transaction);
        product.StockQuantity = newBalance;

        await _db.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Stock adjusted for product {ProductId}: {Previous} -> {New} (delta {Delta}) by user {UserId}. Reason: {Reason}",
            product.Id, previous, newBalance, request.Adjustment, userId, request.Reason);

        // Re-query to get the joined fields
        var saved = await _db.InventoryTransactions
            .IgnoreQueryFilters()
            .Include(t => t.Product)
            .Include(t => t.CreatedByUser)
            .AsNoTracking()
            .FirstAsync(t => t.Id == transaction.Id, ct);

        return new InventoryTransactionDto
        {
            Id = saved.Id,
            ProductId = saved.ProductId,
            ProductName = saved.Product?.Name ?? "—",
            ProductSku = saved.Product?.Sku,
            Type = saved.Type.ToString(),
            Quantity = saved.Quantity,
            BalanceAfter = saved.BalanceAfter,
            ReferenceType = saved.ReferenceType,
            ReferenceId = saved.ReferenceId,
            Notes = saved.Notes,
            CreatedByUserName = saved.CreatedByUser?.FullName,
            CreatedAt = saved.CreatedAt
        };
    }

    public async Task<InventoryValuationDto> GetValuationAsync(Guid businessId, CancellationToken ct = default)
    {
        var products = await _db.Products
            .IgnoreQueryFilters()
            .Where(p => p.BusinessId == businessId && p.IsActive && p.TrackInventory && p.StockQuantity > 0)
            .Select(p => new { p.StockQuantity, p.PurchasePrice, p.SellingPrice })
            .AsNoTracking()
            .ToListAsync(ct);

        return new InventoryValuationDto
        {
            ProductCount = products.Count,
            TotalQuantity = products.Sum(p => p.StockQuantity),
            TotalPurchaseValue = products.Sum(p => p.StockQuantity * p.PurchasePrice),
            TotalSellingValue = products.Sum(p => p.StockQuantity * p.SellingPrice),
            TotalPotentialProfit = products.Sum(p => p.StockQuantity * (p.SellingPrice - p.PurchasePrice))
        };
    }

    public async Task<List<LowStockProductDto>> GetLowStockProductsAsync(Guid businessId, CancellationToken ct = default)
    {
        var products = await _db.Products
            .IgnoreQueryFilters()
            .Include(p => p.Unit)
            .Where(p => p.BusinessId == businessId
                        && p.IsActive
                        && p.TrackInventory
                        && p.StockQuantity <= p.MinStockLevel)
            .OrderBy(p => p.StockQuantity - p.MinStockLevel)
            .AsNoTracking()
            .ToListAsync(ct);

        return products.Select(p => new LowStockProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Sku = p.Sku,
            StockQuantity = p.StockQuantity,
            MinStockLevel = p.MinStockLevel,
            UnitShortName = p.Unit?.ShortName,
            Deficit = p.MinStockLevel - p.StockQuantity
        }).ToList();
    }
}