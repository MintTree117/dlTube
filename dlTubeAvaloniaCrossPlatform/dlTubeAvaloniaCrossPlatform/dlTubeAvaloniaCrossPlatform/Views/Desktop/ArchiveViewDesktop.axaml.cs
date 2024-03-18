using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using dlTubeAvaloniaCrossPlatform.ViewModels;

namespace dlTubeAvaloniaCrossPlatform.Views.Desktop;

public sealed partial class ArchiveViewDesktop : UserControl
{
    readonly ArchiveViewModel _viewModel;
    
    public ArchiveViewDesktop()
    {
        this.DataContext = new ArchiveViewModel();
        _viewModel = (this.DataContext as ArchiveViewModel)!;
        InitializeComponent();
    }
    void InitializeComponent()
    {
        AvaloniaXamlLoader.Load( this );
    }
    void ToggleMenu( object sender, RoutedEventArgs e )
    {
        _viewModel.IsMenuOpen = !_viewModel.IsMenuOpen;
    }
}