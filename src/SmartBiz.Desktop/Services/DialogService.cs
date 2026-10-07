using Microsoft.Extensions.Logging;

namespace SmartBiz.Desktop.Services;

public class DialogService : IDialogService
{
    private readonly ILogger<DialogService> _logger;

    public DialogService(ILogger<DialogService> logger)
    {
        _logger = logger;
    }

    public async Task ShowAlertAsync(string title, string message, string cancel = "OK")
    {
        if (Application.Current?.MainPage is Page page)
            await page.DisplayAlert(title, message, cancel);
    }

    public async Task<bool> ConfirmAsync(string title, string message, string accept = "Yes", string cancel = "No")
    {
        if (Application.Current?.MainPage is Page page)
            return await page.DisplayAlert(title, message, accept, cancel);
        return false;
    }

    public async Task ShowToastAsync(string message)
    {
        _logger.LogInformation("Toast: {Message}", message);
        if (Application.Current?.MainPage is Page page)
            await page.DisplayAlert("Notice", message, "OK");
    }
}