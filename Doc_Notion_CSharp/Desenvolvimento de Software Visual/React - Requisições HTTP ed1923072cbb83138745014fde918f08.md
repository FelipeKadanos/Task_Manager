# React - Requisições HTTP

---

<aside>
💡 Este guia é projetado para ajudá-lo a entender como usar `fetch` em aplicações React com TypeScript para realizar requisições de API. Vamos abordar dois exemplos práticos: exibir uma lista de produtos em uma tabela e realizar o cadastro de novos produtos, utilizando componentes separados para cada funcionalidade.

</aside>

### Estrutura do Projeto

1. **Criar um Novo Projeto React com TypeScript**
    
    ```bash
    npx create-react-app tabela-produtos --template typescript
    cd tabela-produtos
    ```
    
2. **Estrutura de Pastas e Arquivos**
    
    ```
    src/
    ├── components/
    │   ├── CadastroProduto.tsx
    │   ├── ListaProdutos.tsx
    ├── interfaces/
    │   └── Produto.ts
    ├── App.tsx
    ├── App.css
    └── index.tsx
    ```
    

### Definir a Interface de Produto

### src/interfaces/Produto.ts

```tsx
export interface Produto {
    id: number;
    nome: string;
    descricao: string;
    preco: number;
    quantidade: number;
    criadoEm: Date;
}
```

### Componente de Listagem de Produtos

### src/components/ListaProdutos.tsx

```tsx
import React, { useEffect, useState } from 'react';
import { Produto } from '../interfaces/Produto';

const ListaProdutos: React.FC = () => {
    const [produtos, setProdutos] = useState<Produto[]>([]);

    useEffect(() => {
        fetch('<https://api.exemplo.com/produtos>') // Substitua pela URL da sua API
            .then(response => {
                if (!response.ok) {
                    throw new Error('Erro na requisição: ' + response.statusText);
                }
                return response.json();
            })
            .then(data => {
                setProdutos(data);
            })
            .catch(error => {
                console.error('Erro:', error);
            });
    }, []);

    return (
        <div>
            <h1>Lista de Produtos</h1>
            <table border="1">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Nome</th>
                        <th>Descrição</th>
                        <th>Preço</th>
                        <th>Quantidade</th>
                        <th>Criado Em</th>
                    </tr>
                </thead>
                <tbody>
                    {produtos.map(produto => (
                        <tr key={produto.id}>
                            <td>{produto.id}</td>
                            <td>{produto.nome}</td>
                            <td>{produto.descricao}</td>
                            <td>{produto.preco}</td>
                            <td>{produto.quantidade}</td>
                            <td>{new Date(produto.criadoEm).toLocaleDateString()}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
};

export default ListaProdutos;
```

### Componente de Cadastro de Produto

### src/components/CadastroProduto.tsx

```tsx
import React, { useState } from 'react';
import { Produto } from '../interfaces/Produto';

function CadastroProduto() {
    const [nome, setNome] = useState<string>('');
    const [descricao, setDescricao] = useState<string>('');
    const [preco, setPreco] = useState<number>(0);
    const [quantidade, setQuantidade] = useState<number>(0);

    function handleSubmit (e: any) {
        e.preventDefault();

        const novoProduto = {
            nome,
            descricao,
            preco,
            quantidade
        };

        fetch('https://api.exemplo.com/produtos', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(novoProduto)
        })
        .then(response => {
            if (!response.ok) {
                throw new Error('Erro na requisição: ' + response.statusText);
            }
            return response.json();
        })
        .then(data => {
            setNome('');
            setDescricao('');
            setPreco(0);
            setQuantidade(0);
        })
        .catch(error => {
            console.error('Erro:', error);
        });
    };

    return (
        <div>
            <h2>Cadastrar Novo Produto</h2>
            <form onSubmit={handleSubmit}>
                <label>
                    Nome:
                    <input type="text" value={nome} onChange={e => setNome(e.target.value)} required />
                </label>
                <label>
                    Descrição:
                    <input type="text" value={descricao} onChange={e => setDescricao(e.target.value)} required />
                </label>
                <label>
                    Preço:
                    <input type="number" value={preco} onChange={e => setPreco(Number(e.target.value))} required />
                </label>
                <label>
                    Quantidade:
                    <input type="number" value={quantidade} onChange={e => setQuantidade(Number(e.target.value))} required />
                </label>
                <button type="submit">Cadastrar</button>
            </form>
        </div>
    );
};

export default CadastroProduto;
```

### Estilização (Opcional)

### src/App.css

```css
.App {
    text-align: center;
    padding: 20px;
}

table {
    margin: 0 auto;
    border-collapse: collapse;
    width: 80%;
}

th, td {
    padding: 10px;
    border: 1px solid #ddd;
}

th {
    background-color: #f4f4f4;
}

```

### Referências

- [Documentação do Fetch API](https://developer.mozilla.org/pt-BR/docs/Web/API/Fetch_API)
- [Documentação do React](https://pt-br.reactjs.org/docs/getting-started.html)
- [Documentação do TypeScript](https://www.typescriptlang.org/docs/)

### Sugestões Adicionais

- **Validação de Formulário**: Considere adicionar validações de formulário para garantir que os dados enviados sejam válidos.
- **Tratamento de Erros**: Melhore o tratamento de erros para fornecer feedback ao usuário em caso de falha na requisição.
- **Estilização**: Adicione mais estilos para tornar sua aplicação visualmente mais atraente.