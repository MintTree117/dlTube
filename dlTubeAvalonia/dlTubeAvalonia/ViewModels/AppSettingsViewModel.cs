using System;
using System.Reactive;
using System.Threading.Tasks;
using dlTubeAvalonia.Models;
using dlTubeAvalonia.Services;
using ReactiveUI;

namespace dlTubeAvalonia.ViewModels;

public sealed class AppSettingsViewModel : ReactiveObject
{
    readonly string _appSettingsPath;
    
    string _downloadLocation = "";
    bool _settingsChanged = false;

    // Define a command for the save action
    public ReactiveCommand<Unit, Unit> SaveChangesCommand { get; }

    public AppSettingsViewModel()
    {
        SaveChangesCommand = ReactiveCommand.CreateFromTask( SaveSettings );
        _appSettingsPath = AppConfig.GetUserSettingsPath();
        LoadSettings();
    }

    public string DownloadLocation
    {
        get => _downloadLocation;
        set
        {
            this.RaiseAndSetIfChanged( ref _downloadLocation, value );
            SettingsChanged = true;
        }
    }
    public bool SettingsChanged
    {
        get => _settingsChanged;
        set => this.RaiseAndSetIfChanged( ref _settingsChanged, value );
    }

    async void LoadSettings()
    {
        AppSettingsModel? settings = await AppConfig.LoadSettings( _appSettingsPath );

        if ( settings is null )
            return;

        DownloadLocation = settings.DownloadLocation;
        SettingsChanged = false;
    }
    async Task SaveSettings()
    {
        Console.WriteLine( $"Saving Download Location: {DownloadLocation}" );

        AppSettingsModel settings = new()
        {
            DownloadLocation = this.DownloadLocation
        };

        bool success = await AppConfig.SaveSettings( _appSettingsPath, settings );

        SettingsChanged = !success;
    }
}