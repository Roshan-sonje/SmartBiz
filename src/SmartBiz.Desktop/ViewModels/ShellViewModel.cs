using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SmartBiz.Desktop.Navigation;
using SmartBiz.Desktop.Services;

namespace SmartBiz.Desktop.ViewModels;

public partial class ShellViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private readonly ITokenStore _tokenStore;
    private readonly ILogger<ShellViewModel> _logger;

    [ObservableProperty]
    private string _currentRoute = NavRoutes.Dashboard;

    [ObservableProperty]
    private string _pageTitle = "Dashboard";

    [ObservableProperty]
    private string _pageSubtitle = "Overview of your business";

    [ObservableProperty]
    private string _userFullName = "User";

    [ObservableProperty]
    private string _businessName = "Business";

    [ObservableProperty]
    private string _userInitials = "U";

    public ShellViewModel(
        IAuthService authService,
        ITokenStore tokenStore,
        ILogger<ShellViewModel> logger)
    {
        _authService = authService;
        _tokenStore = tokenStore;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        var fullName = await _tokenStore.GetFullNameAsync();
        var email = await _tokenStore.GetEmailAsync();

        UserFullName = string.IsNullOrWhiteSpace(fullName) ? (email ?? "User") : fullName;
        UserInitials = BuildInitials(UserFullName);
        BusinessName = "SmartBiz Demo Co.";
    }

    public void NavigateTo(string route)
    {
        if (string.IsNullOrWhiteSpace(route)) return;

        CurrentRoute = route;

        (PageTitle, PageSubtitle) = route switch
        {
            NavRoutes.Dashboard => ("Dashboard", "Overview of your business"),
            NavRoutes.Customers => ("Customers", "Manage your customer list"),
            NavRoutes.Products => ("Products", "Manage your product catalog"),
            NavRoutes.Sales => ("Sales", "View and create sales"),
            NavRoutes.Purchases => ("Purchases", "Manage supplier purchases"),
            NavRoutes.Payments => ("Payments", "Track incoming and outgoing payments"),
            NavRoutes.Expenses => ("Expenses", "Log business expenses"),
            NavRoutes.Reports => ("Reports", "Business insights and analytics"),
            NavRoutes.Settings => ("Settings", "Configure your business"),
            _ => ("SmartBiz", string.Empty)
        };
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        await _authService.LogoutAsync();
        _logger.LogInformation("User signed out");

        MainThread.BeginInvokeOnMainThread(() =>
        {
            var loginPage = Application.Current?.Handler?.MauiContext?.Services
                .GetService<Views.LoginPage>();
            if (loginPage is not null && Application.Current is not null)
                Application.Current.MainPage = loginPage;
        });
    }

    private static string BuildInitials(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "U";
        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return "U";
        var first = parts[0].Length > 0 ? parts[0][..1] : "U";
        if (parts.Length == 1) return first.ToUpperInvariant();
        var last = parts[^1].Length > 0 ? parts[^1][..1] : string.Empty;
        return (first + last).ToUpperInvariant();
    }
}