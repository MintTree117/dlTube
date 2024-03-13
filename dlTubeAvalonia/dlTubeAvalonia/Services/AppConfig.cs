using System.Threading.Tasks;
using dlTubeAvalonia.Models;

namespace dlTubeAvalonia.Services;

using System;
using System.IO;
using System.Text.Json;

public static class AppConfig
{
    public static string GetUserSettingsPath()
    {
        string appDataFolder = Environment.GetFolderPath( Environment.SpecialFolder.ApplicationData );
        string myAppFolder = Path.Combine( appDataFolder, "dlTubeAvalonia" );
        string appSettingsFile = Path.Combine( myAppFolder, "appsettings.json" );

        if ( !Directory.Exists( myAppFolder ) )
            Directory.CreateDirectory( myAppFolder );

        return appSettingsFile;
        
        if ( !File.Exists( appSettingsFile ) )
            throw new FileNotFoundException( "Application settings file not found.", appSettingsFile );

        string jsonString = File.ReadAllText( appSettingsFile );
        JsonDocument jsonDoc = JsonDocument.Parse( jsonString );
        string? userSettingsPath = jsonDoc.RootElement.GetProperty( "UserSettingsPath" ).GetString();

        if ( string.IsNullOrEmpty( userSettingsPath ) )
            throw new InvalidOperationException( "UserSettingsPath is not set in appsettings.json." );

        return userSettingsPath;
    }

    public static async Task<AppSettingsModel?> LoadSettings( string appSettingsPath )
    {
        if ( !File.Exists( appSettingsPath ) )
            return null;

        try
        {
            string json = await File.ReadAllTextAsync( appSettingsPath );
            AppSettingsModel? settings = JsonSerializer.Deserialize<AppSettingsModel>( json );
            return settings;
        }
        catch ( Exception e )
        {
            Console.WriteLine( e );
            return null;
        }
    }
    public static AppSettingsModel? LoadSettingsS( string appSettingsPath )
    {
        if ( !File.Exists( appSettingsPath ) )
            return null;

        try
        {
            string json = File.ReadAllText( appSettingsPath );
            AppSettingsModel? settings = JsonSerializer.Deserialize<AppSettingsModel>( json );
            return settings;
        }
        catch ( Exception e )
        {
            Console.WriteLine( e );
            return null;
        }
    }
    public static async Task<bool> SaveSettings( string appSettingsPath, AppSettingsModel settings )
    {
        try
        {
            string json = JsonSerializer.Serialize( settings, new JsonSerializerOptions { WriteIndented = true } );
            await File.WriteAllTextAsync( appSettingsPath, json );
            return true;
        }
        catch ( Exception e )
        {
            Console.WriteLine( e );
            return false;
        }
    }
}