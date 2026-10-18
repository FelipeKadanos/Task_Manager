# Terminal no Windows – Comandos básicos

<aside>
💡 Este material apresenta os comandos básicos para utilização do terminal no Windows, utilizando o **Windows Terminal com PowerShell**, permitindo a execução de ferramentas como Git, .NET e Node.js.

</aside>

## 🖥️ Windows Terminal e PowerShell

O **Windows Terminal** é a aplicação recomendada para execução de comandos no Windows. Ele permite utilizar diferentes ambientes, sendo o **PowerShell** o mais comum.

O PowerShell é um interpretador de comandos que permite interagir com o sistema operacional por meio de instruções textuais.

- Permite navegar entre pastas
- Executar programas
- Rodar comandos de ferramentas (Git, .NET, Node)
- Automatizar tarefas

📌 Recomenda-se utilizar sempre o **Windows Terminal com PowerShell** para manter padronização.

---

## ⚠️ Sobre o uso do CMD (Prompt de Comando)

Embora o Windows possua o **CMD (Prompt de Comando)**, não é recomendado utilizá-lo neste contexto.

O CMD possui limitações em relação ao PowerShell, como:

- Menor quantidade de recursos
- Menor compatibilidade com ferramentas modernas
- Experiência menos produtiva para desenvolvimento

📌 Por esse motivo, utilize sempre o **PowerShell dentro do Windows Terminal**.

---

## ⌨️ Auto completar comandos (TAB)

O terminal possui um recurso de **auto completar**, que facilita a digitação de comandos e caminhos.

Ao começar a digitar o nome de uma pasta ou arquivo, pressione a tecla **TAB** para completar automaticamente.

### 🔹 Exemplo:

```bash
cd Doc+TAB
```

👉 O terminal pode completar para:

```bash
cd Documents
```

📌 Isso reduz erros de digitação e aumenta a produtividade.

---

## 📂 Navegação entre pastas

Para trabalhar com projetos, é necessário saber navegar entre diretórios no terminal.

---

### 🔹 Ver arquivos da pasta atual

```bash
dir
```

### 🔹 Entrar em uma pasta

```bash
cd nome-da-pasta
```

### 🔹 Voltar uma pasta

```bash
cd ..
```

### 🔹 Ir para um caminho específico

```bash
cd C:\Users\Usuario\Documents\projeto
```

---

## 📁 Manipulação básica de arquivos e pastas

O terminal também permite criar e organizar arquivos e diretórios.

---

### 🔹 Criar uma pasta

```bash
mkdir nome-da-pasta
```

---

### 🔹 Criar um arquivo

```powershell
New-Item arquivo.txt
```

---

### 🔹 Remover arquivo

```powershell
Remove-Item arquivo.txt
```

---

### 🔹 Remover pasta

```powershell
Remove-Item nome-da-pasta -Recurse
```

---

## 📌 Execução de comandos

No terminal, qualquer ferramenta instalada pode ser executada digitando seu comando correspondente.

Exemplo:

- `git` → comandos do Git
- `dotnet` → comandos do .NET
- `node` → comandos do Node.js

📌 É necessário que a ferramenta esteja instalada e configurada no sistema.

---

## 🐙 Comandos básicos do Git

O Git é utilizado para versionamento de código.

### 🔧 Exemplos:

```bash
git clone https://github.com/usuario/repositorio.git
git add .
git commit -m "Mensagem"
git push
git pull
```

---

## ⚙️ Comandos básicos do .NET

O .NET CLI permite criar, executar e gerenciar aplicações.

### 🔧 Exemplos:

```powershell
dotnet newweb
dotnet run
dotnet restore
dotnet add package NomeDoPacote
```

---

## 🟢 Comandos básicos do Node.js

O Node.js permite executar aplicações JavaScript e gerenciar dependências com o npm.

### 🔧 Exemplos:

```bash
npm init -y
npm install
npm installnome-do-pacote
node index.js
```

---

## 🎨 Personalização do Windows Terminal (opcional)

O Windows Terminal permite diversas formas de personalização, como:

- Alteração de fonte
- Cores e temas
- Ajustes de layout

Essa personalização é opcional, mas pode melhorar a experiência de uso no dia a dia.

📌 Para quem tiver interesse, seguem alguns conteúdos de referência:

- [https://www.youtube.com/watch?v=9zodIcv_7-M](https://www.youtube.com/watch?v=9zodIcv_7-M&utm_source=chatgpt.com)
- [https://www.youtube.com/watch?v=yxzYixOmPHg](https://www.youtube.com/watch?v=yxzYixOmPHg&utm_source=chatgpt.com)

---

## 📌 Observação

- A personalização não é obrigatória para utilização do terminal
- O foco principal deve ser o uso correto dos comandos
- Alterações visuais são apenas um complemento

---

## 📌 Boas práticas

Para melhor utilização do terminal:

- Sempre verificar em qual pasta está antes de executar comandos
- Utilizar nomes de pastas sem espaços ou com hífen
- Utilizar a tecla **TAB** para auto completar comandos
- Executar comandos com atenção para evitar exclusão indevida de arquivos
- Manter o ambiente organizado