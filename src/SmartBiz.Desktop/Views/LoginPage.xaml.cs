using SmartBiz.Desktop.ViewModels;

namespace SmartBiz.Desktop.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}