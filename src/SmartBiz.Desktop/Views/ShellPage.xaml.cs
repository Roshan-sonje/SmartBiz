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
        _viewModel.NavigateTo(route);

        View content = route switch
        {
            NavRoutes.Dashboard => new DashboardPage(),
            _ => BuildPlaceholder(route)
        };

        ContentHost.Content = content;
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
                new Label
                {
                    Text = "🚧",
                    FontSize = 48,
                    HorizontalOptions = LayoutOptions.Center
                },
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