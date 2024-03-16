using ReactiveUI;

namespace dlTubeAvalonia.ViewModels;

public sealed class MainWindowViewModel : ReactiveObject
{
    const string DefaultMenuAccountText = "Login";
    
    bool _ShowLogin;
    bool _IsAuthenticated;

    string _MenuAccountName;
    
    public MainWindowViewModel()
    {
        ShowLogin = true;
        _MenuAccountName = DefaultMenuAccountText;
    }
    
    public bool ShowLogin
    {
        get => _ShowLogin;
        set => this.RaiseAndSetIfChanged( ref _ShowLogin, value );
    }
    public bool IsAuthenticated
    {
        get => _IsAuthenticated;
        set => this.RaiseAndSetIfChanged( ref _IsAuthenticated, value );
    }
    public string MenuAccountName
    {
        get => _MenuAccountName;
        set => this.RaiseAndSetIfChanged( ref _MenuAccountName, value );
    }
}