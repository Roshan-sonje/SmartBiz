using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SmartBiz.Desktop.Exceptions;
using SmartBiz.Desktop.Services;

namespace SmartBiz.Desktop.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private readonly INavigationService _navigation;
    private readonly IDialogService _dialog;
    private readonly ILogger<LoginViewModel> _logger;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private bool _rememberMe = true;

    public LoginViewModel(
        IAuthService authService,
        INavigationService navigation,
        IDialogService dialog,
        ILogger<LoginViewModel> logger)
    {
        _authService = authService;
        _navigation = navigation;
        _dialog = dialog;
        _logger = logger;

        Title = "Sign In";
    }

    public string Title { get; }

    [RelayCommand]
    private async Task SignInAsync()
    {
        ErrorMessage = null;

        if (string.IsNullOrWhiteSpace(Email))
        {
            ErrorMessage = "Please enter your email.";
            return;
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Please enter your password.";
            return;
        }

        IsBusy = true;
        try
        {
            var result = await _authService.LoginAsync(Email.Trim(), Password);

            _logger.LogInformation("Logged in as {Email}", result.Email);

            // Navigate to main shell
            await _navigation.NavigateToAsync("dashboard");
        }
        catch (ApiException ex)
        {
            _logger.LogWarning(ex, "Login failed");
            ErrorMessage = ex.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected login error");
            ErrorMessage = "Something went wrong. Please try again.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ForgotPasswordAsync()
    {
        await _dialog.ShowAlertAsync("Forgot password",
            "Password reset will be available in a future update. Contact support for now.");
    }



    [RelayCommand]
    private async Task GoToRegisterAsync()
    {
        await _navigation.NavigateToAsync("register");
    }
}