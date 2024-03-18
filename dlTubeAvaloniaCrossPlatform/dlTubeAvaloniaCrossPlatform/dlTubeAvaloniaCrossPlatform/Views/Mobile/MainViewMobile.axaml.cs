using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using dlTubeAvaloniaCrossPlatform.Services;
using dlTubeAvaloniaCrossPlatform.ViewModels;

namespace dlTubeAvaloniaCrossPlatform.Views.Mobile;

public partial class MainViewMobile : UserControl
{
    readonly MainViewModel _viewModel;
    readonly YoutubeDownloadView _downloadView;
    YoutubeSearchViewMobile? _youtubeView;

    public MainViewMobile()
    {
        InitializeComponent();
        _downloadView = new YoutubeDownloadView();
        this.DataContext = new MainViewModel();
        _viewModel = ( this.DataContext as MainViewModel )!;
        ViewsMessageBus.Instance.InvokeMobileViewChanged( _downloadView );
    }
    void InitializeComponent()
    {
        AvaloniaXamlLoader.Load( this );
    }

    void OnClickToggleMenu( object? sender, RoutedEventArgs args )
    {
        _viewModel.ShowMobileMenu = !_viewModel.ShowMobileMenu;
    }

    void OnClickViewDownloader( object? sender, RoutedEventArgs args )
    {
        ViewsMessageBus.Instance.InvokeMobileViewChanged( _downloadView );
        _viewModel.ShowMobileMenu = false;
    }
    void OnClickViewYoutube( object? sender, RoutedEventArgs args )
    {
        _youtubeView ??= new YoutubeSearchViewMobile();
        ViewsMessageBus.Instance.InvokeMobileViewChanged( _youtubeView );
        _viewModel.ShowMobileMenu = false;
    }
    void OnClickViewArchive( object? sender, RoutedEventArgs args )
    {
        // _archiveView ??= new ArchiveViewDesktop();
        //MainContent.Content = _archiveView;
        _viewModel.ShowMobileMenu = false;
    }
    void OnClickAppSettings( object? sender, RoutedEventArgs args )
    {
        ViewsMessageBus.Instance.InvokeMobileViewChanged( new AppSettingsView() );
        _viewModel.ShowMobileMenu = false;
    }
    void OnClickLogin( object? sender, RoutedEventArgs args )
    {
        ViewsMessageBus.Instance.InvokeMobileViewChanged( new LoginView() );
        _viewModel.ShowMobileMenu = false;
    }
    void OnClickLogout( object? sender, RoutedEventArgs args )
    {
        ViewsMessageBus.Instance.InvokeMobileViewChanged( _downloadView );
        _viewModel.ShowMobileMenu = false;
    }
    void OnClickAccountSettings( object? sender, RoutedEventArgs args )
    {
        _viewModel.ShowMobileMenu = false;
    }
}