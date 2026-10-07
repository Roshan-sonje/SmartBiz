using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using SmartBiz.Desktop.Services;

namespace SmartBiz.Desktop.ViewModels;

public partial class ShellViewModel : BaseViewModel
{
    [ObservableProperty]
    private string _pageTitle = "Dashboard";

    public ShellViewModel(
        IAuthService authService,
        ITokenStore tokenStore,
        INavigationService navigation,
        IDialogService dialog,
        ILogger<ShellViewModel> logger)
    {
        System.Diagnostics.Debug.WriteLine("[ShellViewModel] Constructor called");
    }

    public Task InitializeAsync()
    {
        System.Diagnostics.Debug.WriteLine("[ShellViewModel] InitializeAsync called");
        return Task.CompletedTask;
    }
}