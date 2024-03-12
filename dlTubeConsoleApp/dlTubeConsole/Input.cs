namespace dlTubeConsole;

public static class Input
{
    public static int GetDownloadOptions( List<string> options )
    {
        if ( options.Count == 0 )
        {
            Console.WriteLine( "This video does not have any download options!" );
            return -1;
        }

        foreach ( string t in options )
        {
            Console.WriteLine( t );
        }

        Console.Write( "Select your download option (1, 2, 3, ...): " );

        return GetInputRange( 0, options.Count );
    }
    public static int GetInputRange( int min, int max )
    {
        while ( true )
        {
            if ( int.TryParse( Console.ReadLine(), out int selection ) && selection >= min && selection <= max )
                return selection;

            Console.WriteLine( "Invalid selection. Please choose from the available options." );
        }
    }
    public static string GetVideoUrl()
    {
        Console.WriteLine( "Enter the video url." );

        string videoUrl = null!;

        while ( true )
        {
            videoUrl = Console.ReadLine()!;

            if ( !string.IsNullOrWhiteSpace( videoUrl ) )
                break;

            Console.WriteLine( "Invalid url! Try again." );
        }

        return videoUrl;
    }
}