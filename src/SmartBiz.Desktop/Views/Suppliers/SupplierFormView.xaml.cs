using SmartBiz.Desktop.Exceptions;
using SmartBiz.Desktop.Models.Suppliers;
using SmartBiz.Desktop.Services;

namespace SmartBiz.Desktop.Views.Suppliers;

public partial class SupplierFormView : ContentView
{
	private readonly ISupplierService _supplierService;
	private Guid? _editingId;

	public event EventHandler? Saved;
	public event EventHandler? Cancelled;

	public SupplierFormView(ISupplierService supplierService)
	{
		InitializeComponent();
		_supplierService = supplierService;
	}

	public async Task InitializeForCreateAsync()
	{
		_editingId = null;
		ClearForm();
		await Task.CompletedTask;
	}

	public async Task InitializeForEditAsync(Guid supplierId)
	{
		try
		{
			var s = await _supplierService.GetSupplierAsync(supplierId);
			_editingId = supplierId;

			NameField.Text = s.Name;
			PhoneField.Text = s.Phone ?? string.Empty;
			EmailField.Text = s.Email ?? string.Empty;
			AddressField.Text = s.Address ?? string.Empty;
			CityField.Text = s.City ?? string.Empty;
			StateField.Text = s.State ?? string.Empty;
			PostalField.Text = s.PostalCode ?? string.Empty;
			GstinField.Text = s.Gstin ?? string.Empty;
			BalanceField.Text = s.OpeningBalance.ToString();

			ErrorLabel.IsVisible = false;
		}
		catch (Exception ex)
		{
			ErrorLabel.Text = ex.Message;
			ErrorLabel.IsVisible = true;
		}
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

	private void OnCancelClicked(object? sender, EventArgs e)
		=> Cancelled?.Invoke(this, EventArgs.Empty);

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
				await _supplierService.CreateSupplierAsync(new CreateSupplierRequest
				{
					Name = NameField.Text.Trim(),
					Phone = NullIfEmpty(PhoneField.Text),
					Email = NullIfEmpty(EmailField.Text),
					Address = NullIfEmpty(AddressField.Text),
					City = NullIfEmpty(CityField.Text),
					State = NullIfEmpty(StateField.Text),
					PostalCode = NullIfEmpty(PostalField.Text),
					Gstin = NullIfEmpty(GstinField.Text),
					OpeningBalance = ParseDecimal(BalanceField.Text)
				});
			}
			else
			{
				await _supplierService.UpdateSupplierAsync(_editingId.Value, new UpdateSupplierRequest
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
					IsActive = true
				});
			}

			Saved?.Invoke(this, EventArgs.Empty);
		}
		catch (ApiException ex)
		{
			ErrorLabel.Text = ex.Message;
			ErrorLabel.IsVisible = true;
		}
		catch (Exception ex)
		{
			ErrorLabel.Text = $"Failed to save: {ex.Message}";
			ErrorLabel.IsVisible = true;
		}
		finally
		{
			SaveButton.IsEnabled = true;
		}
	}

	private static string? NullIfEmpty(string? v) => string.IsNullOrWhiteSpace(v) ? null : v.Trim();
	private static decimal ParseDecimal(string? v) => decimal.TryParse(v, out var r) ? r : 0m;
}