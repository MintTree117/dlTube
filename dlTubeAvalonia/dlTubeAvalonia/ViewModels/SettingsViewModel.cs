using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using ReactiveUI;
using dlTubeAvalonia.Models;

namespace dlTubeAvalonia.ViewModels;

public sealed class SettingsViewModel : BaseViewModel
{
    // Property Fields
    IReadOnlyList<string> _backgroundImages = [ ];
    string _apiKey = string.Empty;
    string _downloadLocation = string.Empty;
    string _selectedBackgroundImage = string.Empty;
    bool _settingsChanged;
    
    // Commands
    public ReactiveCommand<Unit, Unit> SaveChangesCommand { get; }
    
    // Constructor
    public SettingsViewModel() : base( TryGetLogger<SettingsViewModel>() )
    {
        SaveChangesCommand = ReactiveCommand.CreateFromTask( SaveSettings );
        
        BackgroundImages = AppSettingsModel.BackgroundImages;
        ApiKey = SettingsService.Settings.ApiKey;
        DownloadLocation = SettingsService.Settings.DownloadLocation;
        SelectedBackgroundImage = SettingsService.Settings.SelectedBackgroundImage;

        IsFree = true;
    }
    
    // Reactive Properties
    public string ApiKey
    {
        get => _apiKey;
        set
        {
            this.RaiseAndSetIfChanged( ref _apiKey, value );
            SettingsChanged = true;
        }
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
    public string SelectedBackgroundImage
    {
        get => _selectedBackgroundImage;
        set
        {
            this.RaiseAndSetIfChanged( ref _selectedBackgroundImage, value );
            SettingsChanged = true;
        }
    }
    public IReadOnlyList<string> BackgroundImages
    {
        get => _backgroundImages;
        set
        {
            this.RaiseAndSetIfChanged( ref _backgroundImages, value );
            SettingsChanged = true;
        }
    }
    public bool SettingsChanged
    {
        get => _settingsChanged;
        set => this.RaiseAndSetIfChanged( ref _settingsChanged, value );
    }
    
    // Private Methods
    async Task SaveSettings()
    {
        IsFree = false;
        
        HasMessage = true;
        Message = $"Saving Download Location: {DownloadLocation}";

        AppSettingsModel settings = new()
        {
            ApiKey = _apiKey,
            DownloadLocation = _downloadLocation,
            SelectedBackgroundImage = _selectedBackgroundImage
        };

        ServiceReply<bool> reply = await SettingsService.SaveSettings( settings );
        
        Message = reply.Success
            ? "Saved Settings To Disk."
            : reply.PrintDetails();

        HasMessage = true;
        SettingsChanged = false;
        IsFree = true;
    }
}