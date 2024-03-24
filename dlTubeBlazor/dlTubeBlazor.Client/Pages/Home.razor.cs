using Microsoft.AspNetCore.Components;
using dlTubeBlazor.Client.Dtos;
using dlTubeBlazor.Client.Enums;
using dlTubeBlazor.Client.Services;

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
    const string GetStreamText = "Get Stream Info";
    const string DownloadStreamText = "Download Stream";

    // Css
    const string CssHide = "d-none";
    const string CssBlock = "d-block";
    const string CssFlex = "d-flex";

    // Youtube Fields
    readonly List<StreamType> _streamTypes = Enum.GetValues<StreamType>().ToList();
    readonly List<string> _streamTypeNames = GetStreamTypeNames( Enum.GetNames<StreamType>().ToList() );
    List<string> _streamQualities = [ ];
    string _youtubeLink = string.Empty;
    string _streamTitle = DefaultStreamName;
    string _streamAuthor = string.Empty;
    string _streamDuration = string.Empty;
    string _streamImage = string.Empty;
    string _selectedStreamTypeName = string.Empty;
    string _selectedStreamQuality = "Select a stream quality";

    // State
    bool _isLoading;
    bool _hasStream;
    string? _token;
    string? _newToken;

    // Misc Css
    string _loaderCss = CssHide;
    string _submitButtonText = GetStreamText;
    string _imageUrl = DefaultStreamImage;

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
            _selectedStreamTypeName = _streamTypeNames[ 0 ];
            await TryGetToken();
            ToggleLoading( false );
        }
    }
    static List<string> GetStreamTypeNames( List<string> streamTypes )
    {
        List<string> names = [ ];

        foreach ( string t in streamTypes )
        {
            names.Add( $"Stream Type: {t}" );
        }
        
        return names;
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

        if ( !_hasStream )
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
        _submitButtonText = GetStreamText;
        _imageUrl = DefaultStreamImage;

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

        _imageUrl = result.ImageUrl;
        _streamTitle = result.Title;
        _streamDuration = result.Duration;
        _streamImage = result.ImageUrl;
        _streamQualities = result.Qualities;
        _submitButtonText = DownloadStreamText;
        _hasStream = true;

        StateHasChanged();
    }
    async Task GetStreamDownload()
    {
        ToggleLoading( true );

        bool result = await Youtube.TryDownloadStream( GetStreamDownloadParams() );

        if ( result )
        {
            ShowAlert( AlertType.Success, "Downloaded stream." );
        }
        else
        {
            ShowAlert( AlertType.Danger, "Error: Failed to download stream!" );
        }

        ToggleLoading( false );
    }

    // UI Events
    void OnNewLink( ChangeEventArgs e )
    {
        _hasStream = false;
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
    bool GetApiButtonDisabled()
    {
        return _newToken == _token;
    }
    bool GetStreamTypeDropdownDisabled()
    {
        return _isLoading || !_hasStream;
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
        _loaderCss = value ? CssFlex : CssHide;
        StateHasChanged();
    }
    
    Dictionary<string, object> GetStreamInfoParams()
    {
        return new Dictionary<string, object>()
        {
            { "url", _youtubeLink },
            { "type", GetSelectedStreamType() }
            // TODO: auth-service.token
        };
    }
    Dictionary<string, object> GetStreamDownloadParams()
    {
        return new Dictionary<string, object>()
        {
            { "url", _youtubeLink },
            { "type", GetSelectedStreamType() },
            { "quality", GetSelectedQualityIndex() }
        };
    }
    StreamType GetSelectedStreamType()
    {
        int index = _streamTypeNames.IndexOf( _selectedStreamTypeName );
        StreamType type = _streamTypes[ index ];
        return type;
    }
    int GetSelectedQualityIndex()
    {
        int index = _streamQualities.IndexOf( _selectedStreamQuality );
        Console.WriteLine( _selectedStreamQuality );
        return 0; //index;
    }
}