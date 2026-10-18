# Criando Soluções e Projetos com .NET

<aside>
💡 Este material apresenta de forma progressiva como criar, estruturar e executar projetos utilizando a plataforma .NET, com foco em prática real e organização profissional.

</aside>

## 🧠 1. Introdução ao .NET

O **.NET** é uma plataforma de desenvolvimento da Microsoft que permite criar aplicações modernas para web, desktop, mobile, cloud e APIs.

### 🔹 Evolução do .NET

- **.NET Framework** → legado (Windows apenas)
- **.NET Core** → multiplataforma
- **.NET 5+** → unificação da plataforma (atual padrão)

### 🔹 Principais características

- Multiplataforma (Windows, Linux, macOS)
- Alto desempenho
- Forte tipagem (C#)
- Suporte a APIs REST
- Integração com cloud (Azure)

📌 **Analogia:**

Pense no .NET como uma “caixa de ferramentas completa” para construir qualquer tipo de software.

---

## 🏗️ 2. Estrutura de uma Solution (.sln)

Uma **Solution (.sln)** é o nível mais alto de organização em projetos .NET.

📌 **Analogia:**

Uma *Solution* é como uma **pasta principal de um sistema**, e dentro dela ficam vários projetos com responsabilidades diferentes.

### 🔹 Organização comum

- API → entrada da aplicação
- Domain → regras de negócio
- Infrastructure → acesso a dados
- Application → lógica de aplicação

---

## 📦 3. Tipos de projetos no .NET

### 🔹 Console Application

Aplicações simples executadas no terminal.

### 🔹 Class Library

Bibliotecas reutilizáveis (regras de negócio, serviços, etc.)

### 🔹 ASP.NET Core Web API

Criação de APIs REST modernas.

### 🔹 Web App (MVC / Razor Pages)

Aplicações web com interface.

### 🔹 Quando usar?

| Tipo | Uso |
| --- | --- |
| Console | Testes, scripts |
| Class Library | Reuso de código |
| Web API | Backend |
| MVC | Sistemas web completos |

---

## ⚙️ 4. Criação de uma solução na prática

Nesta etapa, vamos criar uma solução completa usando o CLI do .NET.

Primeiro, criamos a solução principal:

```bash
dotnet new sln -o MinhaSolucao
```

Depoois, entramos na pasta da solução:

```bash
cd ./MinhaSolucao/
```

Agora criamos os projetos:

```bash
dotnet new webapi -n MinhaApi
```

Em seguida, adicionamos os projetos à solução:

```bash
dotnet sln add MinhaApi
```

📌 Isso organiza tudo dentro da Solution.

---

## 📦 5. Gerenciamento de dependências (NuGet)

O NuGet é o gerenciador de pacotes do .NET.

Para adicionar um pacote:

```bash
dotnet add package Newtonsoft.Json
```

Para restaurar dependências:

```bash
dotnet restore
```

📌 **Boas práticas:**

- Evite instalar pacotes desnecessários
- Prefira versões estáveis
- Atualize com cuidado

---

## 🧪 6. Fluxo completo passo a passo

1. Criar Solution
2. Criar projetos
3. Adicionar à Solution
4. Referenciar projetos
5. Instalar dependências
6. Configurar appsettings
7. Executar aplicação

📌 **Analogia:**

É como montar uma casa:

- Solution = terreno
- Projetos = cômodos
- Referências = portas entre os cômodos

---

## ⚠️ 7. Erros comuns (iniciantes)

- ❌ Criar tudo em um único projeto
- ❌ Misturar regra de negócio com API
- ❌ Não usar Solution
- ❌ Dependências circulares
- ❌ Ignorar versionamento