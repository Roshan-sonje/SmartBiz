namespace SmartBiz.Desktop.Controls;

[ContentProperty(nameof(CardContent))]
public class SmartCard : ContentView
{
    private readonly Border _border;

    public static readonly BindableProperty CardContentProperty =
        BindableProperty.Create(nameof(CardContent), typeof(View), typeof(SmartCard), null,
            propertyChanged: (b, o, n) => ((SmartCard)b)._border.Content = n as View);

    public new static readonly BindableProperty PaddingProperty =
        BindableProperty.Create(nameof(Padding), typeof(Thickness), typeof(SmartCard), new Thickness(20),
            propertyChanged: (b, o, n) => ((SmartCard)b)._border.Padding = (Thickness)n);

    public View? CardContent
    {
        get => (View?)GetValue(CardContentProperty);
        set => SetValue(CardContentProperty, value);
    }

    public new Thickness Padding
    {
        get => (Thickness)GetValue(PaddingProperty);
        set => SetValue(PaddingProperty, value);
    }

    public SmartCard()
    {
        _border = new Border
        {
            BackgroundColor = Colors.White,
            Stroke = Color.FromArgb("#E5E7EB"),
            StrokeThickness = 1,
            Padding = new Thickness(20),
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 8 }
        };

        Content = _border;
    }
}