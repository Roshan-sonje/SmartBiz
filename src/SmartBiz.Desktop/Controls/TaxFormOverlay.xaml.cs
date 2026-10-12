using SmartBiz.Desktop.Exceptions;
using SmartBiz.Desktop.Models.Taxes;
using SmartBiz.Desktop.Services;

namespace SmartBiz.Desktop.Controls;

public partial class TaxFormOverlay : ContentView
{
    private readonly ITaxService _service;
    public event EventHandler? Saved;

    public TaxFormOverlay() : this(
    Application.Current?.Handler?.MauiContext?.Services.GetRequiredService<ITaxService>()
        ?? throw new InvalidOperationException("Service provider not available"))
    {
    }

    public TaxFormOverlay(ITaxService service)
    {
        InitializeComponent();
        _service = service;
    }

    public void ShowForCreate()
    {
        NameField.Text = string.Empty;
        RateField.Text = string.Empty;
        DescriptionField.Text = string.Empty;
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
        if (!decimal.TryParse(RateField.Text, out var rate) || rate < 0 || rate > 100)
        {
            ErrorLabel.Text = "Rate must be between 0 and 100.";
            ErrorLabel.IsVisible = true;
            return;
        }

        SaveButton.IsEnabled = false;
        try
        {
            await _service.CreateTaxAsync(new CreateTaxRequest
            {
                Name = NameField.Text.Trim(),
                Rate = rate,
                Description = string.IsNullOrWhiteSpace(DescriptionField.Text) ? null : DescriptionField.Text.Trim()
            });
            Hide();
            Saved?.Invoke(this, EventArgs.Empty);
        }
        catch (ApiException ex) { ErrorLabel.Text = ex.Message; ErrorLabel.IsVisible = true; }
        catch (Exception) { ErrorLabel.Text = "Failed to save."; ErrorLabel.IsVisible = true; }
        finally { SaveButton.IsEnabled = true; }
    }
}