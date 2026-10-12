using Microsoft.Extensions.DependencyInjection;
using SmartBiz.Desktop.ViewModels;

namespace SmartBiz.Desktop.Views;

public partial class SuppliersPage : ContentView
{
    private readonly SuppliersViewModel _vm;
    private readonly IServiceProvider _services;
    private bool _loaded;

    public SuppliersPage(SuppliersViewModel vm, IServiceProvider services)
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
        SupplierFormOverlay.Saved -= OnSupplierSaved;
        SupplierFormOverlay.Saved += OnSupplierSaved;
        SupplierFormOverlay.ShowForCreate();
        await Task.CompletedTask;
    }

    private async void OnSupplierSaved(object? sender, EventArgs e)
    {
        await _vm.LoadAsync();
    }

    private async void OnDeleteClicked(object? sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is SupplierRow row)
            await _vm.DeleteSupplierCommand.ExecuteAsync(row);
    }
}