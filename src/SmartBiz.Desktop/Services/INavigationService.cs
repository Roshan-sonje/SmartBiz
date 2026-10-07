namespace SmartBiz.Desktop.Services;

public interface INavigationService
{
    Task NavigateToAsync(string route);
    Task GoBackAsync();
    Task GoToRootAsync();
}