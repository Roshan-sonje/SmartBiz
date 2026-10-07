using Microsoft.Extensions.Logging;
using SmartBiz.Desktop.Configuration;
using SmartBiz.Desktop.Services;
using SmartBiz.Desktop.ViewModels;
using SmartBiz.Desktop.Views;

namespace SmartBiz.Desktop;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddHttpClient<IApiClient, ApiClient>(client =>
        {
            client.BaseAddress = new Uri(AppConfig.ApiBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        // Services
        builder.Services.AddSingleton<ITokenStore, TokenStore>();
        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<INavigationService, NavigationService>();
        builder.Services.AddSingleton<IDialogService, DialogService>();
        builder.Services.AddSingleton<IBusinessService, BusinessService>();
        builder.Services.AddSingleton<IUserService, UserService>();

        // ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<ShellViewModel>();
        builder.Services.AddTransient<BusinessProfileViewModel>();
        builder.Services.AddTransient<UsersViewModel>();
        builder.Services.AddTransient<InvoiceSettingsViewModel>();

        // Views
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<ShellPage>();
        builder.Services.AddTransient<BusinessProfilePage>();
        builder.Services.AddTransient<UsersPage>();
        builder.Services.AddTransient<InvoiceSettingsPage>();
        builder.Services.AddTransient<SettingsPage>();

        builder.Services.AddSingleton<ICustomerService, CustomerService>();
        builder.Services.AddTransient<CustomersViewModel>();
        builder.Services.AddTransient<CustomersPage>();
        builder.Services.AddTransient<CustomerFormDialog>();

#if DEBUG
        builder.Logging.AddDebug();
        builder.Logging.SetMinimumLevel(LogLevel.Information);
#endif

        return builder.Build();
    }
}