using Onyx.Products.API.Models;
using Microsoft.AspNetCore.Mvc;
using Onyx.Products.API.Services;
using System.Runtime.CompilerServices;
using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Authorization;

namespace Onyx.Products.API.Controllers;

[Route("products")]
[ApiController]
public class ProductController(IProductRepository productRepository) : ControllerBase
{
    private readonly IProductRepository _productRepository = Guard.Against.Null(productRepository);

    [Authorize]
    [HttpPost()]
    public async Task<IActionResult> CreateProduct([FromBody] ProductSpec productSpec, CancellationToken cancellationToken)
    {
        var product = await _productRepository.CreateProduct(productSpec, cancellationToken);

        return CreatedAtRoute("GetProduct", new { id = product.ProductId }, product);
    }

    [Authorize]
    [HttpGet()]
    public async IAsyncEnumerable<Product> GetProducts([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var product in _productRepository.GetProducts(cancellationToken))
        {
            yield return product;
        }
    }

    [Authorize]
    [HttpGet("{colour}")]
    public async IAsyncEnumerable<Product> GetProducts(string colour, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var product in _productRepository.GetProducts(colour, cancellationToken))
        {
            yield return product;
        }
    }

    [Authorize]
    [HttpGet("{id:long}", Name = "GetProduct")]
    public async Task<IActionResult> GetProduct(long id, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetProduct(id, cancellationToken);

        if (product == null)
            return NotFound();
                        
        return Ok(product);
    }
}
