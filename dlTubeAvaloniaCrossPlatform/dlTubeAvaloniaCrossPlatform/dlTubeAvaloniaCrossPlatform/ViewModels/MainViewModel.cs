using ReactiveUI;

namespace dlTubeAvaloniaCrossPlatform.ViewModels;

public class MainViewModel : ReactiveObject
{
    const string DefaultMenuAccountText = "Login";

    bool _showLogin;
    bool _isAuthenticated;
    bool _showMobileMenu;

    string _MenuAccountName;

    public MainViewModel()
    {
        ShowLogin = true;
        _MenuAccountName = DefaultMenuAccountText;
    }

    public bool ShowLogin
    {
        get => _showLogin;
        set => this.RaiseAndSetIfChanged( ref _showLogin, value );
    }
    public bool IsAuthenticated
    {
        get => _isAuthenticated;
        set => this.RaiseAndSetIfChanged( ref _isAuthenticated, value );
    }
    public bool ShowMobileMenu
    {
        get => _showMobileMenu;
        set => this.RaiseAndSetIfChanged( ref _showMobileMenu, value );
    }
    public string MenuAccountName
    {
        get => _MenuAccountName;
        set => this.RaiseAndSetIfChanged( ref _MenuAccountName, value );
    }
}