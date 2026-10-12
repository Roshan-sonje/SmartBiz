using SmartBiz.Desktop.Exceptions;
using SmartBiz.Desktop.Models.Customers;
using SmartBiz.Desktop.Services;
using Microsoft.Extensions.DependencyInjection;

namespace SmartBiz.Desktop.Controls;

public partial class CustomerFormOverlay : ContentView
{
    private readonly ICustomerService _service;
    private Guid? _editingId;

    public event EventHandler? Saved;

    public CustomerFormOverlay() : this(
    Application.Current?.Handler?.MauiContext?.Services.GetRequiredService<ICustomerService>()
        ?? throw new InvalidOperationException("Service provider not available"))
    {
    }

    public CustomerFormOverlay(ICustomerService service)
    {
        InitializeComponent();
        _service = service;
    }

    public void ShowForCreate()
    {
        _editingId = null;
        DialogTitle.Text = "Add Customer";
        ClearForm();
        IsVisible = true;
    }

    public async Task ShowForEditAsync(Guid customerId)
    {
        try
        {
            var c = await _service.GetCustomerAsync(customerId);
            _editingId = customerId;
            DialogTitle.Text = "Edit Customer";
            NameField.Text = c.Name;
            PhoneField.Text = c.Phone ?? string.Empty;
            EmailField.Text = c.Email ?? string.Empty;
            AddressField.Text = c.Address ?? string.Empty;
            CityField.Text = c.City ?? string.Empty;
            StateField.Text = c.State ?? string.Empty;
            PostalField.Text = c.PostalCode ?? string.Empty;
            GstinField.Text = c.Gstin ?? string.Empty;
            BalanceField.Text = c.OpeningBalance.ToString();
            CreditLimitField.Text = c.CreditLimit.ToString();
            ErrorLabel.IsVisible = false;
            IsVisible = true;
        }
        catch (Exception ex)
        {
            ErrorLabel.Text = ex.Message;
            ErrorLabel.IsVisible = true;
            IsVisible = true;
        }
    }

    public void Hide() => IsVisible = false;

    private void ClearForm()
    {
        NameField.Text = string.Empty;
        PhoneField.Text = string.Empty;
        EmailField.Text = string.Empty;
        AddressField.Text = string.Empty;
        CityField.Text = string.Empty;
        StateField.Text = string.Empty;
        PostalField.Text = string.Empty;
        GstinField.Text = string.Empty;
        BalanceField.Text = string.Empty;
        CreditLimitField.Text = string.Empty;
        ErrorLabel.IsVisible = false;
    }

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
            if (_editingId is null)
            {
                await _service.CreateCustomerAsync(new CreateCustomerRequest
                {
                    Name = NameField.Text.Trim(),
                    Phone = NullIfEmpty(PhoneField.Text),
                    Email = NullIfEmpty(EmailField.Text),
                    Address = NullIfEmpty(AddressField.Text),
                    City = NullIfEmpty(CityField.Text),
                    State = NullIfEmpty(StateField.Text),
                    PostalCode = NullIfEmpty(PostalField.Text),
                    Gstin = NullIfEmpty(GstinField.Text),
                    OpeningBalance = ParseDecimal(BalanceField.Text),
                    CreditLimit = ParseDecimal(CreditLimitField.Text),
                });
            }
            else
            {
                await _service.UpdateCustomerAsync(_editingId.Value, new UpdateCustomerRequest
                {
                    Name = NameField.Text.Trim(),
                    Phone = NullIfEmpty(PhoneField.Text),
                    Email = NullIfEmpty(EmailField.Text),
                    Address = NullIfEmpty(AddressField.Text),
                    City = NullIfEmpty(CityField.Text),
                    State = NullIfEmpty(StateField.Text),
                    PostalCode = NullIfEmpty(PostalField.Text),
                    Gstin = NullIfEmpty(GstinField.Text),
                    OpeningBalance = ParseDecimal(BalanceField.Text),
                    CreditLimit = ParseDecimal(CreditLimitField.Text),
                    IsActive = true
                });
            }

            Hide();
            Saved?.Invoke(this, EventArgs.Empty);
        }
        catch (ApiException ex) { ErrorLabel.Text = ex.Message; ErrorLabel.IsVisible = true; }
        catch (Exception) { ErrorLabel.Text = "Failed to save. Please try again."; ErrorLabel.IsVisible = true; }
        finally { SaveButton.IsEnabled = true; }
    }

    private static string? NullIfEmpty(string? v) => string.IsNullOrWhiteSpace(v) ? null : v.Trim();
    private static decimal ParseDecimal(string? v) => decimal.TryParse(v, out var r) ? r : 0m;
}