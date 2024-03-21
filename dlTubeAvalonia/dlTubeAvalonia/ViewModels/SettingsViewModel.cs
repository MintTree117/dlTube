using System;
using System.Reactive;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using dlTubeAvalonia.Models;
using dlTubeAvalonia.Services;
namespace dlTubeAvalonia.ViewModels;

public sealed class SettingsViewModel : BaseViewModel
{
    // Property Fields
    string _apiKey = string.Empty;
    string _downloadLocation = string.Empty;
    bool _hasMessage;
    bool _settingsChanged;
    bool _isFree;
    
    // Service
    readonly SettingsService _service = null!;
    
    // Commands
    public ReactiveCommand<Unit, Unit> SaveChangesCommand { get; } = default!;
    
    // Constructor
    public SettingsViewModel() : base( TryGetLogger<SettingsViewModel>() )
    {
        if ( this.SettingsService is null )
            return;

        _service = this.SettingsService;
        
        SaveChangesCommand = ReactiveCommand.CreateFromTask( SaveSettings );

        ApiKey = _service.Settings.ApiKey;
        DownloadLocation = _service.Settings.DownloadLocation;

        LoadInitialSettings();
    }
    void LoadInitialSettings()
    {
        try
        {
            LoadSettings();
        }
        catch ( Exception e )
        {
            Logger?.LogError( e, e.Message );
        }
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

    public bool HasMessage
    {
        get => _hasMessage;
        set => this.RaiseAndSetIfChanged( ref _hasMessage, value );
    }
    public bool SettingsChanged
    {
        get => _settingsChanged;
        set => this.RaiseAndSetIfChanged( ref _settingsChanged, value );
    }
    public bool IsFree
    {
        get => _isFree;
        set => this.RaiseAndSetIfChanged( ref _isFree, value );
    }
    
    // Private Methods
    async void LoadSettings()
    {
        IsFree = false;
        
        ServiceReply<AppSettingsModel> reply = await _service.LoadSettingsAsync();

        if ( !reply.Success )
        {
            Message = reply.PrintDetails();
            HasMessage = true;
        }
        
        SettingsChanged = false;
        IsFree = true;
    }
    async Task SaveSettings()
    {
        IsFree = false;
        
        HasMessage = true;
        Message = $"Saving Download Location: {DownloadLocation}";

        AppSettingsModel settings = new()
        {
            ApiKey = _apiKey,
            DownloadLocation = _downloadLocation
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