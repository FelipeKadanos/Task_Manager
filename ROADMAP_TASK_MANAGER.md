# Task Manager — roadmap de implementação em dupla

Documento preparado em 18/10/2026 para **FelipeKadanos** e **Marcos**.

## Parte 1 — Visão geral e contrato da API

### 1.1. Origem e limites deste roteiro

Este roteiro desenvolve o planejamento aprovado: Felipe entrega a estrutura, banco e CRUD de Projetos e Categorias; depois Marcos implementa o CRUD e os filtros de Tarefas. A referência principal é a documentação do professor em `export_notion_DevSoftware_CSharp`.

O chat compartilhado não pôde ser recuperado nesta sessão. As cinco respostas citadas no planejamento não estavam no trecho fornecido. Portanto, os campos, limites, nomes dos enums e regras de exclusão e vencimento abaixo são **decisões propostas neste roteiro**, não transcrições de requisitos do professor. Se o enunciado original determinar algo diferente, ajustem este contrato antes de implementar.

O objetivo desta entrega é um guia para vocês executarem. A criação deste documento não cria a API, não instala ferramentas, não aplica migrations e não publica commits. Os exemplos C# foram revisados como documentação, mas não foram compilados nem executados nesta entrega.

### 1.2. Escopo e responsabilidades

| Entrega | Responsável | Critério de conclusão |
| --- | --- | --- |
| Ambiente, solution e Minimal API .NET 8 | Felipe | Projeto compila e responde em `/` |
| Entidades, enums e `AppDataContext` | Felipe | Relacionamentos e restrições configurados |
| Migration inicial e SQLite | Felipe | Banco local criado a partir da migration |
| CRUD de Projetos e Categorias | Felipe | Casos válidos e inválidos testados |
| Base no GitHub | Felipe | Base integrada à `main`, pronta para clonagem |
| CRUD de Tarefas e validação de vínculos | Marcos | Categoria obrigatória e projeto opcional funcionando |
| Filtros e cálculo de atraso | Marcos | Filtros isolados e combinados testados |
| Testes `.http` e integração | Ambos | API integrada funciona nos dois computadores |

Tecnologias: C#, .NET 8, ASP.NET Core Minimal APIs, EF Core 8, SQLite, Git/GitHub e VS Code com C#, C# Dev Kit e REST Client. Esta fase cobre o backend; frontend, autenticação e implantação não fazem parte do planejamento aprovado. Node.js não é necessário para executar esta API.

Para acompanhar os exemplos de Minimal API e CRUD do professor, usaremos um projeto, modelos e contexto em pastas e endpoints no `Program.cs`. O material de arquitetura apresenta divisões maiores que podem ser adotadas depois; aqui a escolha privilegia o exercício em dupla.

### 1.3. Modelo e regras propostas

| Entidade | Campos |
| --- | --- |
| Projeto | `Id`, `Nome` obrigatório (até 100 caracteres) |
| Categoria | `Id`, `Nome` obrigatório (até 100 caracteres) |
| Tarefa | `Id`, `Titulo` obrigatório (até 150), `Descricao` opcional (até 2.000), `Status`, `Prioridade`, `CriadaEmUtc`, `DataVencimento` opcional, `CategoriaId` obrigatório, `ProjetoId` opcional |

- Status: `Pendente = 0`, `EmAndamento = 1`, `Concluida = 2`.
- Prioridade: `Baixa = 0`, `Media = 1`, `Alta = 2`.
- Na criação, omitir status ou prioridade aplica `Pendente` e `Media`.
- `CriadaEmUtc` é gerada pelo servidor e nunca vem do corpo da requisição.
- `DataVencimento` é uma data civil no formato `yyyy-MM-dd`, sem horário. Tarefa vencendo hoje ainda não está atrasada. Uma tarefa está atrasada quando o vencimento é anterior ao dia atual em São Paulo e o status não é `Concluida`.
- Tarefa sem vencimento não está atrasada. Datas passadas são aceitas, inclusive na criação.
- O campo de resposta `atrasada` é calculado, não armazenado no banco.
- A categoria deve existir. O projeto, quando informado, também deve existir. `null` significa sem projeto; `0` é inválido.
- Excluir categoria ou projeto com tarefas vinculadas retorna `409 Conflict`. Primeiro alterem/excluam as tarefas vinculadas. Não haverá exclusão em cascata.
- Nomes de projetos e categorias podem se repetir; não há requisito confirmado de unicidade.
- `PUT` substitui os campos editáveis: status e prioridade são obrigatórios; omitir descrição, vencimento ou projeto limpa esses campos. A data de criação permanece igual.

Os enums serão números no JSON e nos filtros. As validações são explícitas: não dependemos de validação automática por atributos nas Minimal APIs do .NET 8.

### 1.4. Endpoints e respostas

| Método | Rota | Resultado esperado |
| --- | --- | --- |
| GET | `/` | `200` com identificação da API |
| GET | `/projetos`, `/categorias`, `/tarefas` | `200` com lista; vazia retorna `[]` |
| GET | `/projetos/{id}`, `/categorias/{id}`, `/tarefas/{id}` | `200` ou `404` |
| POST | `/projetos`, `/categorias`, `/tarefas` | `201` com corpo e cabeçalho `Location` |
| PUT | `/projetos/{id}`, `/categorias/{id}`, `/tarefas/{id}` | `200`, `400` ou `404` |
| DELETE | `/projetos/{id}`, `/categorias/{id}`, `/tarefas/{id}` | `204`, `404`; vínculo impede exclusão com `409` |

