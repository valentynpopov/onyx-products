using Onyx.Products.API.Models;

namespace Onyx.Products.API.Services;

public interface IProductRepository
{
    Task<Product> CreateProduct(ProductSpec productSpec, CancellationToken cancellationToken);
    Task<Product?> GetProduct(long id, CancellationToken cancellationToken);
    IAsyncEnumerable<Product> GetProducts(CancellationToken cancellationToken);
    IAsyncEnumerable<Product> GetProducts(string colour, CancellationToken cancellationToken);
}
