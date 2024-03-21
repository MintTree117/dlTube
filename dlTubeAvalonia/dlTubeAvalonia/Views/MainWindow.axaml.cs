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

namespace dlTubeAvalonia.Views;

public sealed partial class MainWindow : Window
{
    // Services & Views
    readonly SettingsService? SettingsService;
    readonly DownloadView _downloadView;
    YoutubeView? _youtubeView;
    ArchiveView? _archiveView;
    
    // Initialization
    public MainWindow()
    {
        //this.DataContext = new MainWindowViewModel();
        InitializeComponent();
        this.DataContext = new MainWindowViewModel();
        _downloadView = new DownloadView();
        MainContent.Content = _downloadView;

        this.SettingsService = Program.ServiceProvider.GetService<SettingsService>();

        Console.WriteLine( this.SettingsService );
        
        if ( SettingsService is not null )
            SettingsService.SettingsChanged += OnChangeSettings;
        
        OnChangeSettings( this.SettingsService?.Settings );
    }
    protected override void OnClosed( EventArgs e )
    {
        if ( SettingsService is not null )
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
            var _assembly = Assembly.GetExecutingAssembly();
            stream = _assembly.GetManifestResourceStream( img );
        }
        catch ( Exception e )
        {
            Console.WriteLine( e + e.Message );
            throw;
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
}