using Microsoft.Extensions.DependencyInjection;
using SmartBiz.Desktop.Exceptions;
using SmartBiz.Desktop.Models.Inventory;
using SmartBiz.Desktop.Services;

namespace SmartBiz.Desktop.Controls;

public partial class AdjustStockOverlay : ContentView
{
    private readonly IInventoryService _inventoryService;
    private Guid _productId;
    private decimal _currentStock;

    public event EventHandler? Saved;
    public event EventHandler? Closed;

    public AdjustStockOverlay() : this(
        Application.Current?.Handler?.MauiContext?.Services.GetRequiredService<IInventoryService>()
            ?? throw new InvalidOperationException("Service provider not available"))
    {
    }

    public AdjustStockOverlay(IInventoryService inventoryService)
    {
        InitializeComponent();
        _inventoryService = inventoryService;
        AdjustmentField.TextChanged += (_, _) => UpdatePreview();
    }

    public void Show(Guid productId, string productName, decimal currentStock)
    {
        _productId = productId;
        _currentStock = currentStock;

        ProductNameLabel.Text = productName;
        CurrentStockLabel.Text = $"{currentStock:0.##}";
        AdjustmentField.Text = string.Empty;
        ReasonField.Text = string.Empty;
        PreviewLabel.Text = "—";
        ErrorLabel.IsVisible = false;
        SaveButton.IsEnabled = true;

        InputTransparent = false;
        IsVisible = true;
    }

    public void Hide()
    {
        IsVisible = false;
        InputTransparent = true;
        Closed?.Invoke(this, EventArgs.Empty);
    }

    private void UpdatePreview()
    {
        if (decimal.TryParse(AdjustmentField.Text, out var adj) && adj != 0)
        {
            var newBal = _currentStock + adj;
            PreviewLabel.Text = $"{_currentStock:0.##} → {newBal:0.##}";
            PreviewLabel.TextColor = newBal < 0 ? Color.FromArgb("#DC2626") : Color.FromArgb("#2563EB");
        }
        else
        {
            PreviewLabel.Text = "—";
        }
    }

    private void OnCancelClicked(object? sender, EventArgs e)
    {
        Hide();
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;

        if (!decimal.TryParse(AdjustmentField.Text, out var adj) || adj == 0)
        {
            ErrorLabel.Text = "Enter a non-zero adjustment value.";
            ErrorLabel.IsVisible = true;
            return;
        }

        if (string.IsNullOrWhiteSpace(ReasonField.Text))
        {
            ErrorLabel.Text = "Reason is required.";
            ErrorLabel.IsVisible = true;
            return;
        }

        SaveButton.IsEnabled = false;
        try
        {
            await _inventoryService.AdjustStockAsync(new AdjustStockRequest
            {
                ProductId = _productId,
                Adjustment = adj,
                Reason = ReasonField.Text.Trim()
            });

            Hide();
            Saved?.Invoke(this, EventArgs.Empty);
        }
        catch (ApiException ex)
        {
            ErrorLabel.Text = ex.Message;
            ErrorLabel.IsVisible = true;
        }
        catch (Exception)
        {
            ErrorLabel.Text = "Failed to adjust stock. Please try again.";
            ErrorLabel.IsVisible = true;
        }
        finally
        {
            SaveButton.IsEnabled = true;
        }
    }
}