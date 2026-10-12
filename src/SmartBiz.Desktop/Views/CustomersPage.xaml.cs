using Microsoft.Extensions.DependencyInjection;
using SmartBiz.Desktop.ViewModels;

namespace SmartBiz.Desktop.Views;

public partial class CustomersPage : ContentView
{
    private readonly CustomersViewModel _vm;
    private readonly IServiceProvider _services;
    private bool _loaded;

    public CustomersPage(CustomersViewModel vm, IServiceProvider services)
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
        CustomerFormOverlay.Saved -= OnCustomerSaved;
        CustomerFormOverlay.Saved += OnCustomerSaved;
        CustomerFormOverlay.ShowForCreate();
        await Task.CompletedTask;
    }

    private async void OnCustomerSaved(object? sender, EventArgs e)
    {
        await _vm.LoadAsync();
    }

    private async void OnDeleteClicked(object? sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is CustomerRow row)
            await _vm.DeleteCustomerCommand.ExecuteAsync(row);
    }
}