using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SmartBiz.Desktop.Exceptions;
using SmartBiz.Desktop.Services;

namespace SmartBiz.Desktop.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private readonly ILogger<LoginViewModel> _logger;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private bool _rememberMe = true;

    public LoginViewModel(
        IAuthService authService,
        ILogger<LoginViewModel> logger)
    {
        _authService = authService;
        _logger = logger;
    }

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

            // Navigate to ShellPage (direct page swap — no Shell routing)
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var shellPage = Application.Current?.Handler?.MauiContext?.Services
                    .GetService<Views.ShellPage>();

                if (shellPage is null)
                {
                    ErrorMessage = "Unable to create application shell.";
                    return;
                }

                Application.Current!.MainPage = shellPage;
            });
        }
        catch (ApiException ex)
        {
            _logger.LogWarning(ex, "Login failed");
            ErrorMessage = ex.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected login error");
            ErrorMessage = $"DEBUG: {ex.GetType().Name}: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task GoToRegisterAsync()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            var registerPage = Application.Current?.Handler?.MauiContext?.Services
                .GetService<Views.RegisterPage>();

            if (registerPage is not null)
                Application.Current!.MainPage = registerPage;
        });

        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task ForgotPasswordAsync()
    {
        await Task.CompletedTask;
    }
}