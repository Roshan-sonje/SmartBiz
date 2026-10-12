using SmartBiz.Desktop.Exceptions;
using SmartBiz.Desktop.Models.Units;
using SmartBiz.Desktop.Services;

namespace SmartBiz.Desktop.Views.Units;

public partial class UnitFormView : ContentView
{
    private readonly IUnitService _service;

    public event EventHandler? Saved;
    public event EventHandler? Cancelled;

    public UnitFormView(IUnitService service)
    {
        InitializeComponent();
        _service = service;
    }

    public Task InitializeForCreateAsync()
    {
        NameField.Text = string.Empty;
        ShortNameField.Text = string.Empty;
        AllowDecimalSwitch.IsToggled = false;
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