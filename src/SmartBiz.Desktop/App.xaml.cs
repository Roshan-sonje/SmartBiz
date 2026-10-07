using SmartBiz.Desktop.Resources.Styles;

namespace SmartBiz.Desktop;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        AppColors.Register(Resources);

        MainPage = new AppShell();
    }
}