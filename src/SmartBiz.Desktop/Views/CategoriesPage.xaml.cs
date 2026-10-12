using Microsoft.Extensions.DependencyInjection;
using SmartBiz.Desktop.Services;
using SmartBiz.Desktop.ViewModels;
using SmartBiz.Desktop.Views.Categories;

namespace SmartBiz.Desktop.Views;

public partial class CategoriesPage : ContentView
{
    private readonly CategoriesViewModel _vm;
    private readonly IServiceProvider _services;

    public CategoriesPage(CategoriesViewModel vm, IServiceProvider services)
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
        var list = new CategoriesListView(_vm);

        list.AddRequested += async (_, _) => await ShowFormAsync();
        list.DeleteRequested += async (_, row) => await _vm.DeleteCategoryCommand.ExecuteAsync(row);

        ContentHost.Content = list;
    }

    private async Task ShowFormAsync()
    {
        var form = new CategoryFormView(_services.GetRequiredService<ICategoryService>());

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