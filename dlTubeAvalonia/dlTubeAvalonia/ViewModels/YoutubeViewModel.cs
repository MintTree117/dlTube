using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using dlTubeAvalonia.Services;
using ReactiveUI;

namespace dlTubeAvalonia.ViewModels;

public sealed class YoutubeViewModel : ReactiveObject
{
    string _youtubeLink = string.Empty;
    string _videoName = string.Empty;
    string _videoThumbnailUrl = string.Empty;
    Bitmap? _videoThumbnailBitmap;

    bool _isNameAvailable = false;
    bool _isThumbnailAvailable = false;

    List<string> _streamTypes = Enum.GetNames<Filetype>().ToList();
    List<string> _streamQualities = [ "None" ];
    
    string _selectedStreamType = string.Empty;
    string _selectedStreamQuality = string.Empty;

    YoutubeDownloaderService? _dlService;
    
    public YoutubeViewModel()
    {
        // validate and fetch is called on button click and is bound to the reactive model
        ValidateAndFetchCommand = ReactiveCommand.Create( ValidateAndFetch );
        _selectedStreamType = _streamTypes[ 0 ];
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
    public string VideoThumbnailUrl
    {
        get => _videoThumbnailUrl;
        set => this.RaiseAndSetIfChanged( ref _videoThumbnailUrl, value );
    }
    public Bitmap? VideoThumbnailBitmap
    {
        get => _videoThumbnailBitmap;
        set => this.RaiseAndSetIfChanged( ref _videoThumbnailBitmap, value );
    }
    public bool IsNameAvailable
    {
        get => _isNameAvailable;
        set => this.RaiseAndSetIfChanged( ref _isNameAvailable, value );
    }
    public bool IsThumbnailAvailable
    {
        get => _isThumbnailAvailable;
        set => this.RaiseAndSetIfChanged( ref _isThumbnailAvailable, value );
    }
    public List<string> StreamTypes
    {
        get => _streamTypes;
        set => this.RaiseAndSetIfChanged( ref _streamTypes, value );
    }
    public string SelectedStreamType
    {
        get => _selectedStreamType;
        set
        {
            this.RaiseAndSetIfChanged( ref _selectedStreamType, value );
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
        get => _selectedStreamQuality;
        set => this.RaiseAndSetIfChanged( ref _selectedStreamQuality, value );
    }
    public ReactiveCommand<Unit, Unit> ValidateAndFetchCommand { get; }

    async void HandleNewLink()
    {
        VideoName = string.Empty;
        VideoThumbnailUrl = string.Empty;
        IsNameAvailable = false;
        IsThumbnailAvailable = false;
        StreamQualities.Clear();
        _dlService = null;
        
        if ( string.IsNullOrWhiteSpace( _youtubeLink ) )
            return;
        
        _dlService = new YoutubeDownloaderService( _youtubeLink );

        if ( !await _dlService.GetStreamManifest() )
        {
            // show messages
            Console.WriteLine( "Failed to obtain stream manifest!" );
            _dlService = null;
            return;
        }
        
        VideoName = _dlService.VideoName ?? "";
        VideoThumbnailUrl = _dlService.VideoThumbnail ?? "";
        IsNameAvailable = !string.IsNullOrWhiteSpace( VideoName );
        IsThumbnailAvailable = !string.IsNullOrWhiteSpace( VideoThumbnailUrl );

        if ( _isThumbnailAvailable )
            VideoThumbnailBitmap = await LoadImageFromUrlAsync( VideoThumbnailUrl );

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
    void HandleNewStreamType()
    {
        if ( _dlService is null || !StreamTypes.Contains( _selectedStreamType ) )
        {
            throw new Exception( "FAAAAAAAAAAAAAAAAAAAAAAAIL" );
            return;
        }

        int streamType = StreamTypes.IndexOf( _selectedStreamType );
        StreamQualities = _dlService.GetStreamInfo( streamType );
    }
    void ValidateAndFetch()
    {
        // Validation and data fetching logic will go here
    }
}