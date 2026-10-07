using SmartBiz.Desktop.Resources.Styles;

namespace SmartBiz.Desktop;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Register all design tokens in C# (works around MAUI Windows
        // MergedDictionaries crash bug)
        AppColors.Register(Resources);

        MainPage = new ContentPage
        {
            BackgroundColor = (Color)Resources["BrandPrimary"],
            Content = new Label
            {
                Text = "Hello from SmartBiz!",
                FontSize = 32,
                TextColor = Colors.White,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            }
        };
    }
}