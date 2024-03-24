using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

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
    public async Task OpenConnection()
    {
        _connection = await GetOpenConnection();
    }
    
    // Public Methods
    public IEnumerable<string> GetValidIpAddresses()
    {
        const string sql = "SELECT * FROM IpAddresses";
        return _connection.Query<string>( sql, commandType: CommandType.Text );
    }
    public async Task<IEnumerable<string>> GetValidIpAddressesAsync()
    {
        const string sql = "SELECT * FROM IpAddresses";
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