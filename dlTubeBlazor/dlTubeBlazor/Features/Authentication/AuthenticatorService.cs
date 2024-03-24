using System.Security.Cryptography;
using System.Text;

namespace dlTubeBlazor.Features.Authentication;

public sealed class AuthenticatorService
{
    // Fields
    readonly ILogger<AuthenticatorService> _logger;
    readonly AuthenticatorRepository _repository;
    
    // Security Data
    string _key;
    HashSet<string> _allowedIps;
    
    // Initializations
    public AuthenticatorService( ILogger<AuthenticatorService> logger, AuthenticatorRepository repository )
    {
        _logger = logger;
        _repository = repository;
        _allowedIps = _repository.GetValidIpAddresses().ToHashSet();S
        _key = GenerateKey();
    }
    
    // Public Methods
    public bool ValidateIp( string ip )
    {
        return _allowedIps.Contains( ip );
    }
    public bool ValidateToken( string receivedToken )
    {
        string hashedKey = HashStringWithSHA512( _key ); // Hash the key as the original message
        return hashedKey == receivedToken; // Compare the received token with the hashed key
    }
    
    // Private Methods
    static string HashStringWithSHA512( string input )
    {
        using var sha512 = SHA512.Create();
        byte[] inputBytes = Encoding.UTF8.GetBytes( input );
        byte[] hashBytes = sha512.ComputeHash( inputBytes );
        return Convert.ToBase64String( hashBytes );
    }
    static string GenerateKey( int size = 64 )
    {
        return Convert.ToBase64String( RandomNumberGenerator.GetBytes( size ) );
    }
    async Task UpdateIpAddresses()
    {
        _allowedIps = ( await _repository.GetValidIpAddressesAsync() ).ToHashSet();
    }
    void UpdateKey()
    {
        _key = GenerateKey();
    }
}