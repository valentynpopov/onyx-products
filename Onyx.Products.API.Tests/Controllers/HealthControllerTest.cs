using Microsoft.AspNetCore.Mvc;
using Moq;
using Onyx.Products.API.Controllers;
using Onyx.Products.API.Services;
using System.Net;

namespace Onyx.Products.API.Tests.Controllers
{
    public class HealthControllerTest
    {
        [Fact]
        public async void Check_Ok()
        {
            var productConnectionFactoryMock = new Mock<IProductConnectionFactory>();
            var sut = new HealthController(productConnectionFactoryMock.Object);

            var result = await sut.Check(new CancellationToken());

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async void Check_ServiceUnavailable()
        {
            var productConnectionFactoryMock = new Mock<IProductConnectionFactory>();
            productConnectionFactoryMock
                .Setup(m => m.ConnectAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Some database exception"));
            var sut = new HealthController(productConnectionFactoryMock.Object);

            var result = await sut.Check(new CancellationToken());

            Assert.IsType<ObjectResult>(result);
            Assert.Equal((int?)HttpStatusCode.ServiceUnavailable, ((ObjectResult)result).StatusCode);
        }
    }
}
