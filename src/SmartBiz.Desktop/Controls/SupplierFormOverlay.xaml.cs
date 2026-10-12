using SmartBiz.Desktop.Exceptions;
using SmartBiz.Desktop.Models.Suppliers;
using SmartBiz.Desktop.Services;

namespace SmartBiz.Desktop.Controls;

public partial class SupplierFormOverlay : ContentView
{
    private readonly ISupplierService _service;

    public event EventHandler? Saved;

    public SupplierFormOverlay() : this(
    Application.Current?.Handler?.MauiContext?.Services.GetRequiredService<ISupplierService>()
        ?? throw new InvalidOperationException("Service provider not available"))
    {
    }

    public SupplierFormOverlay(ISupplierService service)
    {
        InitializeComponent();
        _service = service;
    }

    public void ShowForCreate()
    {
        ClearForm();
        IsVisible = true;
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
            await _service.CreateSupplierAsync(new CreateSupplierRequest
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
            });

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