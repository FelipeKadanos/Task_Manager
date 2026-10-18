# C# - CRUD com Entity Framework e SQLite

<aside>
💡 Este material fornece um guia completo sobre como implementar operações CRUD (Criar, Ler, Atualizar, Deletar) usando uma Minimal API em C# com o Entity Framework Core. O documento inclui exemplos detalhados de código para configuração do modelo de dados, DbContext, e criação de endpoints específicos para cada operação CRUD. Além disso, oferece um exemplo de arquivo de testes para ser utilizado com o Rest Client, facilitando a execução e validação das funcionalidades da API.

</aside>

## **Configuração Inicial**

- **Modelo de Dados**:
    
    ```csharp
    public class Produto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public decimal Preco { get; set; }
    }
    ```
    
- **DbContext**:
    
    ```csharp
    public class SeuDbContext : DbContext
    {
        public DbSet<Produto> Produtos { get; set; }
    
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=caminho_para_seu_arquivo.db");
        }
    }
    ```
    
- **Configuração da API**:
    
    ```csharp
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddDbContext<SeuDbContext>();
    var app = builder.Build();
    ```
    

## **Operações CRUD**

- **Criação (POST)**
    
    ```csharp
    app.MapPost("/produto", async (Produto produto, SeuDbContext context) =>
    {
        context.Add(produto);
        await context.SaveChangesAsync();
        return Results.Created($"/produto/{produto.Id}", produto);
    });
    ```
    
- **Leitura (GET)**
    - Todos os Produtos:
        
        ```csharp
        app.MapGet("/produtos", async (SeuDbContext context) =>
        {
            var produtos = await context.Produtos.ToListAsync();
            return Results.Ok(produtos);
        });
        ```
        
    - Produto por Nome:
        
        ```csharp
        app.MapGet("/produtos/{nome}", async (string nome, SeuDbContext context) =>
        {
            var produto = await context.Produtos
                                        .Where(p => p.Nome.Contains(nome))
                                        .ToListAsync();
            return produto.Any() ? Results.Ok(produto) : Results.NotFound("Nenhum produto encontrado.");
        });
        ```
        
- **Atualização (PUT)**
    
    ```csharp
    app.MapPut("/produtos/{id}", async (int id, Produto produtoAtualizado, SeuDbContext context) =>
    {
        var produto = await context.Produtos.FindAsync(id);
        if (produto == null)
        {
            return Results.NotFound("Produto não encontrado.");
        }
    
        produto.Nome = produtoAtualizado.Nome;
        produto.Preco = produtoAtualizado.Preco;
        await context.SaveChangesAsync();
        return Results.Ok("Produto atualizado com sucesso.");
    });
    ```
    
- **Remoção (DELETE)**
    
    ```csharp
    app.MapDelete("/produtos/{id}", async (int id, SeuDbContext context) =>
    {
        var produto = await context.Produtos.FindAsync(id);
        if (produto == null)
        {
            return Results.NotFound("Produto não encontrado.");
        }
    
        context.Produtos.Remove(produto);
        await context.SaveChangesAsync();
        return Results.Ok("Produto removido com sucesso.");
    });
    
    ```
    

## **Testes com Rest Client**

Aqui está um exemplo de arquivo para testar a API com o Rest Client (VS Code extension). Você pode criar um arquivo `.http` e colocar o seguinte conteúdo:

```
@baseUrl = <http://localhost:5000>

### Criar Produto
POST {{baseUrl}}/produto
Content-Type: application/json

{
  "Nome": "Coca-Cola",
  "Preco": 5.00
}

### Obter Todos os Produtos
GET {{baseUrl}}/produtos

### Buscar Produto por Nome
GET {{baseUrl}}/produtos/Coca-Cola

### Atualizar Produto
PUT {{baseUrl}}/produtos/1
Content-Type: application/json

{
  "Nome": "Coca-Cola Zero",
  "Preco": 6.00
}

### Deletar Produto
DELETE {{baseUrl}}/produtos/1
```