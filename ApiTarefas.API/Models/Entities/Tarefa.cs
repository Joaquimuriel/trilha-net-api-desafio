using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ApiTarefas.API.Models.Enums;

namespace ApiTarefas.API.Models.Entities
{
    [Table("Tarefas")]
    public class Tarefa
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Título é obrigatório")]
        [StringLength(200, ErrorMessage = "Título deve ter no máximo 200 caracteres")]
        public string Titulo { get; set; } = string.Empty;
        
        [StringLength(1000, ErrorMessage = "Descrição deve ter no máximo 1000 caracteres")]
        public string? Descricao { get; set; }
        
        [Required(ErrorMessage = "Data é obrigatória")]
        public DateTime DataCriacao { get; set; } = DateTime.Now;
        
        public DateTime? DataConclusao { get; set; }
        
        public DateTime? DataVencimento { get; set; }
        
        [Required(ErrorMessage = "Status é obrigatório")]
        public StatusTarefa Status { get; set; } = StatusTarefa.Pendente;
        
        public int Prioridade { get; set; } = 3;
        
        public string? Categoria { get; set; }
        
        public string? Tags { get; set; }
        
        public bool Ativo { get; set; } = true;
        
        public DateTime DataAtualizacao { get; set; } = DateTime.Now;
    }
}
