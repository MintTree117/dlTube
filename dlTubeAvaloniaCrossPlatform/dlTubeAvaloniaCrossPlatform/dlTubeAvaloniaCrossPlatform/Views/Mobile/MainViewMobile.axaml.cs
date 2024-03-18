using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using dlTubeAvaloniaCrossPlatform.ViewModels;

namespace dlTubeAvaloniaCrossPlatform.Views.Mobile;

public partial class MainViewMobile : UserControl
{
    //readonly MainViewModel _viewModel;
    readonly YoutubeDownloadView _downloadView;
    YoutubeSearchViewMobile? _youtubeView;
    
    public MainViewMobile()
    {
        _downloadView = new YoutubeDownloadView();
        InitializeComponent();

        //this.DataContext = new MainViewModel();
        //_viewModel = ( this.DataContext as MainViewModel )!;
        MainContent.Content = _downloadView;
    }
    void InitializeComponent()
    {
        AvaloniaXamlLoader.Load( this );
    }

    void OnClickToggleMenu( object? sender, RoutedEventArgs args )
    {
        //_viewModel.ShowMobileMenu = !_viewModel.ShowMobileMenu;
    }

    void OnClickViewYoutubeDownloader( object? sender, RoutedEventArgs args )
    {
        MainContent.Content = _downloadView;
    }
    void OnClickViewYoutubeSearch( object? sender, RoutedEventArgs args )
    {
        _youtubeView ??= new YoutubeSearchViewMobile();
        MainContent.Content = _youtubeView;
    }
    void OnClickViewArchive( object? sender, RoutedEventArgs args )
    {
        // _archiveView ??= new ArchiveViewDesktop();
        //MainContent.Content = _archiveView;
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