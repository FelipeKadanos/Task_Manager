/* var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run(); */

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Contracts;
using TaskManager.Api.Data;
using TaskManager.Api.Models;

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

// MARCOS: inserir endpoints de tarefas aqui, antes de app.Run().

app.Run();