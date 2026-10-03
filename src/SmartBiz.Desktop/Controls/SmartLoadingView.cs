namespace SmartBiz.Desktop.Controls;

public class SmartLoadingView : ContentView
{
    private readonly Label _messageLabel;

    public static readonly BindableProperty MessageProperty =
        BindableProperty.Create(nameof(Message), typeof(string), typeof(SmartLoadingView),
            "Loading...",
            propertyChanged: (b, o, n) => ((SmartLoadingView)b)._messageLabel.Text = n as string ?? string.Empty);

    public string Message
    {
        get => (string)GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    public SmartLoadingView()
    {
        var spinner = new ActivityIndicator
        {
            IsRunning = true,
            Color = Color.FromArgb("#1E3A8A"),
            WidthRequest = 32,
            HeightRequest = 32,
            HorizontalOptions = LayoutOptions.Center
        };

        _messageLabel = new Label
        {
            FontSize = 13,
            TextColor = Color.FromArgb("#6B7280"),
            HorizontalOptions = LayoutOptions.Center,
            HorizontalTextAlignment = TextAlignment.Center
        };

        Content = new VerticalStackLayout
        {
            Spacing = 12,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Children = { spinner, _messageLabel }
        };
    }
}