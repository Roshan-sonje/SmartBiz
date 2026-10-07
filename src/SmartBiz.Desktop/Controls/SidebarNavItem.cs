namespace SmartBiz.Desktop.Controls;

public class SidebarNavItem : ContentView
{
    private readonly Label _iconLabel;
    private readonly Label _textLabel;
    private readonly Border _container;

    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(SidebarNavItem), string.Empty,
            propertyChanged: (b, o, n) => ((SidebarNavItem)b)._textLabel.Text = n as string ?? string.Empty);

    public static readonly BindableProperty IconProperty =
        BindableProperty.Create(nameof(Icon), typeof(string), typeof(SidebarNavItem), "•",
            propertyChanged: (b, o, n) => ((SidebarNavItem)b)._iconLabel.Text = n as string ?? string.Empty);

    public static readonly BindableProperty RouteProperty =
        BindableProperty.Create(nameof(Route), typeof(string), typeof(SidebarNavItem), string.Empty);

    public static readonly BindableProperty IsActiveProperty =
        BindableProperty.Create(nameof(IsActive), typeof(bool), typeof(SidebarNavItem), false,
            propertyChanged: OnIsActiveChanged);

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public string Route
    {
        get => (string)GetValue(RouteProperty);
        set => SetValue(RouteProperty, value);
    }

    public bool IsActive
    {
        get => (bool)GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    public event EventHandler<string>? NavigateRequested;

    public SidebarNavItem()
    {
        _iconLabel = new Label
        {
            FontSize = 18,
            TextColor = Color.FromArgb("#9CA3AF"),
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = 24,
            HorizontalTextAlignment = TextAlignment.Center
        };

        _textLabel = new Label
        {
            FontSize = 14,
            TextColor = Color.FromArgb("#D1D5DB"),
            VerticalOptions = LayoutOptions.Center
        };

        var grid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(new GridLength(32)),
                new ColumnDefinition(GridLength.Star)
            }
        };
        Grid.SetColumn(_iconLabel, 0);
        Grid.SetColumn(_textLabel, 1);
        grid.Children.Add(_iconLabel);
        grid.Children.Add(_textLabel);

        _container = new Border
        {
            BackgroundColor = Colors.Transparent,
            Padding = new Thickness(12, 10),
            StrokeThickness = 0,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 6 },
            Content = grid
        };

        Content = _container;

        var tap = new TapGestureRecognizer();
        tap.Tapped += OnTapped;
        GestureRecognizers.Add(tap);
    }

    private void OnTapped(object? sender, TappedEventArgs e)
    {
        if (!string.IsNullOrEmpty(Route))
            NavigateRequested?.Invoke(this, Route);
    }

    private static void OnIsActiveChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not SidebarNavItem item) return;
        item.ApplyActiveState((bool)newValue);
    }

    private void ApplyActiveState(bool isActive)
    {
        if (isActive)
        {
            _container.BackgroundColor = Color.FromArgb("#0D9488");
            _textLabel.TextColor = Colors.White;
            _textLabel.FontFamily = "OpenSansSemibold";
            _iconLabel.TextColor = Colors.White;
        }
        else
        {
            _container.BackgroundColor = Colors.Transparent;
            _textLabel.TextColor = Color.FromArgb("#D1D5DB");
            _textLabel.FontFamily = "OpenSansRegular";
            _iconLabel.TextColor = Color.FromArgb("#9CA3AF");
        }
    }
}