Corpos inválidos, enums fora dos valores definidos e vínculos inexistentes retornam `400`. IDs de recurso que não existem retornam `404` em consultas, alterações e exclusões por ID.

Filtros de `GET /tarefas`:

| Parâmetro | Exemplo | Regra |
| --- | --- | --- |
| `status` | `0` | Igualdade com enum |
| `prioridade` | `2` | Igualdade com enum |
| `categoriaId` | `1` | Igualdade com ID |
| `projetoId` | `1` | Igualdade com ID |
| `vencimentoDe` | `2026-10-01` | Limite inicial inclusivo |
| `vencimentoAte` | `2026-10-31` | Limite final inclusivo |
| `atrasadas` | `true` | Somente atrasadas; `false` retorna as não atrasadas |

Filtros são opcionais e combinados com **E**. Intervalo de datas não inclui tarefas sem vencimento. ID positivo inexistente em filtro retorna lista vazia. ID zero/negativo, enum indefinido ou intervalo invertido retorna `400`. Sem filtros, são retornadas todas as tarefas, ordenadas por ID.

### 1.5. Estrutura final planejada

```text
Task_Maneger/
├── ROADMAP_TASK_MANAGER.md
├── README.md
├── .gitignore
├── global.json
├── .config/dotnet-tools.json
├── TaskManager.sln
└── TaskManager.Api/
    ├── TaskManager.Api.csproj
    ├── Program.cs
    ├── Models/Entidades.cs
    ├── Contracts/Requisicoes.cs
    ├── Data/AppDataContext.cs
    ├── Services/RegrasTarefa.cs            # Marcos
    ├── Migrations/                         # gerada pelo EF
    ├── Requests/base.http
    ├── Requests/tarefas.http               # Marcos
    └── taskmanager.db                      # local, fora do Git
```

## Parte 2 — Roadmap do Felipe: estrutura e banco de dados

### F1. Preparar ambiente e identidade Git

Instale o SDK .NET **8** (não somente o runtime), Git e VS Code. Instale as extensões C#, C# Dev Kit e REST Client. Use PowerShell nos exemplos. Reabra o terminal após instalar ferramentas.

Na inspeção desta pasta em 18/10/2026, havia apenas SDK `10.0.300`. Instale também o SDK 8 para seguir este roteiro exatamente. Download: [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0).

```powershell
Set-Location 'C:\Users\thoma\Documents\Task_Maneger'
dotnet --list-sdks
git --version
git status
git remote -v
git config user.name "FelipeKadanos"
git config user.email "SEU_EMAIL_ASSOCIADO_AO_GITHUB"
```

Substitua o e-mail antes de executar. Os comandos `git config` acima valem apenas neste repositório. O remoto encontrado é `https://github.com/FelipeKadanos/Task_Maneger/`, com branch `main`; não é necessário executar `git init` ou cadastrar outro remoto. Os materiais exportados já estavam presentes como arquivos não rastreados: decidam separadamente se devem entrar no Git. Evitem `git add .` para não incluí-los por acidente.

Com as alterações anteriores salvas em commits próprios, inicie sua branch:

```powershell
git switch main
git pull --ff-only origin main
git switch -c feat/base-api
```

**Verificação:** a lista de SDKs contém `8.0.xxx`, o Git reconhece o repositório e a branch atual é `feat/base-api`.

### F2. Criar solution, API e ferramenta EF

Na raiz, crie `global.json` com o conteúdo completo:

```json
{
  "sdk": {
    "version": "8.0.100",
    "rollForward": "latestFeature",
    "allowPrerelease": false
  }
}
```

