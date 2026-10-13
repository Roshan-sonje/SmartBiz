using System.Collections.ObjectModel;
using SmartBiz.Desktop.Models.Products;

namespace SmartBiz.Desktop.Views.Sales;

public partial class ProductPickerView : ContentView
{
    private readonly List<ProductListItem> _allProducts = new();
    private readonly ObservableCollection<ProductPickRow> _filtered = new();

    public event EventHandler<ProductListItem>? ProductSelected;
    public event EventHandler? Cancelled;

    public ProductPickerView()
    {
        InitializeComponent();
        ProductList.ItemsSource = _filtered;
    }

    public void Initialize(List<ProductListItem> products)
    {
        _allProducts.Clear();
        _allProducts.AddRange(products);
        ApplyFilter(string.Empty);
        IsVisible = true;
        SearchEntry.Text = string.Empty;
    }

    private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        ApplyFilter(e.NewTextValue ?? string.Empty);
    }

    private void ApplyFilter(string search)
    {
        _filtered.Clear();

        var query = _allProducts.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(p =>
                p.Name.ToLower().Contains(term) ||
                p.Sku.ToLower().Contains(term));
        }

        foreach (var p in query.Take(100))
        {
            _filtered.Add(new ProductPickRow
            {
                Product = p,
                Name = p.Name,
                Sku = p.Sku,
                CategoryName = p.CategoryName ?? "—",
                StockText = $"{p.StockQuantity:0.##} in stock",
                PriceText = "₹" + p.SellingPrice.ToString("N2")
            });
        }
    }

    private void OnProductSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is ProductPickRow row)
        {
            ProductSelected?.Invoke(this, row.Product);
            IsVisible = false;
            ProductList.SelectedItem = null;
        }
    }

    private void OnCancelClicked(object? sender, EventArgs e)
    {
        IsVisible = false;
        Cancelled?.Invoke(this, EventArgs.Empty);
    }
}

public class ProductPickRow
{
    public ProductListItem Product { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string StockText { get; set; } = string.Empty;
    public string PriceText { get; set; } = string.Empty;
}