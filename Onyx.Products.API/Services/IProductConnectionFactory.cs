
using System.Data.Common;

namespace Onyx.Products.API.Services;

public interface IProductConnectionFactory
{
    Task<DbConnection> ConnectAsync(CancellationToken cancellationToken);
}