using System.Collections.Generic;
using System.Reactive;
using ReactiveUI;

namespace dlTubeAvalonia.ViewModels;

public sealed class YoutubeViewModel : ReactiveObject
{
    string _youtubeLink = string.Empty;
    string _videoName = string.Empty;
    string _videoThumbnailUrl = string.Empty;

    bool _isNameAvailable = false;
    bool _isThumbnailAvailable = false;
    
    List<string> _streamTypes = [ "A", "B", "C" ];
    List<string> _streamQualities = [ "1", "2", "3" ];
    
    string _selectedStreamType = string.Empty;
    string _selectedStreamQuality = string.Empty;

    public YoutubeViewModel()
    {
        // validate and fetch is called on button click and is bound to the reactive model
        ValidateAndFetchCommand = ReactiveCommand.Create( ValidateAndFetch );
        _isThumbnailAvailable = false;
    }

    public string YoutubeLink
    {
        get => _youtubeLink;
        set => this.RaiseAndSetIfChanged( ref _youtubeLink, value );
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
            // Optionally, react to selection change here
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

    private void ValidateAndFetch()
    {
        // Validation and data fetching logic will go here
    }
}