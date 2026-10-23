namespace TaskManager.Api.Models;

public class Tarefa {
    public int Id { get; set; }
    public string Titulo { get; set; } = "";
    public string? Descricao { get; set; }
    public StatusTarefa Status { get; set; } = StatusTarefa.Pendente;
    public PrioridadeTarefa Prioridade { get; set; } = PrioridadeTarefa.Media;
    public DateTime CriadaEmUtc { get; set; }
    public DateOnly? DataVencimento { get; set; }
    public int CategoriaId { get; set; }
    public int? ProjetoId { get; set; }
}
