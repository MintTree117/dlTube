namespace dlTubeAvalonia.ViewModels;

public sealed class MainWindowViewModel : BaseViewModel
{
    public MainWindowViewModel() : base( TryGetLogger<MainWindowViewModel>() )
    {
        IsFree = true;
    }
}