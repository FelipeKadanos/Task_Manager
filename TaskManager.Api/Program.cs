/* var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run(); */

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

app.MapGet("/", () => Results.Ok(new { nome = "Task Manager API" }));

app.MapGet("/projetos", async (AppDataContext db) =>
    Results.Ok(await db.Projetos.AsNoTracking().OrderBy(p => p.Id).ToListAsync())
);

app.MapGet("/projetos/{id:int}", async (int id, AppDataContext db) => {
    var item = await db.Projetos.FindAsync(id);
    return item is null ? Results.NotFound() : Results.Ok(item);
});

app.MapPost("/projetos", async (NomeRequest body, AppDataContext db) => {
    if (string.IsNullOrWhiteSpace(body.Nome) || body.Nome.Trim().Length > 100)
        return Results.BadRequest("Nome é obrigatório e deve ter até 100 caracteres.");
        
    var item = new Projeto { Nome = body.Nome.Trim() };
    db.Projetos.Add(item);

    await db.SaveChangesAsync();
    return Results.Created($"/projetos/{item.Id}", item);
});

app.MapPut("/projetos/{id:int}", async (int id, NomeRequest body, AppDataContext db) => {
    var item = await db.Projetos.FindAsync(id);
    
    if (item is null) return Results.NotFound();

    if (string.IsNullOrWhiteSpace(body.Nome) || body.Nome.Trim().Length > 100)
        return Results.BadRequest("Nome é obrigatório e deve ter até 100 caracteres.");

    item.Nome = body.Nome.Trim();
    await db.SaveChangesAsync();

    return Results.Ok(item);
});

app.MapDelete("/projetos/{id:int}", async (int id, AppDataContext db) => {
    var item = await db.Projetos.FindAsync(id);
    
    if (item is null) return Results.NotFound();
    
    if (await db.Tarefas.AnyAsync(t => t.ProjetoId == id))
        return Results.Conflict("Projeto possui tarefas vinculadas.");
        
    db.Projetos.Remove(item);
    
    try { 
        await db.SaveChangesAsync();
    } catch (DbUpdateException ex) when (ex.InnerException is SqliteException { SqliteExtendedErrorCode: 787 or 1811 }) { 
        return Results.Conflict("Projeto possui tarefas vinculadas."); 
    }

    return Results.NoContent();
});

app.MapGet("/categorias", async (AppDataContext db) => {
    return Results.Ok(await db.Categorias.AsNoTracking().OrderBy(c => c.Id).ToListAsync());
});

app.MapGet("/categorias/{id:int}", async (int id, AppDataContext db) => {
    var item = await db.Categorias.FindAsync(id);

    return item is null ? Results.NotFound() : Results.Ok(item);
});

app.MapPost("/categorias", async (NomeRequest body, AppDataContext db) => {
    if (string.IsNullOrWhiteSpace(body.Nome) || body.Nome.Trim().Length > 100)
        return Results.BadRequest("Nome é obrigatório e deve ter até 100 caracteres.");

    var item = new Categoria { Nome = body.Nome.Trim() };
    db.Categorias.Add(item);
    await db.SaveChangesAsync();

    return Results.Created($"/categorias/{item.Id}", item);
});

app.MapPut("/categorias/{id:int}", async (int id, NomeRequest body, AppDataContext db) => {
    var item = await db.Categorias.FindAsync(id);

    if (item is null) return Results.NotFound();

    if (string.IsNullOrWhiteSpace(body.Nome) || body.Nome.Trim().Length > 100)
        return Results.BadRequest("Nome é obrigatório e deve ter até 100 caracteres.");

    item.Nome = body.Nome.Trim();
    await db.SaveChangesAsync();

    return Results.Ok(item);
});

app.MapDelete("/categorias/{id:int}", async (int id, AppDataContext db) => {
    var item = await db.Categorias.FindAsync(id);

    if (item is null) return Results.NotFound();

    if (await db.Tarefas.AnyAsync(t => t.CategoriaId == id))
        return Results.Conflict("Categoria possui tarefas vinculadas.");

    db.Categorias.Remove(item);

    try {
        await db.SaveChangesAsync();
    } catch (DbUpdateException ex) when (ex.InnerException is SqliteException { SqliteExtendedErrorCode: 787 or 1811 }) { 
        return Results.Conflict("Categoria possui tarefas vinculadas.");
    }
    
    return Results.NoContent();
});

