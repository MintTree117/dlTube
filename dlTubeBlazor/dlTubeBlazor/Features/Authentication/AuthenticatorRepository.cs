using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;

namespace dlTubeBlazor.Features.Authentication;

public sealed class AuthenticatorRepository
{
    // Fields
    readonly IConfiguration _config;
    readonly string _connectionString;
    SqlConnection _connection = null!;

    // Constructor
    public AuthenticatorRepository( IConfiguration config )
    {
        _config = config;
        _connectionString = config.GetConnectionString( "DefaultConnection" ) ?? string.Empty;
    }
    public async Task InitConnectionAsync()
    {
        //_connection = await GetOpenConnection();
    }
    
    // Public Methods
    public IEnumerable<string> GetValidIpAddresses()
    {
        return new List<string>() { "1", "2", "3" };
        
        const string sql = "SELECT * FROM IpAddresses";
        return _connection.Query<string>( sql, commandType: CommandType.Text );
    }
    public async Task<IEnumerable<string>> GetValidIpAddressesAsync()
    {
        const string sql = "SELECT * FROM IpAddresses";
        return await _connection.QueryAsync<string>( sql, commandType: CommandType.Text );
    }

    public IEnumerable<string> GetValidUserKeys()
    {
        return new List<string>() { "a", "b", "c" };
        const string sql = "SELECT * FROM UserKeys";
        return _connection.Query<string>( sql, commandType: CommandType.Text );
    }
    public async Task<IEnumerable<string>> GetValidUserKeysAsync()
    {
        const string sql = "SELECT * FROM UserKeys";
        return await _connection.QueryAsync<string>( sql, commandType: CommandType.Text );
    }
    
    // Get Sql Connection
    SqlConnection CreateConnection() => new( _connectionString );
    async Task<SqlConnection> GetOpenConnection()
    {
        var connection = new SqlConnection( _connectionString );
        await connection.OpenAsync();
        return connection;
    }
}