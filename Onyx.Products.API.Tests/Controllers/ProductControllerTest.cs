using AutoFixture.Xunit2;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Onyx.Products.API.Controllers;
using Onyx.Products.API.Models;
using Onyx.Products.API.Services;

namespace Onyx.Products.API.Tests.Controllers;

public class ProductControllerTest
{
    [Theory, AutoData]
    public async void GetProduct_Ok(Product product)
    {
        var productRepositoryMock = new Mock<IProductRepository>();
        
        productRepositoryMock
            .Setup(m => m.GetProduct(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var sut = new ProductController(productRepositoryMock.Object);

        var result = await sut.GetProduct(product.ProductId, It.IsAny<CancellationToken>());

        Assert.IsType<OkObjectResult>(result);
    }

    [Theory, AutoData]
    public async void GetProduct_NotFound(long productId)
    {
        var productRepositoryMock = new Mock<IProductRepository>();

        productRepositoryMock
            .Setup(m => m.GetProduct(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var sut = new ProductController(productRepositoryMock.Object);

        var result = await sut.GetProduct(productId, It.IsAny<CancellationToken>());

        Assert.IsType<NotFoundResult>(result);
    }

    [Theory, AutoData]
    public async void GetProducts(Product product)
    {
        var productRepositoryMock = new Mock<IProductRepository>();

        var products = new[] { product, product, product };
        productRepositoryMock
            .Setup(m => m.GetProducts(It.IsAny<CancellationToken>()))
            .Returns(products.ToAsyncEnumerable());

        var sut = new ProductController(productRepositoryMock.Object);

        await foreach (var actulProduct in sut.GetProducts(It.IsAny<CancellationToken>()))
        {
            Assert.Equal(product, actulProduct);
        }
    }

    [Theory, AutoData]
    public async void GetProductsByColour(Product product)
    {
        var productRepositoryMock = new Mock<IProductRepository>();

        var products = new[] { product, product, product };
        productRepositoryMock
            .Setup(m => m.GetProducts(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(products.ToAsyncEnumerable());

        var sut = new ProductController(productRepositoryMock.Object);

        await foreach (var actulProduct in sut.GetProducts(It.IsAny<string>(), It.IsAny<CancellationToken>()))
        {
            Assert.Equal(product, actulProduct);
        }
    }
}