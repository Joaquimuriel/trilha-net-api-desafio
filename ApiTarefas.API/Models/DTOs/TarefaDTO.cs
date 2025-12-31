using System.ComponentModel.DataAnnotations;
using ApiTarefas.API.Models.Enums;

namespace ApiTarefas.API.Models.DTOs
{
public class TarefaDTO
{
public int Id { get; set; }
public string Titulo { get; set; } = string.Empty;
public string? Descricao { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataConclusao { get; set; }
        public DateTime? DataVencimento { get; set; }
        public string Status { get; set; } = string.Empty;
        public int Prioridade { get; set; }
        public string? Categoria { get; set; }
        public string[]? Tags { get; set; }
    }

    public class CriarTarefaDTO
    {
        [Required(ErrorMessage = "Título é obrigatório")]
        [StringLength(200, ErrorMessage = "Título deve ter no máximo 200 caracteres")]
        public string Titulo { get; set; } = string.Empty;
        
        [StringLength(1000, ErrorMessage = "Descrição deve ter no máximo 1000 caracteres")]
        public string? Descricao { get; set; }
        
        public DateTime? DataVencimento { get; set; }
        public int Prioridade { get; set; } = 3;
        public string? Categoria { get; set; }
        public string[]? Tags { get; set; }
    }

    public class AtualizarTarefaDTO
    {
        [Required(ErrorMessage = "Título é obrigatório")]
        [StringLength(200, ErrorMessage = "Título deve ter no máximo 200 caracteres")]
        public string Titulo { get; set; } = string.Empty;
        
        [StringLength(1000, ErrorMessage = "Descrição deve ter no máximo 1000 caracteres")]
        public string? Descricao { get; set; }
        
        public DateTime? DataVencimento { get; set; }
        public StatusTarefa Status { get; set; }
        public int Prioridade { get; set; }
        public string? Categoria { get; set; }
        public string[]? Tags { get; set; }
    }

    public class FiltrarTarefaDTO
    {
        public string? Titulo { get; set; }
        public string? Status { get; set; }
        public string? Categoria { get; set; }
        public string? Tag { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public int? Prioridade { get; set; }
        public bool? Concluidas { get; set; }
        public int Pagina { get; set; } = 1;
        public int TamanhoPagina { get; set; } = 20;
        public string OrdenarPor { get; set; } = "DataCriacao";
        public bool OrdemDescendente { get; set; } = true;
    }
}
