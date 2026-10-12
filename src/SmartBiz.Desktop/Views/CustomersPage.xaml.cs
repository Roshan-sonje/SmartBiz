using Microsoft.Extensions.DependencyInjection;
using SmartBiz.Desktop.ViewModels;
using SmartBiz.Desktop.Views.Customers;

namespace SmartBiz.Desktop.Views;

public partial class CustomersPage : ContentView
{
    private readonly CustomersViewModel _vm;
    private readonly IServiceProvider _services;

    public CustomersPage(CustomersViewModel vm, IServiceProvider services)
    {
        InitializeComponent();
        _vm = vm;
        _services = services;
    }

    protected override void OnParentSet()
    {
        base.OnParentSet();
        if (Parent is not null)
        {
            ShowList();
        }
    }

    private void ShowList()
    {
        var list = new CustomersListView(_vm);

        list.AddRequested += async (_, _) => await ShowFormAsync(null);
        list.DeleteRequested += async (_, row) => await _vm.DeleteCustomerCommand.ExecuteAsync(row);

        ContentHost.Content = list;
    }

    private async Task ShowFormAsync(Guid? customerId)
    {
        var form = new CustomerFormView(_services.GetRequiredService<Services.ICustomerService>());

        form.Saved += (_, _) =>
        {
            _ = _vm.LoadAsync();
            ShowList();
        };
        form.Cancelled += (_, _) => ShowList();

        ContentHost.Content = form;

        if (customerId is null)
            await form.InitializeForCreateAsync();
        else
            await form.InitializeForEditAsync(customerId.Value);
    }
}