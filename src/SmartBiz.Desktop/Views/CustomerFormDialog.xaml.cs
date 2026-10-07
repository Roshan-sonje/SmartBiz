using SmartBiz.Desktop.Exceptions;
using SmartBiz.Desktop.Models.Customers;
using SmartBiz.Desktop.Services;

namespace SmartBiz.Desktop.Views;

public partial class CustomerFormDialog : ContentPage
{
    private readonly ICustomerService _customerService;
    private Guid? _editingId;

    public CustomerFormDialog(ICustomerService customerService)
    {
        InitializeComponent();
        _customerService = customerService;
    }

    public async Task ShowForCreateAsync()
    {
        _editingId = null;
        DialogTitle.Text = "Add Customer";
        ClearForm();
        await ShowAsync();
    }

    public async Task ShowForEditAsync(Guid customerId)
    {
        try
        {
            var customer = await _customerService.GetCustomerAsync(customerId);
            _editingId = customerId;
            DialogTitle.Text = "Edit Customer";
            PopulateForm(customer);
            await ShowAsync();
        }
        catch (Exception ex)
        {
            await Application.Current!.MainPage!.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async Task ShowAsync()
    {
        await Application.Current!.MainPage!.Navigation.PushModalAsync(this);
    }

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

    private void PopulateForm(Customer c)
    {
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
            if (_editingId is null)
            {
                var request = new CreateCustomerRequest
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
                };

                await _customerService.CreateCustomerAsync(request);
            }
            else
            {
                var request = new UpdateCustomerRequest
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
                };

                await _customerService.UpdateCustomerAsync(_editingId.Value, request);
            }

            await Application.Current!.MainPage!.Navigation.PopModalAsync();
        }
        catch (ApiException ex)
        {
            ErrorLabel.Text = ex.Message;
            ErrorLabel.IsVisible = true;
        }
        catch (Exception ex)
        {
            ErrorLabel.Text = "Failed to save. Please try again.";
            ErrorLabel.IsVisible = true;
            System.Diagnostics.Debug.WriteLine($"Save error: {ex}");
        }
        finally
        {
            SaveButton.IsEnabled = true;
        }
    }

    private static string? NullIfEmpty(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static decimal ParseDecimal(string? value)
        => decimal.TryParse(value, out var result) ? result : 0m;
}