using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using dlTubeAvaloniaCrossPlatform.ViewModels;
using dlTubeAvaloniaCrossPlatform.Views;
using dlTubeAvaloniaCrossPlatform.Views.Mobile;
using MainViewDesktop = dlTubeAvaloniaCrossPlatform.Views.Desktop.MainViewDesktop;
using MainWindowDesktop = dlTubeAvaloniaCrossPlatform.Views.Desktop.MainWindowDesktop;

namespace dlTubeAvaloniaCrossPlatform;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load( this );
    }
    
    public override void OnFrameworkInitializationCompleted()
    {
        // Desktop vs Responsive view is determined here: entry point for Avalonia UI
        switch ( ApplicationLifetime )
        {
            case IClassicDesktopStyleApplicationLifetime desktop:
                desktop.MainWindow = new MainWindowDesktop
                {
                    DataContext = new MainViewModel()
                };
                return;
            case ISingleViewApplicationLifetime singleViewPlatform:
                singleViewPlatform.MainView = new MainViewMobile()
                {
                    DataContext = new MainViewModel()
                };
                break;
        }

        base.OnFrameworkInitializationCompleted();
    }
}