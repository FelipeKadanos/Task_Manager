using TaskManager.Api.Contracts;
using TaskManager.Api.Data;
using TaskManager.Api.Models;

namespace TaskManager.Api.Services;

public static class RegrasTarefa {
    private static readonly TimeZoneInfo Fuso = ObterFuso();

    private static TimeZoneInfo ObterFuso() {
        try {
            return TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");
        }
        catch (TimeZoneNotFoundException) {
            return TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
        }
    }

    public static DateOnly HojeEmSaoPaulo() {
        var agora = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Fuso);
        return DateOnly.FromDateTime(agora);
    }

    public static bool EstaAtrasada(Tarefa tarefa, DateOnly hoje) {
        return tarefa.DataVencimento.HasValue &&
            tarefa.DataVencimento.Value < hoje &&
            tarefa.Status != StatusTarefa.Concluida;
    }

    // Mantém a informação de atraso na resposta sem gravá-la no banco.
    public static object Resposta(Tarefa tarefa, DateOnly hoje) {
        return new {
            tarefa.Id,
            tarefa.Titulo,
            tarefa.Descricao,
            tarefa.Status,
            tarefa.Prioridade,
            tarefa.CriadaEmUtc,
            tarefa.DataVencimento,
            tarefa.CategoriaId,
            tarefa.ProjetoId,
            Atrasada = EstaAtrasada(tarefa, hoje)
        };
    }

    public static string? Validar(
        TarefaRequest dados, AppDataContext contexto, bool atualizacao) {
        if (string.IsNullOrWhiteSpace(dados.Titulo) || dados.Titulo.Trim().Length > 150) {
            return "Título é obrigatório e deve ter até 150 caracteres.";
        }
        if (dados.Descricao?.Length > 2000) {
            return "Descrição deve ter até 2000 caracteres.";
        }
        if (atualizacao && (dados.Status == null || dados.Prioridade == null)) {
            return "PUT exige status e prioridade.";
        }
        if (dados.Status.HasValue && !Enum.IsDefined(dados.Status.Value)) {
            return "Status inválido.";
        }
        if (dados.Prioridade.HasValue && !Enum.IsDefined(dados.Prioridade.Value)) {
            return "Prioridade inválida.";
        }
        if (dados.CategoriaId <= 0 ||
            !contexto.Categorias.Any(c => c.Id == dados.CategoriaId)) {
            return "Categoria obrigatória e deve existir.";
        }
        if (dados.ProjetoId.HasValue && (dados.ProjetoId.Value <= 0 ||
            !contexto.Projetos.Any(p => p.Id == dados.ProjetoId.Value))) {
            return "Projeto informado deve existir.";
        }
        return null;
    }

    public static void Preencher(Tarefa tarefa, TarefaRequest dados) {
        tarefa.Titulo = dados.Titulo!.Trim();
        tarefa.Descricao = dados.Descricao?.Trim();
        tarefa.CategoriaId = dados.CategoriaId;
        tarefa.ProjetoId = dados.ProjetoId;
        tarefa.DataVencimento = dados.DataVencimento;
        tarefa.Status = dados.Status ?? StatusTarefa.Pendente;
        tarefa.Prioridade = dados.Prioridade ?? PrioridadeTarefa.Media;
    }
}
