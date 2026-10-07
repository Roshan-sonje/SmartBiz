using Microsoft.Extensions.DependencyInjection;
using SmartBiz.Desktop.ViewModels;

namespace SmartBiz.Desktop.Views;

public partial class CustomersPage : ContentView
{
    private readonly CustomersViewModel _viewModel;
    private readonly IServiceProvider _services;
    private bool _hasLoaded;

    public CustomersPage(CustomersViewModel viewModel, IServiceProvider services)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _services = services;
        BindingContext = viewModel;
    }

    protected override void OnParentSet()
    {
        base.OnParentSet();

        if (Parent is not null && !_hasLoaded)
        {
            _hasLoaded = true;
            _ = _viewModel.LoadAsync();
        }
    }

    private async void OnAddClicked(object? sender, EventArgs e)
    {
        var dialog = _services.GetRequiredService<CustomerFormDialog>();
        await dialog.ShowForCreateAsync();
        await _viewModel.LoadAsync();
    }

    private async void OnDeleteClicked(object? sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is CustomerRow row)
        {
            await _viewModel.DeleteCustomerCommand.ExecuteAsync(row);
        }
    }
}