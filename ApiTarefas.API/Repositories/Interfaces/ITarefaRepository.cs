using ApiTarefas.API.Models.Entities;
using ApiTarefas.API.Models.DTOs;

namespace ApiTarefas.API.Repositories.Interfaces
{
    public interface ITarefaRepository
    {
        Task<Tarefa?> ObterPorIdAsync(int id);
        Task<IEnumerable<Tarefa>> ObterTodosAsync();
        Task<IEnumerable<Tarefa>> FiltrarAsync(FiltrarTarefaDTO filtro);
        Task<Tarefa> CriarAsync(Tarefa tarefa);
        Task<Tarefa> AtualizarAsync(Tarefa tarefa);
        Task<bool> ExcluirAsync(int id);
        Task<bool> ExisteAsync(int id);
        Task<int> ContarAsync(FiltrarTarefaDTO? filtro = null);
    }
}