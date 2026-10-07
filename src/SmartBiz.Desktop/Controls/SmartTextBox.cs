namespace SmartBiz.Desktop.Controls;

public class SmartTextBox : ContentView
{
    private readonly Label _label;
    private readonly Entry _entry;
    private readonly Label _errorLabel;
    private readonly Border _border;

    public static readonly BindableProperty LabelProperty =
        BindableProperty.Create(nameof(Label), typeof(string), typeof(SmartTextBox), string.Empty,
            propertyChanged: (b, o, n) => ((SmartTextBox)b)._label.Text = n as string ?? string.Empty);

    public static readonly BindableProperty PlaceholderProperty =
        BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(SmartTextBox), string.Empty,
            propertyChanged: (b, o, n) => ((SmartTextBox)b)._entry.Placeholder = n as string ?? string.Empty);

    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(SmartTextBox), string.Empty,
            BindingMode.TwoWay,
            propertyChanged: (b, o, n) =>
            {
                var ctl = (SmartTextBox)b;
                if (ctl._entry.Text != (n as string)) ctl._entry.Text = n as string ?? string.Empty;
            });

    public static readonly BindableProperty IsPasswordProperty =
        BindableProperty.Create(nameof(IsPassword), typeof(bool), typeof(SmartTextBox), false,
            propertyChanged: (b, o, n) => ((SmartTextBox)b)._entry.IsPassword = (bool)n);

    public static readonly BindableProperty KeyboardProperty =
        BindableProperty.Create(nameof(Keyboard), typeof(Keyboard), typeof(SmartTextBox), Keyboard.Default,
            propertyChanged: (b, o, n) => ((SmartTextBox)b)._entry.Keyboard = (Keyboard)n);

    public static readonly BindableProperty ErrorMessageProperty =
        BindableProperty.Create(nameof(ErrorMessage), typeof(string), typeof(SmartTextBox), null,
            propertyChanged: OnErrorChanged);

    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public bool IsPassword
    {
        get => (bool)GetValue(IsPasswordProperty);
        set => SetValue(IsPasswordProperty, value);
    }

    public Keyboard Keyboard
    {
        get => (Keyboard)GetValue(KeyboardProperty);
        set => SetValue(KeyboardProperty, value);
    }

    public string? ErrorMessage
    {
        get => (string?)GetValue(ErrorMessageProperty);
        set => SetValue(ErrorMessageProperty, value);
    }

    public SmartTextBox()
    {
        _label = new Label
        {
            FontFamily = "OpenSansSemibold",
            FontSize = 12,
            TextColor = Color.FromArgb("#6B7280"),
            Margin = new Thickness(0, 0, 0, 6)
        };

        _entry = new Entry
        {
            FontSize = 14,
            TextColor = Color.FromArgb("#111827"),
            PlaceholderColor = Color.FromArgb("#9CA3AF"),
            BackgroundColor = Colors.Transparent,
            VerticalOptions = LayoutOptions.Center
        };

        _entry.TextChanged += (_, e) =>
        {
            if (Text != e.NewTextValue)
                Text = e.NewTextValue;
        };

        _border = new Border
        {
            BackgroundColor = Colors.White,
            Stroke = Color.FromArgb("#D1D5DB"),
            StrokeThickness = 1,
            Padding = new Thickness(12, 0),
            HeightRequest = 40,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 6 },
            Content = _entry
        };

        _errorLabel = new Label
        {
            FontSize = 11,
            TextColor = Color.FromArgb("#DC2626"),
            Margin = new Thickness(0, 4, 0, 0),
            IsVisible = false
        };

        Content = new VerticalStackLayout
        {
            Spacing = 0,
            Children = { _label, _border, _errorLabel }
        };
    }

    private static void OnErrorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not SmartTextBox ctl) return;

        var msg = newValue as string;
        var hasError = !string.IsNullOrWhiteSpace(msg);

        ctl._errorLabel.Text = msg ?? string.Empty;
        ctl._errorLabel.IsVisible = hasError;
        ctl._border.Stroke = hasError
            ? Color.FromArgb("#DC2626")
            : Color.FromArgb("#D1D5DB");
    }
}