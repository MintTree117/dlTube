using Avalonia.Controls;
using Avalonia.Interactivity;

namespace dlTubeAvaloniaCrossPlatform.Views.Desktop;

public partial class MainViewDesktop : UserControl
{
    readonly YoutubeDownloadView _downloadView;
    YoutubeSearchViewDesktop? _youtubeView;
    ArchiveViewDesktop? _archiveView;

    public MainViewDesktop()
    {
        InitializeComponent();
        _downloadView = new YoutubeDownloadView();
        MainContent.Content = _downloadView;
    }

    void OnClickViewYoutubeDownloader( object? sender, RoutedEventArgs args )
    {
        MainContent.Content = _downloadView;
    }
    void OnClickViewYoutubeSearch( object? sender, RoutedEventArgs args )
    {
        _youtubeView ??= new YoutubeSearchViewDesktop();
        MainContent.Content = _youtubeView;
    }
    void OnClickViewArchive( object? sender, RoutedEventArgs args )
    {
        _archiveView ??= new ArchiveViewDesktop();
        MainContent.Content = _archiveView;
    }
    void OnClickAppSettings( object? sender, RoutedEventArgs args )
    {
        MainContent.Content = new AppSettingsView();
    }
    void OnClickLogin( object? sender, RoutedEventArgs args )
    {
        MainContent.Content = new LoginView();
    }
    void OnClickLogout( object? sender, RoutedEventArgs args )
    {
        MainContent.Content = _downloadView;
    }
    void OnClickAccountSettings( object? sender, RoutedEventArgs args )
    {

    }
}