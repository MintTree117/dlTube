using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using dlTubeAvalonia.Models;
using dlTubeAvalonia.Services;
using ReactiveUI;

namespace dlTubeAvalonia.ViewModels;

public sealed class YoutubeViewModel : ReactiveObject
{
    const string DefaultVideoName = "Video Name";
    const string DefaultImage = "avares://dlTubeAvalonia/Assets/defaultplayer.png";
    const string DefaultQuality = "None";
    readonly string _downloadPath;

    bool _isVideoLoaded = false;
    string _youtubeLink = string.Empty;
    string _videoName = DefaultVideoName;
    string _videoThumbnailUrl = DefaultImage;
    Bitmap? _videoThumbnailBitmap;

    List<string> _streamTypes = Enum.GetNames<StreamType>().ToList();
    List<string> _streamQualities = [ DefaultQuality ];
    
    string _selectedStreamTypeName = null!;
    string _selectedStreamQualityName = string.Empty;

    YoutubeDownloaderService? _dlService;
    
    public ReactiveCommand<Unit, Unit> DownloadCommand { get; }
    
    public YoutubeViewModel()
    {
        // validate and fetch is called on button click and is bound to the reactive model
        DownloadCommand = ReactiveCommand.CreateFromTask( Download );
        SelectedStreamType = StreamTypes[ 0 ];
        SelectedStreamQuality = _streamQualities[ 0 ];
        LoadDefaultImage();

        AppSettingsModel settings = AppConfig.LoadSettingsS( AppConfig.GetUserSettingsPath() ) ?? new AppSettingsModel();
        _downloadPath = settings.DownloadLocation;
    }

    public bool IsVideoLoaded
    {
        get => _isVideoLoaded;
        set => this.RaiseAndSetIfChanged( ref _isVideoLoaded, value );
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

    async void HandleNewLink()
    {
        IsVideoLoaded = false;
        VideoName = string.Empty;
        _videoThumbnailUrl = DefaultImage;
        VideoThumbnailBitmap = null;
        StreamQualities = [ DefaultQuality ];
        SelectedStreamQuality = StreamQualities[ 0 ];
        _dlService = null;

        if ( string.IsNullOrWhiteSpace( _youtubeLink ) )
        {
            LoadDefaultImage();
            return;
        }

        VideoName = "Loading...";
        _dlService = new YoutubeDownloaderService( _youtubeLink );

        if ( !await _dlService.GetStreamManifest() )
        {
            // show messages
            Console.WriteLine( "Failed to obtain stream manifest!" );
            LoadDefaultImage();
            _dlService = null;
            return;
        }

        IsVideoLoaded = true;
        VideoName = _dlService.VideoName ?? "";
        _videoThumbnailUrl = _dlService.VideoThumbnail ?? "";
        
        Bitmap? newThumbnailBitmap = await LoadImageFromUrlAsync( _videoThumbnailUrl );
        if ( newThumbnailBitmap is not null )
            VideoThumbnailBitmap = newThumbnailBitmap;
        else
            LoadDefaultImage(); // Fallback to default image if new image loading fails
        
        HandleNewStreamType();
    }
    async Task<Bitmap?> LoadImageFromUrlAsync( string imageUrl )
    {
        try
        {
            using ( var client = new HttpClient() )
            {
                var response = await client.GetAsync( imageUrl );
                if ( response.IsSuccessStatusCode )
                {
                    using ( var stream = await response.Content.ReadAsStreamAsync() )
                    {
                        return new Bitmap( stream );
                    }
                }
            }
        }
        catch ( Exception ex )
        {
            Console.WriteLine( $"Failed to load image from URL: {ex.Message}" );
        }
        return null;
    }
    void LoadDefaultImage()
    {
        VideoThumbnailBitmap = new Bitmap( AssetLoader.Open( new Uri( DefaultImage ) ) );
    }
    void HandleNewStreamType()
    {
        if ( _dlService is null || !Enum.TryParse( _selectedStreamTypeName, out StreamType streamType ) )
            return;
        
        List<string> streamQualities = _dlService.GetStreamInfo( streamType );

        StreamQualities = streamQualities.Count > 0
            ? streamQualities
            : [ DefaultQuality ];
        
        SelectedStreamQuality = StreamQualities[ 0 ];
    }
    async Task Download()
    {
        if ( _dlService is null || !Enum.TryParse( _selectedStreamTypeName, out StreamType streamType ) )
            return;

        if ( !_streamQualities.Contains( _selectedStreamQualityName ) )
            return;

        bool success = await _dlService.Download( 
            _downloadPath, streamType, _streamQualities.IndexOf( _selectedStreamQualityName ) );

        if ( !success )
        {
            // make error message
        }
    }
}