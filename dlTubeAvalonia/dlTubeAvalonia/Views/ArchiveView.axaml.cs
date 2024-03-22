using Microsoft.Extensions.DependencyInjection;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using dlTubeAvalonia.Services;
using dlTubeAvalonia.ViewModels;

namespace dlTubeAvalonia.Views;

public sealed partial class ArchiveView : UserControl
{
    readonly FileLogger _logger = Program.ServiceProvider.GetService<FileLogger>()!;
    readonly ArchiveViewModel _viewModel;
    
    public ArchiveView()
    {
        _viewModel = new ArchiveViewModel();
        DataContext = _viewModel;
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
        else
        {
            _logger.LogWithConsole( $"OnClickDownload called by inappropriate object : {sender}" );
        }
    }
}