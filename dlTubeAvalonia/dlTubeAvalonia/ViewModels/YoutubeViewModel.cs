using System.Collections.Generic;
using System.Reactive;
using ReactiveUI;

namespace dlTubeAvalonia.ViewModels;

public sealed class YoutubeViewModel : ViewModelBase
{
    string _youtubeLink = string.Empty;
    string _videoThumbnailUrl = string.Empty;
    bool _isThumbnailAvailable;
    
    List<string> _streamTypes = [ ];
    List<string> _streamQualities = [ ];
    
    string _selectedStreamType = string.Empty;
    string _selectedStreamQuality = string.Empty;

    public YoutubeViewModel()
    {
        // validate and fetch is called on button click and is bound to the reactive model
        ValidateAndFetchCommand = ReactiveCommand.Create( ValidateAndFetch );
    }

    public string YoutubeLink
    {
        get => _youtubeLink;
        set => this.RaiseAndSetIfChanged( ref _youtubeLink, value );
    }
    public string VideoThumbnailUrl
    {
        get => _videoThumbnailUrl;
        set => this.RaiseAndSetIfChanged( ref _videoThumbnailUrl, value );
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