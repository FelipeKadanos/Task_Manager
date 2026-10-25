# Passo a passo para criar o TaskManager

Guia para usar no terminal PowerShell. Execute os comandos na ordem. Para criar do zero, comece em uma pasta que ainda não contenha uma pasta `TaskManager`.

## 1. Conferir o SDK

```powershell
dotnet --list-sdks
```

Para seguir o requisito da prova, tenha o SDK .NET 8 instalado. O projeto terá como destino `net8.0`.

## 2. Criar a solução e a API

```powershell
dotnet new sln -o TaskManager
cd .\TaskManager
dotnet new web -n TaskManager.Api -f net8.0
dotnet sln add .\TaskManager.Api\TaskManager.Api.csproj
cd .\TaskManager.Api
```

A partir daqui, execute os comandos dentro da pasta `TaskManager.Api`. A solução pode ser `.sln` ou `.slnx`, conforme o SDK usado.

## 3. Instalar os pacotes

Use as versões do projeto atual para manter os pacotes do Entity Framework compatíveis com o .NET 8:

```powershell
dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 8.0.31
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.31
dotnet restore
```

Opcional: o projeto atual também possui Newtonsoft.Json, mas os endpoints usam o suporte a JSON da própria Minimal API.

```powershell
dotnet add package Newtonsoft.Json --version 13.0.4
```

## 4. Criar as pastas e escrever o código

```powershell
New-Item -ItemType Directory -Path Models, Data, Tests -Force
```

Antes de criar a migração, escreva ou copie os arquivos nesta ordem:

1. `Models`: classes `Categoria`, `Projeto` e `Tarefa`, e enums `StatusTarefa` e `PrioridadeTarefa`.
2. `Data/AppDataContext.cs`: herdar de `DbContext`, declarar os três `DbSet` e configurar `UseSqlite("Data Source=TaskManager.db")`.
3. No contexto, configurar os relacionamentos de `Tarefa` com `Categoria` e `Projeto`, usando os IDs e as propriedades de navegação.
4. `Program.cs`: registrar `builder.Services.AddDbContext<AppDataContext>();` antes de `builder.Build()` e adicionar os endpoints antes de `app.Run()`.
5. `Tests/Test.http`: adicionar as requisições para testar a API.

Os comandos criam a estrutura; as classes e os endpoints precisam ser escritos. Confira a compilação antes de continuar:

```powershell
dotnet build
```

## 5. Preparar a ferramenta do Entity Framework

Confira se já está instalada:

```powershell
dotnet ef --version
```

Se o comando não existir, instale:

```powershell
dotnet tool install --global dotnet-ef --version 8.0.31
```

Se já estiver instalada em outra versão principal, alinhe com o EF Core 8 do projeto:

```powershell
dotnet tool uninstall --global dotnet-ef
dotnet tool install --global dotnet-ef --version 8.0.31
```

## 6. Criar a primeira migração e o banco

Somente ao criar o projeto do zero, sem migrações existentes:

```powershell
dotnet ef migrations add CriacaoInicial
dotnet ef database update
```

A migração descreve as tabelas e os relacionamentos. O `database update` aplica essa estrutura ao SQLite e cria o banco se ele não existir.

Confira o arquivo e se há migrações pendentes:

```powershell
Get-Item .\TaskManager.db
dotnet ef migrations list
```

## 7. Executar e testar

```powershell
dotnet run --launch-profile http
```

Use a URL exibida no terminal em `@baseUrl` no arquivo HTTP. No projeto atual, o perfil HTTP usa `http://localhost:5000`; um projeto novo pode receber outra porta.

Na extensão REST Client do VS Code, clique em **Send Request** e execute os blocos em ordem: cadastros de projeto e categoria, tarefas, consultas, alterações, resumo e exclusões. As referências aos IDs só funcionam depois de executar os respectivos cadastros.

Para verificar a persistência, cadastre um registro, pare a API com `Ctrl+C`, execute novamente e consulte o mesmo ID antes de excluí-lo.

## 8. Quando alterar as classes ou os relacionamentos

Pare a API com `Ctrl+C`. Crie uma nova migração com um nome que descreva a alteração:

```powershell
dotnet build
dotnet ef migrations add AjustaRelacionamentos
dotnet ef database update
dotnet run --launch-profile http
```

Use um nome diferente para cada migração. Alterar apenas a lógica de um endpoint não exige migração.

## Se o projeto já estiver pronto

Não execute os comandos de criação novamente. Entre na pasta `TaskManager.Api`, confira a ferramenta `dotnet ef` conforme o passo 5 e execute:

```powershell
dotnet restore
dotnet build
dotnet ef database update
dotnet run --launch-profile http
```

Esse caminho pressupõe que as migrações já estejam no projeto. Se não houver nenhuma, siga o passo 6.
