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

public sealed partial class MainWindow : Window, IDisposable
{
    readonly SettingsService? SettingsService;
    readonly DownloadView _downloadView;
    YoutubeView? _youtubeView;
    ArchiveView? _archiveView;
    
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
    public void Dispose()
    {
        if ( SettingsService is not null )
            SettingsService.SettingsChanged -= OnChangeSettings;
    }

    void OnChangeSettings( AppSettingsModel? newSettings )
    {
        if ( newSettings is null || string.IsNullOrWhiteSpace( newSettings.SelectedBackgroundImage ) )
            return;

        if ( newSettings.SelectedBackgroundImage == AppSettingsModel.TransparentBackgroundKeyword )
        {
            this.Background = null;
            return;
        }
        
        var _assembly = Assembly.GetExecutingAssembly();
        Stream? stream = _assembly.GetManifestResourceStream( newSettings.SelectedBackgroundImage );

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
        MainContent.IsEnabled = true;
    }
}