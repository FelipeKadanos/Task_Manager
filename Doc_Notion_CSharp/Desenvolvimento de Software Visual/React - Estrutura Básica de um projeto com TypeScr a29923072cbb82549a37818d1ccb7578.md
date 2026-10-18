# React - Estrutura Básica de um projeto com TypeScript

---

<aside>
💡 Este guia ensina a criar um projeto React com TypeScript usando o comando `npx create-react-app my-app --template typescript`. Inclui a remoção de arquivos desnecessários e a modificação dos arquivos `App.tsx` e `index.tsx` para corresponder ao projeto fornecido. O resultado é uma estrutura de projeto limpa e organizada. Recursos adicionais são fornecidos para mais aprendizado.

</aside>

### **Projeto Base:** ‣

## Como criar um projeto React com TypeScript

### Passo a Passo para Criar o Projeto

1. **Instalar o Create React App**:
    - Certifique-se de ter o Node.js instalado em sua máquina.
    - Abra o terminal e execute o seguinte comando para criar um novo projeto React com suporte a TypeScript:
    
    ```bash
    npx create-react-app my-app --template typescript
    ```
    
2. **Navegar até o Diretório do Projeto**:
    - Entre na pasta do projeto recém-criado:
    
    ```bash
    cd my-app
    ```
    
3. **Estrutura Inicial do Projeto**:
Após a criação, a estrutura inicial do projeto será similar a esta:
    
    ```
    my-app
    ├── node_modules
    ├── public
    │   └── index.html
    ├── src
    │   ├── App.css
    │   ├── App.test.tsx
    │   ├── App.tsx
    │   ├── index.css
    │   ├── index.tsx
    │   ├── logo.svg
    │   ├── react-app-env.d.ts
    │   ├── reportWebVitals.ts
    │   └── setupTests.ts
    ├── package.json
    ├── tsconfig.json
    └── README.md
    ```
    

### Modificações para remover arquivos desnecessários do projeto

Para adaptar o projeto recém-criado para ficar igual ao projeto que você enviou, siga os passos abaixo:

1. **Remover Arquivos Desnecessários**:
    - Delete os seguintes arquivos do diretório `src`:
        - `App.css`
        - `App.test.tsx`
        - `logo.svg`
        - `reportWebVitals.ts`
        - `setupTests.ts`
    
    Após a remoção, a estrutura do `src` deve ficar assim:
    
    ```
    src
    ├── App.tsx
    ├── index.css
    ├── index.tsx
    └── react-app-env.d.ts
    ```
    
2. **Modificar o `App.tsx`**:
    - Atualize o conteúdo do `App.tsx`. O conteúdo deve ser:
    
    ```tsx
    import React from "react";
    //1 - Um componente SEMPRE deve começar com a primeira letra
    //maiúscula
    //2 - Todo componente DEVE ser uma função do JS
    //3 - Todo deve retornar apenas UM elemento HTML
    function App() {
      return (
        <div>
          <h1>Projeto base em React com TypeScript</h1>
        </div>
      );
    }
    //4 - OBRIGATORIAMENTE o componente DEVE ser exportado
    export default App;
    
    ```
    
3. **Atualizar o `index.tsx`**:
    - Certifique-se de que o arquivo `index.tsx` importe e renderize corretamente o componente `App`. O conteúdo deve ser:
    
    ```tsx
    import React from 'react';
    import ReactDOM from 'react-dom';
    import './index.css';
    import App from './App';
    
    ReactDOM.render(
      <React.StrictMode>
        <App />
      </React.StrictMode>,
      document.getElementById('root')
    );
    
    ```
    

### Estrutura Final do Projeto

Após realizar as modificações, a estrutura final do projeto deve ser semelhante a esta:

```
my-app
├── node_modules
├── public
│   └── index.html
├── src
│   ├── App.tsx
│   ├── index.css
│   ├── index.tsx
│   └── react-app-env.d.ts
├── package.json
├── tsconfig.json
└── README.md
```