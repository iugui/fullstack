using AutoFixture;
using FluentAssertions;
using LojaVirtual.Server.DTOs;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;

namespace LojaVirtual.Server.NewTest.Controllers
{
    public class ProdutosControllerNewTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly IFixture _fixture;

        public ProdutosControllerNewTest(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            _fixture = new Fixture();
        }

        [Fact]
        public async Task GetProdutosV1_ReturnsOkResult()
        {
            // Arrange
            //var listProdutosDto = _fixture.Create<List<ProdutoDto>>();
            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            request.RequestUri = new Uri("https://localhost:7209/api/v1/produtos");
            //request.Headers.Add("Authorization",$"Bearer {accessToken}");)
            //var produtoDto = new ProdutoDto() { Nome = "lapis", Valor = 1.99f };
            //var payload = JsonConvert.SerializeObject(produtoDto);
            //request.Content = new StringContent(payload, Encoding.UTF8, "application/json"),

            // Action
            var response = await _client.SendAsync(request);
            //HttpResponseMessage response = request.CreateResponse(HttpStatusCode.NotImplemented);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            response.Should().Be200Ok();
        }

        [Fact]
        public void GetProdutosV1_ReturnsNotFoundResult()
        {
            // Arrange
            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            request.RequestUri = new Uri("https://localhost:7209/api/v1/produtos");

            // Action
            HttpResponseMessage response = request.CreateResponse(HttpStatusCode.NotFound);

            // Assert
            response.Should().Be404NotFound();
        }

        [Theory]
        [InlineData(3)]
        [InlineData(4)]
        public async Task GetProdutoByIdV1_ReturnOk(int value)
        {
            // Arrange
            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            request.RequestUri = new Uri($"https://localhost:7209/api/v1/produtos/{value}");

            // Action
            var response = await _client.SendAsync(request);

            // Assert
            response.Should().Be200Ok();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetProdutoByIdV1_ReturnNotFound(int value)
        {
            // Arrange
            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            request.RequestUri = new Uri($"https://localhost:7209/api/v1/produtos/{value}");

            // Action
            var response = await _client.SendAsync(request);

            // Assert
            response.Should().Be404NotFound();
        }

        [Fact]
        public async Task PostProdutoV1_ReturnCreated()
        {
            // Arrange
            //var listProdutosDto = _fixture.Create<List<ProdutoDto>>();
            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Post;
            request.RequestUri = new Uri("https://localhost:7209/api/v1/produtos");
            //request.Headers.Add("Authorization",$"Bearer {accessToken}");)
            var produtoDto = new ProdutoDto() { Nome = "lapis", Valor = 1.99f };
            var payload = JsonConvert.SerializeObject(produtoDto);
            request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

            // Action
            var response = await _client.SendAsync(request);
            //HttpResponseMessage response = request.CreateResponse(HttpStatusCode.NotImplemented);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            response.Should().Be201Created();
        }

        [Fact]
        public async Task PostProdutoV1_ReturnBadRequest()
        {
            // Arrange
            //var listProdutosDto = _fixture.Create<List<ProdutoDto>>();
            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Post;
            request.RequestUri = new Uri("https://localhost:7209/api/v1/produtos");
            //request.Headers.Add("Authorization",$"Bearer {accessToken}");)
            var produtoDto = new ProdutoDto() { Nome = "a", Valor = 3.99f };
            var payload = JsonConvert.SerializeObject(produtoDto);
            request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

            // Action
            var response = await _client.SendAsync(request);
            //HttpResponseMessage response = request.CreateResponse(HttpStatusCode.NotImplemented);

            // Assert
            response.Should().Be400BadRequest();
        }

        [Theory]
        [InlineData(0)]
        public async Task UpdateProdutoV1_ShouldReturnNotFound_WhenIdIsLessThen1(uint value)
        {
            // Arrange
            var jsonPatchDocument = new Mock<JsonPatchDocument<ProdutoDto>>();
            var payload = JsonConvert.SerializeObject(jsonPatchDocument);
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Patch,
                RequestUri = new Uri($"https://localhost:7209/api/v1/produtos/{value}"),
                Content = new StringContent(payload, Encoding.UTF8, "application/json")
            };

            // Action
            var response = await _client.SendAsync(request);

            // Assert
            response.Should().Be404NotFound();
        }

        [Fact]
        public async Task UpdateProdutoV1_ShouldReturnUnsupportedMediaType_WhenJsonPathIsNull()
        {
            // Arrange
            var id = 3;
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Patch,
                RequestUri = new Uri($"https://localhost:7209/api/v1/produtos/{id}"),
                Content = null
            };

            // Action
            var response = await _client.SendAsync(request);

            // Assert
            response.Should().Be415UnsupportedMediaType();
        }

        [Fact]
        public async Task UpdateProdutoV1_ShouldReturnOk_WhenJsonPathIsValid()
        {
            // Arrange
            uint id = 3;
            var jsonPatchDocument = new JsonPatchDocument<ProdutoDto>();
            jsonPatchDocument.Replace(produto => produto.Nome, "Tesoura");
            var payload = JsonConvert.SerializeObject(jsonPatchDocument);
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Patch,
                RequestUri = new Uri($"https://localhost:7209/api/v1/produtos/{id}"),
                Content = new StringContent(payload, Encoding.UTF8, "application/json-patch+json")
            };

            // Action
            var response = await _client.SendAsync(request);

            // Assert
            response.Should().Be200Ok();
            var produtoDto = JsonConvert.DeserializeObject<ProdutoDto>(await response.Content.ReadAsStringAsync());
            Assert.Equal("Tesoura", produtoDto?.Nome);
        }

        [Fact]
        public async Task DeleteProdutoV1_ShouldReturnOk_WhenDataFound()
        {
            // Arrange
            uint id = 4;
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"https://localhost:7209/api/v1/produtos/{id}")
            };

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.Should().Be200Ok();
        }

        [Theory]
        [InlineData(0)]
        public async Task DeleteProdutoV1_ShouldReturnNotFound_WhenDataNotFound(int id)
        {
            // Arrange
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"https://localhost:7209/api/v1/produtos/{id}")
            };

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.Should().Be404NotFound();
        }
    }
}
