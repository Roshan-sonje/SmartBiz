using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SmartBiz.Desktop.Exceptions;
using SmartBiz.Desktop.Services;

namespace SmartBiz.Desktop.ViewModels;

public partial class ProductsViewModel : BaseViewModel
{
    private readonly IProductService _service;
    private readonly IDialogService _dialog;
    private readonly ILogger<ProductsViewModel> _logger;

    public ObservableCollection<ProductRow> Products { get; } = new();

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private bool _isEmpty;
    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private bool _lowStockOnly;
    [ObservableProperty] private int _currentPage = 1;
    [ObservableProperty] private int _pageSize = 20;
    [ObservableProperty] private int _totalCount;
    [ObservableProperty] private int _totalPages;
    [ObservableProperty] private string _pageInfo = "Page 1 of 1";

    public ProductsViewModel(IProductService service, IDialogService dialog, ILogger<ProductsViewModel> logger)
    {
        _service = service;
        _dialog = dialog;
        _logger = logger;
    }

    public async Task LoadAsync()
    {
        IsLoading = true;
        ErrorMessage = null;
        try
        {
            var result = await _service.GetProductsAsync(
                page: CurrentPage,
                pageSize: PageSize,
                search: string.IsNullOrWhiteSpace(SearchText) ? null : SearchText,
                lowStockOnly: LowStockOnly);

            Products.Clear();
            foreach (var p in result.Items)
            {
                Products.Add(new ProductRow
                {
                    Id = p.Id,
                    Name = p.Name,
                    Sku = p.Sku,
                    CategoryName = p.CategoryName ?? "—",
                    BrandName = p.BrandName ?? "—",
                    SellingPriceText = p.SellingPrice.ToString("N2"),
                    StockText = $"{p.StockQuantity:0.##} {p.UnitShortName ?? ""}".Trim(),
                    IsLowStock = p.IsLowStock,
                    StockColor = p.IsLowStock ? "#DC2626" : "#059669"
                });
            }

            TotalCount = result.TotalCount;
            TotalPages = result.TotalPages;
            IsEmpty = Products.Count == 0;
            PageInfo = $"Page {CurrentPage} of {Math.Max(1, TotalPages)}  •  {TotalCount} product{(TotalCount == 1 ? "" : "s")}";
        }
        catch (ApiException ex) { ErrorMessage = ex.Message; }
        catch (Exception ex)
        {
            _logger.LogError(ex, "load products failed");
            ErrorMessage = "Failed to load products.";
        }
        finally { IsLoading = false; }
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        CurrentPage = 1;
        await LoadAsync();
    }

    [RelayCommand]
    private async Task NextPageAsync()
    {
        if (CurrentPage >= TotalPages) return;
        CurrentPage++;
        await LoadAsync();
    }

    [RelayCommand]
    private async Task PrevPageAsync()
    {
        if (CurrentPage <= 1) return;
        CurrentPage--;
        await LoadAsync();
    }

    [RelayCommand] private async Task RefreshAsync() => await LoadAsync();

    partial void OnLowStockOnlyChanged(bool value)
    {
        CurrentPage = 1;
        _ = LoadAsync();
    }

    [RelayCommand]
    private async Task DeleteProductAsync(ProductRow? row)
    {
        if (row is null) return;
        var ok = await _dialog.ConfirmAsync("Delete product",
            $"Delete {row.Name}? This will deactivate the product.", "Delete", "Cancel");
        if (!ok) return;
        try { await _service.DeleteProductAsync(row.Id); await LoadAsync(); }
        catch (ApiException ex) { await _dialog.ShowAlertAsync("Error", ex.Message); }
    }
}

public class ProductRow
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string BrandName { get; set; } = string.Empty;
    public string SellingPriceText { get; set; } = string.Empty;
    public string StockText { get; set; } = string.Empty;
    public bool IsLowStock { get; set; }
    public string StockColor { get; set; } = "#059669";
}