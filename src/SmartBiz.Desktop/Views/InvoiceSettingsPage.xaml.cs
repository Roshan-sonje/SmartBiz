using SmartBiz.Desktop.ViewModels;

namespace SmartBiz.Desktop.Views;

public partial class InvoiceSettingsPage : ContentView
{
    private readonly InvoiceSettingsViewModel _viewModel;
    private bool _hasLoaded;

    public InvoiceSettingsPage(InvoiceSettingsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override void OnParentSet()
    {
        base.OnParentSet();

        if (Parent is not null && !_hasLoaded)
        {
            _hasLoaded = true;
            _ = _viewModel.LoadAsync();
        }
    }
}