using Avalonia.Controls;
using Avalonia.Interactivity;

namespace dlTubeAvalonia.Views;

public sealed partial class MainWindow : Window
{
    readonly YoutubeView _youtubeView;
    ArchiveView? _archiveView;
    
    public MainWindow()
    {
        InitializeComponent();
        _youtubeView = new YoutubeView();
        MainContent.Content = _youtubeView;
    }

    void OnClickViewYoutube( object? sender, RoutedEventArgs args )
    {
        MainContent.Content = _youtubeView;
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