using SmartBiz.Desktop.Exceptions;
using SmartBiz.Desktop.Models.Brands;
using SmartBiz.Desktop.Services;

namespace SmartBiz.Desktop.Views.Brands;

public partial class BrandFormView : ContentView
{
    private readonly IBrandService _service;

    public event EventHandler? Saved;
    public event EventHandler? Cancelled;

    public BrandFormView(IBrandService service)
    {
        InitializeComponent();
        _service = service;
    }

    public Task InitializeForCreateAsync()
    {
        NameField.Text = string.Empty;
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

        SaveButton.IsEnabled = false;
        try
        {
            await _service.CreateBrandAsync(new CreateBrandRequest
            {
                Name = NameField.Text.Trim(),
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