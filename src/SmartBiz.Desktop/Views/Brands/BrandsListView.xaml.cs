using SmartBiz.Desktop.ViewModels;

namespace SmartBiz.Desktop.Views.Brands;

public partial class BrandsListView : ContentView
{
    private readonly BrandsViewModel _vm;
    private bool _loaded;

    public event EventHandler? AddRequested;
    public event EventHandler<BrandRow>? DeleteRequested;

    public BrandsListView(BrandsViewModel vm)
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

    private void OnAddClicked(object? sender, EventArgs e)
        => AddRequested?.Invoke(this, EventArgs.Empty);

    private void OnDeleteClicked(object? sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is BrandRow row)
            DeleteRequested?.Invoke(this, row);
    }
}