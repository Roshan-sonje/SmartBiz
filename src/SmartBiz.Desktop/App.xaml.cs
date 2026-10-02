using Microsoft.Maui.Controls;

namespace SmartBiz.Desktop
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new Views.MainPage();
        }
    }
}
