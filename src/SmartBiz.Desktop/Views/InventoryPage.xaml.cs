using SmartBiz.Desktop.ViewModels;
using SmartBiz.Desktop.Views.Inventory;

namespace SmartBiz.Desktop.Views;

public partial class InventoryPage : ContentView
{
    private readonly InventoryViewModel _vm;

    public InventoryPage(InventoryViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
    }

    protected override void OnParentSet()
    {
        base.OnParentSet();
        if (Parent is not null)
        {
            ContentHost.Content = new InventoryListView(_vm);
        }
    }
}