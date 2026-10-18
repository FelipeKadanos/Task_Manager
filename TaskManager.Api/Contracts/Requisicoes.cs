using TaskManager.Api.Models;

namespace TaskManager.Api.Contracts;

public record NomeRequest(string? Nome);

public record TarefaRequest(
    string? Titulo,
    string? Descricao,
    int CategoriaId,
    int? ProjetoId,
    DateOnly? DataVencimento,
    StatusTarefa? Status,
    PrioridadeTarefa? Prioridade);
