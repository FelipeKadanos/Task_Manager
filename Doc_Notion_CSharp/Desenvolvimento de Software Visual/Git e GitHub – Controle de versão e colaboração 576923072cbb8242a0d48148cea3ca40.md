# Git e GitHub – Controle de versão e colaboração

<aside>
💡 Este material tem como objetivo apresentar os conceitos fundamentais de Git e GitHub, bem como o uso prático no versionamento de código.

****Saiba como funciona o GitHub:
****[https://www.youtube.com/watch?v=w3jLJU7DT5E](https://www.youtube.com/watch?v=w3jLJU7DT5E&t=6s&ab_channel=GitHub)
[https://www.youtube.com/watch?v=pBy1zgt0XPc](https://www.youtube.com/watch?v=pBy1zgt0XPc)

</aside>

## 📁 Repositório

Um repositório é o local onde um projeto é armazenado e versionado. Ele contém todos os arquivos do sistema, além do histórico de alterações realizadas ao longo do tempo.

No GitHub, o repositório também funciona como um ambiente de colaboração, permitindo que múltiplos usuários contribuam para o mesmo projeto.

- Armazena código-fonte e documentação
- Mantém o histórico de alterações
- Permite colaboração entre usuários
- Pode ser público ou privado

---

## 🌿 Branch

As branches permitem a criação de versões paralelas de um projeto, possibilitando o desenvolvimento de novas funcionalidades sem impactar diretamente a versão principal.

Em cenários mais simples, é comum trabalhar apenas com a branch principal (`main ou master`), centralizando todas as alterações.

- Representa uma versão do projeto
- Permite desenvolvimento paralelo
- A branch principal geralmente é chamada de `main ou master`

---

## 📥 Clone

O clone é o processo de copiar um repositório remoto para o ambiente local, permitindo que o desenvolvimento seja realizado diretamente na máquina do usuário.

Essa cópia mantém conexão com o repositório original, possibilitando sincronização de alterações.

### 🔧 Comando:

```bash
git clone https://github.com/usuario/repositorio.git
```

---

## 💾 Commit

O commit representa o registro de alterações realizadas no projeto. Cada commit armazena um conjunto de mudanças, junto com informações sobre quem realizou a alteração e quando ela ocorreu.

As mensagens de commit são importantes para descrever o que foi modificado, facilitando o entendimento do histórico do projeto.

- Representa uma alteração no projeto
- Possui identificador único
- Deve conter mensagem descritiva

### 🔧 Comando:

```bash
git add .
git commit -m "Descrição da alteração"
```

---

## 📤 Push

O push é utilizado para enviar as alterações realizadas localmente para o repositório remoto, tornando-as disponíveis para outros colaboradores.

### 🔧 Comando:

```bash
git push
```

---

## 📥 Pull

O pull é utilizado para atualizar o repositório local com as alterações mais recentes disponíveis no repositório remoto. Esse processo é essencial para manter o ambiente sincronizado.

### 🔧 Comando:

```bash
git pull
```

---

## 📦 Criação do repositório no GitHub

Antes de iniciar o uso do Git, é necessário criar um repositório no GitHub.

Ao criar o repositório, recomenda-se:

- Adicionar um arquivo **README**, que será utilizado para descrever o projeto
- Adicionar um arquivo **.gitignore**, para evitar o versionamento de arquivos desnecessários

📌 Esses arquivos ajudam na organização do projeto e seguem boas práticas utilizadas no mercado.

---

## 🚀 Fluxo básico de uso do Git (clonando um repositório e adicionando arquivos ao projeto)

O fluxo a seguir representa o processo mais comum de uso do Git utilizando um repositório previamente criado no GitHub.

---

## 1. Clonar o repositório

Realiza a cópia do repositório para o seu computador.

### 🔧 Comando:

```bash
git clone https://github.com/usuario/repositorio.git
```

---

## 2. Acessar a pasta do projeto

Após o clone, entre na pasta criada:

```bash
cd repositorio
```

---

## 3. Adicionar arquivos ao projeto

Após clonar o repositório, você terá em sua máquina uma pasta vinculada ao GitHub.

Existem duas possibilidades neste momento:

- **Projeto já existente:**
Copie todos os arquivos do seu projeto para dentro da pasta clonada.
Isso pode ser feito manualmente (copiar e colar) ou movendo os arquivos para essa pasta.
- **Projeto novo:**
Você também pode iniciar o desenvolvimento diretamente dentro da pasta clonada, criando os arquivos e a estrutura do projeto a partir desse local.

📌 Importante:

Todos os arquivos que estiverem dentro da pasta clonada poderão ser versionados pelo Git e enviados ao repositório.

---

## 4. Adicionar arquivos ao controle de versão

Após inserir os arquivos na pasta do projeto, é necessário informar ao Git quais arquivos devem ser acompanhados no versionamento. Esse processo é chamado de **stage**, onde os arquivos são preparados para serem incluídos no próximo commit.

### 🔧 Comando:

```bash
git add .
```

---

## 5. Registrar alterações (commit)

Após adicionar os arquivos ao controle de versão, é necessário registrar essas alterações no histórico do projeto.

Esse registro é feito por meio de um **commit**, que funciona como um ponto de salvamento, armazenando o estado atual dos arquivos e permitindo acompanhar a evolução do projeto ao longo do tempo. Cada commit deve conter uma mensagem descritiva, indicando de forma clara o que foi realizado.

### 🔧 Comando:

```bash
git commit -m "Primeiro commit"
```

---

## 6. Enviar alterações para o GitHub

Após registrar as alterações com o commit, é necessário enviá-las para o repositório remoto no GitHub.

Esse processo garante que o código fique armazenado online e disponível para outros colaboradores, além de manter o repositório atualizado com as últimas modificações realizadas localmente.

### 🔧 Comando:

```bash
git push
```

---

## 👥 Colaboração em projetos

O Git permite que múltiplos usuários trabalhem no mesmo projeto de forma organizada, mantendo controle sobre todas as alterações realizadas.

- Cada usuário possui uma cópia local do projeto
- As alterações são registradas por meio de commits
- O repositório remoto centraliza o histórico
- As atualizações são sincronizadas com push e pull