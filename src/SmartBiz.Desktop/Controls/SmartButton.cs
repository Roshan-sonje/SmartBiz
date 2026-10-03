namespace SmartBiz.Desktop.Controls;

public enum SmartButtonVariant
{
    Primary,
    Secondary,
    Danger,
    Ghost
}

public class SmartButton : Button
{
    public static readonly BindableProperty VariantProperty =
        BindableProperty.Create(
            nameof(Variant),
            typeof(SmartButtonVariant),
            typeof(SmartButton),
            SmartButtonVariant.Primary,
            propertyChanged: OnVariantChanged);

    public SmartButtonVariant Variant
    {
        get => (SmartButtonVariant)GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
    }

    public SmartButton()
    {
        FontFamily = "OpenSansSemibold";
        FontSize = 14;
        CornerRadius = 6;
        HeightRequest = 40;
        Padding = new Thickness(20, 0);
        ApplyVariant(SmartButtonVariant.Primary);
    }

    private static void OnVariantChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is SmartButton button && newValue is SmartButtonVariant variant)
        {
            button.ApplyVariant(variant);
        }
    }

    private void ApplyVariant(SmartButtonVariant variant)
    {
        switch (variant)
        {
            case SmartButtonVariant.Primary:
                BackgroundColor = Color.FromArgb("#1E3A8A");
                TextColor = Colors.White;
                BorderColor = Colors.Transparent;
                BorderWidth = 0;
                break;

            case SmartButtonVariant.Secondary:
                BackgroundColor = Colors.White;
                TextColor = Color.FromArgb("#111827");
                BorderColor = Color.FromArgb("#D1D5DB");
                BorderWidth = 1;
                break;

            case SmartButtonVariant.Danger:
                BackgroundColor = Color.FromArgb("#DC2626");
                TextColor = Colors.White;
                BorderColor = Colors.Transparent;
                BorderWidth = 0;
                break;

            case SmartButtonVariant.Ghost:
                BackgroundColor = Colors.Transparent;
                TextColor = Color.FromArgb("#1E3A8A");
                BorderColor = Colors.Transparent;
                BorderWidth = 0;
                break;
        }
    }
}