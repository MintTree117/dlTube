using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using dlTubeAvalonia.Models;
using dlTubeAvalonia.Services;

namespace dlTubeAvalonia.ViewModels;

public abstract class BaseViewModel : ReactiveObject, IDisposable
{
    // Services
    protected readonly ILogger<BaseViewModel>? Logger;
    protected readonly SettingsService? SettingsService;

    string _message = string.Empty;
    public string Message
    {
        get => _message;
        set => this.RaiseAndSetIfChanged( ref _message, value );
    }
    
    // Constructor
    public void Dispose()
    {
        GC.SuppressFinalize( this ); // Rider Optimization
        
        if ( SettingsService is not null )
            SettingsService.SettingsChanged -= OnAppSettingsChanged;
    }
    protected BaseViewModel( ILogger<BaseViewModel>? logger )
    {
        Logger = logger;
        TryGetSettingsService( ref SettingsService );
    }
    
    // Methods
    protected virtual void OnAppSettingsChanged( AppSettingsModel newSettings )
    {
        
    }
    protected static ILogger<T>? TryGetLogger<T>()
    {
        return Program.ServiceProvider.GetService<ILogger<T>>();
    }
    void TryGetSettingsService( ref SettingsService? settingsService )
    {
        try
        {
            settingsService = Program.ServiceProvider.GetService<SettingsService>();
            
            if ( settingsService is not null )
                settingsService.SettingsChanged += OnAppSettingsChanged;
        }
        catch ( Exception e )
        {
            Logger?.LogError( e, e.Message );
        }
    }
}