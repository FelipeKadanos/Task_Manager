using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDataContext>(); // Carrega o contexto de dados para o banco de dados
var app = builder.Build();

// GET: http://localhost:5000/
app.MapGet("/", () => "Bem-vindo a TaskManager API!");




// ============================================================================================
// Projetos
// ============================================================================================




// GET: http://localhost:5000/api/projetos
app.MapGet("/api/projetos/{id}", ([FromRoute] int id, [FromServices] AppDataContext ctx) => {

    var projeto = ctx.Projetos.Find(id);
    if (projeto == null) {
        return Results.NotFound("Projeto não encontrado.");
    }    
    
    return Results.Ok(projeto);
});

// GET: http://localhost:5000/api/projetos
app.MapGet("/api/projetos", ([FromServices] AppDataContext ctx) => {

    var projeto = ctx.Projetos.ToList();
    if (projeto == null) {
        return Results.NotFound("Nenhum projeto registrado.");
    }    
    
    return Results.Ok(projeto);
});


/* Mostra um resumo de um projeto.
 * Busca tarefas do projeto, conta as tarefas por status e prioridade e calcula o percentual de conclusão.
 */
// GET: http://localhost:5000/api/projetos/{id}/resumo
app.MapGet("/api/projetos/{id}/resumo", ([FromRoute] int id, [FromServices] AppDataContext ctx) => {

    if (id <= 0) { return Results.BadRequest("Id do projeto inválido."); }

    // Expressão lambda
    Projeto? projetoEncontrado = ctx.Projetos.FirstOrDefault(x => x.Id == id);
    if (projetoEncontrado is null) {
        return Results.NotFound("Projeto não encontrado.");
    }

    // Busca somente as tarefas vinculadas ao projeto informado
    var tarefas = ctx.Tarefas.Where(x => x.ProjetoId == id).ToList();

    // Calculamos a quantidade de tarefas por status e prioridade
    var totalTarefas = tarefas.Count;
    var pendentes = tarefas.Count(x => x.Status == StatusTarefa.Pendente);
    var emAndamento = tarefas.Count(x => x.Status == StatusTarefa.EmAndamento);
    var concluidas = tarefas.Count(x => x.Status == StatusTarefa.Concluida);
    var altaPrioridade = tarefas.Count(x => x.Prioridade == PrioridadeTarefa.Alta);

    // Calculamos o percentual de tarefas concluídas
    var percentualConclusao = totalTarefas == 0
        ? 0
        : Math.Round(concluidas * 100.0 / totalTarefas, 2);

    return Results.Ok(new {
        projetoId = projetoEncontrado.Id,
        projetoNome = projetoEncontrado.Nome,
        totalTarefas,
        pendentes,
        emAndamento,
        concluidas,
        altaPrioridade,
        percentualConclusao
    });
});


// POST: http://localhost:5000/api/projetos
app.MapPost("/api/projetos", ([FromBody] Projeto? pjt, [FromServices] AppDataContext ctx) => {
    
    if (pjt is null) { return Results.BadRequest("Projeto não enviado."); }

    // Validações do projeto a ser adicionada
    var erros = new Dictionary<string, string[]>();

    if (string.IsNullOrWhiteSpace(pjt.Nome) || pjt.Nome.Trim().Length > 150) {
        erros.Add("Nome", ["Nome é obrigatório e deve ter até 150 caracteres."]);
    }

    if (erros.Count > 0) {
        return Results.BadRequest(erros);
    }

    // Expressão lambda
    Projeto? projetoEncontrado = ctx.Projetos.FirstOrDefault(x => x.Nome == pjt.Nome);
    if (projetoEncontrado is not null) {
        return Results.BadRequest("Esse projeto já existe!");
    }

    ctx.Projetos.Add(pjt);

    // Tentamos salvar o projeto no banco, 
    try {
        ctx.SaveChanges();
    } 
    // Se houver erro printamos no console e retornamos erro 500
    catch (Exception ex) {
        Console.WriteLine($"Erro ao salvar o projeto: {ex.Message}");

        return Results.Problem(
            title: "Erro interno",
            detail: "Ocorreu um erro ao processar a requisição.",
            statusCode: 500);
    }

    // O Entity Framework Core atualiza o objeto pjt com o Id gerado pelo banco
    return Results.Created($"/api/projetos/{pjt.Id}", pjt);
});


