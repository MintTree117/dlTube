using dlTubeBlazor.Client.Dtos;
using dlTubeBlazor.Client.Enums;
using dlTubeBlazor.Client.Services;
using Microsoft.AspNetCore.Components;

namespace dlTubeBlazor.Client.Pages;

public sealed partial class Home
{   
    // Services
    [Inject] ILogger<Home> Logger { get; init; } = default!;
    [Inject] Authenticator Authenticator { get; init; } = default!;
    [Inject] Youtube Youtube { get; init; } = default!;

    // Constants
    const string DefaultStreamImage = "defaultplayer.png";
    const string DefaultStreamName = "No Video Selected";
    const string LoadingVideoName = "Loading Video...";
    const string InvalidVideoName = "Invalid Video Link";
    const string SuccessDownloadMessage = "Download success!";
    const string FailDownloadMessage = "Failed to download!";

    const string StreamInfoApi = "api/stream/info";
    const string StreamDownloadApi = "api/stream/download";

    // Property Field List Values
    //Bitmap? _videoImageBitmap;
    readonly List<string> _streamTypes = Enum.GetNames<StreamType>().ToList();
    List<string>[] _streamQualities = [ ];
    List<string> _selectedStreamQualities = [ ];
    string _youtubeLink = string.Empty;
    string _streamTitle = DefaultStreamName;
    string _streamAuthor = string.Empty;
    string _streamDuration = string.Empty;
    string _streamImage = string.Empty;
    string _selectedStreamType = string.Empty;
    string _selectedStreamQuality = string.Empty;
    
    bool _isNewLink = true;
    bool _isLinkBoxEnabled;
    bool _isSettingsEnabled;
    bool _isLoading;

    string _alertMessage = string.Empty;
    bool _showAlert;

    string? _token;
    string? _newToken;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }
    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        await base.OnAfterRenderAsync( firstRender );

        if ( firstRender )
        {
            if ( await TryGetToken() )
                ToggleAll( true );
        }
    }
    
    async Task<bool> TryGetToken()
    {
        if ( !await Authenticator.TryLoadToken() )
        {
            ShowAlert( AlertType.Warning, "You need an api token to use this service." );
            return false;
        }

        _token = Authenticator.Token;
        _newToken = _token;
        return true;
    }
    async Task TrySaveToken()
    {
        ToggleAll( false );
        bool success = await Authenticator.TrySetToken( _newToken );

        if ( !success )
            ShowAlert( AlertType.Danger, "IO Error: Failed to save the token to storage!" );
        else
            ShowAlert( AlertType.Success, "Saved the token to storage!" );

        _token = null;
        
        ToggleAll( true );
    }
    async Task OnSubmit()
    {
        if ( _isNewLink )
            await GetInfo();
        else
            await Download();
    }
    async Task GetInfo()
    {
        _isLoading = true;
        ToggleAll( false );
        _streamTitle = LoadingVideoName;
        _streamQualities = [ ];
        _selectedStreamQuality = string.Empty;

        if ( string.IsNullOrWhiteSpace( _youtubeLink ) )
        {
            _streamTitle = DefaultStreamName;
        }

        StreamInfo? result = await Youtube.GetStreamInfo( GetStreamInfoParams() );

        if ( result is null )
        {
            Console.WriteLine( "Result is null!" );
            ShowAlert( AlertType.Danger, "Failed to fetch stream info for url! Check console (f12) for more information." );
            return;
        }

        _streamTitle = result.Title;
        _streamDuration = result.Duration;
        _streamImage = result.ImageUrl;
        _streamQualities = result.Qualities;
        _selectedStreamQualities = SelectStreamQualities();
        Console.WriteLine($"Name: {result.Title}");
        _isLoading = false;
        ToggleAll( true );
    }
    async Task Download()
    {
        ToggleAll( false );
        
        ToggleAll( true );
    }

    void OnNewStreamType( ChangeEventArgs e )
    {
        string? value = e.Value?.ToString();

        if ( string.IsNullOrWhiteSpace( value ) || !_streamTypes.Contains( value ) )
        {
            Console.WriteLine( $"Failed to parse new stream type! : {value}" );
            return;
        }

        _selectedStreamType = value;
        SelectStreamQualities();
    }
    void OnNewStreamQuality( ChangeEventArgs e )
    {
        string? value = e.Value?.ToString();

        if ( string.IsNullOrWhiteSpace( value ) || !_selectedStreamQualities.Contains( value ) )
        {
            Console.WriteLine( $"Failed to parse new stream quality! : {value}" );
            return;
        }

        _selectedStreamQuality = value;
    }
    void OnNewToken( ChangeEventArgs e )
    {
        ToggleAll( false );

        string? value = e.Value?.ToString();
        _newToken = value ?? string.Empty;

        ToggleAll( true );
    }
    
    string GetLoaderCss()
    {
        return _isLoading ? "d-block" : "d-none";
    }
    string GetLinkBoxCss()
    {
        return _isLinkBoxEnabled ? "d-block" : "d-none";
    }
    string GetSettingsCss()
    {
        return _isSettingsEnabled ? "d-block" : "d-none";
    }
    string GetTokenButtonCss()
    {
        return _newToken == _token ? "d-none" : "d-flex";
    }
    string GetImageUrl()
    {
        return !string.IsNullOrWhiteSpace( _streamImage ) ? _streamImage : DefaultStreamImage;
    }
    string GetSubmitText()
    {
        return _isNewLink ? "Get Stream" : "Download Stream";
    }
    
    void ShowAlert( AlertType type, string message )
    {
        _alertMessage = message;
        _showAlert = true;
        // TODO: start fade animation
        StateHasChanged();
    }
    void CloseAlert()
    {
        _alertMessage = string.Empty;
        _showAlert = false;
        StateHasChanged();
    }
    
    void ToggleAll( bool value )
    {
        _isLinkBoxEnabled = value;
        _isSettingsEnabled = value;
        StateHasChanged();
    }

    List<string> SelectStreamQualities()
    {
        return !string.IsNullOrWhiteSpace( _selectedStreamType )
            ? _streamQualities[ _streamTypes.IndexOf( _selectedStreamType ) ]
            : [ ];
    }
    Dictionary<string, object> GetStreamInfoParams()
    {
        Enum.TryParse( _selectedStreamType, out StreamType type );

        int quality = _selectedStreamQualities.Contains( _selectedStreamQuality )
            ? _selectedStreamQualities.IndexOf( _selectedStreamQuality )
            : 0;
        
        return new Dictionary<string, object>()
        {
            { "url", _youtubeLink },
            { "type", type },
            { "quality", quality }
            // TODO: auth-service.token
        };
    }
    Dictionary<string, object> GetStreamDownloadParams()
    {
        return new Dictionary<string, object>()
        {
            { "url", _youtubeLink },
            { "type", _selectedStreamType },
            { "quality", _selectedStreamQuality }
        };
    }
}