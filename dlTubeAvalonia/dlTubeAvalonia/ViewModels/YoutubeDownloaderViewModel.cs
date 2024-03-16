using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using dlTubeAvalonia.Enums;
using dlTubeAvalonia.Services;
using ReactiveUI;

namespace dlTubeAvalonia.ViewModels;

public sealed class YoutubeDownloaderViewModel : ReactiveObject
{
    const string DefaultVideoName = "No Video Selected";
    const string LoadingVideoName = "Loading Video...";
    const string InvalidVideoName = "Invalid Video Link";
    const string SuccessDownloadMessage = "Download success!";
    const string FailDownloadMessage = "Failed to download!";
    const string DefaultVideoImage = "avares://dlTubeAvalonia/Assets/defaultplayer.png";
    const string DefaultVideoQuality = "None";

    readonly string _downloadPath;
    
    string _youtubeLink = string.Empty;
    string _videoName = DefaultVideoName;
    Bitmap? _videoImageBitmap;
    List<string> _streamTypes = Enum.GetNames<StreamType>().ToList();
    List<string> _streamQualities = [ DefaultVideoQuality ];
    string _selectedStreamTypeName = null!;
    string _selectedStreamQualityName = string.Empty;
    string _resultMessage = string.Empty;
    bool _isLinkBoxEnabled;
    bool _isSettingsEnabled;
    
    IYoutubeDownloaderService? _dlService;
    
    public ReactiveCommand<Unit, Unit> DownloadCommand { get; }
    ReactiveCommand<Unit, Unit> LoadDataCommand { get; } // private command for easy async execution of method
    
    public YoutubeDownloaderViewModel()
    {
        _downloadPath = AppConfig.GetDownloadPath();
        
        SelectedStreamType = StreamTypes[ 0 ];
        SelectedStreamQuality = StreamQualities[ 0 ];
        IsLinkBoxEnabled = true;

        LoadDefaultImage();

        DownloadCommand = ReactiveCommand.CreateFromTask( DownloadStream );
        LoadDataCommand = ReactiveCommand.CreateFromTask( HandleNewLink );
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
    public string SelectedStreamType
    {
        get => _selectedStreamTypeName;
        set
        {
            this.RaiseAndSetIfChanged( ref _selectedStreamTypeName, value );
            HandleNewStreamType();
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

    async Task HandleNewLink()
    {
        ResetFieldsAndData();

        if ( string.IsNullOrWhiteSpace( _youtubeLink ) )
            return;
        
        _dlService = new YoutubeDownloaderService( _youtubeLink );

        if ( !await _dlService.GetStreamManifest() )
        {
            Console.WriteLine( "Failed to obtain stream manifest!" );
            VideoName = InvalidVideoName;
            _dlService = null;
            return;
        }

        IsSettingsEnabled = true;
        VideoName = ( _dlService.VideoName ?? DefaultVideoName ) + " - " + _dlService.VideoDuration;

        await LoadImageFromYoutube();
        HandleNewStreamType();
    }
    async Task DownloadStream()
    {
        if ( _dlService is null || !Enum.TryParse( _selectedStreamTypeName, out StreamType streamType ) )
            return;

        if ( !_streamQualities.Contains( _selectedStreamQualityName ) )
            return;

        IsLinkBoxEnabled = false;
        IsSettingsEnabled = false;

        bool success = await _dlService.Download(
            _downloadPath, streamType, _streamQualities.IndexOf( _selectedStreamQualityName ) );

        ResultMessage = success
            ? SuccessDownloadMessage
            : FailDownloadMessage;

        IsLinkBoxEnabled = true;
        IsSettingsEnabled = true;
    }
    async Task LoadImageFromYoutube()
    {
        byte[]? bytes = await _dlService!.GetThumbnailBytes();

        if ( bytes is not null )
        {
            using MemoryStream memoryStream = new( bytes );
            Bitmap newThumbnailBitmap = new( memoryStream );
            VideoImageBitmap = newThumbnailBitmap;
        }
    }

    void ResetFieldsAndData()
    {
        IsSettingsEnabled = false;
        VideoName = string.IsNullOrWhiteSpace( _youtubeLink ) ? DefaultVideoName : LoadingVideoName;
        LoadDefaultImage();
        StreamQualities = [ DefaultVideoQuality ];
        SelectedStreamQuality = StreamQualities[ 0 ];
        ResultMessage = string.Empty;
        _dlService = null;
    }
    void LoadDefaultImage()
    {
        VideoImageBitmap = new Bitmap( AssetLoader.Open( new Uri( DefaultVideoImage ) ) );
    }
    void HandleNewStreamType()
    {
        if ( _dlService is null || !Enum.TryParse( _selectedStreamTypeName, out StreamType streamType ) )
        {
            Console.WriteLine( "" );
            return;   
        }
        
        List<string> streamQualities = _dlService.GetStreamInfo( streamType );

        StreamQualities = streamQualities.Count > 0
            ? streamQualities
            : [ DefaultVideoQuality ];
        
        SelectedStreamQuality = StreamQualities[ 0 ];
    }
}