using AutoFixture;
using FluentAssertions;
using LojaVirtual.Server.Controllers;
using LojaVirtual.Server.DTOs;
using LojaVirtual.Server.Models;
using LojaVirtual.Server.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Moq;

#pragma warning disable

namespace LojaVirtual.Server.Teste.Controllers
{
    public class ProdutosControllerTeste
    {
        private readonly IFixture _fixture; // Utilizado para criar dados fake
        private readonly Mock<IProdutosRepository> _repoMock; // Utilizado para criar um repositório fake
        private readonly ProdutosController _controller; // Será uma instância do nosso controlador

        public ProdutosControllerTeste()
        {
            _fixture = new Fixture(); // vai criar os nossos dadaos
            _repoMock = new Mock<IProdutosRepository>(); // Representará o repositório fake
            //_repoMock = _fixture.Freeze<Mock<IProdutosRepository>>();
            _controller = new ProdutosController(_repoMock.Object); // Cria a implementação em memória
        }

        [Fact]
        public async Task GetProdutos_ShouldReturnOkResponse_WhenDataFound()
        {
            //--------------------- Arrange ------------------------------- //
            // Criando produtos fakes a serem retornados quanco chamarmos o método GetProdutosAsync do repositório.
            var produtosEsperadosMock = _fixture.Create<List<ProdutoDto>>();
            // Configurando que quando chamamarmos o método GetProdutosAsync no reposítório, queremos como resultado produtosEsperadosMock
            _repoMock.Setup(repo => repo.GetProdutosAsync()).ReturnsAsync(produtosEsperadosMock);

            //---------------------- Act ----------------------------------//
            var result = await _controller.GetProdutosV1();

            //--------------------- Arrange --------------------------------//
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<Results<NotFound, Ok<List<ProdutoDto>>>>();
            result.Result.Should().BeAssignableTo<Ok<List<ProdutoDto>>>();
            result.Result.As<Ok<List<ProdutoDto>>>().Value.Should().NotBeNull().And.BeOfType(produtosEsperadosMock.GetType());
            // Ou seja, ambos os valores são uma List<ProdutosDto>
            _repoMock.Verify(repo => repo.GetProdutosAsync(), Times.Once());
        }

        [Fact]
        public async Task GetProdutos_ShouldReturnNotFound_WhenDataNotFound()
        {
            // Arrange
            List<ProdutoDto> listaProdutosDto = [];
            _repoMock.Setup(repo => repo.GetProdutosAsync()).ReturnsAsync(listaProdutosDto);

            // Act
            var result = await _controller.GetProdutosV1();

            //Asser
            result.Should().NotBeNull();
            result.Result.Should().BeAssignableTo<NotFound>();
            _repoMock.Verify(repo => repo.GetProdutosAsync(), Times.Once());
        }

        [Fact]
        public async Task GetProdutoByIdV1_ShouldReturnOkResponse_WhenDataFound()
        {
            // Arrange
            var produtoDtoMock = _fixture.Create<ProdutoDto>();
            var id = _fixture.Create<uint>();
            _repoMock.Setup(repo => repo.GetProdutoAsync(id)).ReturnsAsync(produtoDtoMock);

            // Act
            var result = await _controller.GetProdutoByIdV1(id);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<Results<NotFound, Ok<ProdutoDto>>>();
            result.Result.Should().BeAssignableTo<Ok<ProdutoDto>>();
            result.Result.As<Ok<ProdutoDto>>().Value
                .Should().NotBeNull()
                .And.BeOfType(produtoDtoMock.GetType());
            _repoMock.Verify(repo => repo.GetProdutoAsync(id), Times.Once());
        }

        [Fact]
        public async Task GetProdutoByIdV1_ShouldReturnoNotFoundResponse_WhenNoDataFound()
        {
            // Arrange
            //#pragma warning disable
            ProdutoDto produtoDto = null;
            var id = _fixture.Create<uint>();
            _repoMock.Setup(repo => repo.GetProdutoAsync(id)).ReturnsAsync(produtoDto);

            // Act
            var result = await _controller.GetProdutoByIdV1(id);

            // Assert
            result.Should().NotBeNull();
            result.Result.Should().BeAssignableTo<NotFound>();
            _repoMock.Verify(repo => repo.GetProdutoAsync(id), Times.Once());
        }

        [Fact]
        public async Task PostProdutoV1_ShouldReturnOkResponse_WhenInputIsValid()
        {
            // Arrange
            var produtoRequestDto = _fixture.Create<ProdutoDto>();
            var produtoResponse = _fixture.Create<Produto>();
            _repoMock.Setup(x => x.PostProdutoAsync(produtoRequestDto)).ReturnsAsync(produtoResponse);

            // Act
            var result = await _controller.PostProdutoV1(produtoRequestDto);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<Results<BadRequest, Created<ProdutoDto>>>();
            result.Result.Should().BeAssignableTo<Created<ProdutoDto>>();
            _repoMock.Verify(repo => repo.PostProdutoAsync(produtoRequestDto), Times.Once());
        }

