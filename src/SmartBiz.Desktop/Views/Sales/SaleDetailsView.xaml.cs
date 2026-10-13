using SmartBiz.Desktop.ViewModels;

namespace SmartBiz.Desktop.Views.Sales;

public partial class SaleDetailsView : ContentView
{
    private readonly SaleDetailsViewModel _vm;

    public event EventHandler? Closed;
    public event EventHandler? Cancelled;

    public SaleDetailsView(SaleDetailsViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    public async Task InitializeAsync(Guid saleId)
    {
        await _vm.LoadAsync(saleId);
    }

    private void OnBackClicked(object? sender, EventArgs e)
    {
        Closed?.Invoke(this, EventArgs.Empty);
    }

    private async void OnCancelSaleClicked(object? sender, EventArgs e)
    {
        var confirmed = await Application.Current!.MainPage!.DisplayAlert(
            "Cancel Sale",
            $"Cancel this sale ({_vm.Sale.InvoiceNumber})? Stock will be restored.",
            "Cancel Sale", "Keep");

        if (!confirmed) return;

        var success = await _vm.CancelAsync();
        if (success)
        {
            Cancelled?.Invoke(this, EventArgs.Empty);
        }
    }
}