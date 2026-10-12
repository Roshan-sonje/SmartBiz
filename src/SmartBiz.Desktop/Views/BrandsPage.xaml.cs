using Microsoft.Extensions.DependencyInjection;
using SmartBiz.Desktop.ViewModels;

namespace SmartBiz.Desktop.Views;

public partial class BrandsPage : ContentView
{
    private readonly BrandsViewModel _vm;
    private readonly IServiceProvider _services;
    private bool _loaded;

    public BrandsPage(BrandsViewModel vm, IServiceProvider services)
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

    private void OnAddClicked(object? sender, EventArgs e)
    {
        BrandFormOverlay.Saved -= OnBrandSaved;
        BrandFormOverlay.Saved += OnBrandSaved;
        BrandFormOverlay.ShowForCreate();
    }

    private async void OnBrandSaved(object? sender, EventArgs e)
    {
        await _vm.LoadAsync();
    }

    private async void OnDeleteClicked(object? sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is BrandRow row)
            await _vm.DeleteBrandCommand.ExecuteAsync(row);
    }
}