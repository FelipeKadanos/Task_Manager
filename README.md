# Task Manager

API de tarefas em C# com .NET 8, Minimal API, Entity Framework Core e SQLite.
O código segue o estilo do exemplo de Ecommerce: endpoints diretos, classes simples e validações com `if`.

## Como executar

Na pasta da solução:

```powershell
dotnet tool restore
dotnet restore
dotnet ef database update --project TaskManager.Api
dotnet run --project TaskManager.Api --launch-profile http
```

A API fica em `http://localhost:5000`. O banco é `TaskManager.Api/taskmanager.db`.
As migrações existentes foram mantidas; não é necessário apagar o banco.

## Onde encontrar cada parte

- `TaskManager.Api/Program.cs`: configuração e endpoints de projetos, categorias e tarefas.
- `TaskManager.Api/Models`: uma classe por entidade e os enums de status e prioridade.
- `TaskManager.Api/Contracts`: classes com os dados recebidos nas requisições. Impedem que o cliente defina o ID ou a data de criação.
- `TaskManager.Api/Data/AppDataContext.cs`: tabelas e regras do banco.
- `TaskManager.Api/Services/RegrasTarefa.cs`: validação e preenchimento compartilhados entre cadastro e alteração, além do cálculo de atraso.
- `TaskManager.Api/Requests`: exemplos para a extensão REST Client do VS Code.

Comece pelo POST de categorias em `Program.cs`, depois pelo POST de tarefas.
As operações no banco são síncronas, como no exemplo, para facilitar a leitura neste projeto de estudo.

## Rotas e regras

`/projetos`, `/categorias` e `/tarefas` aceitam GET e POST. Para buscar, alterar ou excluir um registro, use GET, PUT ou DELETE com `/{id}`.

Uma tarefa exige título e categoria existente; o projeto é opcional. No cadastro, status e prioridade omitidos recebem Pendente (0) e Média (1). Na alteração, ambos são obrigatórios.

Status: Pendente (0), Em andamento (1), Concluída (2). Prioridade: Baixa (0), Média (1), Alta (2).

GET `/tarefas` aceita os filtros `status`, `prioridade`, `categoriaId`, `projetoId`, `vencimentoDe`, `vencimentoAte` e `atrasadas`. Datas usam `AAAA-MM-DD`.
Uma tarefa fica atrasada quando seu vencimento é anterior ao dia atual em São Paulo e ela não está concluída.
Projetos e categorias com tarefas vinculadas não podem ser excluídos.

## Verificação

```powershell
dotnet build
powershell -ExecutionPolicy Bypass -File scripts/Testar-Api.ps1
```

O script usa um banco temporário e encerra a instância de teste ao terminar.