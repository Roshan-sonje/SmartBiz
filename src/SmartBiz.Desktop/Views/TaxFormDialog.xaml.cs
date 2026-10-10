using SmartBiz.Desktop.Exceptions;
using SmartBiz.Desktop.Models.Taxes;
using SmartBiz.Desktop.Services;

namespace SmartBiz.Desktop.Views;

public partial class TaxFormDialog : ContentPage
{
    private readonly ITaxService _service;

    public TaxFormDialog(ITaxService service)
    {
        InitializeComponent();
        _service = service;
    }

    public async Task ShowForCreateAsync()
    {
        NameField.Text = string.Empty;
        RateField.Text = string.Empty;
        DescriptionField.Text = string.Empty;
        ErrorLabel.IsVisible = false;
        await Application.Current!.MainPage!.Navigation.PushModalAsync(this);
    }

    private async void OnCancelClicked(object? sender, EventArgs e)
        => await Application.Current!.MainPage!.Navigation.PopModalAsync();

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
            await Application.Current!.MainPage!.Navigation.PopModalAsync();
        }
        catch (ApiException ex) { ErrorLabel.Text = ex.Message; ErrorLabel.IsVisible = true; }
        catch (Exception) { ErrorLabel.Text = "Failed to save."; ErrorLabel.IsVisible = true; }
        finally { SaveButton.IsEnabled = true; }
    }
}