Isso seleciona um SDK 8.0 instalado, aceitando suas versões posteriores de correção e feature band, sem selecionar o SDK 10. Consulte a [regra oficial de seleção por global.json](https://learn.microsoft.com/en-us/dotnet/core/tools/global-json).

Execute na raiz, uma única vez:

```powershell
dotnet --version
dotnet new sln -n TaskManager
dotnet new web -n TaskManager.Api -f net8.0
dotnet sln TaskManager.sln add TaskManager.Api/TaskManager.Api.csproj
dotnet new tool-manifest
dotnet tool install dotnet-ef --version '8.0.*'
dotnet add TaskManager.Api/TaskManager.Api.csproj package Microsoft.EntityFrameworkCore.Sqlite --version '8.0.*'
dotnet add TaskManager.Api/TaskManager.Api.csproj package Microsoft.EntityFrameworkCore.Design --version '8.0.*'
dotnet list TaskManager.Api/TaskManager.Api.csproj package
dotnet tool list --local
New-Item -ItemType Directory -Force -Path TaskManager.Api/Models, TaskManager.Api/Contracts, TaskManager.Api/Data, TaskManager.Api/Requests
```

O comando `new web` segue o material de Minimal APIs e evita código de exemplo desnecessário. Não é preciso criar `Startup.cs` neste modelo.

Escolhemos ferramentas locais para que Marcos use `dotnet tool restore`. Os comandos selecionam uma versão estável 8.0.x; confira os números resolvidos no `.csproj` e no manifesto e use a mesma versão de correção nos dois pacotes e na ferramenta. Se necessário, repita `dotnet add ... --version NUMERO_EXATO` e `dotnet tool update dotnet-ef --version NUMERO_EXATO`. Registre versões exatas no `.csproj` caso tenha permanecido um curinga. Versione esses arquivos. Referência: [CLI do EF Core](https://learn.microsoft.com/en-us/ef/core/cli/dotnet).

No `.gitignore` existente, **acrescente** estas regras, preservando as anteriores:

```gitignore
**/bin/
**/obj/
**/*.db
**/*.db-shm
**/*.db-wal
**/*.db-journal
```

**Verificação:** `dotnet --version` mostra `8.0.xxx`; `dotnet build TaskManager.sln` termina sem erros. A partir daqui, preencha os arquivos antes de compilar novamente.

### F3. Criar entidades e contratos

Crie `TaskManager.Api/Models/Entidades.cs`:

```csharp
namespace TaskManager.Api.Models;

public enum StatusTarefa { Pendente = 0, EmAndamento = 1, Concluida = 2 }
public enum PrioridadeTarefa { Baixa = 0, Media = 1, Alta = 2 }

public class Projeto
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
}

public class Categoria
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
}

public class Tarefa
{
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
```

Os relacionamentos serão configurados pelas chaves estrangeiras no contexto. Não precisamos devolver propriedades de navegação, evitando ciclos no JSON.

Crie `TaskManager.Api/Contracts/Requisicoes.cs`:

```csharp
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
```

Os contratos separam os dados recebidos dos dados armazenados. Assim, o cliente não pode definir o ID nem a data de criação ao cadastrar uma tarefa.

### F4. Configurar contexto e relacionamentos

Crie `TaskManager.Api/Data/AppDataContext.cs`:

```csharp
using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Models;

namespace TaskManager.Api.Data;

public class AppDataContext(DbContextOptions<AppDataContext> options)
    : DbContext(options)
{
    public DbSet<Projeto> Projetos => Set<Projeto>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Tarefa> Tarefas => Set<Tarefa>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Projeto>().Property(p => p.Nome)
            .IsRequired().HasMaxLength(100);
        modelBuilder.Entity<Categoria>().Property(c => c.Nome)
            .IsRequired().HasMaxLength(100);

        var tarefa = modelBuilder.Entity<Tarefa>();
        tarefa.Property(t => t.Titulo).IsRequired().HasMaxLength(150);
        tarefa.Property(t => t.Descricao).HasMaxLength(2000);
        tarefa.Property(t => t.CriadaEmUtc).HasConversion(
            valor => valor,
            valor => DateTime.SpecifyKind(valor, DateTimeKind.Utc));
        tarefa.HasOne<Categoria>().WithMany()
            .HasForeignKey(t => t.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);
        tarefa.HasOne<Projeto>().WithMany()
            .HasForeignKey(t => t.ProjetoId)
            .OnDelete(DeleteBehavior.Restrict);
        tarefa.ToTable("Tarefas", tabela =>
        {
            tabela.HasCheckConstraint("CK_Tarefas_Status", "Status IN (0, 1, 2)");
            tabela.HasCheckConstraint("CK_Tarefas_Prioridade", "Prioridade IN (0, 1, 2)");
        });
    }
}
```

`CategoriaId` é não anulável, enquanto `ProjetoId` aceita `null`. As chaves estrangeiras protegem os vínculos no banco. `HasMaxLength` documenta o modelo, mas SQLite não garante esses limites sozinho; os endpoints validarão o tamanho.

Usamos `DateTime` UTC para criação e `DateOnly` para vencimento. A conversão de leitura restaura a indicação UTC para que o JSON devolva `Z` mesmo depois de consultar o SQLite. A documentação oficial descreve [limitações de tipos do provedor SQLite](https://learn.microsoft.com/en-us/ef/core/providers/sqlite/limitations), incluindo comparações com `DateTimeOffset`.

### F5. Implementar a base e os endpoints de Projetos e Categorias

Substitua todo o conteúdo de `TaskManager.Api/Program.cs` pelo bloco abaixo:

```csharp
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Contracts;
using TaskManager.Api.Data;
using TaskManager.Api.Models;

var builder = WebApplication.CreateBuilder(args);
var conexao = new SqliteConnectionStringBuilder
{
    DataSource = Path.Combine(builder.Environment.ContentRootPath, "taskmanager.db"),
    ForeignKeys = true
}.ToString();
builder.Services.AddDbContext<AppDataContext>(op => op.UseSqlite(conexao));
var app = builder.Build();

app.MapGet("/", () => Results.Ok(new { nome = "Task Manager API" }));

app.MapGet("/projetos", async (AppDataContext db) =>
    Results.Ok(await db.Projetos.AsNoTracking().OrderBy(p => p.Id).ToListAsync()));

app.MapGet("/projetos/{id:int}", async (int id, AppDataContext db) =>
{
    var item = await db.Projetos.FindAsync(id);
    return item is null ? Results.NotFound() : Results.Ok(item);
});

app.MapPost("/projetos", async (NomeRequest body, AppDataContext db) =>
{
    if (string.IsNullOrWhiteSpace(body.Nome) || body.Nome.Trim().Length > 100)
        return Results.BadRequest("Nome é obrigatório e deve ter até 100 caracteres.");
    var item = new Projeto { Nome = body.Nome.Trim() };
    db.Projetos.Add(item);
    await db.SaveChangesAsync();
    return Results.Created($"/projetos/{item.Id}", item);
});

app.MapPut("/projetos/{id:int}", async (int id, NomeRequest body, AppDataContext db) =>
{
    var item = await db.Projetos.FindAsync(id);
    if (item is null) return Results.NotFound();
    if (string.IsNullOrWhiteSpace(body.Nome) || body.Nome.Trim().Length > 100)
        return Results.BadRequest("Nome é obrigatório e deve ter até 100 caracteres.");
    item.Nome = body.Nome.Trim();
    await db.SaveChangesAsync();
    return Results.Ok(item);
});

app.MapDelete("/projetos/{id:int}", async (int id, AppDataContext db) =>
{
    var item = await db.Projetos.FindAsync(id);
    if (item is null) return Results.NotFound();
    if (await db.Tarefas.AnyAsync(t => t.ProjetoId == id))
        return Results.Conflict("Projeto possui tarefas vinculadas.");
    db.Projetos.Remove(item);
    try { await db.SaveChangesAsync(); }
    catch (DbUpdateException ex) when
        (ex.InnerException is SqliteException { SqliteExtendedErrorCode: 787 or 1811 })
    { return Results.Conflict("Projeto possui tarefas vinculadas."); }
    return Results.NoContent();
});

app.MapGet("/categorias", async (AppDataContext db) =>
    Results.Ok(await db.Categorias.AsNoTracking().OrderBy(c => c.Id).ToListAsync()));

app.MapGet("/categorias/{id:int}", async (int id, AppDataContext db) =>
{
    var item = await db.Categorias.FindAsync(id);
    return item is null ? Results.NotFound() : Results.Ok(item);
});

app.MapPost("/categorias", async (NomeRequest body, AppDataContext db) =>
{
    if (string.IsNullOrWhiteSpace(body.Nome) || body.Nome.Trim().Length > 100)
        return Results.BadRequest("Nome é obrigatório e deve ter até 100 caracteres.");
    var item = new Categoria { Nome = body.Nome.Trim() };
    db.Categorias.Add(item);
    await db.SaveChangesAsync();
    return Results.Created($"/categorias/{item.Id}", item);
});

app.MapPut("/categorias/{id:int}", async (int id, NomeRequest body, AppDataContext db) =>
{
    var item = await db.Categorias.FindAsync(id);
    if (item is null) return Results.NotFound();
    if (string.IsNullOrWhiteSpace(body.Nome) || body.Nome.Trim().Length > 100)
        return Results.BadRequest("Nome é obrigatório e deve ter até 100 caracteres.");
    item.Nome = body.Nome.Trim();
    await db.SaveChangesAsync();
    return Results.Ok(item);
});

app.MapDelete("/categorias/{id:int}", async (int id, AppDataContext db) =>
{
    var item = await db.Categorias.FindAsync(id);
    if (item is null) return Results.NotFound();
    if (await db.Tarefas.AnyAsync(t => t.CategoriaId == id))
        return Results.Conflict("Categoria possui tarefas vinculadas.");
    db.Categorias.Remove(item);
    try { await db.SaveChangesAsync(); }
    catch (DbUpdateException ex) when
        (ex.InnerException is SqliteException { SqliteExtendedErrorCode: 787 or 1811 })
    { return Results.Conflict("Categoria possui tarefas vinculadas."); }
    return Results.NoContent();
});

// MARCOS: inserir endpoints de tarefas aqui, antes de app.Run().

app.Run();
```

O caminho do banco parte da pasta da API, evitando arquivos diferentes conforme o diretório do terminal. O contexto é registrado por injeção de dependência, adaptação do exemplo do professor. A consulta prévia permite uma mensagem clara para exclusões bloqueadas; o tratamento da restrição cobre também vínculo criado entre a consulta e a exclusão.

### F6. Criar e aplicar migration

Na raiz do repositório, execute:

```powershell
dotnet build TaskManager.sln
dotnet ef migrations add Inicial --project TaskManager.Api --startup-project TaskManager.Api
dotnet ef database update --project TaskManager.Api --startup-project TaskManager.Api
dotnet ef migrations list --project TaskManager.Api --startup-project TaskManager.Api
dotnet run --project TaskManager.Api --no-launch-profile --urls http://localhost:5000
```

A API ocupa esse terminal; use um segundo terminal para Git. Para encerrá-la, pressione `Ctrl+C`. Não use `EnsureCreated` junto com migrations. Não recrie `Inicial` no computador do Marcos: ele aplicará a migration versionada.

**Verificação:** pasta `Migrations` gerada, banco em `TaskManager.Api/taskmanager.db`, tabelas `Projetos`, `Categorias`, `Tarefas` e `__EFMigrationsHistory`. A migration deve incluir as duas chaves estrangeiras, a categoria obrigatória, projeto opcional e checks dos enums. Confira visualmente com uma ferramenta SQLite, se disponível. Abra `http://localhost:5000/` e confira o JSON.

### F7. Testar a base com REST Client

Crie `TaskManager.Api/Requests/base.http`. Cada bloco separado por `###` é uma requisição. Clique em **Send Request** no VS Code. Os IDs abaixo são exemplos: atualize as variáveis com os IDs retornados pelos POSTs; não presuma que sempre serão 1.

```http
@baseUrl = http://localhost:5000
@projetoId = 1
@categoriaId = 1

### Saúde — 200
GET {{baseUrl}}/

### Criar projeto — 201; anote id e confira Location
POST {{baseUrl}}/projetos
Content-Type: application/json

{"nome":"Trabalho C#"}

### Criar categoria — 201; anote id
POST {{baseUrl}}/categorias
Content-Type: application/json

{"nome":"Estudos"}

### Listar projetos — 200
GET {{baseUrl}}/projetos

### Listar categorias — 200
GET {{baseUrl}}/categorias

### Buscar projeto — 200
GET {{baseUrl}}/projetos/{{projetoId}}

### Buscar categoria — 200
GET {{baseUrl}}/categorias/{{categoriaId}}

### Alterar projeto — 200
PUT {{baseUrl}}/projetos/{{projetoId}}
Content-Type: application/json

{"nome":"Task Manager"}

### Alterar categoria — 200
PUT {{baseUrl}}/categorias/{{categoriaId}}
Content-Type: application/json

{"nome":"Faculdade"}

### Nome inválido — 400
POST {{baseUrl}}/categorias
Content-Type: application/json

{"nome":"   "}

### Projeto inválido — 400
POST {{baseUrl}}/projetos
Content-Type: application/json

{"nome":""}

### Recurso inexistente — 404 (confira que esse ID não existe)
GET {{baseUrl}}/projetos/2147483647

### Excluir projeto sem tarefas — 204; executar só após demais testes
DELETE {{baseUrl}}/projetos/{{projetoId}}

### Excluir categoria sem tarefas — 204; executar só após demais testes
DELETE {{baseUrl}}/categorias/{{categoriaId}}
```

Depois das exclusões, repita GET e DELETE por ID: devem retornar `404`. Teste também PUT com nome em branco e nome maior que 100 caracteres nas duas entidades (`400`). Recrie projeto e categoria para os testes de Marcos. Reinicie a API e confira que os registros permanecem: isso comprova persistência.

### F8. Registrar e entregar a base

Faça commits ao atingir etapas funcionais. Sugestões de mensagens: `chore: cria solution e API .NET 8`, `feat: configura modelos e banco SQLite`, `feat: adiciona CRUD de projetos e categorias`, `docs: adiciona testes HTTP e instrucoes de execucao`.

Na raiz, confira os arquivos que serão incluídos:

```powershell
git status --short
git add global.json TaskManager.sln .config/dotnet-tools.json .gitignore TaskManager.Api ROADMAP_TASK_MANAGER.md
git diff --cached --stat
git diff --cached
git commit -m "feat: entrega base da API com projetos e categorias"
git push -u origin feat/base-api
```

Use o commit único acima se ainda não fez os commits por etapa. Se já os fez, publique a branch sem criar um commit vazio. O `.db`, `bin` e `obj` não devem aparecer no conteúdo preparado. Inclua as migrations e o snapshot.

No GitHub, abra um pull request de `feat/base-api` para `main`, revise os arquivos e integre após os testes. Garanta que Marcos tenha acesso de colaborador. Atualize o README com: objetivo, integrantes, versões, comandos de restore/migration/run, porta 5000 e link para este roteiro. Versione essa atualização também.

**Entrega para Marcos:** informar que a base está na `main`, o hash do commit integrado, como iniciar a API e onde estão os testes. O banco não é compartilhado: Marcos terá um banco próprio e IDs próprios.

Checklist do Felipe:

- [x] SDK 8 selecionado; EF Core e ferramenta em 8.0.x compatíveis.
- [x] Solution compila sem erros.
- [x] Entidades e contratos correspondem à parte 1.
- [x] Migration inicial aplicada e versionada.
- [ ] CRUD de Projetos e Categorias testado, incluindo erros.
- [ ] Dados sobrevivem a reinício da API.
- [ ] `.http`, README e instruções de execução disponíveis.
- [x] Banco e artefatos de compilação fora do Git.
- [x] Base integrada à `main` e acessível ao Marcos.

## Parte 3 — Roadmap do Marcos: tarefas, filtros e integração

### M1. Preparar a cópia local

Instale SDK .NET 8, Git, VS Code e as mesmas extensões. Aguarde a base de Felipe estar integrada. No seu computador, escolha a pasta onde deseja guardar o repositório e execute:

```powershell
git clone https://github.com/FelipeKadanos/Task_Maneger.git
Set-Location Task_Maneger
git config user.name "SEU_NOME_OU_USUARIO"
git config user.email "SEU_EMAIL_ASSOCIADO_AO_GITHUB"
git switch main
git pull --ff-only origin main
git switch -c feat/tarefas-filtros
dotnet --version
dotnet tool restore
dotnet restore TaskManager.sln
dotnet build TaskManager.sln
dotnet ef database update --project TaskManager.Api --startup-project TaskManager.Api
dotnet run --project TaskManager.Api --no-launch-profile --urls http://localhost:5000
```

Se já clonou o repositório, entre na cópia existente e pule `git clone`. Substitua nome/e-mail. Use sua própria identidade para registrar autoria. Não execute novamente `dotnet new`, `migrations add Inicial` ou os comandos de instalação dos pacotes.

Execute os testes da base antes de alterar código. Crie categoria e projeto no seu banco e anote seus IDs. A partir daqui, os caminhos são relativos à sua raiz `Task_Maneger`.

### M2. Implementar validação e cálculo de atraso

Crie a pasta `TaskManager.Api/Services` e o arquivo `TaskManager.Api/Services/RegrasTarefa.cs`:

```csharp
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
```

A data de hoje vem do fuso solicitado, independentemente do fuso do computador. O nome Windows é uma alternativa ao identificador IANA quando ele não estiver disponível. Não use `DateTime.Now` nem subtraia três horas manualmente. A data atual será calculada uma vez por resposta para manter consistência perto da meia-noite.

### M3. Adicionar endpoints ao Program.cs

Acrescente este `using` junto aos demais, no topo de `TaskManager.Api/Program.cs`:

```csharp
using TaskManager.Api.Services;
```

No comentário reservado ao Marcos, **antes de `app.Run()`**, acrescente o bloco a seguir. Preserve todos os endpoints de Felipe e mantenha somente um `app.Run()` no final.

```csharp
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
```

Os filtros são aplicados antes de `ToListAsync`, mantendo a seleção no banco. Somente o formato de resposta e o indicador de atraso são calculados após a consulta. O modelo já existe desde a fase de Felipe: adicionar esses endpoints não exige nova migration.

**Verificação:** pare a API, execute `dotnet build TaskManager.sln` na raiz e reinicie com o comando de M1. Nenhuma rota de Projetos e Categorias deve ter desaparecido.

### M4. Criar testes HTTP de tarefas

Crie `TaskManager.Api/Requests/tarefas.http`. Ajuste os três IDs e as datas conforme os dados reais. Execute os POSTs da base antes; não execute suas exclusões ainda.

```http
@baseUrl = http://localhost:5000
@categoriaId = 1
@projetoId = 1
@tarefaId = 1

### Criar com padrões e sem projeto — 201; status 0, prioridade 1
POST {{baseUrl}}/tarefas
Content-Type: application/json

{
  "titulo": "Revisar Minimal APIs",
  "categoriaId": {{categoriaId}}
}

### Criar com todos os campos — 201; anote id em tarefaId
POST {{baseUrl}}/tarefas
Content-Type: application/json

{
  "titulo": "Implementar filtros",
  "descricao": "Testar status, prioridade e vencimento",
  "categoriaId": {{categoriaId}},
  "projetoId": {{projetoId}},
  "dataVencimento": "2020-01-01",
  "status": 1,
  "prioridade": 2
}

### Listar — 200
GET {{baseUrl}}/tarefas

### Buscar — 200; criadaEmUtc deve terminar com Z também após reinício
GET {{baseUrl}}/tarefas/{{tarefaId}}

### Filtro por status
GET {{baseUrl}}/tarefas?status=1

### Filtro por prioridade
GET {{baseUrl}}/tarefas?prioridade=2

### Filtro por categoria
GET {{baseUrl}}/tarefas?categoriaId={{categoriaId}}

### Filtro por projeto
GET {{baseUrl}}/tarefas?projetoId={{projetoId}}

### Intervalo inclusivo
GET {{baseUrl}}/tarefas?vencimentoDe=2020-01-01&vencimentoAte=2020-01-01

### Atrasadas
GET {{baseUrl}}/tarefas?atrasadas=true

### Não atrasadas — inclui sem vencimento e concluídas
GET {{baseUrl}}/tarefas?atrasadas=false

### Combinar filtros
GET {{baseUrl}}/tarefas?status=1&prioridade=2&categoriaId={{categoriaId}}&projetoId={{projetoId}}&atrasadas=true

### Categoria ausente — 400
POST {{baseUrl}}/tarefas
Content-Type: application/json

{"titulo":"Sem categoria"}

### Categoria inexistente — 400
POST {{baseUrl}}/tarefas
Content-Type: application/json

{"titulo":"Vínculo inválido","categoriaId":2147483647}

### Projeto inexistente — 400
POST {{baseUrl}}/tarefas
Content-Type: application/json

{"titulo":"Projeto inválido","categoriaId":{{categoriaId}},"projetoId":2147483647}

### Enum inválido — 400
POST {{baseUrl}}/tarefas
Content-Type: application/json

{"titulo":"Status inválido","categoriaId":{{categoriaId}},"status":99}

### Título vazio — 400
POST {{baseUrl}}/tarefas
Content-Type: application/json

{"titulo":"  ","categoriaId":{{categoriaId}}}

### Filtro inválido — 400
GET {{baseUrl}}/tarefas?prioridade=99

### Intervalo invertido — 400
GET {{baseUrl}}/tarefas?vencimentoDe=2026-12-31&vencimentoAte=2026-01-01

### Data malformada — 400
GET {{baseUrl}}/tarefas?vencimentoDe=nao-e-data

### Exclusão bloqueada — 409 enquanto existir tarefa com esse projeto
DELETE {{baseUrl}}/projetos/{{projetoId}}

### Exclusão bloqueada — 409 enquanto existir tarefa nessa categoria
DELETE {{baseUrl}}/categorias/{{categoriaId}}

### Concluir e remover projeto — 200; criadaEmUtc deve permanecer igual
PUT {{baseUrl}}/tarefas/{{tarefaId}}
Content-Type: application/json

{
  "titulo": "Implementar filtros",
  "descricao": "Concluído e revisado",
  "categoriaId": {{categoriaId}},
  "projetoId": null,
  "dataVencimento": "2020-01-01",
  "status": 2,
  "prioridade": 2
}

### PUT sem status e prioridade — 400
PUT {{baseUrl}}/tarefas/{{tarefaId}}
Content-Type: application/json

{"titulo":"Atualização incompleta","categoriaId":{{categoriaId}}}

### Excluir tarefa — 204
DELETE {{baseUrl}}/tarefas/{{tarefaId}}

### Buscar excluída — 404
GET {{baseUrl}}/tarefas/{{tarefaId}}
```

Os arquivos `.http` são testes manuais: os comentários descrevem expectativas, não são asserções automáticas. Registre resultados e corrija diferenças antes de integrar.

### M5. Validar limites, fuso e integração com a base

Além das requisições acima, use uma tarefa de teste para cada situação abaixo. Para testar hoje/ontem/amanhã, substitua `dataVencimento` usando o calendário de São Paulo na data da execução. Não fixe essas expectativas na data de elaboração do roteiro.

| Caso | Resultado esperado |
| --- | --- |
| Pendente com vencimento ontem | `atrasada: true`; incluída em `atrasadas=true` |
| Pendente com vencimento hoje | `atrasada: false` |
| Pendente com vencimento amanhã | `atrasada: false` |
| Concluída com vencimento ontem | `atrasada: false`; incluída em `atrasadas=false` |
| Tarefa sem vencimento | Não atrasada; fora de intervalos de vencimento |
| Intervalo com a mesma data nos dois limites | Inclui tarefa vencendo nessa data |
| Filtros sem correspondência | `200` com `[]` |
| Título com 151 caracteres ou descrição com 2.001 | `400` |
| Categoria zero/negativa; projeto zero/negativo | `400` |
| `prioridade: 99` no corpo; `status=99` na URL | `400` |
| PUT com categoria/projeto inexistente | `400`, sem alterar a tarefa salva |
| GET, PUT e DELETE de ID inexistente | `404` |
| PUT que altera título/status | Mantém `id` e `criadaEmUtc` |
| Reiniciar API e consultar tarefa | Dados preservados; criação continua em UTC (`Z`) |
| Excluir categoria/projeto ainda vinculado | `409` e tarefa preservada |
| Remover todos os vínculos e excluir categoria/projeto | `204` |

Teste de raciocínio para a virada do dia: em `2026-10-19T01:00:00Z`, ainda é 18/10 em São Paulo. Uma tarefa pendente com vencimento 18/10 não está atrasada nesse instante. Em `2026-10-19T03:00:00Z`, o dia local passa a 19/10 e ela fica atrasada. Não mude o relógio do computador; para automatizar esse cenário futuramente, forneça um relógio controlável ao serviço. `EstaAtrasada` já recebe o dia explicitamente e pode ser testada com datas fixas.

**Verificação final:** repita os testes de Projetos e Categorias depois dos de Tarefas. Para liberar a categoria criada na base, exclua também a primeira tarefa criada sem projeto; ela continua vinculada à categoria.

### M6. Commits, publicação e resolução de conflitos

Commits sugeridos por etapa:

1. `feat: adiciona CRUD de tarefas e validacao de vinculos`
2. `feat: adiciona filtros e atraso no fuso de Sao Paulo`
3. `test: documenta cenarios HTTP de tarefas`

Na raiz, prepare apenas os arquivos implementados, revise e publique:

```powershell
git status --short
git add TaskManager.Api/Program.cs TaskManager.Api/Services/RegrasTarefa.cs TaskManager.Api/Requests/tarefas.http
git diff --cached
git commit -m "feat: implementa tarefas e filtros"
git push -u origin feat/tarefas-filtros
```

O commit agregado é uma alternativa aos commits por etapa. Não reatribua a autoria de trabalho de Felipe. Abra um pull request de `feat/tarefas-filtros` para `main` e peça a revisão dele.

Se `main` mudou enquanto você trabalhava, com suas alterações já commitadas:

```powershell
git fetch origin
git merge origin/main
```

Se houver conflito em `Program.cs`:

1. Execute `git status` e abra o arquivo no VS Code.
2. Encontre `<<<<<<<`, `=======` e `>>>>>>>`.
3. Combine os trechos preservando os `using`, configuração do contexto, endpoints de Projetos/Categorias e endpoints de Tarefas. Mantenha um único `builder`, um único `app` e `app.Run()` no final.
4. Remova todos os marcadores. Não aceite automaticamente o arquivo inteiro de um lado: isso pode apagar endpoints.
5. Compile e execute os testes das duas fases.
6. Finalize o merge e publique:

```powershell
git add TaskManager.Api/Program.cs
git commit -m "merge: integra base e endpoints de tarefas"
git push
```

Se outros arquivos conflitaram, resolva e adicione cada um antes do commit. Se precisar desistir de um merge ainda em andamento, `git merge --abort` retorna ao estado anterior ao merge; por isso comece com seu trabalho commitado. Não use `push --force` para resolver conflito. Se o `pull --ff-only` falhar na `main`, pare de repetir o comando e confira `git status` e `git log --oneline --graph --all` antes de integrar históricos.

Se futuramente ambos alterarem o modelo, coordenem a ordem das migrations: não apaguem migrations já integradas/aplicadas para resolver conflitos. Neste roteiro, Marcos não modifica o modelo e não precisa gerar migrations.

### M7. Entrega conjunta

Depois de revisar o pull request, integrem em `main`. Para preservar os commits individuais, usem merge que mantenha o histórico. Em cada computador, com a árvore de trabalho limpa:

```powershell
git switch main
git pull --ff-only origin main
dotnet tool restore
dotnet restore TaskManager.sln
dotnet build TaskManager.sln
dotnet ef database update --project TaskManager.Api --startup-project TaskManager.Api
dotnet run --project TaskManager.Api --no-launch-profile --urls http://localhost:5000
```

Reexecutem os cenários essenciais em ambos os computadores. Como teste de reprodução, um integrante pode clonar em uma nova pasta e seguir somente o README: restore, migration, execução e criação dos dados via HTTP devem bastar.

Checklist do Marcos:

- [ ] Base de Felipe validada antes das alterações.
- [ ] CRUD completo de tarefas, com `404` para recurso inexistente.
- [ ] Categoria obrigatória e projeto opcional validados em POST e PUT.
- [ ] Padrões de status/prioridade aplicados na criação.
- [ ] Data de criação UTC preservada em atualização e após leitura do banco.
- [ ] Filtros isolados e combinados funcionando.
- [ ] Vencimento de hoje, atrasado, futuro, concluído e ausente testados.
- [ ] Erros de entrada retornam `400`, sem salvar alterações inválidas.
- [ ] Exclusões de categoria/projeto respeitam vínculos (`409`).
- [ ] Arquivo `.http` e README atualizados conforme o comportamento final.
- [ ] Commits próprios publicados e pull request revisado.

Checklist conjunta:

- [ ] `main` compila nos dois computadores.
- [ ] Banco é reconstruível por migrations; nenhum `.db` foi versionado.
- [ ] Projetos, Categorias e Tarefas funcionam após integração.
- [ ] Histórico registra contribuições dos dois integrantes.
- [ ] README permite reproduzir a execução a partir de um clone novo.
- [ ] Contrato deste roteiro conferido contra o enunciado original antes da entrega acadêmica.

### M8. Problemas comuns

| Sintoma | Como investigar/corrigir |
| --- | --- |
| SDK solicitado não encontrado | Execute `dotnet --list-sdks`; instale SDK 8 para atender ao `global.json` |
| `dotnet ef` não encontrado | Na raiz, execute `dotnet tool restore` e confira `.config/dotnet-tools.json` |
| Pacote incompatível com `net8.0` | Confira `.csproj`: os pacotes EF deste roteiro devem ser 8.0.x |
| `no such table` | Aplique `dotnet ef database update` com os dois parâmetros de projeto do roteiro |
| `404` em todas as rotas de tarefas | Confira inserção dos endpoints antes de `app.Run()` e reinicie a API |
| Porta 5000 em uso | Encerre a outra instância ou use 5001 tanto em `--urls` quanto nos arquivos `.http` |
| Categoria/projeto inexistente | Crie no seu banco; IDs não são compartilhados entre computadores |
| `409` ao excluir categoria/projeto | Liste tarefas, remova/altere os vínculos e tente de novo |
| Banco bloqueado | Feche transações de ferramentas SQLite e instâncias duplicadas da API |
| Filtro retorna vazio | Confira valores dos enums, IDs, datas e combinação com E |
| Alteração não aparece | Salve o arquivo, encerre a execução antiga, compile e execute novamente |

### Referências de estudo

Os nomes abaixo correspondem aos arquivos locais dentro de `export_notion_DevSoftware_CSharp/Desenvolvimento de Software Visual` (os nomes reais também incluem um identificador do Notion):

| Material do professor | Aplicação neste roteiro |
| --- | --- |
| `Configuração de Ambiente` | SDK 8, VS Code, extensões e Git |
| `CLI - C#` | CLI, dependências e migrations |
| `Criando Soluções e Projetos com NET` | Solution e organização do projeto |
| `C# - Minimal APIs` | `Program.cs`, rotas e respostas HTTP |
| `C# - Conexão da Aplicação com Banco de Dados` | SQLite, contexto e migrations |
| `C# - CRUD com Entity Framework e SQLite` | Operações assíncronas e testes `.http` |
| `Git e GitHub – Controle de versão e colaboração` | Clone, branch, commit, push e pull |

Os exemplos foram adaptados ao Task Manager: acrescentam contratos de entrada, validações, relacionamentos, versões EF compatíveis com .NET 8 e regras de datas. As referências oficiais complementares estão junto das decisões correspondentes nas seções F2 e F4.
