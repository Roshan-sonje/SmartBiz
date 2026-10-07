using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SmartBiz.Desktop.Exceptions;
using SmartBiz.Desktop.Services;

namespace SmartBiz.Desktop.ViewModels;

public partial class CategoriesViewModel : BaseViewModel
{
    private readonly ICategoryService _service;
    private readonly IDialogService _dialog;
    private readonly ILogger<CategoriesViewModel> _logger;

    public ObservableCollection<CategoryRow> Categories { get; } = new();

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private bool _isEmpty;
    [ObservableProperty] private string _searchText = string.Empty;

    public CategoriesViewModel(ICategoryService service, IDialogService dialog, ILogger<CategoriesViewModel> logger)
    {
        _service = service;
        _dialog = dialog;
        _logger = logger;
    }

    public async Task LoadAsync()
    {
        IsLoading = true;
        ErrorMessage = null;

        try
        {
            var result = await _service.GetCategoriesAsync(
                pageSize: 100,
                search: string.IsNullOrWhiteSpace(SearchText) ? null : SearchText);

            Categories.Clear();
            foreach (var c in result.Items)
            {
                Categories.Add(new CategoryRow
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description ?? "—",
                    ParentName = string.IsNullOrWhiteSpace(c.ParentCategoryName) ? "—" : c.ParentCategoryName
                });
            }

            IsEmpty = Categories.Count == 0;
        }
        catch (ApiException ex)
        {
            _logger.LogWarning(ex, "Failed to load categories");
            ErrorMessage = ex.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error");
            ErrorMessage = "Failed to load categories.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task SearchAsync() => await LoadAsync();

    [RelayCommand]
    private async Task RefreshAsync() => await LoadAsync();

    [RelayCommand]
    private async Task DeleteCategoryAsync(CategoryRow? row)
    {
        if (row is null) return;

        var ok = await _dialog.ConfirmAsync("Delete category",
            $"Delete {row.Name}?", "Delete", "Cancel");
        if (!ok) return;

        try
        {
            await _service.DeleteCategoryAsync(row.Id);
            await LoadAsync();
        }
        catch (ApiException ex)
        {
            await _dialog.ShowAlertAsync("Error", ex.Message);
        }
    }
}

public class CategoryRow
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ParentName { get; set; } = string.Empty;
}