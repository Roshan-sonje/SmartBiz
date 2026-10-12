using Microsoft.Extensions.DependencyInjection;
using SmartBiz.Desktop.Exceptions;
using SmartBiz.Desktop.Models.Categories;
using SmartBiz.Desktop.Models.Brands;
using SmartBiz.Desktop.Models.Units;
using SmartBiz.Desktop.Models.Taxes;
using SmartBiz.Desktop.Models.Products;
using SmartBiz.Desktop.Services;

namespace SmartBiz.Desktop.Controls;

public partial class ProductFormOverlay : ContentView
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly IBrandService _brandService;
    private readonly IUnitService _unitService;
    private readonly ITaxService _taxService;

    private List<CategoryListItem> _categories = new();
    private List<BrandListItem> _brands = new();
    private List<UnitListItem> _units = new();
    private List<TaxListItem> _taxes = new();

    public event EventHandler? Saved;
    public event EventHandler? Closed;

    public ProductFormOverlay() : this(
        Application.Current?.Handler?.MauiContext?.Services.GetRequiredService<IProductService>()
            ?? throw new InvalidOperationException("Service provider not available"),
        Application.Current?.Handler?.MauiContext?.Services.GetRequiredService<ICategoryService>()
            ?? throw new InvalidOperationException("Service provider not available"),
        Application.Current?.Handler?.MauiContext?.Services.GetRequiredService<IBrandService>()
            ?? throw new InvalidOperationException("Service provider not available"),
        Application.Current?.Handler?.MauiContext?.Services.GetRequiredService<IUnitService>()
            ?? throw new InvalidOperationException("Service provider not available"),
        Application.Current?.Handler?.MauiContext?.Services.GetRequiredService<ITaxService>()
            ?? throw new InvalidOperationException("Service provider not available"))
    {
    }

    public ProductFormOverlay(
        IProductService productService,
        ICategoryService categoryService,
        IBrandService brandService,
        IUnitService unitService,
        ITaxService taxService)
    {
        InitializeComponent();
        _productService = productService;
        _categoryService = categoryService;
        _brandService = brandService;
        _unitService = unitService;
        _taxService = taxService;
    }

    public async Task ShowForCreateAsync()
    {
        ClearForm();
        ErrorLabel.IsVisible = false;

        InputTransparent = false;
        IsVisible = true;

        await LoadLookupsAsync();
    }

    public void Hide()
    {
        IsVisible = false;
        InputTransparent = true;
        Closed?.Invoke(this, EventArgs.Empty);
    }

    private async Task LoadLookupsAsync()
    {
        try
        {
            var cats = await _categoryService.GetCategoriesAsync(pageSize: 100);
            _categories = cats.Items.ToList();
            CategoryPicker.ItemsSource = _categories.Select(c => c.Name).ToList();

            var brands = await _brandService.GetBrandsAsync(pageSize: 100);
            _brands = brands.Items.ToList();
            BrandPicker.ItemsSource = _brands.Select(b => b.Name).ToList();

            var units = await _unitService.GetUnitsAsync(pageSize: 100);
            _units = units.Items.ToList();
            UnitPicker.ItemsSource = _units.Select(u => $"{u.Name} ({u.ShortName})").ToList();

            var taxes = await _taxService.GetTaxesAsync(pageSize: 100);
            _taxes = taxes.Items.ToList();
            TaxPicker.ItemsSource = _taxes.Select(t => t.Name).ToList();
        }
        catch (Exception ex)
        {
            ErrorLabel.Text = $"Failed to load lookups: {ex.Message}";
            ErrorLabel.IsVisible = true;
        }
    }

    private void ClearForm()
    {
        NameField.Text = string.Empty;
        SkuField.Text = string.Empty;
        BarcodeField.Text = string.Empty;
        DescriptionField.Text = string.Empty;
        PurchasePriceField.Text = string.Empty;
        SellingPriceField.Text = string.Empty;
        StockQuantityField.Text = string.Empty;
        MinStockField.Text = string.Empty;
        TrackInventorySwitch.IsToggled = true;

        CategoryPicker.SelectedIndex = -1;
        BrandPicker.SelectedIndex = -1;
        UnitPicker.SelectedIndex = -1;
        TaxPicker.SelectedIndex = -1;
    }

    private void OnCancelClicked(object? sender, EventArgs e) => Hide();

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;

        if (string.IsNullOrWhiteSpace(NameField.Text))
        {
            ErrorLabel.Text = "Product name is required.";
            ErrorLabel.IsVisible = true;
            return;
        }

        if (string.IsNullOrWhiteSpace(SkuField.Text))
        {
            ErrorLabel.Text = "SKU is required.";
            ErrorLabel.IsVisible = true;
            return;
        }

        if (UnitPicker.SelectedIndex < 0 || UnitPicker.SelectedIndex >= _units.Count)
        {
            ErrorLabel.Text = "Please select a unit.";
            ErrorLabel.IsVisible = true;
            return;
        }

        SaveButton.IsEnabled = false;
        try
        {
            var request = new CreateProductRequest
            {
                Name = NameField.Text.Trim(),
                Sku = SkuField.Text.Trim(),
                Barcode = NullIfEmpty(BarcodeField.Text),
                Description = NullIfEmpty(DescriptionField.Text),
                CategoryId = CategoryPicker.SelectedIndex >= 0 && CategoryPicker.SelectedIndex < _categories.Count
                    ? _categories[CategoryPicker.SelectedIndex].Id : null,
                BrandId = BrandPicker.SelectedIndex >= 0 && BrandPicker.SelectedIndex < _brands.Count
                    ? _brands[BrandPicker.SelectedIndex].Id : null,
                UnitId = _units[UnitPicker.SelectedIndex].Id,
                TaxId = TaxPicker.SelectedIndex >= 0 && TaxPicker.SelectedIndex < _taxes.Count
                    ? _taxes[TaxPicker.SelectedIndex].Id : null,
                PurchasePrice = ParseDecimal(PurchasePriceField.Text),
                SellingPrice = ParseDecimal(SellingPriceField.Text),
                StockQuantity = ParseDecimal(StockQuantityField.Text),
                MinStockLevel = ParseDecimal(MinStockField.Text),
                TrackInventory = TrackInventorySwitch.IsToggled
            };

            await _productService.CreateProductAsync(request);

            Hide();
            Saved?.Invoke(this, EventArgs.Empty);
        }
        catch (ApiException ex)
        {
            ErrorLabel.Text = ex.Message;
            ErrorLabel.IsVisible = true;
        }
        catch (Exception ex)
        {
            ErrorLabel.Text = $"Failed to save: {ex.Message}";
            ErrorLabel.IsVisible = true;
        }
        finally
        {
            SaveButton.IsEnabled = true;
        }
    }

    private static string? NullIfEmpty(string? v) => string.IsNullOrWhiteSpace(v) ? null : v.Trim();
    private static decimal ParseDecimal(string? v) => decimal.TryParse(v, out var r) ? r : 0m;
}