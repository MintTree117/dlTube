using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace dlTubeAvaloniaCrossPlatform.Views.Desktop;

public partial class AppNavViewDesktop : UserControl
{
    public AppNavViewDesktop()
    {
        InitializeComponent();
    }
    void InitializeComponent()
    {
        AvaloniaXamlLoader.Load( this );
    }
}