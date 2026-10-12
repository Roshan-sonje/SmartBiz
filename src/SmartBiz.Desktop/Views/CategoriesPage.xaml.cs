using Microsoft.Extensions.DependencyInjection;
using SmartBiz.Desktop.ViewModels;

namespace SmartBiz.Desktop.Views;

public partial class CategoriesPage : ContentView
{
    private readonly CategoriesViewModel _vm;
    private readonly IServiceProvider _services;
    private bool _loaded;

    public CategoriesPage(CategoriesViewModel vm, IServiceProvider services)
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
        CategoryFormOverlay.Saved -= OnCategorySaved;
        CategoryFormOverlay.Saved += OnCategorySaved;
        CategoryFormOverlay.ShowForCreate();
        await Task.CompletedTask;
    }

    private async void OnCategorySaved(object? sender, EventArgs e)
    {
        await _vm.LoadAsync();
    }

    private async void OnDeleteClicked(object? sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is CategoryRow row)
            await _vm.DeleteCategoryCommand.ExecuteAsync(row);
    }
}