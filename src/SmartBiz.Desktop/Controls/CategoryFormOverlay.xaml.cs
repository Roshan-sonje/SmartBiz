using SmartBiz.Desktop.Exceptions;
using SmartBiz.Desktop.Models.Categories;
using SmartBiz.Desktop.Services;

namespace SmartBiz.Desktop.Controls;

public partial class CategoryFormOverlay : ContentView
{
    private readonly ICategoryService _service;
    public event EventHandler? Saved;

    public CategoryFormOverlay() : this(
    Application.Current?.Handler?.MauiContext?.Services.GetRequiredService<ICategoryService>()
        ?? throw new InvalidOperationException("Service provider not available"))
    {
    }

    public CategoryFormOverlay(ICategoryService service)
    {
        InitializeComponent();
        _service = service;
    }

    public void ShowForCreate()
    {
        NameField.Text = string.Empty;
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

        SaveButton.IsEnabled = false;
        try
        {
            await _service.CreateCategoryAsync(new CreateCategoryRequest
            {
                Name = NameField.Text.Trim(),
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