// PUT: http://localhost:5000/api/projetos
app.MapPut("/api/projetos", ([FromBody] Projeto? pjt, [FromServices] AppDataContext ctx) => {
    
    if (pjt is null) { return Results.BadRequest("Projeto não enviada."); }

    // Validações do projeto a ser atualizada
    var erros = new Dictionary<string, string[]>();

    if (string.IsNullOrWhiteSpace(pjt.Id.ToString()) || pjt.Id <= 0) {
        erros.Add("Id", ["Id é obrigatório e deve ser um número positivo."]);
    }
    if (string.IsNullOrWhiteSpace(pjt.Nome) || pjt.Nome.Trim().Length > 150) {
        erros.Add("Nome", ["Nome é obrigatório e deve ter até 150 caracteres."]);
    }

    if (erros.Count > 0) {
        return Results.BadRequest(erros);
    }

    // Expressão lambda
    Projeto? projetoEncontrado = ctx.Projetos.FirstOrDefault(x => x.Id == pjt.Id);
    if (projetoEncontrado is null) {
        return Results.BadRequest("Projeto não encontrado!");
    }

    // Atualizamos os dados do projeto encontrado
    projetoEncontrado.Nome = pjt.Nome;

    // Tentamos salvar o projeto no banco, 
    try {
        ctx.SaveChanges();
    } 
    // Se houver erro printamos no console e retornamos erro 500
    catch (Exception ex) {
        Console.WriteLine($"Erro ao salvar o projeto: {ex.Message}");

        return Results.Problem(
            title: "Erro interno",
            detail: "Ocorreu um erro ao processar a requisição.",
            statusCode: 500);
    }

    return Results.Ok("Projeto atualizada com sucesso!");
});


// DELETE: http://localhost:5000/api/projetos
app.MapDelete("/api/projetos/{id}", ([FromRoute] int id, [FromServices] AppDataContext ctx) => {
    
    if (id <= 0) { return Results.BadRequest("Id da projeto não enviado."); }

    // Expressão lambda
    Projeto? projetoEncontrado = ctx.Projetos.FirstOrDefault(x => x.Id == id);
    if (projetoEncontrado is null) {
        return Results.BadRequest("Projeto não encontrado!");
    }

    ctx.Projetos.Remove(projetoEncontrado);

    // Tentamos salvar o projeto no banco, 
    try {
        ctx.SaveChanges();
    } 
    // Se houver erro printamos no console e retornamos erro 500
    catch (Exception ex) {
        Console.WriteLine($"Erro ao remover o projeto: {ex.Message}");

        return Results.Problem(
            title: "Erro interno",
            detail: "Ocorreu um erro ao processar a requisição.",
            statusCode: 500);
    }

    return Results.Ok("Projeto removido com sucesso!");
});




// ============================================================================================
// Categorias
// ============================================================================================




// GET: http://localhost:5000/api/categorias
app.MapGet("/api/categorias/{id}", ([FromRoute] int id, [FromServices] AppDataContext ctx) => {

    var categoria = ctx.Categorias.Find(id);
    if (categoria == null) {
        return Results.NotFound("Categoria não encontrada.");
    }    
    
    return Results.Ok(categoria);
});


// GET: http://localhost:5000/api/categorias
app.MapGet("/api/categorias", ([FromServices] AppDataContext ctx) => {

    var categoria = ctx.Categorias.ToList();
    if (categoria == null) {
        return Results.NotFound("Nenhuma categoria registrada.");
    }    
    
    return Results.Ok(categoria);
});


