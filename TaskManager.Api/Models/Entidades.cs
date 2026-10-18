namespace TaskManager.Api.Models;

public enum StatusTarefa { Pendente = 0, EmAndamento = 1, Concluida = 2 }
public enum PrioridadeTarefa { Baixa = 0, Media = 1, Alta = 2 }

public class Projeto {
    public int Id { get; set; }
    public string Nome { get; set; } = "";
}

public class Categoria {
    public int Id { get; set; }
    public string Nome { get; set; } = "";
}

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