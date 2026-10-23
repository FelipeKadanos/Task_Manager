using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Contracts;
using TaskManager.Api.Data;
using TaskManager.Api.Models;
using TaskManager.Api.Services;

var builder = WebApplication.CreateBuilder(args);
var conexao = new SqliteConnectionStringBuilder {
    DataSource = Path.Combine(builder.Environment.ContentRootPath, "taskmanager.db"),
    ForeignKeys = true
}.ToString();

builder.Services.AddDbContext<AppDataContext>(op => op.UseSqlite(conexao));

var app = builder.Build();

// GET /
app.MapGet("/", () => Results.Ok(new { nome = "Task Manager API" }));

// GET /projetos
app.MapGet("/projetos", (AppDataContext contexto) => {
    return Results.Ok(contexto.Projetos.AsNoTracking().OrderBy(p => p.Id).ToList());
});

// GET /projetos/{id:int}
app.MapGet("/projetos/{id:int}", (int id, AppDataContext contexto) => {
    var projeto = contexto.Projetos.Find(id);
    if (projeto == null) {
        return Results.NotFound();
    }

    return Results.Ok(projeto);
});

// POST /projetos
app.MapPost("/projetos", (NomeRequest dados, AppDataContext contexto) => {
    if (string.IsNullOrWhiteSpace(dados.Nome) || dados.Nome.Trim().Length > 100) {
        return Results.BadRequest("Nome é obrigatório e deve ter até 100 caracteres.");
    }

    var projeto = new Projeto { Nome = dados.Nome.Trim() };
    contexto.Projetos.Add(projeto);

    contexto.SaveChanges();
    return Results.Created($"/projetos/{projeto.Id}", projeto);
});

// PUT /projetos/{id:int}
app.MapPut("/projetos/{id:int}", (int id, NomeRequest dados, AppDataContext contexto) => {
    var projeto = contexto.Projetos.Find(id);

    if (projeto == null) {
        return Results.NotFound();
    }

    if (string.IsNullOrWhiteSpace(dados.Nome) || dados.Nome.Trim().Length > 100) {
        return Results.BadRequest("Nome é obrigatório e deve ter até 100 caracteres.");
    }

    projeto.Nome = dados.Nome.Trim();
    contexto.SaveChanges();

    return Results.Ok(projeto);
});

// DELETE /projetos/{id:int}
app.MapDelete("/projetos/{id:int}", (int id, AppDataContext contexto) => {
    var projeto = contexto.Projetos.Find(id);

    if (projeto == null) {
        return Results.NotFound();
    }

    if (contexto.Tarefas.Any(t => t.ProjetoId == id)) {
        return Results.Conflict("Projeto possui tarefas vinculadas.");
    }

    contexto.Projetos.Remove(projeto);

    try {
        contexto.SaveChanges();
    }
    catch (DbUpdateException ex) when (ex.InnerException is SqliteException sqlite &&
        (sqlite.SqliteExtendedErrorCode == 787 || sqlite.SqliteExtendedErrorCode == 1811)) {
        return Results.Conflict("Projeto possui tarefas vinculadas.");
    }

    return Results.NoContent();
});

// GET /categorias
app.MapGet("/categorias", (AppDataContext contexto) => {
    return Results.Ok(contexto.Categorias.AsNoTracking().OrderBy(c => c.Id).ToList());
});

// GET /categorias/{id:int}
app.MapGet("/categorias/{id:int}", (int id, AppDataContext contexto) => {
    var categoria = contexto.Categorias.Find(id);

    if (categoria == null) {
        return Results.NotFound();
    }

    return Results.Ok(categoria);
});

// POST /categorias
app.MapPost("/categorias", (NomeRequest dados, AppDataContext contexto) => {
    if (string.IsNullOrWhiteSpace(dados.Nome) || dados.Nome.Trim().Length > 100) {
        return Results.BadRequest("Nome é obrigatório e deve ter até 100 caracteres.");
    }

    var categoria = new Categoria { Nome = dados.Nome.Trim() };
    contexto.Categorias.Add(categoria);
    contexto.SaveChanges();

    return Results.Created($"/categorias/{categoria.Id}", categoria);
});

// PUT /categorias/{id:int}
app.MapPut("/categorias/{id:int}", (int id, NomeRequest dados, AppDataContext contexto) => {
    var categoria = contexto.Categorias.Find(id);

    if (categoria == null) {
        return Results.NotFound();
    }

    if (string.IsNullOrWhiteSpace(dados.Nome) || dados.Nome.Trim().Length > 100) {
        return Results.BadRequest("Nome é obrigatório e deve ter até 100 caracteres.");
    }

    categoria.Nome = dados.Nome.Trim();
    contexto.SaveChanges();

    return Results.Ok(categoria);
});

// DELETE /categorias/{id:int}
app.MapDelete("/categorias/{id:int}", (int id, AppDataContext contexto) => {
    var categoria = contexto.Categorias.Find(id);

    if (categoria == null) {
        return Results.NotFound();
    }

    if (contexto.Tarefas.Any(t => t.CategoriaId == id)) {
        return Results.Conflict("Categoria possui tarefas vinculadas.");
    }

    contexto.Categorias.Remove(categoria);

    try {
        contexto.SaveChanges();
    }
    catch (DbUpdateException ex) when (ex.InnerException is SqliteException sqlite &&
        (sqlite.SqliteExtendedErrorCode == 787 || sqlite.SqliteExtendedErrorCode == 1811)) {
        return Results.Conflict("Categoria possui tarefas vinculadas.");
    }

    return Results.NoContent();
});

