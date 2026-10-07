using SmartBiz.Desktop.Exceptions;
using SmartBiz.Desktop.Models.Suppliers;
using SmartBiz.Desktop.Services;

namespace SmartBiz.Desktop.Views;

public partial class SupplierFormDialog : ContentPage
{
    private readonly ISupplierService _supplierService;
    private Guid? _editingId;

    public SupplierFormDialog(ISupplierService supplierService)
    {
        InitializeComponent();
        _supplierService = supplierService;
    }

    public async Task ShowForCreateAsync()
    {
        _editingId = null;
        DialogTitle.Text = "Add Supplier";
        ClearForm();
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
        ErrorLabel.IsVisible = false;
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
            var request = new CreateSupplierRequest
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
            };

            await _supplierService.CreateSupplierAsync(request);
            await Application.Current!.MainPage!.Navigation.PopModalAsync();
        }
        catch (ApiException ex)
        {
            ErrorLabel.Text = ex.Message;
            ErrorLabel.IsVisible = true;
        }
        catch (Exception)
        {
            ErrorLabel.Text = "Failed to save. Please try again.";
            ErrorLabel.IsVisible = true;
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