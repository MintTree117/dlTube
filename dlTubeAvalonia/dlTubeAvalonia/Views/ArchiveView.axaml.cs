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
        _viewModel = new ArchiveViewModel();
        this.DataContext = _viewModel;
        InitializeComponent();
    }
    
    void InitializeComponent()
    {
        AvaloniaXamlLoader.Load( this );
    }
    void OnClickDownload( object? sender, RoutedEventArgs args )
    {
        if ( sender is Button { Tag: string parameter } )
        {
            _viewModel.DownloadCommand.Execute( parameter );
        }
    }
}