using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ReactiveUI;
using dlTubeAvalonia.Enums;
using dlTubeAvalonia.Models;
using dlTubeAvalonia.Services;

namespace dlTubeAvalonia.ViewModels;

public sealed class YtDownloaderViewModel : BaseViewModel
{
    // Services
    YtDownloaderService? _dlService;
    
    // Constants
    const string DefaultVideoName = "No Video Selected";
    const string LoadingVideoName = "Loading Video...";
    const string InvalidVideoName = "Invalid Video Link";
    const string SuccessDownloadMessage = "Download success!";
    const string FailDownloadMessage = "Failed to download!";
    const string DefaultVideoImage = "avares://dlTubeAvalonia/Assets/defaultplayer3.jpg";
    
    // Property Field List Values
    Bitmap? _videoImageBitmap;
    List<string> _streamTypes = Enum.GetNames<StreamType>().ToList();
    List<string> _streamQualities = [ ];
    string _youtubeLink = string.Empty;
    string _videoName = DefaultVideoName;
    string _selectedStreamTypeName = string.Empty; // saves state between downloads for user convenience
    string _selectedStreamQualityName = string.Empty;
    string _resultMessage = string.Empty;
    bool _isLinkBoxEnabled;
    bool _isSettingsEnabled;
    bool _hasResultMessage;

    // Commands
    public ReactiveCommand<Unit, Unit> DownloadCommand { get; }
    
    ReactiveCommand<Unit, Unit> LoadDataCommand { get; } // Private command for easy async execution of method
    ReactiveCommand<Unit, Unit> NewStreamCommand { get; }

    // Constructor
    public YtDownloaderViewModel() : base( TryGetLogger<YtDownloaderViewModel>() )
    {
        TryGetDownloadService( ref _dlService );
        
        LoadDataCommand = ReactiveCommand.CreateFromTask( HandleNewLink );
        DownloadCommand = ReactiveCommand.CreateFromTask( DownloadStream );
        NewStreamCommand = ReactiveCommand.CreateFromTask( HandleNewStreamType );
        
        IsLinkBoxEnabled = true;

        LoadDefaultImage();
        
        // Load Initial Settings
        OnAppSettingsChanged( SettingsService?.Settings ?? new AppSettingsModel() );
    }

    void TryGetDownloadService( ref YtDownloaderService? dlService )
    {
        try
        {
            dlService = Program.ServiceProvider.GetService<YtDownloaderService>();
        }
        catch ( Exception e )
        {
            Logger?.LogError( e, e.Message );
        }
    }

    // Reactive Properties
    public Bitmap? VideoImageBitmap
    {
        get => _videoImageBitmap;
        set => this.RaiseAndSetIfChanged( ref _videoImageBitmap, value );
    }
    public List<string> StreamTypes
    {
        get => _streamTypes;
        set => this.RaiseAndSetIfChanged( ref _streamTypes, value );
    }
    public List<string> StreamQualities
    {
        get => _streamQualities;
        set => this.RaiseAndSetIfChanged( ref _streamQualities, value );
    }
    public string YoutubeLink
    {
        get => _youtubeLink;
        set
        {
            this.RaiseAndSetIfChanged( ref _youtubeLink, value );
            LoadDataCommand.Execute();
        }
    }
    public string VideoName
    {
        get => _videoName;
        set => this.RaiseAndSetIfChanged( ref _videoName, value );
    }
    public string SelectedStreamType // saves state between downloads for user convenience
    {
        get => _selectedStreamTypeName;
        set
        {
            this.RaiseAndSetIfChanged( ref _selectedStreamTypeName, value );
            NewStreamCommand.Execute();
        }
    }
    public string SelectedStreamQuality
    {
        get => _selectedStreamQualityName;
        set => this.RaiseAndSetIfChanged( ref _selectedStreamQualityName, value );
    }
    public string ResultMessage
    {
        get => _resultMessage;
        set => this.RaiseAndSetIfChanged( ref _resultMessage, value );
    }
    public bool IsLinkBoxEnabled
    {
        get => _isLinkBoxEnabled;
        set => this.RaiseAndSetIfChanged( ref _isLinkBoxEnabled, value );
    }
    public bool IsSettingsEnabled
    {
        get => _isSettingsEnabled;
        set => this.RaiseAndSetIfChanged( ref _isSettingsEnabled, value );
    }
    public bool HasResultMessage
    {
        get => _hasResultMessage;
        set => this.RaiseAndSetIfChanged( ref _hasResultMessage, value );
    }

