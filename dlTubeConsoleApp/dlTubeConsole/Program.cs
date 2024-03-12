using System.Reflection;
using dlTubeConsole;

string? settingsFilepath = $"{( Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location ) ?? "" )}/ConfigurationFilename";

bool running = true;
while ( running )
    await Run();
return;

async Task Run()
{
    Console.WriteLine( "Welcome to dlTube!" );
    Console.WriteLine( "Select an option:" );

    Console.WriteLine( "Press # to configure the output directory for downloads." );
    Console.WriteLine( "q: exit" );
    Console.WriteLine( "1: download a video with audio" );
    Console.WriteLine( "2: download audio only" );
    Console.WriteLine( "3: download video only" );
    Console.WriteLine( "4: configure download directory" );

    while ( true )
    {
        string choice = Console.ReadLine()!;

        switch ( choice )
        {
            case "q":
                running = false;
                return;
            case "1":
                await DownloadMedia( DownloadType.MIXED );
                break;
            case "2":
                await DownloadMedia( DownloadType.AUDIO );
                break;
            case "3":
                await DownloadMedia( DownloadType.VIDEO );
                break;
            case "4":
                Configuration.ConfigureOutputDirectory( settingsFilepath );
                break;
            default:
                Console.WriteLine( "Invalid input! Try again, q to exit." );
                return;
        }
    }
}

async Task DownloadMedia( DownloadType type )
{
    string videoUrl = Input.GetVideoUrl();
    string outputDirectory = Configuration.GetOutputDirectory( settingsFilepath );

    Downloader downloader = new( videoUrl, type );

    if ( !await downloader.GetStreamManifest() )
    {
        Console.WriteLine( "Failed to get metadata (stream manifest) for this video!" );
        return;
    }

    List<string> options = downloader.GetStreams();
    int selection = Input.GetDownloadOptions( options );

    Console.WriteLine( "Downloading file..." );
    await downloader.Download( outputDirectory, selection );
    Console.WriteLine( "Download completed! Press enter to continue" );
}