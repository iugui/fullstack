using System.ComponentModel.DataAnnotations;

namespace LojaVirtual.Server.DTOs
{
    public class ProdutoDto
    {
        [StringLength(255, MinimumLength = 3, ErrorMessage = "O nome deve possuir no mínimo 3 letras e no máximo 255")]
        public required string Nome { get; set; }
        public string? Descricao { get; set; }
        [Range(0.01, 99999)]
        public required float Valor { get; set; }
    }
}
