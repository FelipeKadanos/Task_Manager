# C# - Conexão da Aplicação com Banco de Dados

<aside>
💡 Este material agora inclui informações sobre ferramentas e métodos para visualizar e interagir com a base de dados, além das seções anteriores sobre conexão com banco de dados, configuração de uma Minimal API em C# usando SQLite, e gerenciamento do banco de dados com migrações.

</aside>

## Informações Gerais

---

1. **Escolha do Banco de Dados**
    - Bancos de dados relacionais (MySQL, PostgreSQL, SQL Server) para dados estruturados e relações complexas.
    - Bancos de dados NoSQL (MongoDB, Cassandra, Firebase) para grandes volumes de dados com estruturas variadas.
2. **Instalação e Configuração**
    - Instale o banco de dados escolhido e configure-o (usuários, permissões, rede).
3. **Drivers e Bibliotecas**
    - Use drivers ou bibliotecas específicas para a sua linguagem de programação para conectar sua aplicação ao banco de dados.
4. **String de Conexão**
    - Uma string de conexão é necessária para informar como se conectar ao banco de dados.
5. **CRUD Operations**
    - Aprenda a realizar operações de CRUD (Create, Read, Update, Delete).
6. **Segurança**
    - Proteja sua aplicação e banco de dados contra ataques comuns. Use conexões criptografadas e armazene dados sensíveis de forma segura.
7. **Práticas Recomendadas**
    - Utilize pool de conexões, ORMs para aumentar a produtividade, e monitore e otimize o desempenho.

## Minimal API em C# com SQLite

---

1. **Adicionar Pacotes Necessários**
    - Adicione `Microsoft.EntityFrameworkCore.Sqlite` e `Microsoft.EntityFrameworkCore.Design`.
    
    ```bash
    dotnet add package Microsoft.EntityFrameworkCore.Sqlite
    ```
    
    ```bash
    dotnet add package Microsoft.EntityFrameworkCore.Design
    ```
    
2. **Configurar o SQLite e Definir o Modelo de Dados**
    - Crie um modelo de dados e uma classe de contexto do banco de dados (`AppDbContext`).
    
    ```csharp
    public class Produto
    {
    	public int Id { get; set; }
    	public string Nome { get; set; }
    	public decimal Preco { get; set; }
    }
    ```
    
    - Crie um modelo de dados e uma classe de contexto do banco de dados (`AppDbContext`).
    
    ```csharp
    using Microsoft.EntityFrameworkCore;
    
    public class AppDbContext : DbContext
    {
        public DbSet<Produto> Produtos { get; set; }
    
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=nomeDoSeuBanco.db");
        }
    }
    ```
    

## Gerenciamento do Banco de Dados com Migrações

---

Para criar e gerenciar o banco de dados SQLite em seu projeto .NET com Entity Framework Core, você pode usar as migrações. As migrações permitem que você defina alterações incrementais no seu esquema de banco de dados de forma programática. Isso facilita a criação, atualização e versionamento do banco de dados conforme o desenvolvimento do seu projeto. Aqui estão os passos:

1. **Preparar o Projeto para Migrações**
    - Depois de definir seu modelo de dados e o contexto do banco de dados (`AppDbContext`), como mencionado anteriormente, você está pronto para usar migrações para criar ou atualizar o banco de dados.
2.  **Adicionar a Primeira Migração**
    - No terminal, dentro do diretório do seu projeto, execute o comando para adicionar sua primeira migração. Este comando gera os arquivos de migração que descrevem como criar o banco de dados. Substitua `InitialCreate` pelo nome que deseja dar a esta migração.
    
    ```bash
    dotnet ef migrations add InitialCreate
    ```
    
    - Se você encontrar um erro dizendo que o comando `ef` não foi encontrado, talvez seja necessário instalar a ferramenta global do Entity Framework Core, usando o seguinte comando:
    
    ```bash
    dotnet tool install --global dotnet-ef
    ```
    
3. **Aplicar a Migração para Criar o Banco de Dados**
    - Após adicionar sua primeira migração, aplique-a para criar o banco de dados. O Entity Framework Core usará a string de conexão definida no seu contexto (`AppDbContext`) para saber onde e como criar o banco de dados. Execute o seguinte comando:
    
    ```bash
    dotnet ef database update
    ```
    
    - Este comando irá aplicar as migrações pendentes ao banco de dados, efetivamente criando ou atualizando o esquema do banco de dados.
4. **Fazer Alterações no Modelo e Adicionar Novas Migrações**
    - Sempre que você fizer alterações nos seus modelos de dados que precisam ser refletidas no banco de dados, você deve adicionar uma nova migração. Por exemplo, se você adicionar uma nova propriedade a um modelo, execute o comando:
    
    ```bash
    dotnet ef migrations add NomeDaMigracaoParaAlteracao
    ```
    
    - Substitua `NomeDaMigracaoParaAlteracao` por um nome descritivo da alteração que você está fazendo.
5. **Atualizar o Banco de Dados com Novas Migrações**
    - Após adicionar a nova migração, aplique-a com o comando `dotnet ef database update`, assim como você fez com a primeira migração. Isso atualizará o banco de dados para refletir as alterações feitas nos modelos.

## Visualização da Base de Dados

---

Após configurar e gerenciar sua base de dados com as migrações, é útil saber como visualizar e interagir com os dados armazenados. Aqui estão algumas ferramentas e métodos que você pode usar para visualizar e gerenciar o banco de dados SQLite de forma eficaz.

### Ferramentas de Visualização de Banco de Dados

1. **DB Browser for SQLite**
    - Uma ferramenta visual de código aberto que permite visualizar, criar, projetar e editar arquivos de banco de dados SQLite.
    - **Como usar**: Baixe e instale o DB Browser, abra o arquivo do banco de dados SQLite, e você pode visualizar as tabelas, executar consultas SQL e editar dados diretamente.
2. **SQLite Studio**
    - Outra ferramenta gratuita e open-source para gerenciar bancos de dados SQLite. Oferece uma interface gráfica para facilitar a visualização e edição.
    - **Como usar**: Instale o SQLite Studio, conecte-se ao seu arquivo de banco de dados, e explore as tabelas e dados.
3. **Visual Studio Code com Extensões de SQLite**
    - Se você já usa o Visual Studio Code para desenvolvimento, pode adicionar uma extensão para SQLite que permite gerenciar e visualizar os bancos de dados diretamente da IDE.
    - **Extensões recomendadas**: SQLite Viewer, SQLite.
    - **Como usar**: Instale a extensão, abra a paleta de comandos (Ctrl+Shift+P), e digite 'SQLite' para ver os comandos disponíveis.