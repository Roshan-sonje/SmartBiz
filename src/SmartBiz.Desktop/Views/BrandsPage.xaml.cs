using Microsoft.Extensions.DependencyInjection;
using SmartBiz.Desktop.Services;
using SmartBiz.Desktop.ViewModels;
using SmartBiz.Desktop.Views.Brands;

namespace SmartBiz.Desktop.Views;

public partial class BrandsPage : ContentView
{
    private readonly BrandsViewModel _vm;
    private readonly IServiceProvider _services;

    public BrandsPage(BrandsViewModel vm, IServiceProvider services)
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
        var list = new BrandsListView(_vm);

        list.AddRequested += async (_, _) => await ShowFormAsync();
        list.DeleteRequested += async (_, row) => await _vm.DeleteBrandCommand.ExecuteAsync(row);

        ContentHost.Content = list;
    }

    private async Task ShowFormAsync()
    {
        var form = new BrandFormView(_services.GetRequiredService<IBrandService>());

        form.Saved += (_, _) =>
        {
            _ = _vm.LoadAsync();
            ShowList();
        };
        form.Cancelled += (_, _) => ShowList();

        ContentHost.Content = form;
        await form.InitializeForCreateAsync();
    }
}