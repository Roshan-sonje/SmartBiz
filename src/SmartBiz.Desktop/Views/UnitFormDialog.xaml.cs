using SmartBiz.Desktop.Exceptions;
using SmartBiz.Desktop.Models.Units;
using SmartBiz.Desktop.Services;

namespace SmartBiz.Desktop.Views;

public partial class UnitFormDialog : ContentPage
{
    private readonly IUnitService _service;

    public UnitFormDialog(IUnitService service)
    {
        InitializeComponent();
        _service = service;
    }

    public async Task ShowForCreateAsync()
    {
        NameField.Text = string.Empty;
        ShortNameField.Text = string.Empty;
        AllowDecimalSwitch.IsToggled = false;
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
            await Application.Current!.MainPage!.Navigation.PopModalAsync();
        }
        catch (ApiException ex) { ErrorLabel.Text = ex.Message; ErrorLabel.IsVisible = true; }
        catch (Exception) { ErrorLabel.Text = "Failed to save."; ErrorLabel.IsVisible = true; }
        finally { SaveButton.IsEnabled = true; }
    }
}