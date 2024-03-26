namespace dlTubeBlazor.Features.Authentication;

public sealed class AuthenticatorService
{
    // Fields
    readonly ILogger<AuthenticatorService> _logger;
    readonly AuthenticatorRepository _repository;
    
    // Security Data
    IReadOnlySet<string> _validIps;
    IReadOnlySet<string> _validKeys;
    
    // Initializations
    public AuthenticatorService( ILogger<AuthenticatorService> logger, AuthenticatorRepository repository )
    {
        _logger = logger;
        _repository = repository;
        _validIps = _repository.GetValidIpAddresses().ToHashSet();
        _validKeys = _repository.GetValidUserKeys().ToHashSet();
    }
    
    // Public Methods
    public bool ValidateIp( string ip )
    {
        return _validIps.Contains( ip );
    }
    public bool ValidateToken( string receivedToken )
    {
        return _validKeys.Contains( receivedToken );
    }
    
    // Private Methods
    async Task UpdateIpAddresses()
    {
        _validIps = ( await _repository.GetValidIpAddressesAsync() ).ToHashSet();
    }
    async Task UpdateValidKeys()
    {
        _validKeys = ( await _repository.GetValidUserKeysAsync() ).ToHashSet();
    }
}