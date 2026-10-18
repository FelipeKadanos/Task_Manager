# Configuração de Ambiente

<aside>
💡 Para a configuração do ambiente você precisa instalar o **SDK .NET** *(backend)*, o **node.js** *(gerenciador de pacotes - NPM)***, Visual Studio Code** com **extensões** *(editor de texto)* e o Git para versionamento do projeto**.**

</aside>

## SDK .NET

---

O **SDK .NET** será utilizado para que seja possível criar aplicações utilizando a linguagem de programação **C#,** na disciplina criaremos projetos **Console Application** *(fundamentos)* ****e **Web API** *(projeto da disciplina)***.**

**Instalação:** instalação padrão do software sem nenhuma configuração adicional, seguido do teste abaixo para verificar se a instalação foi bem sucedida. Instalar a versão 8**.0** para a sua plataforma *(SO)* e processador *(x86 ou x64)*.

**Página de download:** [https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download)

**Teste da instalação:** executar o terminal na sua máquina com o comando abaixo. O terminal deve mostrar uma mensagem com a versão instalada da aplicação, caso contrário, verificar o problema ocorrido e instalar novamente.

```bash
dotnet --info
dotnet --version
8.x.xx
```

---

## Visual Studio Code

---

O **Visual Studio Code** é um editor de código-fonte leve, mas poderoso, que é executado nas plataformas **Windows, macOS** e **Linux.** Ele vem com suporte integrado para JavaScript, TypeScript e Node.js e tem um rico ecossistema de extensões para outras linguagens (como C ++, C #, Java, Python, PHP, Go) e tempos de execução (como .NET e Unity).

**Instalação:** instalação padrão do software sem nenhuma configuração adicional, seguida das sugestões abaixo para personalização do ambiente e aumento da produtividade.

**Página de download:** [https://code.visualstudio.com/](https://code.visualstudio.com/)

Extensões:

- **~~Angular Language Service:** Extensão que auxiliará no desenvolvimento de projetos em Angular;~~
- **Auto Import:** Auxilia no importação de módulos externos;
- **Auto-Save on Window Change:** salva os arquivos ao mudar de janela;
- **Auto Rename Tag:** Permite renomear uma tag sem a necessidade de alterar o fechamento da tag;
- **~~Bracket Pair Colorize 2:** Cria marcações coloridas na abertura e fechamento de tags, métodos e etc;~~
- **C#**: Extensão que auxiliará no desenvolvimento de projetos em C#;
- **C# Dev Kit**: Extensão que auxiliará no desenvolvimento de projetos em C#;
- **Color Highlight:** Identifica e colore trechos de código onde cores são identificadas;
- **Dracula Official:** Modifica o tema do ambiente de trabalho;
- **EditorConfig for VS Code:** Auxilia na construção do arquivo .editorconfig;
- **ES7+ React/Redux/React-Native snippets:** Extensão que auxiliará no desenvolvimento de projetos em React;
- **HTML CSS Support:** Dá suporte para a escrita de código HTML e CSS;
- **Live Server:** Permite executar e alterar trechos de código sem a necessidade de recarregar o navegador;
- **Material Icon Theme:** Altera os ícones de pastas e arquivos;
- **Path Intelisense:** Ajuda encontrar caminhos, arquivos e pastas dentro da workspace;
- **Prettier:** Organiza, formata e identa o código;
- **Rest Client** - Permite executar requisições HTTP direto do Visual Studio Code;
- **~~Settings Sync:** Permite guardar as configurações do Visual Studio Code para utilizar em qualquer máquina;~~
- **~~Visual Studio IntelliCode:** Recurso de autocompletar para várias linguagens.~~

![](Configura%C3%A7%C3%A3o%20de%20Ambiente/Untitled.png)

## Node.js

---

**Node.js** é um motor de tempo de execução JavaScript de código aberto. Ele executa o código JavaScript fora de um navegador. Para a disciplina utilizaremos apenas o **NPM** (gerenciador de pacotes do node) para instalar o **Angular** e executar alguns comandos.

**Instalação:** instalação padrão do software sem nenhuma configuração adicional, seguido do teste abaixo para verificar se a instalação foi bem sucedida. Instalar a versão LTS.

**Página de download:** [https://nodejs.org/en/](https://nodejs.org/en/)

**Teste da instalação:** executar o terminal na sua máquina com o comando abaixo. O terminal deve mostrar uma mensagem com a versão instalada da aplicação, caso contrário, verificar o problema ocorrido e instalar novamente.

```bash
node --version
17.x+

npm --version
8.x+

npx --version
8.x+
```

## React

---

Não é necessária nenhuma instalação adicional para criar um projeto React, apenas com a instação do Node.js, já conseguimos criar um projeto em React.

```bash
npx create-react-app my-app --template typescript
```

## Git Bash

---

**Git** é um sistema de controle de versão distribuído gratuito e de código aberto projetado para lidar com tudo, desde projetos pequenos até muito grandes, com velocidade e eficiência.

**Instalação:** instalação padrão do software sem nenhuma configuração adicional, seguido do teste abaixo para verificar se a instalação foi bem sucedida. **Você pode instalar a ferramenta da sua preferência para trabalhar com o GitHub.**

**Página de download:** [https://git-scm.com/downloads](https://git-scm.com/downloads)

**Teste da instalação:** executar o terminal na sua máquina com o comando abaixo. O terminal deve mostrar uma mensagem com a versão instalada da aplicação, caso contrário, verificar o problema ocorrido e instalar novamente.

```bash
git --version
git version 2.30.0.windows.2
```