using SmartBiz.Desktop.Views;

namespace SmartBiz.Desktop;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register explicit routes for navigation
        Routing.RegisterRoute("register", typeof(RegisterPage));
        Routing.RegisterRoute("dashboard", typeof(DashboardPage));
    }
}