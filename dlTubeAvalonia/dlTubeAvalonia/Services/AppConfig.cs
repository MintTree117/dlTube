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
}