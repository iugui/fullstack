using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LojaVirtual.Server.Models
{
    [Table("Produtos")]
    public class Produto
    {
        public uint Id { get; set; }
        [StringLength(255, MinimumLength = 3, ErrorMessage = "O nome deve possuir no mínimo 3 letras e no máximo 255")]
        public required string Nome { get; set; }
        public string? Descricao { get; set; }
        [Range(0,99999)]
        public required float Valor { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.Now;
    }
}
