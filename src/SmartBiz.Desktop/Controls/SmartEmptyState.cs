namespace SmartBiz.Desktop.Controls;

[ContentProperty(nameof(ActionView))]
public class SmartEmptyState : ContentView
{
    private readonly Label _titleLabel;
    private readonly Label _descriptionLabel;
    private readonly ContentView _actionContainer;

    public static readonly BindableProperty IconProperty =
        BindableProperty.Create(nameof(Icon), typeof(string), typeof(SmartEmptyState), "📄",
            propertyChanged: OnIconChanged);

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(SmartEmptyState), "No data",
            propertyChanged: (b, o, n) => ((SmartEmptyState)b)._titleLabel.Text = n as string ?? string.Empty);

    public static readonly BindableProperty DescriptionProperty =
        BindableProperty.Create(nameof(Description), typeof(string), typeof(SmartEmptyState), null,
            propertyChanged: OnDescriptionChanged);

    public static readonly BindableProperty ActionViewProperty =
        BindableProperty.Create(nameof(ActionView), typeof(View), typeof(SmartEmptyState), null,
            propertyChanged: (b, o, n) => ((SmartEmptyState)b)._actionContainer.Content = n as View);

    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string? Description
    {
        get => (string?)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public View? ActionView
    {
        get => (View?)GetValue(ActionViewProperty);
        set => SetValue(ActionViewProperty, value);
    }

    private readonly Label _iconLabel;

    public SmartEmptyState()
    {
        _iconLabel = new Label
        {
            FontSize = 48,
            HorizontalOptions = LayoutOptions.Center
        };

        _titleLabel = new Label
        {
            FontFamily = "OpenSansSemibold",
            FontSize = 18,
            TextColor = Color.FromArgb("#111827"),
            HorizontalOptions = LayoutOptions.Center
        };

        _descriptionLabel = new Label
        {
            FontSize = 14,
            TextColor = Color.FromArgb("#6B7280"),
            HorizontalOptions = LayoutOptions.Center,
            HorizontalTextAlignment = TextAlignment.Center,
            IsVisible = false,
            MaximumWidthRequest = 360
        };

        _actionContainer = new ContentView
        {
            HorizontalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 8, 0, 0)
        };

        Content = new VerticalStackLayout
        {
            Spacing = 12,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Padding = new Thickness(40),
            Children = { _iconLabel, _titleLabel, _descriptionLabel, _actionContainer }
        };
    }

    private static void OnIconChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is SmartEmptyState ctl)
            ctl._iconLabel.Text = newValue as string ?? string.Empty;
    }

    private static void OnDescriptionChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not SmartEmptyState ctl) return;

        var desc = newValue as string;
        ctl._descriptionLabel.Text = desc ?? string.Empty;
        ctl._descriptionLabel.IsVisible = !string.IsNullOrWhiteSpace(desc);
    }
}