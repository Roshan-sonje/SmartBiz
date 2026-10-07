using Microsoft.Extensions.DependencyInjection;
using SmartBiz.Desktop.Navigation;
using SmartBiz.Desktop.ViewModels;

namespace SmartBiz.Desktop.Views;

public partial class ShellPage : ContentPage
{
    private readonly ShellViewModel _viewModel;

    public ShellPage(ShellViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync();
        NavigateTo(NavRoutes.Dashboard);
    }

    private void OnNavRequested(object? sender, string route)
    {
        NavigateTo(route);
    }

    private void NavigateTo(string route)
    {
        try
        {
            _viewModel.NavigateTo(route);

            View content = route switch
            {
                NavRoutes.Dashboard => new DashboardPage(),
                NavRoutes.Settings => ResolvePage<SettingsPage>(),
                _ => BuildPlaceholder(route)
            };

            ContentHost.Content = content;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("===== NAVIGATION ERROR =====");
            System.Diagnostics.Debug.WriteLine($"Route: {route}");
            System.Diagnostics.Debug.WriteLine($"Error: {ex.GetType().Name}: {ex.Message}");
            System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            System.Diagnostics.Debug.WriteLine("============================");

            // Show error in content area instead of crashing
            ContentHost.Content = new VerticalStackLayout
            {
                Padding = new Thickness(40),
                Spacing = 12,
                VerticalOptions = LayoutOptions.Center,
                Children =
                {
                    new Label { Text = "⚠️ Navigation error", FontSize = 24, TextColor = Colors.Red, HorizontalOptions = LayoutOptions.Center },
                    new Label { Text = $"{ex.GetType().Name}: {ex.Message}", FontSize = 14, TextColor = Color.FromArgb("#6B7280"), HorizontalOptions = LayoutOptions.Center },
                    new Label { Text = route, FontSize = 12, TextColor = Color.FromArgb("#9CA3AF"), HorizontalOptions = LayoutOptions.Center }
                }
            };
        }
    }

    private T ResolvePage<T>() where T : View
    {
        var services = Application.Current?.Handler?.MauiContext?.Services;
        if (services is null)
            throw new InvalidOperationException("Service provider is not available.");

        var page = services.GetService<T>();
        if (page is null)
            throw new InvalidOperationException($"DI cannot resolve {typeof(T).Name}. Did you register it in MauiProgram?");

        return page;
    }

    private static View BuildPlaceholder(string route)
    {
        return new VerticalStackLayout
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
                    Text = char.ToUpper(route[0]) + route[1..],
                    FontSize = 24,
                    FontFamily = "OpenSansSemibold",
                    TextColor = Color.FromArgb("#111827"),
                    HorizontalOptions = LayoutOptions.Center
                },
                new Label
                {
                    Text = "This page will be built in a future phase.",
                    FontSize = 14,
                    TextColor = Color.FromArgb("#6B7280"),
                    HorizontalOptions = LayoutOptions.Center
                }
            }
        };
    }
}