// POST: http://localhost:5000/api/categorias
app.MapPost("/api/categorias", ([FromBody] Categoria? cat, [FromServices] AppDataContext ctx) => {
    
    if (cat is null) { return Results.BadRequest("Categoria não enviada."); }

    // Validações da categoria a ser adicionada
    var erros = new Dictionary<string, string[]>();

    if (string.IsNullOrWhiteSpace(cat.Nome) || cat.Nome.Trim().Length > 150) {
        erros.Add("Nome", ["Nome é obrigatório e deve ter até 150 caracteres."]);
    }

    if (erros.Count > 0) {
        return Results.BadRequest(erros);
    }

    // Expressão lambda
    Categoria? categoriaEncontrado = ctx.Categorias.FirstOrDefault(x => x.Nome == cat.Nome);
    if (categoriaEncontrado is not null) {
        return Results.BadRequest("Esse categoria já existe!");
    }

    ctx.Categorias.Add(cat);

    // Tentamos salvar a categoria no banco, 
    try {
        ctx.SaveChanges();
    } 
    // Se houver erro printamos no console e retornamos erro 500
    catch (Exception ex) {
        Console.WriteLine($"Erro ao salvar a categoria: {ex.Message}");

        return Results.Problem(
            title: "Erro interno",
            detail: "Ocorreu um erro ao processar a requisição.",
            statusCode: 500);
    }

    // O Entity Framework Core atualiza o objeto cat com o Id gerado pelo banco
    return Results.Created($"/api/categorias/{cat.Id}", cat);
});


// PUT: http://localhost:5000/api/categorias
app.MapPut("/api/categorias", ([FromBody] Categoria? cat, [FromServices] AppDataContext ctx) => {
    
    if (cat is null) { return Results.BadRequest("Categoria não enviada."); }

    // Validações da categoria a ser atualizada
    var erros = new Dictionary<string, string[]>();

    if (string.IsNullOrWhiteSpace(cat.Id.ToString()) || cat.Id <= 0) {
        erros.Add("Id", ["Id é obrigatório e deve ser um número positivo."]);
    }
    if (string.IsNullOrWhiteSpace(cat.Nome) || cat.Nome.Trim().Length > 150) {
        erros.Add("Nome", ["Nome é obrigatório e deve ter até 150 caracteres."]);
    }

    if (erros.Count > 0) {
        return Results.BadRequest(erros);
    }

    // Expressão lambda
    Categoria? categoriaEncontrado = ctx.Categorias.FirstOrDefault(x => x.Id == cat.Id);
    if (categoriaEncontrado is null) {
        return Results.BadRequest("Categoria não encontrada!");
    }

    // Atualizamos os dados da categoria encontrada
    categoriaEncontrado.Nome = cat.Nome;

    // Tentamos salvar a categoria no banco, 
    try {
        ctx.SaveChanges();
    } 
    // Se houver erro printamos no console e retornamos erro 500
    catch (Exception ex) {
        Console.WriteLine($"Erro ao salvar a categoria: {ex.Message}");

        return Results.Problem(
            title: "Erro interno",
            detail: "Ocorreu um erro ao processar a requisição.",
            statusCode: 500);
    }

    return Results.Ok("Categoria atualizada com sucesso!");
});


// DELETE: http://localhost:5000/api/categorias
app.MapDelete("/api/categorias/{id}", ([FromRoute] int id, [FromServices] AppDataContext ctx) => {
    
    if (id <= 0) { return Results.BadRequest("Id da categoria não enviado."); }

    // Expressão lambda
    Categoria? categoriaEncontrado = ctx.Categorias.FirstOrDefault(x => x.Id == id);
    if (categoriaEncontrado is null) {
        return Results.BadRequest("Categoria não encontrada!");
    }

    ctx.Categorias.Remove(categoriaEncontrado);

    // Tentamos salvar a categoria no banco, 
    try {
        ctx.SaveChanges();
    } 
    // Se houver erro printamos no console e retornamos erro 500
    catch (Exception ex) {
        Console.WriteLine($"Erro ao remover a categoria: {ex.Message}");

        return Results.Problem(
            title: "Erro interno",
            detail: "Ocorreu um erro ao processar a requisição.",
            statusCode: 500);
    }

    return Results.Ok("Categoria removida com sucesso!");
});