    // Command Delegates
    async Task HandleNewLink()
    {
        if ( LinkIsEmptyAfterChangesApplied() )
            return;
        
        _dlService = new YtDownloaderService( _youtubeLink );

        ServiceReply<bool> reply = await _dlService.TryInitialize();
        
        if ( !reply.Success )
        {
            Logger?.LogError( $"Failed to obtain stream manifest! Reply message: {reply.PrintDetails()}" );
            VideoName = InvalidVideoName;
            ResultMessage = PrintError( reply.ErrorType.ToString() ); //reply.PrintDetails();
            HasResultMessage = true;
            _dlService = null;
            return;
        }

        IsSettingsEnabled = true;
        VideoName = $"{_dlService.VideoName ?? DefaultVideoName} : Length = {_dlService.VideoDuration}";

        SetImageBitmap();
        await HandleNewStreamType();
    }
    async Task DownloadStream()
    {
        if ( !ValidateBeforeTryDownload( out StreamType streamType ) )
            return;

        IsLinkBoxEnabled = false;
        IsSettingsEnabled = false;
        
        // Execute Download
        ServiceReply<bool> reply = await _dlService!.Download(
            GetDownloadPath(), streamType, _streamQualities.IndexOf( _selectedStreamQualityName ) );

        ResultMessage = reply.Success
            ? SuccessDownloadMessage
            : PrintError( reply.PrintDetails() );

        HasResultMessage = true;
        IsLinkBoxEnabled = true;
        IsSettingsEnabled = true;
    }

    // Private Methods
    static string PrintError( string message )
    {
        return $"{FailDownloadMessage} : {message}";
    }
    async Task HandleNewStreamType()
    {
        if ( _dlService is null || !Enum.TryParse( _selectedStreamTypeName, out StreamType streamType ) )
        {
            Logger?.LogError( $"Failed to handle new stream type!" );
            ResultMessage = PrintError( ServiceErrorType.AppError.ToString() );
            return;
        }

        List<string> streamQualities = await _dlService.GetStreamInfo( streamType );

        StreamQualities = streamQualities.Count > 0
            ? streamQualities
            : [ ];//[ DefaultVideoQuality ];

        SelectedStreamQuality = string.Empty;
    }
    string GetDownloadPath()
    {
        return SettingsService is not null
            ? SettingsService.Settings.DownloadLocation
            : SettingsService.DefaultDownloadDirectory;
    }
    void SetImageBitmap()
    {
        byte[]? bytes = _dlService!.ThumbnailBytes;

        if ( bytes is null )
            return;

        using MemoryStream memoryStream = new( bytes );
        Bitmap newThumbnailBitmap = new( memoryStream );
        VideoImageBitmap = newThumbnailBitmap;
    }
    bool LinkIsEmptyAfterChangesApplied()
    {
        bool linkIsEmpty = string.IsNullOrWhiteSpace( _youtubeLink );

        LoadDefaultImage();
        IsSettingsEnabled = false;
        HasResultMessage = false;
        SelectedStreamType = string.Empty;
        ResultMessage = string.Empty;
        VideoName = linkIsEmpty ? DefaultVideoName : LoadingVideoName;
        StreamQualities = [ ];
        _dlService = null;

        return linkIsEmpty;
    }
    void LoadDefaultImage()
    {
        VideoImageBitmap = new Bitmap( AssetLoader.Open( new Uri( DefaultVideoImage ) ) );
    }
    bool ValidateBeforeTryDownload( out StreamType streamType )
    {
        streamType = StreamType.Mixed; 
        
        // No Service
        if ( _dlService is null )
        {
            Logger?.LogError( "Youtube download service is null!" );
            ResultMessage = PrintError( ServiceErrorType.AppError.ToString() );
            return false;
        }
        // Invalid Selected Stream Type
        if ( !Enum.TryParse( _selectedStreamTypeName, out streamType ) )
        {
            Logger?.LogError( "Invalid Stream Type!" );
            ResultMessage = PrintError( ServiceErrorType.AppError.ToString() );
            return false;
        }
        // Invalid Selected Stream Quality
        if ( !_streamQualities.Contains( _selectedStreamQualityName ) )
        {
            Logger?.LogError( "Invalid _selectedStreamQualityName!" );
            ResultMessage = PrintError( ServiceErrorType.AppError.ToString() );
            return false;
        }

        return true;
    }
}