using System;
using System.IO;
using System.Reactive;
using System.Text.Json;
using dlTubeAvalonia.Models;
using dlTubeAvalonia.Services;
using ReactiveUI;

namespace dlTubeAvalonia.ViewModels;

public sealed class AppSettingsViewModel : ReactiveObject
{
    readonly string _appSettingsPath;
    
    string _downloadLocation = "";
    bool _downloadLocationChanged = false;

    // Define a command for the save action
    public ReactiveCommand<Unit, Unit> SaveDownloadLocationCommand { get; }

    public AppSettingsViewModel()
    {
        SaveDownloadLocationCommand = ReactiveCommand.Create( SaveSettings );
        _appSettingsPath = AppConfig.GetUserSettingsPath();
        LoadSettings();
    }

    public string DownloadLocation
    {
        get => _downloadLocation;
        set
        {
            this.RaiseAndSetIfChanged( ref _downloadLocation, value );
            DownloadLocationChanged = true;
        }
    }
    public bool DownloadLocationChanged
    {
        get => _downloadLocationChanged;
        set => this.RaiseAndSetIfChanged( ref _downloadLocationChanged, value );
    }

    async void LoadSettings()
    {
        if ( !File.Exists( _appSettingsPath ) )
            return;

        try
        {
            string json = await File.ReadAllTextAsync( _appSettingsPath );
            AppSettingsModel? settings = JsonSerializer.Deserialize<AppSettingsModel>( json );

            if ( settings is null )
                return;

            DownloadLocation = settings.DownloadLocation;
        }
        catch ( Exception e )
        {
            Console.WriteLine( e );
        }
    }
    async void SaveSettings()
    {
        Console.WriteLine( $"Saving Download Location: {DownloadLocation}" );

        AppSettingsModel settings = new()
        {
            DownloadLocation = this.DownloadLocation
        };

        try
        {
            string json = JsonSerializer.Serialize( settings, new JsonSerializerOptions { WriteIndented = true } );
            await File.WriteAllTextAsync( _appSettingsPath, json );
            DownloadLocationChanged = false;   
        }
        catch ( Exception e )
        {
            Console.WriteLine( e );
        }
    }
}