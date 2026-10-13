using Microsoft.Extensions.DependencyInjection;
using SmartBiz.Desktop.Services;
using SmartBiz.Desktop.ViewModels;
using SmartBiz.Desktop.Views.Sales;

namespace SmartBiz.Desktop.Views;

public partial class SalesPage : ContentView
{
    private readonly SalesViewModel _vm;
    private readonly IServiceProvider _services;

    public SalesPage(SalesViewModel vm, IServiceProvider services)
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
        var list = new SalesListView(_vm);

        list.NewSaleRequested += async (_, _) =>
        {
            var createVm = _services.GetRequiredService<CreateSaleViewModel>();
            var createView = new CreateSaleView(
                createVm,
                _services.GetRequiredService<IProductService>());

            createView.Saved += async (_, invoice) =>
            {
                await _vm.LoadAsync();
                ShowList();
                await Application.Current!.MainPage!.DisplayAlert("Success",
                    $"Sale {invoice} created successfully.", "OK");
            };
            createView.Cancelled += (_, _) => ShowList();

            ContentHost.Content = createView;
            await createView.InitializeAsync();
        };

        ContentHost.Content = list;
    }
}