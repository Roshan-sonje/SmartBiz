using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SmartBiz.Desktop.Exceptions;
using SmartBiz.Desktop.Services;

namespace SmartBiz.Desktop.ViewModels;

public partial class TaxesViewModel : BaseViewModel
{
    private readonly ITaxService _service;
    private readonly IDialogService _dialog;
    private readonly ILogger<TaxesViewModel> _logger;

    public ObservableCollection<TaxRow> Taxes { get; } = new();

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private bool _isEmpty;
    [ObservableProperty] private string _searchText = string.Empty;

    public TaxesViewModel(ITaxService service, IDialogService dialog, ILogger<TaxesViewModel> logger)
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
            var result = await _service.GetTaxesAsync(
                pageSize: 100,
                search: string.IsNullOrWhiteSpace(SearchText) ? null : SearchText);

            Taxes.Clear();
            foreach (var t in result.Items)
            {
                Taxes.Add(new TaxRow
                {
                    Id = t.Id,
                    Name = t.Name,
                    RateText = $"{t.Rate:0.##}%",
                    Description = t.Description ?? "—"
                });
            }
            IsEmpty = Taxes.Count == 0;
        }
        catch (ApiException ex) { ErrorMessage = ex.Message; }
        catch (Exception ex) { _logger.LogError(ex, "load taxes failed"); ErrorMessage = "Failed to load taxes."; }
        finally { IsLoading = false; }
    }

    [RelayCommand] private async Task SearchAsync() => await LoadAsync();
    [RelayCommand] private async Task RefreshAsync() => await LoadAsync();

    [RelayCommand]
    private async Task DeleteTaxAsync(TaxRow? row)
    {
        if (row is null) return;
        var ok = await _dialog.ConfirmAsync("Delete tax", $"Delete {row.Name}?", "Delete", "Cancel");
        if (!ok) return;
        try { await _service.DeleteTaxAsync(row.Id); await LoadAsync(); }
        catch (ApiException ex) { await _dialog.ShowAlertAsync("Error", ex.Message); }
    }
}

public class TaxRow
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string RateText { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}