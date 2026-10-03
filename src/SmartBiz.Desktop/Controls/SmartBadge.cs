namespace SmartBiz.Desktop.Controls;

public enum SmartBadgeVariant
{
    Success,
    Warning,
    Danger,
    Info,
    Neutral
}

public class SmartBadge : Border
{
    private readonly Label _label;

    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(SmartBadge), string.Empty,
            propertyChanged: (b, o, n) => ((SmartBadge)b)._label.Text = n as string ?? string.Empty);

    public static readonly BindableProperty VariantProperty =
        BindableProperty.Create(nameof(Variant), typeof(SmartBadgeVariant), typeof(SmartBadge),
            SmartBadgeVariant.Neutral,
            propertyChanged: (b, o, n) => ((SmartBadge)b).ApplyVariant((SmartBadgeVariant)n));

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public SmartBadgeVariant Variant
    {
        get => (SmartBadgeVariant)GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
    }

    public SmartBadge()
    {
        _label = new Label
        {
            FontFamily = "OpenSansSemibold",
            FontSize = 11,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center
        };

        Padding = new Thickness(8, 3);
        StrokeThickness = 0;
        StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 4 };
        HorizontalOptions = LayoutOptions.Start;
        VerticalOptions = LayoutOptions.Center;
        Content = _label;

        ApplyVariant(SmartBadgeVariant.Neutral);
    }

    private void ApplyVariant(SmartBadgeVariant variant)
    {
        switch (variant)
        {
            case SmartBadgeVariant.Success:
                BackgroundColor = Color.FromArgb("#D1FAE5");
                _label.TextColor = Color.FromArgb("#059669");
                break;

            case SmartBadgeVariant.Warning:
                BackgroundColor = Color.FromArgb("#FEF3C7");
                _label.TextColor = Color.FromArgb("#D97706");
                break;

            case SmartBadgeVariant.Danger:
                BackgroundColor = Color.FromArgb("#FEE2E2");
                _label.TextColor = Color.FromArgb("#DC2626");
                break;

            case SmartBadgeVariant.Info:
                BackgroundColor = Color.FromArgb("#DBEAFE");
                _label.TextColor = Color.FromArgb("#2563EB");
                break;

            case SmartBadgeVariant.Neutral:
            default:
                BackgroundColor = Color.FromArgb("#F3F4F6");
                _label.TextColor = Color.FromArgb("#6B7280");
                break;
        }
    }
}