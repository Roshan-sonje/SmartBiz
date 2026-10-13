using Microsoft.Extensions.DependencyInjection;
using SmartBiz.Desktop.ViewModels;
using SmartBiz.Desktop.Views.Sales;

namespace SmartBiz.Desktop.Views;

public partial class SalesPage : ContentView
{
    private readonly SalesViewModel _vm;

    public SalesPage(SalesViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
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

        list.NewSaleRequested += (_, _) =>
        {
            // Placeholder — will be wired to Create Sale form in Phase 9C
            Application.Current!.MainPage!.DisplayAlert("Coming Soon",
                "Create Sale form is coming in Phase 9C.", "OK");
        };

        ContentHost.Content = list;
    }
}