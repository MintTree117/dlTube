namespace dlTubeBlazor.Client.Services;

using Blazored.LocalStorage;

public sealed class ClientAuthenticator
{
    ILogger<ClientAuthenticator> Logger { get; init; }
    ILocalStorageService Storage { get; init; }

    const string TokenStorageKey = "Token";

    public string? Token { get; private set; }

    public ClientAuthenticator( ILogger<ClientAuthenticator> logger, ILocalStorageService storage )
    {
        Logger = logger;
        Storage = storage;
    }

    public async Task<bool> TryLoadToken()
    {
        try
        {
            Token = await Storage.GetItemAsStringAsync( TokenStorageKey );
            return !string.IsNullOrWhiteSpace( Token );
        }
        catch ( Exception e )
        {
            Logger.LogError( e, e.Message );
            return false;
        }
    }
    public async Task<bool> TrySetToken( string? newToken )
    {
        try
        {
            Token = newToken;
            await Storage.SetItemAsStringAsync( TokenStorageKey, newToken ?? string.Empty );
            return true;
        }
        catch ( Exception e )
        {
            Logger.LogError( e, e.Message );
            return false;
        }
    }
}