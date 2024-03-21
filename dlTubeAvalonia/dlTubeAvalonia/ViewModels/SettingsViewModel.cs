using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using ReactiveUI;
using dlTubeAvalonia.Models;
using dlTubeAvalonia.Services;

namespace dlTubeAvalonia.ViewModels;

public sealed class SettingsViewModel : BaseViewModel
{
    // Property Fields
    IReadOnlyList<string> _backgroundImages = [ ];
    string _apiKey = string.Empty;
    string _downloadLocation = string.Empty;
    string _selectedBackgroundImage = string.Empty;
    bool _settingsChanged;
    
    // Service
    readonly SettingsService _service = null!;
    
    // Commands
    public ReactiveCommand<Unit, Unit> SaveChangesCommand { get; } = default!;
    
    // Constructor
    public SettingsViewModel() : base( TryGetLogger<SettingsViewModel>() )
    {
        BackgroundImages = AppSettingsModel.BackgroundImages;
        
        if ( this.SettingsService is null )
            return;

        _service = this.SettingsService;
        
        SaveChangesCommand = ReactiveCommand.CreateFromTask( SaveSettings );

        ApiKey = _service.Settings.ApiKey;
        DownloadLocation = _service.Settings.DownloadLocation;
        SelectedBackgroundImage = _service.Settings.SelectedBackgroundImage;

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

        ServiceReply<bool> reply = await _service.SaveSettings( settings );
        
        Message = reply.Success
            ? "Saved Settings To Disk."
            : reply.PrintDetails();

        HasMessage = true;
        SettingsChanged = false;
        IsFree = true;
    }
}