// GET /tarefas
app.MapGet("/tarefas", (
    StatusTarefa? status, PrioridadeTarefa? prioridade,
    int? categoriaId, int? projetoId,
    DateOnly? vencimentoDe, DateOnly? vencimentoAte,
    bool? atrasadas, AppDataContext contexto) => {
        if (status.HasValue && !Enum.IsDefined(status.Value)) {
            return Results.BadRequest("Status inválido.");
        }
        if (prioridade.HasValue && !Enum.IsDefined(prioridade.Value)) {
            return Results.BadRequest("Prioridade inválida.");
        }
        if (categoriaId <= 0 || projetoId <= 0) {
            return Results.BadRequest("IDs dos filtros devem ser positivos.");
        }
        if (vencimentoDe.HasValue && vencimentoAte.HasValue &&
            vencimentoDe.Value > vencimentoAte.Value) {
            return Results.BadRequest("Intervalo de vencimento invertido.");
        }

        var hoje = RegrasTarefa.HojeEmSaoPaulo();
        var consulta = contexto.Tarefas.AsNoTracking();
        if (status.HasValue) {
            consulta = consulta.Where(t => t.Status == status.Value);
        }
        if (prioridade.HasValue) {
            consulta = consulta.Where(t => t.Prioridade == prioridade.Value);
        }
        if (categoriaId.HasValue) {
            consulta = consulta.Where(t => t.CategoriaId == categoriaId.Value);
        }
        if (projetoId.HasValue) {
            consulta = consulta.Where(t => t.ProjetoId == projetoId.Value);
        }
        if (vencimentoDe.HasValue) {
            consulta = consulta.Where(t => t.DataVencimento >= vencimentoDe.Value);
        }
        if (vencimentoAte.HasValue) {
            consulta = consulta.Where(t => t.DataVencimento <= vencimentoAte.Value);
        }
        if (atrasadas == true) {
            consulta = consulta.Where(t => t.DataVencimento != null &&
                t.DataVencimento < hoje && t.Status != StatusTarefa.Concluida);
        }
        else if (atrasadas == false) {
            consulta = consulta.Where(t => t.DataVencimento == null ||
                t.DataVencimento >= hoje || t.Status == StatusTarefa.Concluida);
        }

        var tarefas = consulta.OrderBy(t => t.Id).ToList();
        return Results.Ok(tarefas.Select(t => RegrasTarefa.Resposta(t, hoje)));
    });

// GET /tarefas/{id:int}
app.MapGet("/tarefas/{id:int}", (int id, AppDataContext contexto) => {
    var tarefa = contexto.Tarefas.Find(id);
    if (tarefa == null) {
        return Results.NotFound();
    }

    return Results.Ok(RegrasTarefa.Resposta(tarefa, RegrasTarefa.HojeEmSaoPaulo()));
});

// POST /tarefas
app.MapPost("/tarefas", (TarefaRequest dados, AppDataContext contexto) => {
    var erro = RegrasTarefa.Validar(dados, contexto, atualizacao: false);
    if (erro != null) {
        return Results.BadRequest(erro);
    }
    var tarefa = new Tarefa { CriadaEmUtc = DateTime.UtcNow };
    RegrasTarefa.Preencher(tarefa, dados);
    contexto.Tarefas.Add(tarefa);
    try {
        contexto.SaveChanges();
    }
    catch (DbUpdateException ex) when
        (ex.InnerException is SqliteException sqlite && sqlite.SqliteExtendedErrorCode == 787) {
        return Results.BadRequest("Categoria ou projeto deixou de existir.");
    }
    return Results.Created($"/tarefas/{tarefa.Id}",
        RegrasTarefa.Resposta(tarefa, RegrasTarefa.HojeEmSaoPaulo()));
});

// PUT /tarefas/{id:int}
app.MapPut("/tarefas/{id:int}", (
    int id, TarefaRequest dados, AppDataContext contexto) => {
        var tarefa = contexto.Tarefas.Find(id);
        if (tarefa == null) {
            return Results.NotFound();
        }
        var erro = RegrasTarefa.Validar(dados, contexto, atualizacao: true);
        if (erro != null) {
            return Results.BadRequest(erro);
        }
        RegrasTarefa.Preencher(tarefa, dados);
        try {
            contexto.SaveChanges();
        }
        catch (DbUpdateException ex) when
            (ex.InnerException is SqliteException sqlite && sqlite.SqliteExtendedErrorCode == 787) {
            return Results.BadRequest("Categoria ou projeto deixou de existir.");
        }
        return Results.Ok(RegrasTarefa.Resposta(tarefa, RegrasTarefa.HojeEmSaoPaulo()));
    });

// DELETE /tarefas/{id:int}
app.MapDelete("/tarefas/{id:int}", (int id, AppDataContext contexto) => {
    var tarefa = contexto.Tarefas.Find(id);
    if (tarefa == null) {
        return Results.NotFound();
    }
    contexto.Tarefas.Remove(tarefa);
    contexto.SaveChanges();
    return Results.NoContent();
});

app.Run();
