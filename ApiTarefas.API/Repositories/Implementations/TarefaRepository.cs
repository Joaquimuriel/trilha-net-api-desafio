using Microsoft.EntityFrameworkCore;
using ApiTarefas.API.Data.Context;
using ApiTarefas.API.Models.Entities;
using ApiTarefas.API.Models.DTOs;
using ApiTarefas.API.Models.Enums; 
using ApiTarefas.API.Repositories.Interfaces;

namespace ApiTarefas.API.Repositories.Implementations
{
    public class TarefaRepository : ITarefaRepository
    {
        private readonly AppDbContext _context;

        public TarefaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Tarefa?> ObterPorIdAsync(int id)
        {
            return await _context.Tarefas
                .FirstOrDefaultAsync(t => t.Id == id && t.Ativo);
        }

        public async Task<IEnumerable<Tarefa>> ObterTodosAsync()
        {
            return await _context.Tarefas
                .Where(t => t.Ativo)
                .OrderByDescending(t => t.DataCriacao)
                .ToListAsync();
        }

        public async Task<IEnumerable<Tarefa>> FiltrarAsync(FiltrarTarefaDTO filtro)
        {
            var query = _context.Tarefas
                .Where(t => t.Ativo)
                .AsQueryable();

            // Aplicar filtros
            if (!string.IsNullOrEmpty(filtro.Titulo))
                query = query.Where(t => t.Titulo.Contains(filtro.Titulo));

            if (!string.IsNullOrEmpty(filtro.Status))
            {
                if (Enum.TryParse<StatusTarefa>(filtro.Status, true, out var status))
                    query = query.Where(t => t.Status == status);
            }

            if (!string.IsNullOrEmpty(filtro.Categoria))
                query = query.Where(t => t.Categoria == filtro.Categoria);

            if (!string.IsNullOrEmpty(filtro.Tag))
                query = query.Where(t => t.Tags != null && t.Tags.Contains(filtro.Tag));

            if (filtro.DataInicio.HasValue)
                query = query.Where(t => t.DataCriacao >= filtro.DataInicio.Value);

            if (filtro.DataFim.HasValue)
                query = query.Where(t => t.DataCriacao <= filtro.DataFim.Value);

            if (filtro.Prioridade.HasValue)
                query = query.Where(t => t.Prioridade == filtro.Prioridade.Value);

            if (filtro.Concluidas.HasValue)
            {
                query = filtro.Concluidas.Value
                    ? query.Where(t => t.Status == StatusTarefa.Concluida)
                    : query.Where(t => t.Status != StatusTarefa.Concluida);
            }

            // Ordenação
            query = filtro.OrdenarPor?.ToLower() switch
            {
                "titulo" => filtro.OrdemDescendente 
                    ? query.OrderByDescending(t => t.Titulo)
                    : query.OrderBy(t => t.Titulo),
                "datavencimento" => filtro.OrdemDescendente
                    ? query.OrderByDescending(t => t.DataVencimento)
                    : query.OrderBy(t => t.DataVencimento),
                "prioridade" => filtro.OrdemDescendente
                    ? query.OrderByDescending(t => t.Prioridade)
                    : query.OrderBy(t => t.Prioridade),
                _ => filtro.OrdemDescendente
                    ? query.OrderByDescending(t => t.DataCriacao)
                    : query.OrderBy(t => t.DataCriacao)
            };

            // Paginação
            var skip = (filtro.Pagina - 1) * filtro.TamanhoPagina;
            query = query.Skip(skip).Take(filtro.TamanhoPagina);

            return await query.ToListAsync();
        }

        public async Task<Tarefa> CriarAsync(Tarefa tarefa)
        {
            _context.Tarefas.Add(tarefa);
            await _context.SaveChangesAsync();
            return tarefa;
        }

        public async Task<Tarefa> AtualizarAsync(Tarefa tarefa)
        {
            _context.Tarefas.Update(tarefa);
            await _context.SaveChangesAsync();
            return tarefa;
        }

        public async Task<bool> ExcluirAsync(int id)
        {
            var tarefa = await ObterPorIdAsync(id);
            if (tarefa == null) return false;

            // Soft delete (não remove fisicamente)
            tarefa.Ativo = false;
            tarefa.DataAtualizacao = DateTime.Now;
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExisteAsync(int id)
        {
            return await _context.Tarefas
                .AnyAsync(t => t.Id == id && t.Ativo);
        }

        public async Task<int> ContarAsync(FiltrarTarefaDTO? filtro = null)
        {
            var query = _context.Tarefas
                .Where(t => t.Ativo)
                .AsQueryable();

            if (filtro != null)
            {
                if (!string.IsNullOrEmpty(filtro.Titulo))
                    query = query.Where(t => t.Titulo.Contains(filtro.Titulo));

                if (!string.IsNullOrEmpty(filtro.Status))
                {
                    if (Enum.TryParse<StatusTarefa>(filtro.Status, true, out var status))
                        query = query.Where(t => t.Status == status);
                }
            }

            return await query.CountAsync();
        }
    }
}