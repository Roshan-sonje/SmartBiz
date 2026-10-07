using SmartBiz.Desktop.ViewModels;

namespace SmartBiz.Desktop.Views;

public partial class ShellPage : ContentPage
{
    public ShellPage(ShellViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}