// ============================================================================================
// Tarefas
// ============================================================================================




// GET: http://localhost:5000/api/tarefas
app.MapGet("/api/tarefas/{id}", ([FromRoute] int id, [FromServices] AppDataContext ctx) => {

    // Mostra a tarefa e também a categoria e o projeto associados a ela
    var tarefa = ctx.Tarefas
        .Include(t => t.Categoria)
        .Include(t => t.Projeto)
        .FirstOrDefault(t => t.Id == id);

    return tarefa is null ? Results.NotFound() : Results.Ok(tarefa);
});


// GET: http://localhost:5000/api/tarefas
app.MapGet("/api/tarefas", ([FromServices] AppDataContext ctx) => {

    // Mostra a tarefa e também a categoria e o projeto associados a ela
    var tarefas = ctx.Tarefas
        .Include(t => t.Categoria)
        .Include(t => t.Projeto)
        .ToList();
    
    return tarefas is null ? Results.NotFound() : Results.Ok(tarefas);
});


// POST: http://localhost:5000/api/tarefas
app.MapPost("/api/tarefas", ([FromBody] Tarefa? trf, [FromServices] AppDataContext ctx) => {
    
    if (trf is null) { return Results.BadRequest("Tarefa não enviada."); }

    // Validações da tarefa a ser adicionada
    var erros = new Dictionary<string, string[]>();

    if (string.IsNullOrWhiteSpace(trf.Titulo) || trf.Titulo.Trim().Length > 150) {
        erros.Add("Titulo", ["Título é obrigatório e deve ter até 150 caracteres."]);
    }
    if (trf.Descricao?.Length > 2000) {
        erros.Add("Descricao", ["Descrição deve ter até 2000 caracteres."]);
    }
    if (!Enum.IsDefined(typeof(StatusTarefa), trf.Status)) {
        erros.Add("Status", ["Status inválido."]);
    }
    if (!Enum.IsDefined(typeof(PrioridadeTarefa), trf.Prioridade)) {
        erros.Add("Prioridade", ["Prioridade inválida."]);
    }
    // Se o id da categoria ou do projeto for enviado, verificamos se ele existe no banco
    if (trf.CategoriaId.HasValue && !ctx.Categorias.Any(c => c.Id == trf.CategoriaId.Value)) {
        erros.Add("Categoria", ["Categoria inválida."]);
    }
    if (trf.ProjetoId.HasValue && !ctx.Projetos.Any(p => p.Id == trf.ProjetoId.Value)) {
        erros.Add("Projeto", ["Projeto inválido."]);
    }

    if (erros.Count > 0) {
        return Results.BadRequest(erros);
    }

    // Expressão lambda
    Tarefa? tarefaEncontrada = ctx.Tarefas.FirstOrDefault(x => x.Titulo == trf.Titulo);
    if (tarefaEncontrada is not null) {
        return Results.BadRequest("Essa tarefa ja existe!");
    }

    ctx.Tarefas.Add(trf);

    // Tentamos salvar a tarefa no banco, 
    try {
        ctx.SaveChanges();
    } 
    // Se houver erro printamos no console e retornamos erro 500
    catch (Exception ex) {
        Console.WriteLine($"Erro ao salvar a tarefa: {ex.Message}");

        return Results.Problem(
            title: "Erro interno",
            detail: "Ocorreu um erro ao processar a requisição.",
            statusCode: 500);
    }

    // O Entity Framework Core atualiza o objeto trf com o Id gerado pelo banco
    return Results.Created($"/api/tarefas/{trf.Id}", trf);
});


