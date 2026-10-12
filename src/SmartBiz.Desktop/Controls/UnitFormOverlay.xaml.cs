using SmartBiz.Desktop.Exceptions;
using SmartBiz.Desktop.Models.Units;
using SmartBiz.Desktop.Services;

namespace SmartBiz.Desktop.Controls;

public partial class UnitFormOverlay : ContentView
{
    private readonly IUnitService _service;
    public event EventHandler? Saved;

    public UnitFormOverlay() : this(
    Application.Current?.Handler?.MauiContext?.Services.GetRequiredService<IUnitService>()
        ?? throw new InvalidOperationException("Service provider not available"))
    {
    }

    public UnitFormOverlay(IUnitService service)
    {
        InitializeComponent();
        _service = service;
    }

    public void ShowForCreate()
    {
        NameField.Text = string.Empty;
        ShortNameField.Text = string.Empty;
        AllowDecimalSwitch.IsToggled = false;
        ErrorLabel.IsVisible = false;
        IsVisible = true;
    }

    public void Hide() => IsVisible = false;

    private void OnCancelClicked(object? sender, EventArgs e) => Hide();

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;

        if (string.IsNullOrWhiteSpace(NameField.Text))
        {
            ErrorLabel.Text = "Name is required.";
            ErrorLabel.IsVisible = true;
            return;
        }
        if (string.IsNullOrWhiteSpace(ShortNameField.Text))
        {
            ErrorLabel.Text = "Short name is required.";
            ErrorLabel.IsVisible = true;
            return;
        }

        SaveButton.IsEnabled = false;
        try
        {
            await _service.CreateUnitAsync(new CreateUnitRequest
            {
                Name = NameField.Text.Trim(),
                ShortName = ShortNameField.Text.Trim(),
                AllowDecimal = AllowDecimalSwitch.IsToggled
            });
            Hide();
            Saved?.Invoke(this, EventArgs.Empty);
        }
        catch (ApiException ex) { ErrorLabel.Text = ex.Message; ErrorLabel.IsVisible = true; }
        catch (Exception) { ErrorLabel.Text = "Failed to save."; ErrorLabel.IsVisible = true; }
        finally { SaveButton.IsEnabled = true; }
    }
}