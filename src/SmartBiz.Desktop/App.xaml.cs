using SmartBiz.Desktop.Resources.Styles;
using SmartBiz.Desktop.Views;


namespace SmartBiz.Desktop;

public partial class App : Application
{
    private readonly IServiceProvider _services;

    public App(IServiceProvider services)
    {
        InitializeComponent();

        _services = services;
        AppColors.Register(Resources);

        // Resolve LoginPage from DI so its ViewModel is injected
        MainPage = _services.GetRequiredService<LoginPage>();

        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
        {
            System.Diagnostics.Debug.WriteLine("===== UNHANDLED =====");
            System.Diagnostics.Debug.WriteLine(e.ExceptionObject?.ToString());
            System.Diagnostics.Debug.WriteLine("=====================");
        };
    }
}