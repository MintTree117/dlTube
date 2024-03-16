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

    string _downloadLocation = string.Empty;
    bool _settingsChanged;
    
    public ReactiveCommand<Unit, Unit> SaveChangesCommand { get; }

    public AppSettingsViewModel()
    {
        _appSettingsPath = AppConfig.GetUserSettingsPath();
        SaveChangesCommand = ReactiveCommand.CreateFromTask( SaveSettings );
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
        AppSettingsModel? settings = await AppConfig.LoadSettings( _appSettingsPath ) ?? new AppSettingsModel();

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