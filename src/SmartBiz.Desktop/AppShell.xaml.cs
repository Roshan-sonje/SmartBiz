using SmartBiz.Desktop.Views;

namespace SmartBiz.Desktop;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register non-Shell routes (for navigation without ShellContent)
        Routing.RegisterRoute("register", typeof(RegisterPage));
    }
}