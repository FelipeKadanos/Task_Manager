# ✅ TaskManager

>**C# · .NET 8 · Minimal API · Entity Framework Core · SQLite**

## 👥 Identificação

| Informação | Detalhes |
| --- | --- |
| Disciplina | Desenvolvimento de Software Visual |
| Curso | Ciência da Computação |
| Integrantes | Felipe Kadanos
|  | Marcos Jose Ferrari

## 📘 Resumo

O TaskManager ferramenta para organizar tarefas e acompanhar o andamento de projetos. O sistema permite cadastrar tarefas com descrição, prazo, status e prioridade, além de vinculá-las opcionalmente a categorias e projetos.

## 🚀 Como rodar o projeto

**Pré-requisitos:** Git e SDK .NET 8 instalados. Execute os comandos abaixo no PowerShell.

**1. Clone o repositório e entre na pasta da API:**

```powershell
git clone https://github.com/FelipeKadanos/Task_Manager.git
cd .\TaskManager\TaskManager.Api
```

**2. Restaure as dependências:**

```powershell
dotnet restore
```

**3. Confira a ferramenta do Entity Framework:**

```powershell
dotnet ef --version
```

Se ela não estiver instalada, execute:

```powershell
dotnet tool install --global dotnet-ef --version 8.0.31
```

Use uma versão 8.x da ferramenta, compatível com os pacotes do projeto.

**4. Prepare o banco SQLite:**

```powershell
dotnet ef migrations list
```

Se não houver nenhuma migração, crie a primeira:

```powershell
dotnet ef migrations add CriacaoInicial
```

Em seguida, aplique as migrações. Esse comando cria o banco `TaskManager.db` e suas tabelas, caso ainda não existam:

```powershell
dotnet ef database update
```

**5. Inicie a API na porta 5000:**

```powershell
dotnet run --no-launch-profile --urls http://localhost:5000
```

Durante o desenvolvimento:

```powershell
dotnet watch run --no-launch-profile --urls http://localhost:5000
```
Ao salvar uma alteração, o `dotnet watch` aplica as mudanças compatíveis com Hot Reload. Quando for necessário reiniciar a aplicação, a ferramenta pode solicitar confirmação no terminal.

Acesse **http://localhost:5000** para conferir a mensagem de boas-vindas. A opção `--no-launch-profile` evita que a porta configurada no perfil de execução substitua a porta desejada.

Para testar pelo arquivo HTTP, configure `@baseUrl = http://localhost:5000` em [Tests/Test.http](TaskManager.Api/Tests/Test.http). Para encerrar a API, pressione `Ctrl+C` no terminal.

---
### Configurar o React

**1. Entrando na pasta do front-end**
```powershell
cd ..
cd .\task_manager.front
```

**2. Restaurando dependencias**
```powershell
npm i
```

## 🔎 Descrição das funcionalidades

### Gerenciamento de tarefas

Cada tarefa possui: 
 * Título
 * Descrição opcional
 * Data de criação
 * Vencimento opcional
 * Status 
 * Prioridade

| Valor | Status | Prioridade |
| --- | --- | --- |
| `0` | Pendente | Baixa |
| `1` | Em andamento | Média |
| `2` | Concluída | Alta |

### Categorias e projetos

Categorias classificam as tarefas, enquanto projetos agrupam atividades relacionadas. A exclusão de uma categoria ou projeto está configurada para remover o vínculo, preservando a tarefa.

### Atualizações individuais

Os endpoints abaixo permitem alterar somente o status ou a prioridade, sem reenviar os demais dados da tarefa:

```http
PATCH /api/tarefas/{id}/status
PATCH /api/tarefas/{id}/prioridade
```

Envie `Content-Type: application/json` e o número desejado no corpo. Por exemplo, `2` status concluído ou prioridade alta, conforme o endpoint.

### Resumo de produtividade

```http
GET /api/projetos/{id}/resumo
```

O resumo considera apenas as tarefas vinculadas ao projeto consultado. Tarefas de alta prioridade são contadas independentemente do status. Projetos sem tarefas retornam os indicadores zerados.

Exemplo de resposta:

```json
{
  "projetoId": 1,
  "projetoNome": "Projeto da disciplina",
  "totalTarefas": 3,
  "pendentes": 1,
  "emAndamento": 1,
  "concluidas": 1,
  "altaPrioridade": 1,
  "percentualConclusao": 33.33
}
```

## ▶️ Execução e testes

Consulte o [guia de instruções](Instructions.md) para os comandos de criação do projeto, instalação dos pacotes, preparação do banco e execução da API.

As requisições para testes manuais estão em [Tests/Test.http](TaskManager.Api/Tests/Test.http). Execute-as em ordem pela extensão REST Client do VS Code, utilizando um banco de teste e a URL exibida no terminal. Os cadastros precisam ser executados antes das requisições que utilizam seus IDs.

## 🔗 Repositório

**[Link do repositório:](https://github.com/FelipeKadanos/Task_Manager)** [ `https://github.com/FelipeKadanos/Task_Manager` ]

## 🤖 Uso de IA

- **Ferramenta utilizada:** Codex, da OpenAI.
- **Formas de uso:** 
  * Elaboração do projeto:
    * Elaboração das funcionalidades
    * Elaboração do banco de dados e relacionamentos
  * Elaboração e organização deste README.
  * Criação dos `Test/Test.http`
  * Criação do `.gitignore`
  * Ajuste na ordem do json de resposta
- **Exemplo de solicitação:** “Crie o README da maneira que a documentação do trabalho pede. Ele pode ser simples e direto, mas deve ser bonito.”
- **Revisões realizadas pela equipe:** 
  * Execução dos testes
  * Conferencia dos codigos escritos para o banco de dados
  * Ajustes no escopo inicial do projeto
