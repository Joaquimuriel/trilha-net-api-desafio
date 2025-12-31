using ApiTarefas.API.Models.DTOs;

namespace ApiTarefas.API.Services.Interfaces
{
    public interface ITarefaService
    {
        Task<TarefaDTO?> ObterPorIdAsync(int id);
        Task<IEnumerable<TarefaDTO>> ObterTodasAsync();
        Task<IEnumerable<TarefaDTO>> FiltrarAsync(FiltrarTarefaDTO filtro);
        Task<TarefaDTO> CriarAsync(CriarTarefaDTO dto);
        Task<TarefaDTO?> AtualizarAsync(int id, AtualizarTarefaDTO dto);
        Task<bool> ExcluirAsync(int id);
        Task<bool> ConcluirTarefaAsync(int id);
        Task<(IEnumerable<TarefaDTO> Tarefas, int Total, int Paginas)> ObterPaginadoAsync(FiltrarTarefaDTO filtro);
    }
}