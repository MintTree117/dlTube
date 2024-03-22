using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using dlTubeAvalonia.Models;
using dlTubeAvalonia.Services;
using dlTubeAvalonia.ViewModels;
using Microsoft.Extensions.Logging;

namespace dlTubeAvalonia.Views;

public sealed partial class MainWindow : Window
{
    // Services & Views
    readonly ILogger<MainWindow>? _logger = Program.ServiceProvider.GetService<ILogger<MainWindow>>();
    readonly SettingsManager SettingsService = Program.ServiceProvider.GetService<SettingsManager>()!;
    readonly YtDownloaderView _downloadView;
    YtSearchView? _youtubeView;
    ArchiveView? _archiveView;
    
    // Initialization
    public MainWindow()
    {
        InitializeComponent();
        this.DataContext = new MainWindowViewModel();
        
        _downloadView = new YtDownloaderView();
        MainContent.Content = _downloadView;

        SettingsService.SettingsChanged += OnChangeSettings;
        OnChangeSettings( SettingsService.Settings );
    }
    protected override void OnClosed( EventArgs e )
    {
        SettingsService.SettingsChanged -= OnChangeSettings;
        base.OnClosed( e );
    }
    
    // Private Methods
    void OnChangeSettings( AppSettingsModel? newSettings )
    {
        string? img = newSettings?.SelectedBackgroundImage;
        
        if ( string.IsNullOrWhiteSpace( img ) )
            return;

        if ( img == AppSettingsModel.TransparentBackgroundKeyword )
        {
            Background = null;
            return;
        }

        Stream? stream;
        
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            stream = assembly.GetManifestResourceStream( img );
        }
        catch ( Exception e )
        {
            _logger?.LogError( e, e.Message );
            return;
        }
        
        if ( stream is null )
        {
            stream?.Dispose();
            return;
        }

        Background = new ImageBrush( new Bitmap( stream ) )
        {
            Stretch = Stretch.UniformToFill
        };

        stream.Dispose();
    }
    void OnNewPage()
    {
        MainContent.IsEnabled = true;
    }
    void OnClickViewYoutubeDownloader( object? sender, RoutedEventArgs args )
    {
        MainContent.Content = _downloadView;
        OnNewPage();
    }
    void OnClickViewYoutubeSearch( object? sender, RoutedEventArgs args )
    {
        _youtubeView ??= new YtSearchView();
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
}