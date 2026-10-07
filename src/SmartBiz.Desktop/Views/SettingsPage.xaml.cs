using Microsoft.Extensions.DependencyInjection;

namespace SmartBiz.Desktop.Views;

public partial class SettingsPage : ContentView
{
    private readonly IServiceProvider _services;

    public SettingsPage(IServiceProvider services)
    {
        InitializeComponent();
        _services = services;

        Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        ShowTab("profile");
    }

    private void OnTabClicked(object? sender, TappedEventArgs e)
    {
        if (e.Parameter is string tab)
            ShowTab(tab);
    }

    private void ShowTab(string tab)
    {
        var primary = (Color)Application.Current!.Resources["BrandPrimary"];
        var secondary = (Color)Application.Current!.Resources["TextSecondary"];

        // Reset all tabs
        SetTabStyle(TabProfile, tab == "profile", primary, secondary);
        SetTabStyle(TabUsers, tab == "users", primary, secondary);
        SetTabStyle(TabInvoice, tab == "invoice", primary, secondary);

        // Swap content
        SettingsContent.Content = tab switch
        {
            "profile" => _services.GetRequiredService<BusinessProfilePage>(),
            "users" => _services.GetRequiredService<UsersPage>(),
            "invoice" => BuildPlaceholder("Invoice Settings"),
            _ => BuildPlaceholder(tab)
        };
    }

    private static void SetTabStyle(Label tab, bool isActive, Color primary, Color secondary)
    {
        tab.TextColor = isActive ? primary : secondary;
        tab.FontFamily = isActive ? "OpenSansSemibold" : "OpenSansRegular";
    }

    private static View BuildPlaceholder(string text) => new VerticalStackLayout
    {
        Padding = new Thickness(40),
        Spacing = 12,
        VerticalOptions = LayoutOptions.Center,
        HorizontalOptions = LayoutOptions.Center,
        Children =
        {
            new Label { Text = "🚧", FontSize = 48, HorizontalOptions = LayoutOptions.Center },
            new Label
            {
                Text = text,
                FontSize = 22,
                FontFamily = "OpenSansSemibold",
                HorizontalOptions = LayoutOptions.Center
            },
            new Label
            {
                Text = "Coming in a future phase.",
                FontSize = 14,
                TextColor = Color.FromArgb("#6B7280"),
                HorizontalOptions = LayoutOptions.Center
            }
        }
    };
}