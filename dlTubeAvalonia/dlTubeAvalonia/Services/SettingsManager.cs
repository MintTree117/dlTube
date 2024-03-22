using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using dlTubeAvalonia.Models;

namespace dlTubeAvalonia.Services;

// Singleton Service
public sealed class SettingsManager : BaseService
{
    // Change Event
    public event Action<AppSettingsModel>? SettingsChanged;
    
    // Constants
    public const string DefaultDownloadDirectory = "./";
    const string CacheDirectory = "./Cache";
    const string CachePath = CacheDirectory + "/Cache.txt";
    const string FailLoadMessage = "Failed to load settings file! You can still make changes, but they might not be saved once you close the app.";
    const string FailedSaveMessage = "Failed to save settings to disk! Changes will still persist until you close the app.";
    
    // Settings Model
    public AppSettingsModel Settings { get; private set; }
    readonly JsonSerializerOptions _serializerOptions = new() { WriteIndented = true };

    // Constructor
    public SettingsManager()
    {
        Settings = LoadSettings();
    }
    AppSettingsModel LoadSettings()
    {
        try
        {
            if ( !File.Exists( CachePath ) )
            {
                Logger.LogWithConsole( $"{ServiceErrorType.IoError} : Settings file doesn't exist" );
                return new AppSettingsModel();
            }
            
            string json = File.ReadAllText( CachePath );
            var loadedSettings = JsonSerializer.Deserialize<AppSettingsModel>( json );
            bool loaded = loadedSettings is not null;
            
            if ( loaded )
                Settings = loadedSettings!;

            return Settings;
        }
        catch ( Exception e )
        {
            Logger.LogWithConsole( ExString( e ) );
            return new AppSettingsModel();
        }
    }
    
    // Public Methods
    public async Task<ServiceReply<AppSettingsModel>> LoadSettingsAsync()
    {
        try
        {
            if ( !File.Exists( CachePath ) )
                return new ServiceReply<AppSettingsModel>( Settings, ServiceErrorType.NotFound, FailLoadMessage );
            
            string json = await File.ReadAllTextAsync( CachePath );
            var loadedSettings = JsonSerializer.Deserialize<AppSettingsModel>( json );
            
            bool loaded = loadedSettings is not null;

            if ( loaded )
                Settings = loadedSettings!;

            return loaded
                ? new ServiceReply<AppSettingsModel>( Settings )
                : new ServiceReply<AppSettingsModel>( Settings, ServiceErrorType.NotFound, FailLoadMessage );
        }
        catch ( Exception e )
        {
            Logger.LogWithConsole( ExString( e ) );
            return new ServiceReply<AppSettingsModel>( ServiceErrorType.IoError, FailLoadMessage );
        }
    }
    public async Task<ServiceReply<bool>> SaveSettings( AppSettingsModel newSettings )
    {
        try
        {
            if ( !Directory.Exists( CacheDirectory ) )
                Directory.CreateDirectory( CacheDirectory );

            //string json = JsonSerializer.Serialize( newSettings, _serializerOptions );
            byte[] jsonBytes = JsonSerializer.SerializeToUtf8Bytes( newSettings, _serializerOptions );

            await using FileStream fs = new FileStream( CachePath, FileMode.Create );
            await fs.WriteAsync( jsonBytes );
            //await File.WriteAllTextAsync( CachePath, json );
            return new ServiceReply<bool>( true );
        }
        catch ( Exception e )
        {
            Logger.LogWithConsole( ExString( e ) );
            return new ServiceReply<bool>( ServiceErrorType.IoError, FailedSaveMessage );
        }
        finally
        {
            Settings = newSettings;
            SettingsChanged?.Invoke( Settings );
        }
    }
}