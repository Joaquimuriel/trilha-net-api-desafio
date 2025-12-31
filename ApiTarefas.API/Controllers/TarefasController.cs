using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using ApiTarefas.API.Models.DTOs;
using ApiTarefas.API.Services.Interfaces;
using ApiTarefas.API.Extensions;

namespace ApiTarefas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class TarefasController : ControllerBase
    {
        private readonly ITarefaService _tarefaService;
        private readonly ILogger<TarefasController> _logger;

        public TarefasController(ITarefaService tarefaService, ILogger<TarefasController> logger)
        {
            _tarefaService = tarefaService;
            _logger = logger;
        }

        /// <summary>
        /// Obter todas as tarefas
        /// </summary>
        /// <returns>Lista de tarefas</returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Obter todas as tarefas")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<TarefaDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterTodas()
        {
            try
            {
                _logger.LogInformation("Obtendo todas as tarefas");
                var tarefas = await _tarefaService.ObterTodasAsync();
                return Ok(ApiResponse<IEnumerable<TarefaDTO>>.SuccessResponse(tarefas, "Tarefas obtidas com sucesso"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter todas as tarefas");
                return StatusCode(500, ApiResponse.ErrorResponse("Erro interno do servidor"));
            }
        }

        /// <summary>
        /// Obter tarefa por ID
        /// </summary>
        /// <param name="id">ID da tarefa</param>
        /// <returns>Tarefa encontrada</returns>
        [HttpGet("{id:int}")]
        [SwaggerOperation(Summary = "Obter tarefa por ID")]
        [ProducesResponseType(typeof(ApiResponse<TarefaDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterPorId(int id)
        {
            try
            {
                _logger.LogInformation("Obtendo tarefa com ID {Id}", id);
                var tarefa = await _tarefaService.ObterPorIdAsync(id);

                if (tarefa == null)
                {
                    _logger.LogWarning("Tarefa com ID {Id} não encontrada", id);
                    return NotFound(ApiResponse.ErrorResponse($"Tarefa com ID {id} não encontrada"));
                }

                return Ok(ApiResponse<TarefaDTO>.SuccessResponse(tarefa, "Tarefa obtida com sucesso"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter tarefa com ID {Id}", id);
                return StatusCode(500, ApiResponse.ErrorResponse("Erro interno do servidor"));
            }
        }

        /// <summary>
        /// Filtrar tarefas com múltiplos critérios
        /// </summary>
        /// <param name="filtro">Objeto de filtro</param>
        /// <returns>Tarefas filtradas</returns>
        [HttpGet("filtrar")]
        [SwaggerOperation(Summary = "Filtrar tarefas com múltiplos critérios")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<TarefaDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Filtrar([FromQuery] FiltrarTarefaDTO filtro)
        {
            try
            {
                _logger.LogInformation("Filtrando tarefas com critérios");
                
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    
                    return BadRequest(ApiResponse.ErrorResponse("Dados de filtro inválidos", errors));
                }

                var tarefas = await _tarefaService.FiltrarAsync(filtro);
                return Ok(ApiResponse<IEnumerable<TarefaDTO>>.SuccessResponse(tarefas, "Tarefas filtradas com sucesso"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao filtrar tarefas");
                return StatusCode(500, ApiResponse.ErrorResponse("Erro interno do servidor"));
            }
        }

        /// <summary>
        /// Obter tarefas paginadas
        /// </summary>
        /// <param name="filtro">Objeto de filtro com paginação</param>
        /// <returns>Tarefas paginadas com metadados</returns>
        [HttpGet("paginadas")]
        [SwaggerOperation(Summary = "Obter tarefas paginadas")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterPaginadas([FromQuery] FiltrarTarefaDTO filtro)
        {
            try
            {
                _logger.LogInformation("Obtendo tarefas paginadas - Página {Pagina}", filtro.Pagina);
                
                if (filtro.Pagina < 1 || filtro.TamanhoPagina < 1)
                    return BadRequest(ApiResponse.ErrorResponse("Página e tamanho da página devem ser maiores que zero"));

                var (tarefas, total, paginas) = await _tarefaService.ObterPaginadoAsync(filtro);

                var response = new
                {
                    Tarefas = tarefas,
                    Paginacao = new
                    {
                        PaginaAtual = filtro.Pagina,
                        TamanhoPagina = filtro.TamanhoPagina,
                        TotalItens = total,
                        TotalPaginas = paginas,
                        TemProxima = filtro.Pagina < paginas,
                        TemAnterior = filtro.Pagina > 1
                    }
                };

                return Ok(ApiResponse<object>.SuccessResponse(response, "Tarefas paginadas obtidas com sucesso"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter tarefas paginadas");
                return StatusCode(500, ApiResponse.ErrorResponse("Erro interno do servidor"));
            }
        }

        /// <summary>
        /// Criar nova tarefa
        /// </summary>
        /// <param name="dto">Dados da tarefa</param>
        /// <returns>Tarefa criada</returns>
        [HttpPost]
        [SwaggerOperation(Summary = "Criar nova tarefa")]
        [ProducesResponseType(typeof(ApiResponse<TarefaDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Criar([FromBody] CriarTarefaDTO dto)
        {
            try
            {
                _logger.LogInformation("Criando nova tarefa: {Titulo}", dto.Titulo);
                
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    
                    return BadRequest(ApiResponse.ErrorResponse("Dados inválidos", errors));
                }

                var tarefa = await _tarefaService.CriarAsync(dto);
                
                return CreatedAtAction(
                    nameof(ObterPorId),
                    new { id = tarefa.Id },
                    ApiResponse<TarefaDTO>.SuccessResponse(tarefa, "Tarefa criada com sucesso")
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar tarefa");
                return StatusCode(500, ApiResponse.ErrorResponse("Erro interno do servidor"));
            }
        }

        /// <summary>
        /// Atualizar tarefa existente
        /// </summary>
        /// <param name="id">ID da tarefa</param>
        /// <param name="dto">Novos dados da tarefa</param>
        /// <returns>Tarefa atualizada</returns>
        [HttpPut("{id:int}")]
        [SwaggerOperation(Summary = "Atualizar tarefa existente")]
        [ProducesResponseType(typeof(ApiResponse<TarefaDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarTarefaDTO dto)
        {
            try
            {
                _logger.LogInformation("Atualizando tarefa com ID {Id}", id);
                
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    
                    return BadRequest(ApiResponse.ErrorResponse("Dados inválidos", errors));
                }

                var tarefa = await _tarefaService.AtualizarAsync(id, dto);

                if (tarefa == null)
                {
                    _logger.LogWarning("Tarefa com ID {Id} não encontrada para atualização", id);
                    return NotFound(ApiResponse.ErrorResponse($"Tarefa com ID {id} não encontrada"));
                }

                return Ok(ApiResponse<TarefaDTO>.SuccessResponse(tarefa, "Tarefa atualizada com sucesso"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar tarefa com ID {Id}", id);
                return StatusCode(500, ApiResponse.ErrorResponse("Erro interno do servidor"));
            }
        }

        /// <summary>
        /// Excluir tarefa (soft delete)
        /// </summary>
        /// <param name="id">ID da tarefa</param>
        [HttpDelete("{id:int}")]
        [SwaggerOperation(Summary = "Excluir tarefa (soft delete)")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Excluir(int id)
        {
            try
            {
                _logger.LogInformation("Excluindo tarefa com ID {Id}", id);
                
                var sucesso = await _tarefaService.ExcluirAsync(id);

                if (!sucesso)
                {
                    _logger.LogWarning("Tarefa com ID {Id} não encontrada para exclusão", id);
                    return NotFound(ApiResponse.ErrorResponse($"Tarefa com ID {id} não encontrada"));
                }

                return Ok(ApiResponse.SuccessResponse("Tarefa excluída com sucesso"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir tarefa com ID {Id}", id);
                return StatusCode(500, ApiResponse.ErrorResponse("Erro interno do servidor"));
            }
        }

        /// <summary>
        /// Marcar tarefa como concluída
        /// </summary>
        /// <param name="id">ID da tarefa</param>
        [HttpPatch("{id:int}/concluir")]
        [SwaggerOperation(Summary = "Marcar tarefa como concluída")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Concluir(int id)
        {
            try
            {
                _logger.LogInformation("Concluindo tarefa com ID {Id}", id);
                
                var sucesso = await _tarefaService.ConcluirTarefaAsync(id);

                if (!sucesso)
                {
                    _logger.LogWarning("Tarefa com ID {Id} não encontrada para conclusão", id);
                    return NotFound(ApiResponse.ErrorResponse($"Tarefa com ID {id} não encontrada"));
                }

                return Ok(ApiResponse.SuccessResponse("Tarefa concluída com sucesso"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao concluir tarefa com ID {Id}", id);
                return StatusCode(500, ApiResponse.ErrorResponse("Erro interno do servidor"));
            }
        }

        /// <summary>
        /// Obter estatísticas das tarefas
        /// </summary>
        /// <returns>Estatísticas gerais</returns>
        [HttpGet("estatisticas")]
        [SwaggerOperation(Summary = "Obter estatísticas das tarefas")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Estatisticas()
        {
            try
            {
                _logger.LogInformation("Obtendo estatísticas das tarefas");
                
                var todasTarefas = await _tarefaService.ObterTodasAsync();
                
                var estatisticas = new
                {
                    Total = todasTarefas.Count(),
                    Pendentes = todasTarefas.Count(t => t.Status == "Pendente"),
                    EmAndamento = todasTarefas.Count(t => t.Status == "EmAndamento"),
                    Concluidas = todasTarefas.Count(t => t.Status == "Concluida"),
                    Atrasadas = todasTarefas.Count(t => 
                        t.DataVencimento.HasValue && 
                        t.DataVencimento.Value < DateTime.Now && 
                        t.Status != "Concluida"),
                    PorPrioridade = new
                    {
                        Alta = todasTarefas.Count(t => t.Prioridade == 1),
                        Media = todasTarefas.Count(t => t.Prioridade == 2),
                        Baixa = todasTarefas.Count(t => t.Prioridade == 3)
                    }
                };

                return Ok(ApiResponse<object>.SuccessResponse(estatisticas, "Estatísticas obtidas com sucesso"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter estatísticas das tarefas");
                return StatusCode(500, ApiResponse.ErrorResponse("Erro interno do servidor"));
            }
        }
    }
}