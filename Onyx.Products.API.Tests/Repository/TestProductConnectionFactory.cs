using Onyx.Products.API.Services;
using System.Data.Common;
using System.Data.SQLite;

namespace Onyx.Products.API.Tests.Repository;

public class TestProductConnectionFactory : IProductConnectionFactory
{
    public async Task<DbConnection> ConnectAsync(CancellationToken cancellationToken)
    {
        var connection = new SQLiteConnection("Data Source=C:\\Github\\onyx-products\\Onyx.Products.API.Tests\\TestProducts.db");
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}