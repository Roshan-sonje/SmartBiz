using SmartBiz.Desktop.ViewModels;

namespace SmartBiz.Desktop.Views;

public partial class RegisterPage : ContentPage
{
    public RegisterPage(RegisterViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}