using Ardalis.GuardClauses;
using Dapper;
using Onyx.Products.API.Models;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Onyx.Products.API.Services;

public class ProductRepository(IProductConnectionFactory productConnectionFactory) : IProductRepository
{
    private readonly IProductConnectionFactory _productConnectionFactory = Guard.Against.Null(productConnectionFactory);

    public async Task<Product> CreateProduct(ProductSpec productSpec, CancellationToken cancellationToken)
    {
        using var connection = await _productConnectionFactory.ConnectAsync(cancellationToken);

        const string sql = "INSERT INTO Product(SKU, Name, Colour, Description) " +
            "VALUES (@SKU, @Name, @Colour, @Description) " +
            "RETURNING ProductId, SKU, Name, Colour, Description";

        return await connection.QuerySingleAsync<Product>(
            sql, 
            new { productSpec.SKU, productSpec.Name, productSpec.Colour, productSpec.Description });
    }

    public async IAsyncEnumerable<Product> GetProducts(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using var connection = await _productConnectionFactory.ConnectAsync(cancellationToken);

        const string sql = "SELECT ProductId, SKU, Name, Colour, Description FROM Product";

        await foreach(var product in connection.QueryUnbufferedAsync<Product>(sql))
        {
            if (cancellationToken.IsCancellationRequested)
                yield break;

            yield return product;
        }
    }

    public async IAsyncEnumerable<Product> GetProducts(
        string colour,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using var connection = await _productConnectionFactory.ConnectAsync(cancellationToken);

        const string sql = "SELECT ProductId, SKU, Name, Colour, Description FROM Product " +
            "WHERE colour = @colour COLLATE NOCASE";

        await foreach (var product in connection.QueryUnbufferedAsync<Product>(sql, new { colour }))
        {
            if (cancellationToken.IsCancellationRequested)
                yield break;

            yield return product;
        }
    }

    public async Task<Product?> GetProduct(long id, CancellationToken cancellationToken)
    {
        using var connection = await _productConnectionFactory.ConnectAsync(cancellationToken);

        const string sql = "SELECT ProductId, SKU, Name, Colour, Description FROM Product " +
        "WHERE ProductId = @id";

        return await connection.QuerySingleOrDefaultAsync<Product>(sql, new { id });
    }
}
