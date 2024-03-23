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
    [Inject] HttpService Http { get; init; } = default!;

    // Constants
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
    string _streamName = DefaultStreamName;
    string _streamDuration = string.Empty;
    string _streamImage = string.Empty;
    string _selectedStreamType = string.Empty;
    string _selectedStreamQuality = string.Empty;
    bool _isLinkBoxEnabled = false;
    bool _isSettingsEnabled = false;

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
    
    async Task OnNewLink( ChangeEventArgs e )
    {
        ToggleAll( false );

        _streamName = LoadingVideoName;
        _streamQualities = [ ];
        _selectedStreamQuality = string.Empty;
        
        string? value = e.Value?.ToString();

        if ( string.IsNullOrWhiteSpace( value ) )
        {
            _streamName = DefaultStreamName;
        }

        StreamInfo? result = await Http.TryGetRequest<StreamInfo>( StreamInfoApi );

        if ( result is null )
        {
            ShowAlert( AlertType.Danger, "Failed to fetch stream info for url! Check console (f12) for more information." );
            return;
        }

        _streamName = result.Title;
        _streamDuration = result.Duration;
        _streamImage = result.ImageUrl;
        
        ToggleAll( true );
    }
    async Task OnNewStreamType( ChangeEventArgs e )
    {
        string? value = e.Value?.ToString();

        if ( string.IsNullOrWhiteSpace( value ) || !_streamTypes.Contains( value ) )
        {
            Console.WriteLine( $"Failed to parse new stream type! : {value}" );
            return;
        }
        
        _selectedStreamType = value;
        
        // await service
    }
    async Task OnNewStreamQuality( ChangeEventArgs e )
    {
        string? value = e.Value?.ToString();
        Console.WriteLine( "oooooooooooooooooooooooooooooooooooooooooo" );

        /*if ( string.IsNullOrWhiteSpace( value ) || !_streamQualities.Contains( value ) )
        {
            Console.WriteLine( $"Failed to parse new stream quality! : {value}" );
            return;
        }*/

        _selectedStreamQuality = value;
        
        // await service
    }
    void OnNewToken( ChangeEventArgs e )
    {
        ToggleAll( false );

        string? value = e.Value?.ToString();
        _newToken = value ?? string.Empty;

        ToggleAll( true );
    }
    
    async Task Download()
    {
        ToggleAll( false );
        // await service
        ToggleAll( true );
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
}