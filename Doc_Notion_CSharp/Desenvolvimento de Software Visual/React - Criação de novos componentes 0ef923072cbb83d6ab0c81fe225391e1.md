# React - Criação de novos componentes

---

<aside>
💡 Este guia ensina a criar componentes funcionais em React, destacando a importância de começar com letra maiúscula, retornar um único elemento HTML e exportar o componente. Inclui exemplos práticos de utilização de `props` e `state` com Hooks. Seguir essas práticas mantém o código organizado e eficiente. Recursos adicionais são fornecidos para mais aprendizado.

</aside>

### Componentes Funcionais

Componentes funcionais são funções JavaScript que retornam elementos React. Eles são a forma preferida de criar componentes, especialmente com a introdução dos Hooks no React 16.8, que permitem usar estado e outros recursos sem escrever classes.

### Estrutura de um Componente Funcional

```tsx
import React from "react";

function App() {
  return (
    <div>
      <h1>Projeto base em React com TypeScript</h1>
    </div>
  );
}

export default App;
```

### Comentários Importantes

1. **Nomeação**: Um componente React deve começar com uma letra maiúscula. Isso ajuda o React a distinguir entre componentes e elementos HTML nativos.
2. **Forma do Componente**: Componentes podem ser funções ou classes. Atualmente, funções são recomendadas devido à simplicidade e ao suporte a Hooks.
3. **Retorno Único**: Um componente deve retornar um único elemento HTML. Se múltiplos elementos são necessários, use um contêiner como `<div>`, `<React.Fragment>`, ou `<>`.
4. **Exportação**: Para utilizar um componente em outras partes da aplicação, ele deve ser exportado.

### Utilizando Props e State

Componentes podem receber dados através de `props` e gerenciar seu próprio estado usando `state`.

### Exemplo com Props

```tsx
import React from "react";

function Greeting({ name }: { name: string }) {
  return <h1>Olá, {name}!</h1>;
}

export default Greeting;
```

### Exemplo com State (Usando Hooks)

```tsx
import React, { useState } from "react";

function Counter() {
  const [count, setCount] = useState(0);

  return (
    <div>
      <p>Você clicou {count} vezes</p>
      <button onClick={() => setCount(count + 1)}>Clique aqui</button>
    </div>
  );
}

export default Counter;
```

### Conclusão

Criar componentes em React é fundamental para construir interfaces de usuário modernas e eficientes. Usando componentes funcionais e Hooks, você pode aproveitar ao máximo as funcionalidades do React de maneira simples e clara. Siga as melhores práticas discutidas aqui para manter seu código organizado e fácil de manter.

### Recursos Adicionais

- [Documentação oficial do React](https://reactjs.org/docs/getting-started.html)
- [React Hooks](https://reactjs.org/docs/hooks-intro.html)
- [Componentes e Props](https://reactjs.org/docs/components-and-props.html)