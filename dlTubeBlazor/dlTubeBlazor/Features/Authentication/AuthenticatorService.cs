namespace dlTubeBlazor.Features.Authentication;

public sealed class AuthenticatorService
{
    // Const
    const int CacheIntervalHours = 1;
    const int MaximumIps = 3000;
    const int MaximumKeys = 1000; 
    
    // Fields
    readonly ILogger<AuthenticatorService> _logger;
    readonly AuthenticatorRepository _repository;
    readonly CancellationTokenSource _cancellationToken;
    
    // Security Data
    readonly HashSet<string> _cachedIps = new( MaximumIps );
    readonly HashSet<string> _cachedKeys = new( MaximumKeys );
    
    // Initializations
    public AuthenticatorService( ILogger<AuthenticatorService> logger, AuthenticatorRepository repository )
    {
        _logger = logger;
        _repository = repository;
        _cancellationToken = new CancellationTokenSource();

        AddCachedIps( _repository.GetValidIpAddresses() );
        AddCachedKeys( _repository.GetValidUserKeys() );

        Task.Run( StartAsyncCacheTimer );
    }
    
    // Public Methods
    public bool ValidateIp( string ip )
    {
        return _cachedIps.Contains( ip );
    }
    public bool ValidateToken( string receivedToken )
    {
        return _cachedKeys.Contains( receivedToken );
    }
    
    // Private Methods
    async Task StartAsyncCacheTimer()
    {
        try
        {
            while ( !_cancellationToken.Token.IsCancellationRequested )
            {
                AddCachedIps( await _repository.GetValidIpAddressesAsync() );
                AddCachedKeys( await _repository.GetValidUserKeysAsync() );

                await Task.Delay( TimeSpan.FromHours( CacheIntervalHours ), _cancellationToken.Token );
            }
        }
        catch ( Exception e )
        {
            _logger.LogError( e, e.Message );
        }
    }
    void AddCachedIps( IEnumerable<string> items )
    {
        _cachedIps.Clear();
        
        foreach ( string s in items )
        {
            _cachedIps.Add( s );
        }
    }
    void AddCachedKeys( IEnumerable<string> items )
    {
        _cachedKeys.Clear();

        foreach ( string s in items )
        {
            _cachedKeys.Add( s );
        }
    }
}