// PUT: http://localhost:5000/api/tarefas
app.MapPut("/api/tarefas", ([FromBody] Tarefa? trf, [FromServices] AppDataContext ctx) => {
    
    if (trf is null) { return Results.BadRequest("Tarefa não enviada."); }

    // Validações da tarefa a ser atualizada
    var erros = new Dictionary<string, string[]>();

    if (string.IsNullOrWhiteSpace(trf.Id.ToString()) || trf.Id <= 0) {
        erros.Add("Id", ["Id é obrigatório e deve ser um número positivo."]);
    }
    if (string.IsNullOrWhiteSpace(trf.Titulo) || trf.Titulo.Trim().Length > 150) {
        erros.Add("Titulo", ["Título é obrigatório e deve ter até 150 caracteres."]);
    }
    if (trf.Descricao?.Length > 2000) {
        erros.Add("Descricao", ["Descrição deve ter até 2000 caracteres."]);
    }
    if (!Enum.IsDefined(typeof(StatusTarefa), trf.Status)) {
        erros.Add("Status", ["Status inválido."]);
    }
    if (!Enum.IsDefined(typeof(PrioridadeTarefa), trf.Prioridade)) {
        erros.Add("Prioridade", ["Prioridade inválida."]);
    }
    // Se o id da categoria ou do projeto for enviado, verificamos se ele existe no banco
    if (trf.CategoriaId.HasValue && !ctx.Categorias.Any(c => c.Id == trf.CategoriaId.Value)) {
        erros.Add("Categoria", ["Categoria inválida."]);
    }
    if (trf.ProjetoId.HasValue && !ctx.Projetos.Any(p => p.Id == trf.ProjetoId.Value)) {
        erros.Add("Projeto", ["Projeto inválido."]);
    }

    if (erros.Count > 0) {
        return Results.BadRequest(erros);
    }

    // Expressão lambda
    Tarefa? tarefaEncontrada = ctx.Tarefas.FirstOrDefault(x => x.Id == trf.Id);
    if (tarefaEncontrada is null) {
        return Results.BadRequest("Tarefa não encontrada!");
    }

    // Atualizamos os dados da tarefa encontrada
    tarefaEncontrada.Titulo = trf.Titulo;
    tarefaEncontrada.Descricao = trf.Descricao;
    tarefaEncontrada.Status = trf.Status;
    tarefaEncontrada.Prioridade = trf.Prioridade;
    tarefaEncontrada.Criacao = trf.Criacao;
    tarefaEncontrada.Vencimento = trf.Vencimento;
    tarefaEncontrada.CategoriaId = trf.CategoriaId;
    tarefaEncontrada.ProjetoId = trf.ProjetoId;

    // Tentamos salvar a tarefa no banco, 
    try {
        ctx.SaveChanges();
    } 
    // Se houver erro printamos no console e retornamos erro 500
    catch (Exception ex) {
        Console.WriteLine($"Erro ao salvar a tarefa: {ex.Message}");

        return Results.Problem(
            title: "Erro interno",
            detail: "Ocorreu um erro ao processar a requisição.",
            statusCode: 500);
    }

    return Results.Ok("Tarefa atualizada com sucesso!");
});


