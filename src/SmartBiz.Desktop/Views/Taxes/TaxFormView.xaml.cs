using SmartBiz.Desktop.Exceptions;
using SmartBiz.Desktop.Models.Taxes;
using SmartBiz.Desktop.Services;

namespace SmartBiz.Desktop.Views.Taxes;

public partial class TaxFormView : ContentView
{
    private readonly ITaxService _service;

    public event EventHandler? Saved;
    public event EventHandler? Cancelled;

    public TaxFormView(ITaxService service)
    {
        InitializeComponent();
        _service = service;
    }

    public Task InitializeForCreateAsync()
    {
        NameField.Text = string.Empty;
        RateField.Text = string.Empty;
        DescriptionField.Text = string.Empty;
        ErrorLabel.IsVisible = false;
        return Task.CompletedTask;
    }

    private void OnCancelClicked(object? sender, EventArgs e)
        => Cancelled?.Invoke(this, EventArgs.Empty);

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
            ErrorLabel.Text = "Rate must be a number between 0 and 100.";
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

            Saved?.Invoke(this, EventArgs.Empty);
        }
        catch (ApiException ex)
        {
            ErrorLabel.Text = ex.Message;
            ErrorLabel.IsVisible = true;
        }
        catch (Exception ex)
        {
            ErrorLabel.Text = $"Failed to save: {ex.Message}";
            ErrorLabel.IsVisible = true;
        }
        finally
        {
            SaveButton.IsEnabled = true;
        }
    }
}