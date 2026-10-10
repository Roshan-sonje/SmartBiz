using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SmartBiz.Desktop.Exceptions;
using SmartBiz.Desktop.Models.Business;
using SmartBiz.Desktop.Services;
using System.Diagnostics.Metrics;
using System.Net;
using System.Xml.Linq;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace SmartBiz.Desktop.ViewModels;

public partial class BusinessProfileViewModel : BaseViewModel
{
    private readonly IBusinessService _businessService;
    private readonly IDialogService _dialog;
    private readonly ILogger<BusinessProfileViewModel> _logger;

    // Form fields
    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private string? _legalName;
    [ObservableProperty] private string? _gstin;
    [ObservableProperty] private string? _phone;
    [ObservableProperty] private string? _email;
    [ObservableProperty] private string? _address;
    [ObservableProperty] private string? _city;
    [ObservableProperty] private string? _state;
    [ObservableProperty] private string? _postalCode;
    [ObservableProperty] private string? _country;

    // Metadata
    [ObservableProperty] private string _status = string.Empty;
    [ObservableProperty] private string _currency = "INR";
    [ObservableProperty] private DateTime? _trialEndsAt;

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private bool _isSaving;

    public BusinessProfileViewModel(
        IBusinessService businessService,
        IDialogService dialog,
        ILogger<BusinessProfileViewModel> logger)
    {
        _businessService = businessService;
        _dialog = dialog;
        _logger = logger;
    }

    public async Task LoadAsync()
    {
        IsLoading = true;
        ErrorMessage = null;

        try
        {
            var profile = await _businessService.GetProfileAsync();

            Name = profile.Name;
            LegalName = profile.LegalName;
            Gstin = profile.Gstin;
            Phone = profile.Phone;
            Email = profile.Email;
            Address = profile.Address;
            City = profile.City;
            State = profile.State;
            PostalCode = profile.PostalCode;
            Country = profile.Country;
            Status = profile.Status;
            Currency = profile.Currency;
            TrialEndsAt = profile.TrialEndsAt;
        }
        catch (ApiException ex)
        {
            _logger.LogWarning(ex, "Failed to load business profile");
            ErrorMessage = ex.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error loading profile");
            ErrorMessage = "Failed to load business profile.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        ErrorMessage = null;

        if (string.IsNullOrWhiteSpace(Name))
        {
            ErrorMessage = "Business name is required.";
            return;
        }

        IsSaving = true;
        try
        {
            var request = new UpdateBusinessProfileRequest
            {
                Name = Name.Trim(),
                LegalName = NullIfEmpty(LegalName),
                Gstin = NullIfEmpty(Gstin),
                Phone = NullIfEmpty(Phone),
                Email = NullIfEmpty(Email),
                Address = NullIfEmpty(Address),
                City = NullIfEmpty(City),
                State = NullIfEmpty(State),
                PostalCode = NullIfEmpty(PostalCode),
                Country = NullIfEmpty(Country)
            };

            await _businessService.UpdateProfileAsync(request);

            await _dialog.ShowAlertAsync("Success", "Business profile updated successfully.");
        }
        catch (ApiException ex)
        {
            _logger.LogWarning(ex, "Failed to save business profile");
            ErrorMessage = ex.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error saving profile");
            ErrorMessage = "Failed to save changes. Please try again.";
        }
        finally
        {
            IsSaving = false;
        }
    }

    private static string? NullIfEmpty(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}