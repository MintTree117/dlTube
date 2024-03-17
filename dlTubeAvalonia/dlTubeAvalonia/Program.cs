using Avalonia;
using Avalonia.ReactiveUI;
using System;
using dlTubeAvalonia.Services;
using Microsoft.Extensions.DependencyInjection;

namespace dlTubeAvalonia;

sealed class Program
{
    public static IServiceProvider ServiceProvider { get; private set; } = default!;

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread] 
    public static void Main( string[] args )
    {
        ServiceCollection serviceCollection = [ ];
        ConfigureServices( serviceCollection );
        
        ServiceProvider = serviceCollection.BuildServiceProvider();
        
        BuildAvaloniaApp().StartWithClassicDesktopLifetime( args );
    }

    static void ConfigureServices( IServiceCollection services )
    {
        services.AddScoped<HttpService>();
        services.AddScoped<YoutubeDownloaderService>();
        services.AddScoped<YoutubeSearchService>();
        services.AddSingleton<FFmpegService>();
        services.AddScoped<ArchiveService>();
        services.AddLogging( configure => { } );
    }

    static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
                     .UsePlatformDetect()
                     .WithInterFont()
                     .LogToTrace()
                     .UseReactiveUI();
}