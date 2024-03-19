using ReactiveUI;

namespace dlTubeAvalonia.ViewModels;

public sealed class MainWindowViewModel : ReactiveObject
{
    const string DefaultMenuAccountText = "Login";
    
    bool _showLogin;
    bool _isAuthenticated;
    bool _isPopoutMenuOpen;
    string _menuAccountName;
    
    public MainWindowViewModel()
    {
        ShowLogin = true;
        _menuAccountName = DefaultMenuAccountText;
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
    public bool IsPopoutMenuOpen
    {
        get => _isPopoutMenuOpen;
        set => this.RaiseAndSetIfChanged( ref _isPopoutMenuOpen, value );
    }
    public string MenuAccountName
    {
        get => _menuAccountName;
        set => this.RaiseAndSetIfChanged( ref _menuAccountName, value );
    }
}