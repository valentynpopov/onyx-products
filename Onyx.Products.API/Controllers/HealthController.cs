using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Mvc;
using Onyx.Products.API.Services;
using System.Net;

namespace Onyx.Products.API.Controllers;

[Route("health")]
[ApiController]
public class HealthController(IProductConnectionFactory productConnectionFactory) : ControllerBase
{
    private readonly IProductConnectionFactory _productConnectionFactory = Guard.Against.Null(productConnectionFactory);

    [HttpGet()]
    public async Task<IActionResult> Check(CancellationToken cancellationToken)
    {
        try
        {
            using var _ = await _productConnectionFactory.ConnectAsync(cancellationToken);
        }
        catch
        {
            return Problem(statusCode: (int?)HttpStatusCode.ServiceUnavailable);
        }
        return Ok();
    }
}