using Microsoft.Extensions.DependencyInjection;
using SmartBiz.Desktop.ViewModels;

namespace SmartBiz.Desktop.Views;

public partial class ProductsPage : ContentView
{
    private readonly ProductsViewModel _vm;
    private readonly IServiceProvider _services;
    private bool _loaded;

    public ProductsPage(ProductsViewModel vm, IServiceProvider services)
    {
        InitializeComponent();
        _vm = vm;
        _services = services;
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

    private async void OnAddClicked(object? sender, EventArgs e)
    {
        var dlg = _services.GetRequiredService<ProductFormDialog>();
        await dlg.ShowForCreateAsync();
        await _vm.LoadAsync();
    }

    private async void OnDeleteClicked(object? sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is ProductRow row)
            await _vm.DeleteProductCommand.ExecuteAsync(row);
    }
}