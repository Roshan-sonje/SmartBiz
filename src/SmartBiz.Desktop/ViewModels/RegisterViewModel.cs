using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SmartBiz.Desktop.Exceptions;
using SmartBiz.Desktop.Models.Auth;
using SmartBiz.Desktop.Services;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace SmartBiz.Desktop.ViewModels;

public partial class RegisterViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private readonly INavigationService _navigation;
    private readonly ILogger<RegisterViewModel> _logger;

    [ObservableProperty] private string _fullName = string.Empty;
    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private string _password = string.Empty;
    [ObservableProperty] private string _confirmPassword = string.Empty;
    [ObservableProperty] private string _businessName = string.Empty;
    [ObservableProperty] private string? _phone;

    public RegisterViewModel(
        IAuthService authService,
        INavigationService navigation,
        ILogger<RegisterViewModel> logger)
    {
        _authService = authService;
        _navigation = navigation;
        _logger = logger;
    }

    [RelayCommand]
    private async Task CreateAccountAsync()
    {
        ErrorMessage = null;

        if (string.IsNullOrWhiteSpace(FullName)) { ErrorMessage = "Enter your full name."; return; }
        if (string.IsNullOrWhiteSpace(Email)) { ErrorMessage = "Enter your email."; return; }
        if (Password.Length < 8) { ErrorMessage = "Password must be at least 8 characters."; return; }
        if (Password != ConfirmPassword) { ErrorMessage = "Passwords do not match."; return; }
        if (string.IsNullOrWhiteSpace(BusinessName)) { ErrorMessage = "Enter your business name."; return; }

        IsBusy = true;
        try
        {
            var request = new RegisterRequest
            {
                FullName = FullName.Trim(),
                Email = Email.Trim().ToLowerInvariant(),
                Password = Password,
                Phone = string.IsNullOrWhiteSpace(Phone) ? null : Phone.Trim(),
                BusinessName = BusinessName.Trim()
            };

            var result = await _authService.RegisterAsync(request);
            _logger.LogInformation("Registered user {Email} for business {Business}", result.Email, result.BusinessName);

            await _navigation.NavigateToAsync("//shell");
        }
        catch (ApiException ex)
        {
            _logger.LogWarning(ex, "Registration failed");
            ErrorMessage = ex.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected register error");
            ErrorMessage = "Something went wrong. Please try again.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task GoBackToLoginAsync()
    {
        await _navigation.GoBackAsync();
    }
}