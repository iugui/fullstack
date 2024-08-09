using LojaVirtual.Server.Contexts;
using LojaVirtual.Server.DTOs;
using LojaVirtual.Server.Models;
using Mapster;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace LojaVirtual.Server.Repositories
{
    public class ProdutoRepository : IProdutosRepository
    {
        private AppDbContext _context;
        public ProdutoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProdutoDto>> GetProdutosAsync()
        {
            var produtos = await _context.Produtos.AsNoTracking().ToListAsync();
            #pragma warning disable
            if (produtos.Count == 0 || produtos is null) { return null; }
            return produtos.Adapt<List<ProdutoDto>>();
        }

        public async Task<ProdutoDto> GetProdutoAsync(uint id) 
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto is null) { return null; }
            var produtoDto = produto.Adapt<ProdutoDto>();
            return produtoDto;
        }

        public async Task<Produto> PostProdutoAsync(ProdutoDto produtoDto) 
        {
            var produto = produtoDto.Adapt<Produto>();
            await _context.AddAsync(produto);
            await _context.SaveChangesAsync();
            return produto;
        }

        public async Task<ProdutoDto> UpdateProdutoAsync(uint id, JsonPatchDocument<ProdutoDto> jsonPatchDocument)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto is null) { return null; }

            var produtoDto = new ProdutoDto
            {
                Nome = produto.Nome,
                Descricao = produto.Descricao,
                Valor = produto.Valor,
            };

            // Aplicando as alterações ao Dto
            jsonPatchDocument.ApplyTo(produtoDto);

            produto.Nome = produtoDto.Nome;
            produto.Descricao = produtoDto.Descricao;
            produto.Valor = produtoDto.Valor;

            await _context.SaveChangesAsync();
            return produtoDto;
        }

        public async Task<ProdutoDto> DeleteProdutoAsync(uint id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto is null) { return null; }
            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();
            var produtoDto = produto.Adapt<ProdutoDto>();
            return produtoDto;
        }
    }
}
