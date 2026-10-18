# React - Navegação entre componentes

---

<aside>
💡 Com essa configuração, você tem um exemplo de navegação entre os componentes  utilizando `BrowserRouter` do React Router v6.

</aside>

### Passos para Adicionar Navegação

1. **Instalação do React Router**
    
    Certifique-se de instalar a versão correta do `react-router-dom`:
    
    ```bash
    npm install react-router-dom
    ```
    
    ```bash
    npm install @types/react-router-dom
    ```
    
2. **Configuração do Router em `App.tsx`**
    
    No arquivo `App.tsx`, configure o `BrowserRouter` e as rotas.
    
    **Nota:** Certifique-se de que os componentes `ListaProdutos` e `CadastroProduto` já estejam criados.
    
    ```tsx
    import React from 'react';
    import { BrowserRouter, Route, Routes, Link } from 'react-router-dom';
    import ListaProdutos from './components/ListaProdutos';
    import CadastroProduto from './components/CadastroProduto';
    import './App.css';
    
    const App: React.FC = () => {
        return (
            <BrowserRouter>
                <div className="App">
                    <nav>
                        <ul>
                            <li>
                                <Link to="/">Lista de Produtos</Link>
                            </li>
                            <li>
                                <Link to="/cadastro">Cadastro de Produto</Link>
                            </li>
                        </ul>
                    </nav>
                    <Routes>
                        <Route path="/" element={<ListaProdutos />} />
                        <Route path="/cadastro" element={<CadastroProduto />} />
                    </Routes>
                </div>
            </BrowserRouter>
        );
    };
    
    export default App;
    ```
    
3. **Estilização (opcional)**
    
    Você pode adicionar alguma estilização básica em `App.css` para melhorar a apresentação da sua aplicação:
    
    ```css
    .App {
        text-align: center;
        padding: 20px;
    }
    
    nav ul {
        list-style: none;
        padding: 0;
    }
    
    nav ul li {
        display: inline;
        margin-right: 10px;
    }
    
    nav ul li a {
        text-decoration: none;
        color: blue;
    }
    
    table {
        margin: 20px auto;
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
    

### Conclusão

Com essa configuração, você tem um exemplo de navegação entre os componentes `ListaProdutos` e `CadastroProduto` utilizando `BrowserRouter` do React Router v6.23.1. Este exemplo pode ser adicionado diretamente ao Notion para documentação ou uso futuro.