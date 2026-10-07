using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SmartBiz.Desktop.Exceptions;
using SmartBiz.Desktop.Services;

namespace SmartBiz.Desktop.ViewModels;

public partial class BrandsViewModel : BaseViewModel
{
    private readonly IBrandService _service;
    private readonly IDialogService _dialog;
    private readonly ILogger<BrandsViewModel> _logger;

    public ObservableCollection<BrandRow> Brands { get; } = new();

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private bool _isEmpty;
    [ObservableProperty] private string _searchText = string.Empty;

    public BrandsViewModel(IBrandService service, IDialogService dialog, ILogger<BrandsViewModel> logger)
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
            var result = await _service.GetBrandsAsync(
                pageSize: 100,
                search: string.IsNullOrWhiteSpace(SearchText) ? null : SearchText);

            Brands.Clear();
            foreach (var b in result.Items)
            {
                Brands.Add(new BrandRow
                {
                    Id = b.Id,
                    Name = b.Name,
                    Description = b.Description ?? "—"
                });
            }
            IsEmpty = Brands.Count == 0;
        }
        catch (ApiException ex) { ErrorMessage = ex.Message; }
        catch (Exception ex) { _logger.LogError(ex, "load brands failed"); ErrorMessage = "Failed to load brands."; }
        finally { IsLoading = false; }
    }

    [RelayCommand] private async Task SearchAsync() => await LoadAsync();
    [RelayCommand] private async Task RefreshAsync() => await LoadAsync();

    [RelayCommand]
    private async Task DeleteBrandAsync(BrandRow? row)
    {
        if (row is null) return;
        var ok = await _dialog.ConfirmAsync("Delete brand", $"Delete {row.Name}?", "Delete", "Cancel");
        if (!ok) return;
        try { await _service.DeleteBrandAsync(row.Id); await LoadAsync(); }
        catch (ApiException ex) { await _dialog.ShowAlertAsync("Error", ex.Message); }
    }
}

public class BrandRow
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}