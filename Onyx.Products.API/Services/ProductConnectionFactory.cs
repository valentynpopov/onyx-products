using Ardalis.GuardClauses;
using System.Data.Common;
using System.Data.SQLite;

namespace Onyx.Products.API.Services;

public class ProductConnectionFactory(IConfiguration configuration) : IProductConnectionFactory
{
    private readonly IConfiguration configuration = Guard.Against.Null(configuration);

    public async Task<DbConnection> ConnectAsync(CancellationToken cancellationToken)
    {
        var connectionString = Guard.Against.NullOrEmpty(
            configuration.GetConnectionString("Products"));

        var connection = new SQLiteConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }        
}