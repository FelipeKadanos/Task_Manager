using System.Text.Json.Serialization;

public class Tarefa {

    // Atributos sem "?" são obrigatórios 
    // Estamos usando [JsonPropertyOrder(0)] para definir a ordem de serialização dos atributos no JSON
    [JsonPropertyOrder(0)]
    public int Id { get; set; }
    [JsonPropertyOrder(1)]
    public string Titulo { get; set; } = "";
    [JsonPropertyOrder(2)]
    public string? Descricao { get; set; }
    [JsonPropertyOrder(3)]
    public StatusTarefa Status { get; set; } = StatusTarefa.Pendente;
    [JsonPropertyOrder(5)]
    public PrioridadeTarefa Prioridade { get; set; } = PrioridadeTarefa.Media;

    // Mostra os nomes do status e da prioridade junto dos valores numéricos
    [JsonPropertyOrder(4)]
    public string StatusNome => Status.ToString();
    [JsonPropertyOrder(6)]
    public string PrioridadeNome => Prioridade.ToString();

    [JsonPropertyOrder(7)]
    public DateTime Criacao { get; set; } = DateTime.Now;
    [JsonPropertyOrder(8)]
    public DateOnly? Vencimento { get; set; }
    [JsonPropertyOrder(9)]
    public int? CategoriaId { get; set; }
    [JsonPropertyOrder(10)]
    public Categoria? Categoria { get; set; }
    [JsonPropertyOrder(11)]
    public int? ProjetoId { get; set; }
    [JsonPropertyOrder(12)]
    public Projeto? Projeto { get; set; }
}
