using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SmartBiz.Desktop.Exceptions;
using SmartBiz.Desktop.Services;

namespace SmartBiz.Desktop.ViewModels;

public partial class UsersViewModel : BaseViewModel
{
    private readonly IUserService _userService;
    private readonly IDialogService _dialog;
    private readonly ILogger<UsersViewModel> _logger;

    public ObservableCollection<UserRow> Users { get; } = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _hasUsers;

    [ObservableProperty]
    private bool _isEmpty;

    public UsersViewModel(
        IUserService userService,
        IDialogService dialog,
        ILogger<UsersViewModel> logger)
    {
        _userService = userService;
        _dialog = dialog;
        _logger = logger;
    }

    public async Task LoadAsync()
    {
        IsLoading = true;
        IsEmpty = false;
        ErrorMessage = null;

        try
        {
            _logger.LogInformation("UsersViewModel.LoadAsync starting");

            var users = await _userService.GetUsersAsync();

            _logger.LogInformation("API returned {Count} users", users.Count);

            Users.Clear();

            foreach (var u in users)
            {
                Users.Add(new UserRow
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Phone = u.Phone,
                    Status = u.Status,
                    RolesText = u.Roles != null && u.Roles.Count > 0
                        ? string.Join(", ", u.Roles)
                        : "—",
                    LastLoginText = u.LastLoginAt.HasValue
                        ? u.LastLoginAt.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm")
                        : "Never"
                });
            }

            HasUsers = Users.Count > 0;
            IsEmpty = Users.Count == 0;

            _logger.LogInformation("Loaded {Count} users into collection", Users.Count);
        }
        catch (ApiException ex)
        {
            _logger.LogWarning(ex, "API error loading users");
            ErrorMessage = ex.Message;
            IsEmpty = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error loading users");
            ErrorMessage = "Failed to load users. Please try again.";
            IsEmpty = true;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task DeactivateUserAsync(UserRow? row)
    {
        if (row is null) return;

        var confirmed = await _dialog.ConfirmAsync(
            "Deactivate user",
            $"Deactivate {row.FullName}?",
            "Deactivate", "Cancel");

        if (!confirmed) return;

        try
        {
            await _userService.DeactivateUserAsync(row.Id);
            await _dialog.ShowAlertAsync("Done", $"{row.FullName} deactivated.");
            await LoadAsync();
        }
        catch (ApiException ex)
        {
            await _dialog.ShowAlertAsync("Error", ex.Message);
        }
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        await LoadAsync();
    }
}

public class UserRow
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Status { get; set; } = string.Empty;
    public string RolesText { get; set; } = string.Empty;
    public string LastLoginText { get; set; } = string.Empty;
}