        [Fact]
        public async Task PostProdutoV1_ShouldReturnBadRequest_WhenInputIsInvalid()
        {
            // Arrange
            var request = _fixture.Create<ProdutoDto>();
            _controller.ModelState.AddModelError("Nome", "O campo Nome é obrigatório");

            // Act
            var result = await _controller.PostProdutoV1(request);

            // Assert
            result.Should().NotBeNull();
            result.Result.Should().BeAssignableTo<BadRequest>();
            _repoMock.Verify(repo => repo.PostProdutoAsync(request), Times.Never());
        }

        [Fact]
        public async Task UpdateProdutoV1_ShouldReturnNotFound_WhenInputIsLessThen1()
        {
            // Arrange
            uint id = 0;
            var jsonPatchDocument = new Mock<JsonPatchDocument<ProdutoDto>>();

            // Act
            var result = await _controller.UpdateProdutoV1(id, jsonPatchDocument.Object);

            // Assert
            result.Should().NotBeNull();
            result.Result.Should().BeAssignableTo<NotFound>();
            //_repoMock.Verify(repo => repo.UpdateProdutoAsync(id, jsonPatchDocument.Object), Times.Never());
        }

        [Fact]
        public async Task UpdateProdutoV1_ShouldReturnNotFound_WhenNoDataFound()
        {
            // Arrange
            uint id = 9999;
            var jsonPatchDocument = new Mock<JsonPatchDocument<ProdutoDto>>();
            ProdutoDto produtoDto = null;
            _repoMock.Setup(repo => repo.UpdateProdutoAsync(id, jsonPatchDocument.Object)).ReturnsAsync(produtoDto);

            // Act
            var result = await _controller.UpdateProdutoV1(id, jsonPatchDocument.Object);

            // Assert
            result.Should().NotBeNull();
            result.Result.Should().BeAssignableTo<NotFound>();
            _repoMock.Verify(repo => repo.UpdateProdutoAsync(id, jsonPatchDocument.Object), Times.Once());
        }

        [Fact]
        public async Task UpdateProdutoV1_ShouldReturnOk_WhenDataFound()
        {
            // Arrange
            var id = _fixture.Create<uint>();
            var jsonPatchDocument = new Mock<JsonPatchDocument<ProdutoDto>>();
            var produtoDto = _fixture.Create<ProdutoDto>();
            _repoMock.Setup(repo => repo.UpdateProdutoAsync(id, jsonPatchDocument.Object)).ReturnsAsync( produtoDto);

            // Act
            var result = await _controller.UpdateProdutoV1(id, jsonPatchDocument.Object);

            // Assert
            result.Should().NotBeNull();
            result.Result.Should().BeAssignableTo<Ok<ProdutoDto>>();
            _repoMock.Verify(repo => repo.UpdateProdutoAsync(id, jsonPatchDocument.Object), Times.Once());
        }

        [Fact]
        public async Task DeleteProdutoV1_ShouldReturnNotFound_WhenInputIsLessThen1()
        {
            // Arrange
            uint id = 0;

            // Act
            var result = await _controller.DeleteProdutoV1(id);

            // Assert
            result.Should().NotBeNull();
            result.Result.Should().BeAssignableTo<NotFound>();
            //_repoMock.Verify(repo => repo.DeleteProdutoAsync(id), Times.Never());
        }

        [Fact]
        public async Task DeleteProdutoV1_ShouldReturnNotFound_WhenDataNotFound()
        {
            // Arrange
            uint id = 9999;
            ProdutoDto produtoDto = null;
            _repoMock.Setup(repo => repo.DeleteProdutoAsync(id)).ReturnsAsync(produtoDto);

            // Act
            var result = await _controller.DeleteProdutoV1(id);

            // Assert
            result.Should().NotBeNull();
            result.Result.Should().BeAssignableTo<NotFound>();
            _repoMock.Verify(repo => repo.DeleteProdutoAsync(id), Times.Once());
        }

        [Fact]
        public async Task DeleteProdutoV1_ShouldReturnOk_WhenDataFound()
        {
            // Arrange
            var id = _fixture.Create<uint>();
            var produtoDto = _fixture.Create<ProdutoDto>();
            _repoMock.Setup(repo => repo.DeleteProdutoAsync(id)).ReturnsAsync(produtoDto);

            // Act
            var result = await _controller.DeleteProdutoV1(id);

            // Assert
            result.Should().NotBeNull();
            result.Result.Should().BeAssignableTo<Ok<ProdutoDto>>();
            _repoMock.Verify(repo => repo.DeleteProdutoAsync(id), Times.Once());
        }
    }
}
