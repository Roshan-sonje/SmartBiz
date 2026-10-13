using SmartBiz.Desktop.ViewModels;

namespace SmartBiz.Desktop.Views.Sales;

public partial class SalesListView : ContentView
{
    private readonly SalesViewModel _vm;
    private bool _loaded;

    public event EventHandler? NewSaleRequested;
    public event EventHandler<SaleRow>? RowTapped;

    public SalesListView(SalesViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    protected override void OnParentSet()
    {
        base.OnParentSet();
        if (Parent is not null && !_loaded)
        {
            _loaded = true;
            _ = _vm.LoadAsync();
        }
    }

    private void OnNewSaleClicked(object? sender, EventArgs e)
    {
        NewSaleRequested?.Invoke(this, EventArgs.Empty);
    }

    private void OnRowTapped(object? sender, TappedEventArgs e)
    {
        if (sender is Grid grid && grid.BindingContext is SaleRow row)
        {
            RowTapped?.Invoke(this, row);
        }
    }
}