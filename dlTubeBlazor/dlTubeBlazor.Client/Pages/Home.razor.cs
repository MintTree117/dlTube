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

    // Defaults
    const string DefaultStreamImage = "defaultplayer.png";
    const string DefaultStreamName = "No Stream Selected";
    const string FailGetStreamName = "Failed to get Stream";
    const string SuccessDownloadMessage = "Download Success";
    const string FailDownloadMessage = "Failed to Download";
    const string LoadingStreamName = "Loading Stream Information...";
    const string DownloadingStreamName = "Downloading Stream...";
    
    // Css
    const string CssHide = "d-none";
    const string CssBlock = "d-block";
    const string CssFlex = "d-flex";

    // Youtube Fields
    readonly List<string> _streamTypes = Enum.GetNames<StreamType>().ToList();
    List<string>[] _streamQualities = [ ];
    List<string>? _selectedStreamQualities = [ ];
    string _youtubeLink = string.Empty;
    string _streamTitle = DefaultStreamName;
    string _streamAuthor = string.Empty;
    string _streamDuration = string.Empty;
    string _streamImage = string.Empty;
    string _selectedStreamType = string.Empty;
    string _selectedStreamQuality = string.Empty;
    
    // State
    bool _isNewLink = true;
    bool _isLoading;
    string? _token;
    string? _newToken;
    
    // Alerts
    string _alertMessage = string.Empty;
    string _alertCss = CssHide;
    string _alertButtonCss = string.Empty;
    
    // Initialization
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _isLoading = true;
    }
    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        await base.OnAfterRenderAsync( firstRender );

        if ( firstRender )
        {
            if ( await TryGetToken() )
                ToggleLoading( false );
        }
    }
    
    // User Actions
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
        ToggleLoading( true );
        bool success = await Authenticator.TrySetToken( _newToken );

        if ( !success )
            ShowAlert( AlertType.Danger, "IO Error: Failed to save the token to storage!" );
        else
            ShowAlert( AlertType.Success, "Saved the token to storage!" );

        _token = null;
        
        ToggleLoading( false );
    }
    async Task OnSubmit()
    {
        ToggleLoading( true );
        
        if ( _isNewLink )
            await GetStreamInfo();
        else
            await GetStreamDownload();

        ToggleLoading( false );
    }
    async Task GetStreamInfo()
    {
        _streamTitle = LoadingStreamName;
        _streamDuration = string.Empty;
        _streamImage = string.Empty;
        _selectedStreamQuality = string.Empty;
        _streamQualities = [ ];

        if ( string.IsNullOrWhiteSpace( _youtubeLink ) )
        {
            _streamTitle = DefaultStreamName;
            return;
        }

        StreamInfo? result = await Youtube.GetStreamInfo( GetStreamInfoParams() );

        if ( result is null )
        {
            _streamTitle = FailGetStreamName;
            Console.WriteLine( "Result is null!" );
            ShowAlert( AlertType.Danger, "Failed to fetch stream info for url! Check console (f12) for more information." );
            return;
        }

        _streamTitle = result.Title;
        _streamDuration = result.Duration;
        _streamImage = result.ImageUrl;
        _streamQualities = result.Qualities;
        _selectedStreamQuality = string.Empty;

        _selectedStreamQualities = new( SelectStreamQualities() );
        StateHasChanged();
    }
    async Task GetStreamDownload()
    {
        
    }
    
    // UI Events
    void OnNewLink( ChangeEventArgs e )
    {
        _isNewLink = true;
    }
    void OnNewStreamType( ChangeEventArgs e )
    {
        string? value = e.Value?.ToString();

        if ( string.IsNullOrWhiteSpace( value ) || !_streamTypes.Contains( value ) )
        {
            Console.WriteLine( $"Failed to parse new stream type! : {value}" );
            return;
        }
        
        //_selectedStreamQualities = new List<string>() { "a", "b", "c" };
        _selectedStreamQualities = SelectStreamQualities();
        _selectedStreamType = value;
        StateHasChanged();
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
        StateHasChanged();
    }
    void OnNewToken( ChangeEventArgs e )
    {
        ToggleLoading( true );

        string? value = e.Value?.ToString();
        _newToken = value ?? string.Empty;

        ToggleLoading( false );
    }
    
    // Utilities
    string GetLoaderCss()
    {
        return _isLoading ? CssBlock : CssHide;
    }
    bool GetApiButtonDisabled()
    {
        return _newToken == _token;
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
        SetAlertCss( type );
        // TODO: start fade animation
        StateHasChanged();
    }
    void CloseAlert()
    {
        _alertMessage = string.Empty;
        _alertCss = CssHide;
        StateHasChanged();
    }
    void SetAlertCss( AlertType type )
    {
         _alertCss = type switch
        {
            AlertType.Success => $"alert-success {CssFlex}",
            AlertType.Warning => $"alert-warning {CssFlex}",
            AlertType.Danger => $"alert-danger {CssFlex}",
            _ => $"alert-danger {CssFlex}"
        };
        _alertButtonCss = type switch
        {
            AlertType.Success => "btn-success",
            AlertType.Warning => "btn-warning",
            AlertType.Danger => "btn-danger",
            _ => "btn-danger"
        };
    }
    
    void ToggleLoading( bool value )
    {
        _isLoading = value;
        StateHasChanged();
    }

    List<string> SelectStreamQualities()
    {
        if ( string.IsNullOrWhiteSpace( _selectedStreamType ) )
        {
            Console.WriteLine( "Steam type null" );
            return [ ];
        }

        if ( !_streamTypes.Contains( _selectedStreamType ) )
        {
            Console.WriteLine( "Steam type invalid" );
            return [ ];
        }

        int index = _streamTypes.IndexOf( _selectedStreamType );
        
        Console.WriteLine( _selectedStreamType );
        Console.WriteLine( index );
        Console.WriteLine( _streamQualities.Length );
        
        if ( index < 0 || index >= _streamQualities.Length )
        {
            Console.WriteLine( "Stream index invalid" );
            return [ ];
        }

        if ( _streamQualities[ index ] is null )
        {
            Console.WriteLine( "Stream qualities is null" );
            return [ ];
        }

        foreach ( string s in _streamQualities[ index ] )
        {
            Console.WriteLine( s );
        }

        return _streamQualities[ index ];
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

    List<string> DeepCopyStringList( List<string> listToCopy )
    {
        List<string> listToReturn = [ ];
        listToReturn.AddRange( from s in listToCopy select new string( s ) );
        return listToReturn;
    }
}