namespace SmartBiz.Desktop.Services;

public interface IDialogService
{
    Task ShowAlertAsync(string title, string message, string cancel = "OK");
    Task<bool> ConfirmAsync(string title, string message, string accept = "Yes", string cancel = "No");
    Task ShowToastAsync(string message);
}