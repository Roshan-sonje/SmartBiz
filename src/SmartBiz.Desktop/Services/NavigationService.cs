using Microsoft.Extensions.Logging;

namespace SmartBiz.Desktop.Services;

public class NavigationService : INavigationService
{
    private readonly ILogger<NavigationService> _logger;

    public NavigationService(ILogger<NavigationService> logger)
    {
        _logger = logger;
    }

    public async Task NavigateToAsync(string route)
    {
        _logger.LogInformation("Navigating to {Route}", route);
        var shell = Shell.Current;
        if (shell is not null && !string.IsNullOrEmpty(route))
            await shell.GoToAsync(route);
    }

    public async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    public async Task GoToRootAsync()
    {
        await Shell.Current.GoToAsync("//");
    }
}