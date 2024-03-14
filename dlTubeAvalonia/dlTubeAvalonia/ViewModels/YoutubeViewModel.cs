using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using dlTubeAvalonia.Services;
using ReactiveUI;

namespace dlTubeAvalonia.ViewModels;

public sealed class YoutubeViewModel : ReactiveObject
{
    const string DefaultVideoName = "No Video Selected";
    const string LoadingVideoName = "Loading Video...";
    const string InvalidVideoName = "Invalid Video Link";
    const string SuccessDownloadMessage = "Download success!";
    const string FailDownloadMessage = "Failed to download!";
    const string DefaultVideoImage = "avares://dlTubeAvalonia/Assets/defaultplayer.png";
    const string DefaultVideoQuality = "None";
    
    string _youtubeLink = string.Empty;
    string _videoName = DefaultVideoName;
    Bitmap? _videoThumbnailBitmap;
    List<string> _streamTypes = Enum.GetNames<StreamType>().ToList();
    List<string> _streamQualities = [ DefaultVideoQuality ];
    string _selectedStreamTypeName = null!;
    string _selectedStreamQualityName = string.Empty;
    string _resultMessage = string.Empty;

    readonly string _downloadPath;
    YoutubeDownloaderService? _dlService;
    bool _isLinkBoxEnabled;
    bool _isSettingsEnabled;
    
    public ReactiveCommand<Unit, Unit> DownloadCommand { get; }
    
    public YoutubeViewModel()
    {
        SelectedStreamType = StreamTypes[ 0 ];
        SelectedStreamQuality = _streamQualities[ 0 ];
        LoadDefaultImage();

        DownloadCommand = ReactiveCommand.CreateFromTask( Download ); // validate and fetch is called on button click and is bound to the reactive model
        _downloadPath = AppConfig.GetDownloadPath();

        IsLinkBoxEnabled = true;
    }
    
    public string YoutubeLink
    {
        get => _youtubeLink;
        set
        {
            this.RaiseAndSetIfChanged( ref _youtubeLink, value );
            HandleNewLink();
        }
    }
    public string VideoName
    {
        get => _videoName;
        set => this.RaiseAndSetIfChanged( ref _videoName, value );
    }
    public Bitmap? VideoThumbnailBitmap
    {
        get => _videoThumbnailBitmap;
        set => this.RaiseAndSetIfChanged( ref _videoThumbnailBitmap, value );
    }
    public List<string> StreamTypes
    {
        get => _streamTypes;
        set => this.RaiseAndSetIfChanged( ref _streamTypes, value );
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
    public List<string> StreamQualities
    {
        get => _streamQualities;
        set => this.RaiseAndSetIfChanged( ref _streamQualities, value );
    }
    public string SelectedStreamQuality
    {
        get => _selectedStreamQualityName;
        set => this.RaiseAndSetIfChanged( ref _selectedStreamQualityName, value );
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
    public string ResultMessage
    {
        get => _resultMessage;
        set => this.RaiseAndSetIfChanged( ref _resultMessage, value );
    }

    async void HandleNewLink()
    {
        IsSettingsEnabled = false;
        VideoName = InvalidVideoName;
        LoadDefaultImage();
        StreamQualities = [ DefaultVideoQuality ];
        SelectedStreamQuality = StreamQualities[ 0 ];
        ResultMessage = string.Empty;
        _dlService = null;

        if ( string.IsNullOrWhiteSpace( _youtubeLink ) )
        {
            VideoName = DefaultVideoName;
            return;
        }

        VideoName = LoadingVideoName;
        _dlService = new YoutubeDownloaderService( _youtubeLink );

        if ( !await _dlService.GetStreamManifest() )
        {
            Console.WriteLine( "Failed to obtain stream manifest!" );
            VideoName = InvalidVideoName;
            _dlService = null;
            return;
        }

        IsSettingsEnabled = true;
        VideoName = _dlService.VideoName ?? DefaultVideoName;

        byte[]? bytes = await _dlService.GetThumbnailBytes();

        if ( bytes is not null )
        {
            using MemoryStream memoryStream = new( bytes );
            Bitmap newThumbnailBitmap = new( memoryStream );
            VideoThumbnailBitmap = newThumbnailBitmap;
        }

        HandleNewStreamType();
    }

    void LoadDefaultImage()
    {
        VideoThumbnailBitmap = new Bitmap( AssetLoader.Open( new Uri( DefaultVideoImage ) ) );
    }
    void HandleNewStreamType()
    {
        if ( _dlService is null || !Enum.TryParse( _selectedStreamTypeName, out StreamType streamType ) )
            return;
        
        List<string> streamQualities = _dlService.GetStreamInfo( streamType );

        StreamQualities = streamQualities.Count > 0
            ? streamQualities
            : [ DefaultVideoQuality ];
        
        SelectedStreamQuality = StreamQualities[ 0 ];
    }
    async Task Download()
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
}