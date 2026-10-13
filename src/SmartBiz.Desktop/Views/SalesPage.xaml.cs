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

        list.NewSaleRequested += async (_, _) => await ShowCreateFormAsync();
        list.RowTapped += async (_, row) => await ShowDetailsAsync(row.Id);

        ContentHost.Content = list;
    }

    private async Task ShowCreateFormAsync()
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
                $"Sale {invoice} created.", "OK");
        };
        createView.Cancelled += (_, _) => ShowList();

        ContentHost.Content = createView;
        await createView.InitializeAsync();
    }

    private async Task ShowDetailsAsync(Guid saleId)
    {
        var detailsVm = _services.GetRequiredService<SaleDetailsViewModel>();
        var detailsView = new SaleDetailsView(detailsVm);

        detailsView.Closed += async (_, _) =>
        {
            await _vm.LoadAsync();
            ShowList();
        };

        detailsView.Cancelled += async (_, _) =>
        {
            await _vm.LoadAsync();
            ShowList();
            await Application.Current!.MainPage!.DisplayAlert("Cancelled",
                "Sale has been cancelled and stock restored.", "OK");
        };

        ContentHost.Content = detailsView;
        await detailsView.InitializeAsync(saleId);
    }
}