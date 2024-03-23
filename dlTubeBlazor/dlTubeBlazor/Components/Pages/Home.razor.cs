using dlTubeBlazor.Enums;
using Microsoft.AspNetCore.Components;

namespace dlTubeBlazor.Components.Pages;

public partial class Home
{   
    // Logger From DI
    [Inject] ILogger<Home> Logger { get; init; } = default!;

    // Constants
    const string DefaultStreamName = "No Video Selected";
    const string LoadingVideoName = "Loading Video...";
    const string InvalidVideoName = "Invalid Video Link";
    const string SuccessDownloadMessage = "Download success!";
    const string FailDownloadMessage = "Failed to download!";

    // Property Field List Values
    //Bitmap? _videoImageBitmap;
    readonly List<string> _streamTypes = Enum.GetNames<StreamType>().ToList();
    List<string> _streamQualities = [ ];
    string _youtubeLink = string.Empty;
    string _videoName = DefaultStreamName;
    string _selectedStreamType = string.Empty;
    string _selectedStreamQuality = string.Empty;
    bool _isLinkBoxEnabled = false;
    bool _isSettingsEnabled = false;

    string _alertMessage = string.Empty;
    bool _showAlert;

    async Task TryGetApiKey()
    {
        ToggleAll( false );
        // await service
        ToggleAll( true );
    }
    async Task TrySetApiKey()
    {
        ToggleAll( false );
        // await service
        ToggleAll( true );
    }

    async Task HandleNewApiKey()
    {
        ToggleAll( false );
        // await service
        ToggleAll( true );
    }
    async Task HanldeNewLink()
    {
        ToggleAll( false );
        // await service
        ToggleAll( true );
    }
    async Task HandleNewStreamType( ChangeEventArgs e )
    {
        string? value = e.Value?.ToString();

        if ( string.IsNullOrWhiteSpace( value ) || !_streamTypes.Contains( value ) )
        {
            Logger.LogWarning( $"Failed to parse new stream type! : {value}" );
            return;
        }
        
        _selectedStreamType = value;
        
        // await service
    }
    async Task HandleNewStreamQuality( ChangeEventArgs e )
    {
        string? value = e.Value?.ToString();

        if ( string.IsNullOrWhiteSpace( value ) || !_streamQualities.Contains( value ) )
        {
            Logger.LogWarning( $"Failed to parse new stream quality! : {value}" );
            return;
        }

        _selectedStreamQuality = value;
        
        // await service
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
    
    void ShowAlert( string message )
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
    }
}