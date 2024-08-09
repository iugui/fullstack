using LojaVirtual.Server.DTOs;
using LojaVirtual.Server.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace LojaVirtual.Server.Repositories
{
    public interface IProdutosRepository
    {
        public Task<List<ProdutoDto>> GetProdutosAsync();
        public Task<ProdutoDto> GetProdutoAsync(uint id);
        public Task<Produto> PostProdutoAsync(ProdutoDto produtoDto);
        public Task<ProdutoDto> UpdateProdutoAsync(uint id, JsonPatchDocument<ProdutoDto> jsonPatchDocument);
        public Task<ProdutoDto> DeleteProdutoAsync(uint id);
    }
}
