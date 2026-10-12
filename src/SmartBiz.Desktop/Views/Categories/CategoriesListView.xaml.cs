using SmartBiz.Desktop.ViewModels;

namespace SmartBiz.Desktop.Views.Categories;

public partial class CategoriesListView : ContentView
{
    private readonly CategoriesViewModel _vm;
    private bool _loaded;

    public event EventHandler? AddRequested;
    public event EventHandler<CategoryRow>? DeleteRequested;

    public CategoriesListView(CategoriesViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
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

    private void OnAddClicked(object? sender, EventArgs e)
        => AddRequested?.Invoke(this, EventArgs.Empty);

    private void OnDeleteClicked(object? sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is CategoryRow row)
            DeleteRequested?.Invoke(this, row);
    }
}