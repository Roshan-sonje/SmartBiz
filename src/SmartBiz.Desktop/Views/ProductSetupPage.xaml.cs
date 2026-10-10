using Microsoft.Extensions.DependencyInjection;

namespace SmartBiz.Desktop.Views;

public partial class ProductSetupPage : ContentView
{
    private readonly IServiceProvider _services;

    public ProductSetupPage(IServiceProvider services)
    {
        InitializeComponent();
        _services = services;
        Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, EventArgs e) => ShowTab("categories");

    private void OnTabClicked(object? sender, TappedEventArgs e)
    {
        if (e.Parameter is string tab) ShowTab(tab);
    }

    private void ShowTab(string tab)
    {
        var primary = (Color)Application.Current!.Resources["BrandPrimary"];
        var secondary = (Color)Application.Current!.Resources["TextSecondary"];

        SetTabStyle(TabCategories, tab == "categories", primary, secondary);
        SetTabStyle(TabBrands, tab == "brands", primary, secondary);
        SetTabStyle(TabUnits, tab == "units", primary, secondary);
        SetTabStyle(TabTaxes, tab == "taxes", primary, secondary);

        SetupContent.Content = tab switch
        {
            "categories" => _services.GetRequiredService<CategoriesPage>(),
            "brands" => _services.GetRequiredService<BrandsPage>(),
            "units" => _services.GetRequiredService<UnitsPage>(),
            "taxes" => _services.GetRequiredService<TaxesPage>(),
            _ => BuildPlaceholder(tab)
        };
    }

    private static void SetTabStyle(Label tab, bool active, Color primary, Color secondary)
    {
        tab.TextColor = active ? primary : secondary;
        tab.FontFamily = active ? "OpenSansSemibold" : "OpenSansRegular";
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
            new Label { Text = text, FontSize = 22, FontFamily = "OpenSansSemibold", HorizontalOptions = LayoutOptions.Center },
            new Label { Text = "Coming in Phase 7C.", FontSize = 14, TextColor = Color.FromArgb("#6B7280"), HorizontalOptions = LayoutOptions.Center }
        }
    };
}