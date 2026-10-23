using TaskManager.Api.Models;

namespace TaskManager.Api.Contracts;

// Id e data de criação são definidos pela API, não pelo cliente.
public class TarefaRequest {
    public string? Titulo { get; set; }
    public string? Descricao { get; set; }
    public int CategoriaId { get; set; }
    public int? ProjetoId { get; set; }
    public DateOnly? DataVencimento { get; set; }
    public StatusTarefa? Status { get; set; }
    public PrioridadeTarefa? Prioridade { get; set; }
}