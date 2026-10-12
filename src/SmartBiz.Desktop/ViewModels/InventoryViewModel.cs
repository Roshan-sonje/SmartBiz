using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SmartBiz.Desktop.Exceptions;
using SmartBiz.Desktop.Services;

namespace SmartBiz.Desktop.ViewModels;

public partial class InventoryViewModel : BaseViewModel
{
    private readonly IInventoryService _inventoryService;
    private readonly ILogger<InventoryViewModel> _logger;

    public ObservableCollection<TransactionRow> Transactions { get; } = new();
    public ObservableCollection<LowStockRow> LowStockProducts { get; } = new();

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private bool _hasLowStock;

    // Valuation stats
    [ObservableProperty] private int _productCount;
    [ObservableProperty] private string _totalQuantityText = "0";
    [ObservableProperty] private string _purchaseValueText = "₹0";
    [ObservableProperty] private string _potentialProfitText = "₹0";

    [ObservableProperty] private string _filterMode = "all";   // "all" | "low" | "today"

    public InventoryViewModel(IInventoryService inventoryService, ILogger<InventoryViewModel> logger)
    {
        _inventoryService = inventoryService;
        _logger = logger;
    }

    public async Task LoadAsync()
    {
        IsLoading = true;
        ErrorMessage = null;

        try
        {
            // Load valuation
            var valuation = await _inventoryService.GetValuationAsync();
            ProductCount = valuation.ProductCount;
            TotalQuantityText = valuation.TotalQuantity.ToString("N0");
            PurchaseValueText = "₹" + valuation.TotalPurchaseValue.ToString("N0");
            PotentialProfitText = "₹" + valuation.TotalPotentialProfit.ToString("N0");

            // Load low stock
            var lowStock = await _inventoryService.GetLowStockProductsAsync();
            LowStockProducts.Clear();
            foreach (var p in lowStock)
            {
                LowStockProducts.Add(new LowStockRow
                {
                    Id = p.Id,
                    Name = p.Name,
                    Sku = p.Sku,
                    StockText = $"{p.StockQuantity:0.##} {p.UnitShortName}",
                    MinText = $"min: {p.MinStockLevel:0.##}",
                    DeficitText = p.StockQuantity <= 0
                        ? "Out of stock"
                        : $"Short by {p.Deficit:0.##}"
                });
            }
            HasLowStock = LowStockProducts.Count > 0;

            // Load transactions
            var result = await _inventoryService.GetTransactionsAsync(pageSize: 50);
            Transactions.Clear();
            foreach (var t in result.Items)
            {
                var qtySign = t.Quantity > 0 ? "+" : "";
                Transactions.Add(new TransactionRow
                {
                    Id = t.Id,
                    DateText = t.CreatedAt.ToLocalTime().ToString("MM-dd HH:mm"),
                    ProductName = t.ProductName,
                    TypeText = t.Type,
                    QuantityText = $"{qtySign}{t.Quantity:0.##}",
                    QuantityColor = t.Quantity > 0 ? "#059669" : "#DC2626",
                    BalanceText = $"{t.BalanceAfter:0.##}",
                    ReasonText = string.IsNullOrWhiteSpace(t.Notes) ? "—" : t.Notes,
                    UserText = t.CreatedByUserName ?? "system"
                });
            }

            _logger.LogInformation("Inventory loaded: {Products} products, {Low} low stock, {Tx} transactions",
                valuation.ProductCount, LowStockProducts.Count, result.TotalCount);
        }
        catch (ApiException ex)
        {
            _logger.LogWarning(ex, "Failed to load inventory");
            ErrorMessage = ex.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error loading inventory");
            ErrorMessage = "Failed to load inventory data.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task RefreshAsync() => await LoadAsync();
}

public class TransactionRow
{
    public Guid Id { get; set; }
    public string DateText { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string TypeText { get; set; } = string.Empty;
    public string QuantityText { get; set; } = string.Empty;
    public string QuantityColor { get; set; } = "#111827";
    public string BalanceText { get; set; } = string.Empty;
    public string ReasonText { get; set; } = string.Empty;
    public string UserText { get; set; } = string.Empty;
}

public class LowStockRow
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public string StockText { get; set; } = string.Empty;
    public string MinText { get; set; } = string.Empty;
    public string DeficitText { get; set; } = string.Empty;
}