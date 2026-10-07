using SmartBiz.Desktop.ViewModels;

namespace SmartBiz.Desktop.Views;

public partial class UsersPage : ContentView
{
    private readonly UsersViewModel _viewModel;
    private bool _hasLoaded;

    public UsersPage(UsersViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override void OnParentSet()
    {
        base.OnParentSet();

        // Only load once per instance, and only when added to a parent
        if (Parent is not null && !_hasLoaded)
        {
            _hasLoaded = true;
            _ = _viewModel.LoadAsync();
        }
    }
}