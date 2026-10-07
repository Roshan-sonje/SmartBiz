using SmartBiz.Desktop.Exceptions;
using SmartBiz.Desktop.Models.Categories;
using SmartBiz.Desktop.Services;

namespace SmartBiz.Desktop.Views;

public partial class CategoryFormDialog : ContentPage
{
    private readonly ICategoryService _service;

    public CategoryFormDialog(ICategoryService service)
    {
        InitializeComponent();
        _service = service;
    }

    public async Task ShowForCreateAsync()
    {
        DialogTitle.Text = "Add Category";
        NameField.Text = string.Empty;
        DescriptionField.Text = string.Empty;
        ErrorLabel.IsVisible = false;
        await Application.Current!.MainPage!.Navigation.PushModalAsync(this);
    }

    private async void OnCancelClicked(object? sender, EventArgs e)
    {
        await Application.Current!.MainPage!.Navigation.PopModalAsync();
    }

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
            await Application.Current!.MainPage!.Navigation.PopModalAsync();
        }
        catch (ApiException ex)
        {
            ErrorLabel.Text = ex.Message;
            ErrorLabel.IsVisible = true;
        }
        catch (Exception)
        {
            ErrorLabel.Text = "Failed to save.";
            ErrorLabel.IsVisible = true;
        }
        finally
        {
            SaveButton.IsEnabled = true;
        }
    }
}