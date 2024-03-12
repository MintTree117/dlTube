namespace dlTubeConsole;

public static class Configuration
{
    public static void ConfigureOutputDirectory( string settingsFilepath )
    {
        string outputDirectory;

        while ( true )
        {
            Console.WriteLine( "Enter the filepath you want your downloads to go to:" );
            
            outputDirectory = Console.ReadLine()!;

            if ( Directory.Exists( outputDirectory ) )
                break;

            Console.WriteLine( "This directory does not exist!" );
        }

        File.WriteAllText( settingsFilepath, outputDirectory );
        Console.WriteLine( "Directory saved." );
    }

    public static string GetOutputDirectory( string settingsFilepath )
    {
        string outputDirectory = string.Empty;

        try
        {
            string content = null!;

            if ( File.Exists( settingsFilepath ) )
                content = File.ReadAllText( settingsFilepath );
            
            if ( Directory.Exists( content ) )
                outputDirectory = content;
        }
        catch ( Exception ex )
        {
            Console.WriteLine( $"An error occurred while loading the file path: {ex.Message}" );
        }

        return outputDirectory;
    }
}