using SmartBiz.Desktop.ViewModels;

namespace SmartBiz.Desktop.Views.Inventory;

public partial class InventoryListView : ContentView
{
    private readonly InventoryViewModel _vm;
    private bool _loaded;

    public InventoryListView(InventoryViewModel vm)
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
}