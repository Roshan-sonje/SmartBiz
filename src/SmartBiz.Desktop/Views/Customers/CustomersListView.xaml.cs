using SmartBiz.Desktop.ViewModels;

namespace SmartBiz.Desktop.Views.Customers;

public partial class CustomersListView : ContentView
{
    private readonly CustomersViewModel _vm;
    private bool _loaded;

    public event EventHandler? AddRequested;
    public event EventHandler<CustomerRow>? DeleteRequested;

    public CustomersListView(CustomersViewModel vm)
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
    {
        AddRequested?.Invoke(this, EventArgs.Empty);
    }

    private void OnDeleteClicked(object? sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is CustomerRow row)
            DeleteRequested?.Invoke(this, row);
    }
}