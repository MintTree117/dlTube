using Avalonia.Controls;
using Avalonia.Interactivity;
using dlTubeAvalonia.ViewModels;

namespace dlTubeAvalonia.Views;

public sealed partial class MainWindow : Window
{
    readonly MainWindowViewModel _viewModel;
    readonly DownloadView _downloadView;
    YoutubeView? _youtubeView;
    ArchiveView? _archiveView;
    
    public MainWindow()
    {
        this.DataContext = new MainWindowViewModel();
        InitializeComponent();
        _viewModel = (this.DataContext as MainWindowViewModel)!;

        _downloadView = new DownloadView();
        MainContent.Content = _downloadView;
    }
    
    void OnClickViewYoutubeDownloader( object? sender, RoutedEventArgs args )
    {
        MainContent.Content = _downloadView;
        OnNewPage();
    }
    void OnClickViewYoutubeSearch( object? sender, RoutedEventArgs args )
    {
        _youtubeView ??= new YoutubeView();
        MainContent.Content = _youtubeView;
        OnNewPage();
    }
    void OnClickViewArchive( object? sender, RoutedEventArgs args )
    {
        _archiveView ??= new ArchiveView();
        MainContent.Content = _archiveView;
        OnNewPage();
    }
    void OnClickSettings( object? sender, RoutedEventArgs args )
    {
        MainContent.Content = new SettingsView();
        OnNewPage();
    }

    void OnNewPage()
    {
        popout.IsVisible = false;
        MainContent.IsEnabled = true;
    }
}