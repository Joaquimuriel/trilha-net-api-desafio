using ApiTarefas.API.Models.Entities;
using ApiTarefas.API.Models.DTOs;
using ApiTarefas.API.Models.Enums; 
using ApiTarefas.API.Repositories.Interfaces;
using ApiTarefas.API.Services.Interfaces;

namespace ApiTarefas.API.Services.Implementations
{
    public class TarefaService : ITarefaService
    {
        private readonly ITarefaRepository _repository;
        private readonly ILogger<TarefaService> _logger;

        public TarefaService(ITarefaRepository repository, ILogger<TarefaService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<TarefaDTO?> ObterPorIdAsync(int id)
        {
            try
            {
                var tarefa = await _repository.ObterPorIdAsync(id);
                return tarefa != null ? MapToDTO(tarefa) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter tarefa com ID {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<TarefaDTO>> ObterTodasAsync()
        {
            try
            {
                var tarefas = await _repository.ObterTodosAsync();
                return tarefas.Select(MapToDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter todas as tarefas");
                throw;
            }
        }

        public async Task<IEnumerable<TarefaDTO>> FiltrarAsync(FiltrarTarefaDTO filtro)
        {
            try
            {
                var tarefas = await _repository.FiltrarAsync(filtro);
                return tarefas.Select(MapToDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao filtrar tarefas");
                throw;
            }
        }

        public async Task<TarefaDTO> CriarAsync(CriarTarefaDTO dto)
        {
            try
            {
                var tarefa = new Tarefa
                {
                    Titulo = dto.Titulo,
                    Descricao = dto.Descricao,
                    DataVencimento = dto.DataVencimento,
                    Prioridade = dto.Prioridade,
                    Categoria = dto.Categoria,
                    Tags = dto.Tags != null ? string.Join(",", dto.Tags) : null,
                    Status = StatusTarefa.Pendente,
                    DataCriacao = DateTime.Now,
                    DataAtualizacao = DateTime.Now,
                    Ativo = true
                };

                var tarefaCriada = await _repository.CriarAsync(tarefa);
                return MapToDTO(tarefaCriada);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar tarefa");
                throw;
            }
        }

        public async Task<TarefaDTO?> AtualizarAsync(int id, AtualizarTarefaDTO dto)
        {
            try
            {
                var tarefa = await _repository.ObterPorIdAsync(id);
                if (tarefa == null) return null;

                tarefa.Titulo = dto.Titulo;
                tarefa.Descricao = dto.Descricao;
                tarefa.DataVencimento = dto.DataVencimento;
                tarefa.Status = dto.Status;
                tarefa.Prioridade = dto.Prioridade;
                tarefa.Categoria = dto.Categoria;
                tarefa.Tags = dto.Tags != null ? string.Join(",", dto.Tags) : null;
                tarefa.DataAtualizacao = DateTime.Now;

                // Se a tarefa foi concluída, atualiza a data de conclusão
                if (dto.Status == StatusTarefa.Concluida && tarefa.DataConclusao == null)
                    tarefa.DataConclusao = DateTime.Now;
                else if (dto.Status != StatusTarefa.Concluida)
                    tarefa.DataConclusao = null;

                var tarefaAtualizada = await _repository.AtualizarAsync(tarefa);
                return MapToDTO(tarefaAtualizada);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar tarefa com ID {Id}", id);
                throw;
            }
        }

        public async Task<bool> ExcluirAsync(int id)
        {
            try
            {
                return await _repository.ExcluirAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir tarefa com ID {Id}", id);
                throw;
            }
        }

        public async Task<bool> ConcluirTarefaAsync(int id)
        {
            try
            {
                var tarefa = await _repository.ObterPorIdAsync(id);
                if (tarefa == null) return false;

                tarefa.Status = StatusTarefa.Concluida;
                tarefa.DataConclusao = DateTime.Now;
                tarefa.DataAtualizacao = DateTime.Now;

                await _repository.AtualizarAsync(tarefa);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao concluir tarefa com ID {Id}", id);
                throw;
            }
        }

        public async Task<(IEnumerable<TarefaDTO> Tarefas, int Total, int Paginas)> ObterPaginadoAsync(FiltrarTarefaDTO filtro)
        {
            try
            {
                var tarefas = await _repository.FiltrarAsync(filtro);
                var total = await _repository.ContarAsync(filtro);
                var paginas = (int)Math.Ceiling(total / (double)filtro.TamanhoPagina);

                var tarefasDTO = tarefas.Select(MapToDTO);
                return (tarefasDTO, total, paginas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter tarefas paginadas");
                throw;
            }
        }

        private TarefaDTO MapToDTO(Tarefa tarefa)
        {
            return new TarefaDTO
            {
                Id = tarefa.Id,
                Titulo = tarefa.Titulo,
                Descricao = tarefa.Descricao,
                DataCriacao = tarefa.DataCriacao,
                DataConclusao = tarefa.DataConclusao,
                DataVencimento = tarefa.DataVencimento,
                Status = tarefa.Status.ToString(),
                Prioridade = tarefa.Prioridade,
                Categoria = tarefa.Categoria,
                Tags = !string.IsNullOrEmpty(tarefa.Tags) 
                    ? tarefa.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries) 
                    : Array.Empty<string>()
            };
        }
    }
}