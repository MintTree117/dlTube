using System.Collections.Generic;
using System.Net.Http;
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
    public static string GetDownloadPath()
    {
        AppSettingsModel settings = AppConfig.LoadSettingsS( AppConfig.GetUserSettingsPath() ) ?? new AppSettingsModel();
        return settings.DownloadLocation;
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

    public static async Task<List<byte>?> LoadImageBytesFromUrlAsync( string imageUrl )
    {
        try
        {
            using var client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync( imageUrl );

            if ( response.IsSuccessStatusCode )
            {
                await using Stream stream = await response.Content.ReadAsStreamAsync();
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync( memoryStream ); // Copy the stream to a MemoryStream
                return new List<byte>( memoryStream.ToArray() ); // Convert to List<byte>
            }
        }
        catch ( Exception ex )
        {
            Console.WriteLine( $"Failed to load image from URL: {ex.Message}" );
        }

        return null;
    }
}