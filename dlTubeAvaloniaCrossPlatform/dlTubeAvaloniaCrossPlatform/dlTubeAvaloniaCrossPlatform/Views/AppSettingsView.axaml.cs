using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using dlTubeAvaloniaCrossPlatform.ViewModels;

namespace dlTubeAvaloniaCrossPlatform.Views;

public sealed partial class AppSettingsView : UserControl
{
    public AppSettingsView()
    {
        InitializeComponent();
        this.DataContext = new AppSettingsViewModel();
    }
    void InitializeComponent()
    {
        AvaloniaXamlLoader.Load( this );
    }
}