app.MapGet("/tarefas", async (
    StatusTarefa? status, PrioridadeTarefa? prioridade,
    int? categoriaId, int? projetoId,
    DateOnly? vencimentoDe, DateOnly? vencimentoAte,
    bool? atrasadas, AppDataContext db) =>
{
    if (status is { } s && !Enum.IsDefined(s))
        return Results.BadRequest("Status inválido.");
    if (prioridade is { } p && !Enum.IsDefined(p))
        return Results.BadRequest("Prioridade inválida.");
    if (categoriaId <= 0 || projetoId <= 0)
        return Results.BadRequest("IDs dos filtros devem ser positivos.");
    if (vencimentoDe.HasValue && vencimentoAte.HasValue &&
        vencimentoDe.Value > vencimentoAte.Value)
        return Results.BadRequest("Intervalo de vencimento invertido.");

    var hoje = RegrasTarefa.HojeEmSaoPaulo();
    var consulta = db.Tarefas.AsNoTracking().AsQueryable();
    if (status.HasValue) consulta = consulta.Where(t => t.Status == status.Value);
    if (prioridade.HasValue)
        consulta = consulta.Where(t => t.Prioridade == prioridade.Value);
    if (categoriaId.HasValue)
        consulta = consulta.Where(t => t.CategoriaId == categoriaId.Value);
    if (projetoId.HasValue)
        consulta = consulta.Where(t => t.ProjetoId == projetoId.Value);
    if (vencimentoDe.HasValue)
        consulta = consulta.Where(t => t.DataVencimento >= vencimentoDe.Value);
    if (vencimentoAte.HasValue)
        consulta = consulta.Where(t => t.DataVencimento <= vencimentoAte.Value);
    if (atrasadas == true)
        consulta = consulta.Where(t => t.DataVencimento != null &&
            t.DataVencimento < hoje && t.Status != StatusTarefa.Concluida);
    else if (atrasadas == false)
        consulta = consulta.Where(t => t.DataVencimento == null ||
            t.DataVencimento >= hoje || t.Status == StatusTarefa.Concluida);

    var tarefas = await consulta.OrderBy(t => t.Id).ToListAsync();
    return Results.Ok(tarefas.Select(t => RegrasTarefa.Resposta(t, hoje)));
});

app.MapGet("/tarefas/{id:int}", async (int id, AppDataContext db) =>
{
    var tarefa = await db.Tarefas.FindAsync(id);
    return tarefa is null ? Results.NotFound() :
        Results.Ok(RegrasTarefa.Resposta(tarefa, RegrasTarefa.HojeEmSaoPaulo()));
});

app.MapPost("/tarefas", async (TarefaRequest body, AppDataContext db) =>
{
    var erro = await RegrasTarefa.ValidarAsync(body, db, false);
    if (erro is not null) return Results.BadRequest(erro);
    var tarefa = new Tarefa { CriadaEmUtc = DateTime.UtcNow };
    RegrasTarefa.Preencher(tarefa, body);
    db.Tarefas.Add(tarefa);
    try { await db.SaveChangesAsync(); }
    catch (DbUpdateException ex) when
        (ex.InnerException is SqliteException { SqliteExtendedErrorCode: 787 })
    { return Results.BadRequest("Categoria ou projeto deixou de existir."); }
    return Results.Created($"/tarefas/{tarefa.Id}",
        RegrasTarefa.Resposta(tarefa, RegrasTarefa.HojeEmSaoPaulo()));
});

app.MapPut("/tarefas/{id:int}", async (
    int id, TarefaRequest body, AppDataContext db) =>
{
    var tarefa = await db.Tarefas.FindAsync(id);
    if (tarefa is null) return Results.NotFound();
    var erro = await RegrasTarefa.ValidarAsync(body, db, true);
    if (erro is not null) return Results.BadRequest(erro);
    RegrasTarefa.Preencher(tarefa, body);
    try { await db.SaveChangesAsync(); }
    catch (DbUpdateException ex) when
        (ex.InnerException is SqliteException { SqliteExtendedErrorCode: 787 })
    { return Results.BadRequest("Categoria ou projeto deixou de existir."); }
    return Results.Ok(RegrasTarefa.Resposta(tarefa, RegrasTarefa.HojeEmSaoPaulo()));
});

app.MapDelete("/tarefas/{id:int}", async (int id, AppDataContext db) =>
{
    var tarefa = await db.Tarefas.FindAsync(id);
    if (tarefa is null) return Results.NotFound();
    db.Tarefas.Remove(tarefa);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();