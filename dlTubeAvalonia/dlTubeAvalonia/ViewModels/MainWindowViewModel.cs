using ReactiveUI;

namespace dlTubeAvalonia.ViewModels;

public sealed class MainWindowViewModel : ReactiveObject
{
#pragma warning disable CA1822 // Mark members as static
    public string Greeting => "Welcome to Avalonia!";
#pragma warning restore CA1822 // Mark members as static
}