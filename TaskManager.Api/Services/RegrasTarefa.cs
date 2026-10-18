using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Contracts;
using TaskManager.Api.Data;
using TaskManager.Api.Models;

namespace TaskManager.Api.Services;

public static class RegrasTarefa
{
    private static readonly TimeZoneInfo Fuso = ObterFuso();

    private static TimeZoneInfo ObterFuso()
    {
        try { return TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo"); }
        catch (TimeZoneNotFoundException)
        { return TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"); }
    }

    public static DateOnly HojeEmSaoPaulo() => DateOnly.FromDateTime(
        TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Fuso));

    public static bool EstaAtrasada(Tarefa tarefa, DateOnly hoje) =>
        tarefa.DataVencimento is DateOnly data && data < hoje &&
        tarefa.Status != StatusTarefa.Concluida;

    public static object Resposta(Tarefa tarefa, DateOnly hoje) => new
    {
        tarefa.Id, tarefa.Titulo, tarefa.Descricao, tarefa.Status,
        tarefa.Prioridade, tarefa.CriadaEmUtc, tarefa.DataVencimento,
        tarefa.CategoriaId, tarefa.ProjetoId,
        Atrasada = EstaAtrasada(tarefa, hoje)
    };

    public static async Task<string?> ValidarAsync(
        TarefaRequest body, AppDataContext db, bool atualizacao)
    {
        if (string.IsNullOrWhiteSpace(body.Titulo) || body.Titulo.Trim().Length > 150)
            return "Título é obrigatório e deve ter até 150 caracteres.";
        if (body.Descricao?.Length > 2000)
            return "Descrição deve ter até 2000 caracteres.";
        if (atualizacao && (body.Status is null || body.Prioridade is null))
            return "PUT exige status e prioridade.";
        if (body.Status is { } status && !Enum.IsDefined(status))
            return "Status inválido.";
        if (body.Prioridade is { } prioridade && !Enum.IsDefined(prioridade))
            return "Prioridade inválida.";
        if (body.CategoriaId <= 0 ||
            !await db.Categorias.AnyAsync(c => c.Id == body.CategoriaId))
            return "Categoria obrigatória e deve existir.";
        if (body.ProjetoId is int projetoId && (projetoId <= 0 ||
            !await db.Projetos.AnyAsync(p => p.Id == projetoId)))
            return "Projeto informado deve existir.";
        return null;
    }

    public static void Preencher(Tarefa tarefa, TarefaRequest body)
    {
        tarefa.Titulo = body.Titulo!.Trim();
        tarefa.Descricao = body.Descricao?.Trim();
        tarefa.CategoriaId = body.CategoriaId;
        tarefa.ProjetoId = body.ProjetoId;
        tarefa.DataVencimento = body.DataVencimento;
        tarefa.Status = body.Status ?? StatusTarefa.Pendente;
        tarefa.Prioridade = body.Prioridade ?? PrioridadeTarefa.Media;
    }
}