// DELETE: http://localhost:5000/api/tarefas
app.MapDelete("/api/tarefas/{id}", ([FromRoute] int id, [FromServices] AppDataContext ctx) => {
    
    if (id <= 0) { return Results.BadRequest("Id da tarefa não enviado."); }

    // Expressão lambda
    Tarefa? tarefaEncontrada = ctx.Tarefas.FirstOrDefault(x => x.Id == id);
    if (tarefaEncontrada is null) {
        return Results.BadRequest("Tarefa não encontrada!");
    }

    ctx.Tarefas.Remove(tarefaEncontrada);

    // Tentamos salvar a tarefa no banco, 
    try {
        ctx.SaveChanges();
    } 
    // Se houver erro printamos no console e retornamos erro 500
    catch (Exception ex) {
        Console.WriteLine($"Erro ao remover a tarefa: {ex.Message}");

        return Results.Problem(
            title: "Erro interno",
            detail: "Ocorreu um erro ao processar a requisição.",
            statusCode: 500);
    }

    return Results.Ok("Tarefa removida com sucesso!");
});




// ============================================================================================
// Atualizando Status
// ============================================================================================




// PATCH: http://localhost:5000/api/tarefas/{id}/status
app.MapPatch("/api/tarefas/{id}/status", ([FromRoute] int id, [FromBody] StatusTarefa? status, [FromServices] AppDataContext ctx) => {
    
    if (id <= 0) { return Results.BadRequest("Id da tarefa não enviado."); }
    if (status is null) { return Results.BadRequest("Status não enviado."); }

    // Validações do status a ser atualizado
    var erros = new Dictionary<string, string[]>();

    if (!Enum.IsDefined(typeof(StatusTarefa), status.Value)) {
        erros.Add("Status", ["Status inválido."]);
    }

    if (erros.Count > 0) {
        return Results.BadRequest(erros);
    }

    // Expressão lambda
    Tarefa? tarefaEncontrada = ctx.Tarefas.FirstOrDefault(x => x.Id == id);
    if (tarefaEncontrada is null) {
        return Results.BadRequest("Tarefa não encontrada!");
    }

    tarefaEncontrada.Status = status.Value;

    // Tentamos salvar o status da tarefa no banco, 
    try {
        ctx.SaveChanges();
    } 
    // Se houver erro printamos no console e retornamos erro 500
    catch (Exception ex) {
        Console.WriteLine($"Erro ao salvar o status da tarefa: {ex.Message}");

        return Results.Problem(
            title: "Erro interno",
            detail: "Ocorreu um erro ao processar a requisição.",
            statusCode: 500);
    }

    return Results.Ok("Status atualizado com sucesso!");
});




// ============================================================================================
// Atualizando Prioridade
// ============================================================================================



// PATCH: http://localhost:5000/api/tarefas/{id}/prioridade
app.MapPatch("/api/tarefas/{id}/prioridade", ([FromRoute] int id, [FromBody] PrioridadeTarefa? prioridade, [FromServices] AppDataContext ctx) => {
    
    if (id <= 0) { return Results.BadRequest("Id da tarefa não enviado."); }
    if (prioridade is null) { return Results.BadRequest("Prioridade não enviada."); }

    // Validações da prioridade a ser atualizada
    var erros = new Dictionary<string, string[]>();

    if (!Enum.IsDefined(typeof(PrioridadeTarefa), prioridade.Value)) {
        erros.Add("Prioridade", ["Prioridade inválida."]);
    }

    if (erros.Count > 0) {
        return Results.BadRequest(erros);
    }

    // Expressão lambda
    Tarefa? tarefaEncontrada = ctx.Tarefas.FirstOrDefault(x => x.Id == id);
    if (tarefaEncontrada is null) {
        return Results.BadRequest("Tarefa não encontrada!");
    }

    tarefaEncontrada.Prioridade = prioridade.Value;

    // Tentamos salvar a prioridade da tarefa no banco, 
    try {
        ctx.SaveChanges();
    } 
    // Se houver erro printamos no console e retornamos erro 500
    catch (Exception ex) {
        Console.WriteLine($"Erro ao salvar a prioridade da tarefa: {ex.Message}");

        return Results.Problem(
            title: "Erro interno",
            detail: "Ocorreu um erro ao processar a requisição.",
            statusCode: 500);
    }

    return Results.Ok("Prioridade atualizada com sucesso!");
});




app.Run();
