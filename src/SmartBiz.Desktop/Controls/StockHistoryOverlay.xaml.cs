using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;
using SmartBiz.Desktop.Services;

namespace SmartBiz.Desktop.Controls;

public partial class StockHistoryOverlay : ContentView
{
    private readonly IInventoryService _inventoryService;

    public event EventHandler? Closed;

    public StockHistoryOverlay() : this(
        Application.Current?.Handler?.MauiContext?.Services.GetRequiredService<IInventoryService>()
            ?? throw new InvalidOperationException("Service provider not available"))
    {
    }

    public StockHistoryOverlay(IInventoryService inventoryService)
    {
        InitializeComponent();
        _inventoryService = inventoryService;
    }

    public async Task ShowAsync(Guid productId, string productName, string sku)
    {
        TitleLabel.Text = "Stock History";
        SubtitleLabel.Text = $"{productName}  •  SKU: {sku}";
        HistoryList.ItemsSource = null;
        SummaryLabel.Text = "Loading...";

        InputTransparent = false;
        IsVisible = true;

        await LoadAsync(productId);
    }

    private async Task LoadAsync(Guid productId)
    {
        try
        {
            var result = await _inventoryService.GetProductHistoryAsync(productId, pageSize: 100);

            var rows = new ObservableCollection<HistoryRow>();
            foreach (var t in result.Items)
            {
                var qtySign = t.Quantity > 0 ? "+" : "";
                rows.Add(new HistoryRow
                {
                    DateText = t.CreatedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm"),
                    TypeText = t.Type,
                    QuantityText = $"{qtySign}{t.Quantity:0.##}",
                    QuantityColor = t.Quantity > 0 ? Color.FromArgb("#059669") : Color.FromArgb("#DC2626"),
                    BalanceText = $"{t.BalanceAfter:0.##}",
                    ReasonText = string.IsNullOrWhiteSpace(t.Notes) ? "—" : t.Notes,
                    UserText = t.CreatedByUserName ?? "system"
                });
            }

            HistoryList.ItemsSource = rows;
            SummaryLabel.Text = $"{result.TotalCount} transaction{(result.TotalCount == 1 ? "" : "s")}";
        }
        catch (Exception ex)
        {
            SummaryLabel.Text = $"Error: {ex.Message}";
        }
    }

    private void OnCloseClicked(object? sender, EventArgs e)
    {
        IsVisible = false;
        InputTransparent = true;
        Closed?.Invoke(this, EventArgs.Empty);
    }
}

public class HistoryRow
{
    public string DateText { get; set; } = string.Empty;
    public string TypeText { get; set; } = string.Empty;
    public string QuantityText { get; set; } = string.Empty;
    public Color QuantityColor { get; set; } = Colors.Black;
    public string BalanceText { get; set; } = string.Empty;
    public string ReasonText { get; set; } = string.Empty;
    public string UserText { get; set; } = string.Empty;
}