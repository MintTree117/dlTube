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

    void OnClickTogglePopoutMenu( object? sender, RoutedEventArgs args )
    {
        popout.IsVisible = !popout.IsVisible;
        MainContent.IsEnabled = !popout.IsVisible; // disable main content while accessing menu
        MainContentOverlay.IsVisible = popout.IsVisible;
    }
    void OnClickViewYoutubeDownloader( object? sender, RoutedEventArgs args )
    {
        MainContent.Content = _downloadView;
        popout.IsVisible = false;
    }
    void OnClickViewYoutubeSearch( object? sender, RoutedEventArgs args )
    {
        _youtubeView ??= new YoutubeView();
        MainContent.Content = _youtubeView;
        popout.IsVisible = false;
    }
    void OnClickViewArchive( object? sender, RoutedEventArgs args )
    {
        _archiveView ??= new ArchiveView();
        MainContent.Content = _archiveView;
        popout.IsVisible = false;
    }
    void OnClickAppSettings( object? sender, RoutedEventArgs args )
    {
        MainContent.Content = new AppSettingsView();
        popout.IsVisible = false;
    }
    void OnClickLogin( object? sender, RoutedEventArgs args )
    {
        MainContent.Content = new LoginView();
        popout.IsVisible = false;
    }
    void OnClickAccountSettings( object? sender, RoutedEventArgs args )
    {
        popout.IsVisible = false;
    }
    void OnClickLogout( object? sender, RoutedEventArgs args )
    {
        MainContent.Content = _downloadView;
        popout.IsVisible = false;
    }
}