using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using dlTubeAvalonia.ViewModels;

namespace dlTubeAvalonia.Views;

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