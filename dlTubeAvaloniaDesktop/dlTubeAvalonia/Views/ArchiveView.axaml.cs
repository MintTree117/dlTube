using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using dlTubeAvalonia.ViewModels;

namespace dlTubeAvalonia.Views;

public sealed partial class ArchiveView : UserControl
{
    readonly ArchiveViewModel _viewModel;
    
    public ArchiveView()
    {
        this.DataContext = new ArchiveViewModel();
        _viewModel = this.DataContext as ArchiveViewModel;
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