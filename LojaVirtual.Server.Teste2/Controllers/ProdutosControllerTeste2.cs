

using Microsoft.AspNetCore.Mvc.Testing;

namespace LojaVirtual.Server.Teste2.Controllers
{


    public class ProdutosControllerTest2 : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public ProdutosControllerTest2(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetProdutosV1_ReturnsOkResult()
        {
            // Arrange
            var client = _factory.CreateClient();
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://localhost:7209/api/v1/produtos")
            };

            // Act
            var response = await client.SendAsync(request);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
        }
    }
}
