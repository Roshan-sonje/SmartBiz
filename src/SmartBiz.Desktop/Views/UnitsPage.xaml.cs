using Microsoft.Extensions.DependencyInjection;
using SmartBiz.Desktop.Services;
using SmartBiz.Desktop.ViewModels;
using SmartBiz.Desktop.Views.Units;

namespace SmartBiz.Desktop.Views;

public partial class UnitsPage : ContentView
{
    private readonly UnitsViewModel _vm;
    private readonly IServiceProvider _services;

    public UnitsPage(UnitsViewModel vm, IServiceProvider services)
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
        var list = new UnitsListView(_vm);

        list.AddRequested += async (_, _) => await ShowFormAsync();
        list.DeleteRequested += async (_, row) => await _vm.DeleteUnitCommand.ExecuteAsync(row);

        ContentHost.Content = list;
    }

    private async Task ShowFormAsync()
    {
        var form = new UnitFormView(_services.GetRequiredService<IUnitService>());

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