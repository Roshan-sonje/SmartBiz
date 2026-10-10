using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SmartBiz.Desktop.Exceptions;
using SmartBiz.Desktop.Services;

namespace SmartBiz.Desktop.ViewModels;

public partial class UnitsViewModel : BaseViewModel
{
    private readonly IUnitService _service;
    private readonly IDialogService _dialog;
    private readonly ILogger<UnitsViewModel> _logger;

    public ObservableCollection<UnitRow> Units { get; } = new();

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private bool _isEmpty;
    [ObservableProperty] private string _searchText = string.Empty;

    public UnitsViewModel(IUnitService service, IDialogService dialog, ILogger<UnitsViewModel> logger)
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
            var result = await _service.GetUnitsAsync(
                pageSize: 100,
                search: string.IsNullOrWhiteSpace(SearchText) ? null : SearchText);

            Units.Clear();
            foreach (var u in result.Items)
            {
                Units.Add(new UnitRow
                {
                    Id = u.Id,
                    Name = u.Name,
                    ShortName = u.ShortName,
                    AllowDecimal = u.AllowDecimal ? "Yes" : "No"
                });
            }
            IsEmpty = Units.Count == 0;
        }
        catch (ApiException ex) { ErrorMessage = ex.Message; }
        catch (Exception ex) { _logger.LogError(ex, "load units failed"); ErrorMessage = "Failed to load units."; }
        finally { IsLoading = false; }
    }

    [RelayCommand] private async Task SearchAsync() => await LoadAsync();
    [RelayCommand] private async Task RefreshAsync() => await LoadAsync();

    [RelayCommand]
    private async Task DeleteUnitAsync(UnitRow? row)
    {
        if (row is null) return;
        var ok = await _dialog.ConfirmAsync("Delete unit", $"Delete {row.Name}?", "Delete", "Cancel");
        if (!ok) return;
        try { await _service.DeleteUnitAsync(row.Id); await LoadAsync(); }
        catch (ApiException ex) { await _dialog.ShowAlertAsync("Error", ex.Message); }
    }
}

public class UnitRow
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public string AllowDecimal { get; set; } = string.Empty;
}