using SmartBiz.Desktop.ViewModels;

namespace SmartBiz.Desktop.Views;

public partial class BusinessProfilePage : ContentView
{
    private readonly BusinessProfileViewModel _viewModel;

    public BusinessProfilePage(BusinessProfileViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;

        Loaded += OnLoaded;
    }

    private async void OnLoaded(object? sender, EventArgs e)
    {
        await _viewModel.LoadAsync();
    }
}