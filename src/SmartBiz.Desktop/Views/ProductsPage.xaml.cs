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

    private void ShowList()
    {
        var host = this.FindByName<ContentView>("ContentHost");
        var listRoot = this.FindByName<ContentView>("ListRoot");
        if (host is null || listRoot is null) return;

        host.IsVisible = false;
        host.InputTransparent = true;
        host.Content = null;

        listRoot.IsVisible = true;
        listRoot.InputTransparent = false;
    }

    private void ShowInlineView(ContentView view)
    {
        var host = this.FindByName<ContentView>("ContentHost");
        var listRoot = this.FindByName<ContentView>("ListRoot");
        if (host is null || listRoot is null) return;

        listRoot.IsVisible = false;
        listRoot.InputTransparent = true;

        host.Content = view;
        host.IsVisible = true;
        host.InputTransparent = false;
    }

    private async void OnAddClicked(object? sender, EventArgs e)
    {
        var form = new Controls.ProductFormOverlay();
        form.Saved += async (_, _) =>
        {
            await _vm.LoadAsync();
            ShowList();
        };
        form.Closed += (_, _) => ShowList();

        ShowInlineView(form);
        await form.ShowForCreateAsync();
    }

    private async void OnDeleteClicked(object? sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is ProductRow row)
            await _vm.DeleteProductCommand.ExecuteAsync(row);
    }

    private void OnAdjustClicked(object? sender, EventArgs e)
    {
        if (sender is not Button btn || btn.CommandParameter is not ProductRow row) return;

        var adjust = new Controls.AdjustStockOverlay();
        adjust.Saved += async (_, _) =>
        {
            await _vm.LoadAsync();
            ShowList();
        };
        adjust.Closed += (_, _) => ShowList();

        ShowInlineView(adjust);
        adjust.Show(row.Id, row.Name, row.StockQuantity);
    }

    private async void OnHistoryClicked(object? sender, EventArgs e)
    {
        if (sender is not Button btn || btn.CommandParameter is not ProductRow row) return;

        var hist = new Controls.StockHistoryOverlay();
        hist.Closed += (_, _) => ShowList();

        ShowInlineView(hist);
        await hist.ShowAsync(row.Id, row.Name, row.Sku);
    }
}