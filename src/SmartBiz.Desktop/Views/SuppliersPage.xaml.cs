using Microsoft.Extensions.DependencyInjection;
using SmartBiz.Desktop.Services;
using SmartBiz.Desktop.ViewModels;
using SmartBiz.Desktop.Views.Suppliers;

namespace SmartBiz.Desktop.Views;

public partial class SuppliersPage : ContentView
{
    private readonly SuppliersViewModel _vm;
    private readonly IServiceProvider _services;

    public SuppliersPage(SuppliersViewModel vm, IServiceProvider services)
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
        var list = new SuppliersListView(_vm);

        list.AddRequested += async (_, _) => await ShowFormAsync(null);
        list.DeleteRequested += async (_, row) => await _vm.DeleteSupplierCommand.ExecuteAsync(row);

        ContentHost.Content = list;
    }

    private async Task ShowFormAsync(Guid? supplierId)
    {
        var form = new SupplierFormView(_services.GetRequiredService<ISupplierService>());

        form.Saved += (_, _) =>
        {
            _ = _vm.LoadAsync();
            ShowList();
        };
        form.Cancelled += (_, _) => ShowList();

        ContentHost.Content = form;

        if (supplierId is null)
            await form.InitializeForCreateAsync();
        else
            await form.InitializeForEditAsync(supplierId.Value);
    }
}