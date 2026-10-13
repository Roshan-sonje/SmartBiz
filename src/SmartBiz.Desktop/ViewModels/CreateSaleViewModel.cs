using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SmartBiz.Desktop.Exceptions;
using SmartBiz.Desktop.Models.Customers;
using SmartBiz.Desktop.Models.Products;
using SmartBiz.Desktop.Models.Sales;
using SmartBiz.Desktop.Services;

namespace SmartBiz.Desktop.ViewModels;

public partial class CreateSaleViewModel : BaseViewModel
{
    private readonly ISaleService _saleService;
    private readonly ICustomerService _customerService;
    private readonly IProductService _productService;
    private readonly ILogger<CreateSaleViewModel> _logger;

    public ObservableCollection<CustomerOption> Customers { get; } = new();
    public ObservableCollection<SaleLineItem> LineItems { get; } = new();

    [ObservableProperty] private CustomerOption? _selectedCustomer;
    [ObservableProperty] private DateTime _invoiceDate = DateTime.Today;
    [ObservableProperty] private string? _notes;

    // Payment
    [ObservableProperty] private string _paymentMethod = "Cash";
    [ObservableProperty] private decimal _paidAmount;

    public List<string> PaymentMethods { get; } = new() { "Cash", "Card", "Upi", "BankTransfer", "Cheque", "Other" };

    // Totals — computed from LineItems
    [ObservableProperty] private decimal _subtotal;
    [ObservableProperty] private decimal _totalDiscount;
    [ObservableProperty] private decimal _totalTax;
    [ObservableProperty] private decimal _grandTotal;
    [ObservableProperty] private decimal _roundOff;
    [ObservableProperty] private decimal _dueAmount;

    // UI State
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private bool _isSaving;

    public CreateSaleViewModel(
        ISaleService saleService,
        ICustomerService customerService,
        IProductService productService,
        ILogger<CreateSaleViewModel> logger)
    {
        _saleService = saleService;
        _customerService = customerService;
        _productService = productService;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        await LoadCustomersAsync();
    }

    private async Task LoadCustomersAsync()
    {
        try
        {
            var result = await _customerService.GetCustomersAsync(pageSize: 500);

            Customers.Clear();
            Customers.Add(new CustomerOption { Id = null, Name = "Walk-in Customer" });
            foreach (var c in result.Items)
            {
                Customers.Add(new CustomerOption
                {
                    Id = c.Id,
                    Name = c.Name,
                    Phone = c.Phone
                });
            }

            SelectedCustomer = Customers.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load customers");
        }
    }

    public void AddProductLine(ProductListItem product)
    {
        var existing = LineItems.FirstOrDefault(l => l.ProductId == product.Id);
        if (existing is not null)
        {
            existing.Quantity += 1;
            RecalculateLine(existing);
            RecalculateTotals();
            return;
        }

        var line = new SaleLineItem
        {
            ProductId = product.Id,
            ProductName = product.Name,
            ProductSku = product.Sku,
            AvailableStock = product.StockQuantity,
            Quantity = 1,
            UnitPrice = product.SellingPrice,
            DiscountAmount = 0,
            TaxRate = 18m   // TODO: pull from product.TaxRate when available
        };
        RecalculateLine(line);
        LineItems.Add(line);
        RecalculateTotals();
    }

    [RelayCommand]
    private void RemoveLine(SaleLineItem? line)
    {
        if (line is null) return;
        LineItems.Remove(line);
        RecalculateTotals();
    }

    public void RecalculateLine(SaleLineItem line)
    {
        var lineSubtotal = line.Quantity * line.UnitPrice;
        var afterDiscount = lineSubtotal - line.DiscountAmount;
        line.TaxAmount = afterDiscount * (line.TaxRate / 100m);
        line.LineTotal = afterDiscount + line.TaxAmount;
    }

    public void RecalculateTotals()
    {
        Subtotal = LineItems.Sum(l => l.Quantity * l.UnitPrice);
        TotalDiscount = LineItems.Sum(l => l.DiscountAmount);
        TotalTax = LineItems.Sum(l => l.TaxAmount);

        var raw = Subtotal - TotalDiscount + TotalTax;
        RoundOff = Math.Round(raw) - raw;
        GrandTotal = Math.Round(raw);
        DueAmount = GrandTotal - PaidAmount;
    }

    partial void OnPaidAmountChanged(decimal value) => RecalculateTotals();

    public async Task<Sale?> SaveAsync()
    {
        ErrorMessage = null;

        if (LineItems.Count == 0)
        {
            ErrorMessage = "Add at least one product before saving.";
            return null;
        }

        // Validate stock
        foreach (var line in LineItems)
        {
            if (line.Quantity > line.AvailableStock)
            {
                ErrorMessage = $"Insufficient stock for '{line.ProductName}'. Available: {line.AvailableStock}";
                return null;
            }
        }

        IsSaving = true;
        try
        {
            var request = new CreateSaleRequest
            {
                CustomerId = SelectedCustomer?.Id,
                InvoiceDate = InvoiceDate,
                Notes = string.IsNullOrWhiteSpace(Notes) ? null : Notes.Trim(),
                PaidAmount = PaidAmount,
                PaymentMethod = PaymentMethod,
                Items = LineItems.Select(l => new CreateSaleItemRequest
                {
                    ProductId = l.ProductId,
                    Quantity = l.Quantity,
                    UnitPrice = l.UnitPrice,
                    DiscountAmount = l.DiscountAmount,
                    TaxRate = l.TaxRate
                }).ToList()
            };

            var sale = await _saleService.CreateSaleAsync(request);
            _logger.LogInformation("Sale created: {InvoiceNumber}", sale.InvoiceNumber);
            return sale;
        }
        catch (ApiException ex)
        {
            ErrorMessage = ex.Message;
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save sale");
            ErrorMessage = "Failed to save sale. Please try again.";
            return null;
        }
        finally
        {
            IsSaving = false;
        }
    }
}

public class CustomerOption
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
}

public partial class SaleLineItem : ObservableObject
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ProductSku { get; set; }
    public decimal AvailableStock { get; set; }

    [ObservableProperty] private decimal _quantity = 1;
    [ObservableProperty] private decimal _unitPrice;
    [ObservableProperty] private decimal _discountAmount;
    [ObservableProperty] private decimal _taxRate;
    [ObservableProperty] private decimal _taxAmount;
    [ObservableProperty] private decimal _lineTotal;
}