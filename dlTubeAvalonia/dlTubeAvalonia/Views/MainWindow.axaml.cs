using Avalonia.Controls;
using Avalonia.Interactivity;

namespace dlTubeAvalonia.Views;

public sealed partial class MainWindow : Window
{
    readonly DownloadView _youtubeView;
    ArchiveView? _archiveView;
    
    public MainWindow()
    {
        InitializeComponent();
        _youtubeView = new DownloadView();
        MainContent.Content = _youtubeView;
    }

    void OnClickViewYoutubeDownloader( object? sender, RoutedEventArgs args )
    {
        MainContent.Content = _youtubeView;
    }
    void OnClickViewYoutubeSearch( object? sender, RoutedEventArgs args )
    {
        MainContent.Content = new YoutubeView();
    }
    void OnClickViewArchive( object? sender, RoutedEventArgs args )
    {
        _archiveView ??= new ArchiveView();
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
        MainContent.Content = _youtubeView;
    }
    void OnClickAccountSettings( object? sender, RoutedEventArgs args )
    {
        
    }
}