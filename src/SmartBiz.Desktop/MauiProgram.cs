using Microsoft.Extensions.Logging;
using SmartBiz.Desktop.Configuration;
using SmartBiz.Desktop.Controls;
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

        // HTTP
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
        builder.Services.AddSingleton<ICustomerService, CustomerService>();
        builder.Services.AddSingleton<ISupplierService, SupplierService>();
        builder.Services.AddSingleton<ICategoryService, CategoryService>();
        builder.Services.AddSingleton<IBrandService, BrandService>();
        builder.Services.AddSingleton<IUnitService, UnitService>();
        builder.Services.AddSingleton<ITaxService, TaxService>();
        builder.Services.AddSingleton<IProductService, ProductService>();
        builder.Services.AddSingleton<IInventoryService, InventoryService>();

        // ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<ShellViewModel>();
        builder.Services.AddTransient<BusinessProfileViewModel>();
        builder.Services.AddTransient<UsersViewModel>();
        builder.Services.AddTransient<InvoiceSettingsViewModel>();
        builder.Services.AddTransient<CustomersViewModel>();
        builder.Services.AddTransient<SuppliersViewModel>();
        builder.Services.AddTransient<ProductsViewModel>();
        builder.Services.AddTransient<CategoriesViewModel>();
        builder.Services.AddTransient<BrandsViewModel>();
        builder.Services.AddTransient<UnitsViewModel>();
        builder.Services.AddTransient<TaxesViewModel>();

        // Views
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<ShellPage>();
        builder.Services.AddTransient<BusinessProfilePage>();
        builder.Services.AddTransient<UsersPage>();
        builder.Services.AddTransient<InvoiceSettingsPage>();
        builder.Services.AddTransient<CustomersPage>();
        builder.Services.AddTransient<SuppliersPage>();
        builder.Services.AddTransient<ProductsPage>();
        builder.Services.AddTransient<CategoriesPage>();
        builder.Services.AddTransient<BrandsPage>();
        builder.Services.AddTransient<UnitsPage>();
        builder.Services.AddTransient<TaxesPage>();
        builder.Services.AddTransient<SettingsPage>();
        builder.Services.AddTransient<ProductSetupPage>();

        // Overlays (for XAML — parameterless constructors pull deps from DI)
        builder.Services.AddTransient<AdjustStockOverlay>();
        builder.Services.AddTransient<StockHistoryOverlay>();
        builder.Services.AddTransient<ProductFormOverlay>();

        builder.Services.AddTransient<InventoryViewModel>();
        builder.Services.AddTransient<InventoryPage>();

        builder.Services.AddSingleton<ISaleService, SaleService>();
        builder.Services.AddTransient<SalesViewModel>();
        builder.Services.AddTransient<SalesPage>();

        builder.Services.AddTransient<CreateSaleViewModel>();

        builder.Services.AddTransient<SaleDetailsViewModel>();


#if DEBUG
        builder.Logging.AddDebug();
        builder.Logging.SetMinimumLevel(LogLevel.Information);
#endif

        return builder.Build();
    }
}