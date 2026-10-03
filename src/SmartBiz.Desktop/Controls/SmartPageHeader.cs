namespace SmartBiz.Desktop.Controls;

[ContentProperty(nameof(Actions))]
public class SmartPageHeader : ContentView
{
    private readonly Label _titleLabel;
    private readonly Label _subtitleLabel;
    private readonly HorizontalStackLayout _actionsLayout;

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(SmartPageHeader), string.Empty,
            propertyChanged: (b, o, n) => ((SmartPageHeader)b)._titleLabel.Text = n as string ?? string.Empty);

    public static readonly BindableProperty SubtitleProperty =
        BindableProperty.Create(nameof(Subtitle), typeof(string), typeof(SmartPageHeader), null,
            propertyChanged: OnSubtitleChanged);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string? Subtitle
    {
        get => (string?)GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    public IList<IView> Actions => _actionsLayout.Children;

    public SmartPageHeader()
    {
        _titleLabel = new Label
        {
            FontFamily = "OpenSansSemibold",
            FontSize = 28,
            TextColor = Color.FromArgb("#111827")
        };

        _subtitleLabel = new Label
        {
            FontSize = 14,
            TextColor = Color.FromArgb("#6B7280"),
            IsVisible = false,
            Margin = new Thickness(0, 4, 0, 0)
        };

        _actionsLayout = new HorizontalStackLayout
        {
            Spacing = 8,
            VerticalOptions = LayoutOptions.Center
        };

        var textStack = new VerticalStackLayout
        {
            Spacing = 0,
            VerticalOptions = LayoutOptions.Center,
            Children = { _titleLabel, _subtitleLabel }
        };

        var grid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto)
            }
        };
        Grid.SetColumn(textStack, 0);
        Grid.SetColumn(_actionsLayout, 1);
        grid.Children.Add(textStack);
        grid.Children.Add(_actionsLayout);

        Padding = new Thickness(0, 0, 0, 20);
        Content = grid;
    }

    private static void OnSubtitleChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not SmartPageHeader ctl) return;

        var subtitle = newValue as string;
        var hasSubtitle = !string.IsNullOrWhiteSpace(subtitle);
        ctl._subtitleLabel.Text = subtitle ?? string.Empty;
        ctl._subtitleLabel.IsVisible = hasSubtitle;
    }
}