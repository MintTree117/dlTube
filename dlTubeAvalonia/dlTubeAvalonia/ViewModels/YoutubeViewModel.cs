using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using dlTubeAvalonia.Services;
using ReactiveUI;

namespace dlTubeAvalonia.ViewModels;

public sealed class YoutubeViewModel : ReactiveObject
{
    const string DefaultVideoName = "Video";
    const string DefaultImage = "avares://dlTubeAvalonia/Assets/defaultplayer.png";
    
    string _youtubeLink = string.Empty;
    string _videoName = DefaultVideoName;
    string _videoThumbnailUrl = DefaultImage;
    Bitmap? _videoThumbnailBitmap;

    List<string> _streamTypes = Enum.GetNames<Filetype>().ToList();
    List<string> _streamQualities = [ "None" ];
    
    string _selectedStreamType;
    string _selectedStreamQuality = string.Empty;

    YoutubeDownloaderService? _dlService;
    
    public YoutubeViewModel()
    {
        // validate and fetch is called on button click and is bound to the reactive model
        ValidateAndFetchCommand = ReactiveCommand.Create( ValidateAndFetch );
        SelectedStreamType = StreamTypes[ 0 ];
        LoadDefaultImage();
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
        _videoThumbnailUrl = string.Empty;
        VideoThumbnailBitmap = null;
        StreamQualities.Clear();
        _dlService = null;

        if ( string.IsNullOrWhiteSpace( _youtubeLink ) )
        {
            LoadDefaultImage();
            return;
        }
        
        _dlService = new YoutubeDownloaderService( _youtubeLink );

        if ( !await _dlService.GetStreamManifest() )
        {
            // show messages
            Console.WriteLine( "Failed to obtain stream manifest!" );
            LoadDefaultImage();
            _dlService = null;
            return;
        }
        
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
        if ( _dlService is null || !StreamTypes.Contains( _selectedStreamType ) )
            return;

        int streamType = StreamTypes.IndexOf( _selectedStreamType );
        StreamQualities = _dlService.GetStreamInfo( streamType );
        SelectedStreamQuality = StreamQualities.FirstOrDefault() ?? "";
    }
    void ValidateAndFetch()
    {
        // Validation and data fetching logic will go here
    }
}