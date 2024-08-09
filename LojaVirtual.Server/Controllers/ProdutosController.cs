using Asp.Versioning;
using LojaVirtual.Server.Contexts;
using LojaVirtual.Server.DTOs;
using LojaVirtual.Server.Models;
using LojaVirtual.Server.Repositories;
using Mapster;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LojaVirtual.Server.Controllers
{
    
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly IProdutosRepository _produtosRepository;
        public ProdutosController(IProdutosRepository  produtosRepository)
        {
            _produtosRepository = produtosRepository;
        }

        // GET: api/v?/produtos
        /// <summary>
        /// Recupera os produtos do banco de dados
        /// </summary>
        /// <returns>Uma lista de produtos</returns>
        /// <response code="200">Retorna uma lista de produtos</response>
        /// <response code="404">Não encontrou nenhum produto</response>
        /// <response code="500">Algum erro aconteceu</response>
        [HttpGet]
        [EnableCors]
        [MapToApiVersion("1.0")]
        [ProducesResponseType<List<ProdutoDto>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<Results<NotFound, Ok<List<ProdutoDto>>>> GetProdutosV1()
        {
            var produtosDto = await _produtosRepository.GetProdutosAsync();
            if (produtosDto is null || produtosDto.Count == 0) { return TypedResults.NotFound(); }
            return TypedResults.Ok(produtosDto);
        }

        // GET: api/v?/produtos/id
        /// <summary>
        /// Recupera um produto específico do banco de dados
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Um produto específico</returns>
        /// <response code="200">Retorna um produto específico</response>
        /// <response code="404">Não encontrou nenhum produto com o id passado</response>
        /// <response code="500">Algum erro aconteceu</response>
        [HttpGet]
        [MapToApiVersion("1.0")]
        [Route("{id:int:required:min(1)}", Name = "GetProduto")]
        [ProducesResponseType<ProdutoDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<Results<NotFound, Ok<ProdutoDto>>> GetProdutoByIdV1([FromRoute] uint id)
        {
            var produtoDto = await _produtosRepository.GetProdutoAsync(id);
            if (produtoDto is null) { return TypedResults.NotFound(); }
            return TypedResults.Ok(produtoDto);
        }

        // POST: api/v?/produtos
        /// <summary>
        /// Cria um produto novo.
        /// </summary>
        /// <returns>O novo produto criado</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /produtos
        ///     {
        ///        "nome": "produto",
        ///        "descricao": "descricao do produto",
        ///        "preco": 9.99
        ///     }
        ///
        /// </remarks>
        /// <response code="201">Se o item foi criado com sucesso</response>
        /// <response code="400">Se o item não foi criado com sucesso</response>
        [HttpPost]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProdutoDto>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<Results<BadRequest, Created<ProdutoDto>>> PostProdutoV1([FromBody] ProdutoDto novoProdutoDto)
        {
            if (!ModelState.IsValid)
            {
                return TypedResults.BadRequest();
            }
            var produto = await _produtosRepository.PostProdutoAsync(novoProdutoDto);
            return TypedResults.Created($"produtos/{produto.Id}", novoProdutoDto);
        }

        // PATCH: api/v?/produtos/id
        /// <summary>
        /// Atualiza um campo de um produto
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PATCH /produtos/id
        ///     [
        ///         {
        ///             "path": "/nome",
        ///             "op": "replace",
        ///             "value": "caneta"
        ///         },
        ///         {
        ///             "path" : "/descricao",
        ///             "op" : "replace",
        ///             "value" : "nova descrição da caneta"
        ///         }
        ///     ]
        ///
        /// </remarks>
        /// <response code="200">Se a atualização for feita com sucesso</response>
        /// <response code="404">Se for passado um id inválido</response>
        /// <response code="400">Se o id for menor que 1 ou se a atualização for nula</response>
        [HttpPatch]
        [Route("{id:int:required:min(1)}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        public async Task<Results<NotFound, Ok<ProdutoDto>>> UpdateProdutoV1([FromRoute]uint id, [FromBody] JsonPatchDocument<ProdutoDto> jsonPatchDocument)
        {
            var produtoDto = await _produtosRepository.UpdateProdutoAsync(id, jsonPatchDocument);
            if (produtoDto is null) {return TypedResults.NotFound(); }
            return TypedResults.Ok(produtoDto);
        }

        // DELETE: api/v?/produtos/id
        /// <summary>
        /// Deleta um produto específico do banco de dados
        /// </summary>
        /// <param name="id"></param>
        /// <returns>O produto deletado</returns>
        /// <response code="200">Retorna o produto deletado</response>
        /// <response code="404">Não encontrou nenhum produto com o id passado</response>
        /// <response code="500">Algum erro aconteceu</response>
        [HttpDelete]
        [Route("{id:int:required:min(1)}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<Results<NotFound, Ok<ProdutoDto>>> DeleteProdutoV1([FromRoute]uint id)
        {
            var produtoDto = await _produtosRepository.DeleteProdutoAsync(id);
            if (produtoDto is null) { return TypedResults.NotFound(); }
            return TypedResults.Ok(produtoDto);
        }
    }
}
