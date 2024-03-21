using System;
using System.Reactive;
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
    
    // Reactive Property Fields
    bool _isFree = false;
    bool _hasMessage;
    string _message = string.Empty;
    
    // Commands
    public ReactiveCommand<Unit, Unit> CloseMessageCommand { get; }
    
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
        CloseMessageCommand = ReactiveCommand.Create( CloseMessage );
        TryGetSettingsService( ref SettingsService );
    }
    
    // Reactive Properties
    public bool IsFree
    {
        get => _isFree;
        set => this.RaiseAndSetIfChanged( ref _isFree, value );
    }
    public string Message
    {
        get => _message;
        set => this.RaiseAndSetIfChanged( ref _message, value );
    }
    public bool HasMessage
    {
        get => _hasMessage;
        set => this.RaiseAndSetIfChanged( ref _hasMessage, value );
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
    void CloseMessage()
    {
        HasMessage = false;
    }
}