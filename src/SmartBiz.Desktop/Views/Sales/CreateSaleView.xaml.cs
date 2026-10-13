using SmartBiz.Desktop.Models.Products;
using SmartBiz.Desktop.Services;
using SmartBiz.Desktop.ViewModels;

namespace SmartBiz.Desktop.Views.Sales;

public partial class CreateSaleView : ContentView
{
    private readonly CreateSaleViewModel _vm;
    private readonly IProductService _productService;
    private List<ProductListItem> _allProducts = new();

    public event EventHandler? Cancelled;
    public event EventHandler<string>? Saved;

    public CreateSaleView(CreateSaleViewModel vm, IProductService productService)
    {
        InitializeComponent();
        _vm = vm;
        _productService = productService;
        BindingContext = vm;

        ProductPicker.ProductSelected += OnProductPicked;
    }

    public async Task InitializeAsync()
    {
        await _vm.InitializeAsync();
        await LoadProductsAsync();
    }

    private async Task LoadProductsAsync()
    {
        try
        {
            var result = await _productService.GetProductsAsync(pageSize: 500);
            _allProducts = result.Items.ToList();
        }
        catch
        {
            // ignore for now
        }
    }

    private void OnAddProductClicked(object? sender, EventArgs e)
    {
        ProductPicker.Initialize(_allProducts);
    }

    private void OnProductPicked(object? sender, ProductListItem product)
    {
        _vm.AddProductLine(product);
    }

    private void OnQuantityChanged(object? sender, TextChangedEventArgs e)
    {
        if (sender is not Entry entry || entry.BindingContext is not SaleLineItem line) return;
        if (decimal.TryParse(e.NewTextValue, out var q))
        {
            line.Quantity = q;
            _vm.RecalculateLine(line);
            _vm.RecalculateTotals();
        }
    }

    private void OnUnitPriceChanged(object? sender, TextChangedEventArgs e)
    {
        if (sender is not Entry entry || entry.BindingContext is not SaleLineItem line) return;
        if (decimal.TryParse(e.NewTextValue, out var p))
        {
            line.UnitPrice = p;
            _vm.RecalculateLine(line);
            _vm.RecalculateTotals();
        }
    }

    private void OnDiscountChanged(object? sender, TextChangedEventArgs e)
    {
        if (sender is not Entry entry || entry.BindingContext is not SaleLineItem line) return;
        if (decimal.TryParse(e.NewTextValue, out var d))
        {
            line.DiscountAmount = d;
            _vm.RecalculateLine(line);
            _vm.RecalculateTotals();
        }
    }

    private void OnTaxRateChanged(object? sender, TextChangedEventArgs e)
    {
        if (sender is not Entry entry || entry.BindingContext is not SaleLineItem line) return;
        if (decimal.TryParse(e.NewTextValue, out var r))
        {
            line.TaxRate = r;
            _vm.RecalculateLine(line);
            _vm.RecalculateTotals();
        }
    }

    private void OnRemoveLineClicked(object? sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is SaleLineItem line)
        {
            _vm.RemoveLineCommand.Execute(line);
        }
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        if (decimal.TryParse(PaidAmountField.Text, out var paid))
            _vm.PaidAmount = paid;

        var sale = await _vm.SaveAsync();
        if (sale is not null)
        {
            Saved?.Invoke(this, sale.InvoiceNumber);
        }
    }

    private void OnCancelClicked(object? sender, EventArgs e)
    {
        Cancelled?.Invoke(this, EventArgs.Empty);
    }
}