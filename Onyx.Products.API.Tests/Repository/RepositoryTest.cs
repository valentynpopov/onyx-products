using Onyx.Products.API.Services;
using Dapper;
using System.ComponentModel.DataAnnotations;
using AutoFixture;
using Onyx.Products.API.Models;
using System.Net.Http.Headers;

namespace Onyx.Products.API.Tests.Repository
{
    public class RepositoryTest
    {
        [Fact]
        public async void GetProducts()
        {
            var factory = new TestProductConnectionFactory();
            var sut = new ProductRepository(factory);

            var products = await sut.GetProducts(new CancellationToken()).ToListAsync();

            Assert.NotEmpty(products);
        }

        [Fact]
        public async void GetProductsByColour()
        {
            var factory = new TestProductConnectionFactory();

            string colour;

            using (var connection = await factory.ConnectAsync(new CancellationToken()))
            {
                colour = connection.QueryFirst<string>("SELECT Colour FROM Product");
            }

            var sut = new ProductRepository(factory);

            var products = await sut.GetProducts(colour, new CancellationToken()).ToListAsync();

            Assert.NotEmpty(products);
            Assert.All(products, p => string.Equals(p.Colour, colour, StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async void GetProduct()
        {
            var factory = new TestProductConnectionFactory();

            long productId;

            using (var connection = await factory.ConnectAsync(new CancellationToken()))
            {
                productId = connection.QueryFirst<long>("SELECT ProductId FROM Product");
            }

            var sut = new ProductRepository(factory);

            var product = await sut.GetProduct(productId, new CancellationToken());

            Assert.NotNull(product);
            Assert.Equal(productId, product.ProductId);
        }

        [Fact]
        public async void CreateProduct()
        {
            var factory = new TestProductConnectionFactory();
            async Task<long> GetCount()
            {
                using var connection = await factory.ConnectAsync(new CancellationToken());
                return await connection.QuerySingleAsync<long>("SELECT COUNT(*) FROM Product");
            }
            var countBefore = await GetCount();

            var sut = new ProductRepository(factory);

            const int createdProductCount = 10;
            var createdProducts = new List<Product>();

            var fixture = new Fixture();
            var productSpecs = fixture.CreateMany<ProductSpec>(createdProductCount).ToList();
            foreach(var productSpec in productSpecs)
            {
                var createdProduct = await sut.CreateProduct(productSpec, new CancellationToken());

                Assert.Equal(productSpec.SKU, createdProduct.SKU);
                Assert.Equal(productSpec.Name, createdProduct.Name);
                Assert.Equal(productSpec.Colour, createdProduct.Colour);
                Assert.Equal(productSpec.Description, createdProduct.Description);

                createdProducts.Add(createdProduct);
            }
            var countAfter = await GetCount();

            Assert.Equal(countAfter, countBefore + createdProductCount);

            // db cleanup
            var createdProductIds = createdProducts.Select(p => p.ProductId);
            using var connection = await factory.ConnectAsync(new CancellationToken());
            await connection.ExecuteAsync("DELETE FROM Product WHERE ProductId IN @createdProductIds", new { createdProductIds });
        }
    }
}
