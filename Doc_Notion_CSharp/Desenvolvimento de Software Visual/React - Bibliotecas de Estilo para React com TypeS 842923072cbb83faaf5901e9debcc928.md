# React - Bibliotecas de Estilo para React com TypeScript

---

<aside>
💡 Você pode escolher qualquer uma dessas bibliotecas de estilo com base nas suas necessidades e preferências de design. Cada uma delas oferece um conjunto robusto de componentes e estilos para acelerar o desenvolvimento da sua aplicação. Escolha a que melhor se adapta ao seu projeto e comece a estilizar seus componentes de maneira eficiente e eficaz.

</aside>

### 1. Styled Components

**Styled Components** permite que você escreva estilos CSS em JavaScript e os associe diretamente aos seus componentes React.

**Documentação:** [Styled Components](https://styled-components.com/)

**Instalação:**

```bash
npm install styled-components @types/styled-components
```

**Uso:**

```tsx
import styled from 'styled-components';

const Button = styled.button`
  background: palevioletred;
  color: white;
  font-size: 1em;
  margin: 1em;
  padding: 0.25em 1em;
  border: 2px solid palevioletred;
  border-radius: 3px;
`;

const App: React.FC = () => {
    return (
        <div className="App">
            <Button>Styled Button</Button>
        </div>
    );
};

export default App;
```

### 2. Material-UI (MUI)

**Material-UI (MUI)** é uma biblioteca de componentes React que implementa o design system do Material Design do Google.

**Documentação:** [Material-UI](https://mui.com/)

**Instalação:**

```bash
npm install @mui/material @emotion/react @emotion/styled
```

**Uso:**

```tsx
import React from 'react';
import Button from '@mui/material/Button';

const App: React.FC = () => {
    return (
        <div className="App">
            <Button variant="contained" color="primary">
                MUI Button
            </Button>
        </div>
    );
};

export default App;
```

### 3. Bootstrap

**Bootstrap** é uma das bibliotecas de CSS mais populares, oferecendo uma vasta gama de componentes prontos para uso.

**Documentação:** [Bootstrap](https://getbootstrap.com/)

**Instalação:**

```bash
npm install bootstrap
```

**Uso:**
Adicione o CSS do Bootstrap no arquivo `index.tsx`:

```tsx
import 'bootstrap/dist/css/bootstrap.min.css';
```

```tsx
import React from 'react';
import 'bootstrap/dist/css/bootstrap.min.css';
import { Button } from 'react-bootstrap';

const App: React.FC = () => {
    return (
        <div className="App">
            <Button variant="primary">Bootstrap Button</Button>
        </div>
    );
};

export default App;
```

### 4. Tailwind CSS

**Tailwind CSS** é um framework de utilitários CSS altamente configurável que facilita a estilização de seus componentes.

**Documentação:** [Tailwind CSS](https://tailwindcss.com/)

**Instalação:**

```bash
npm install -D tailwindcss
npx tailwindcss init
```

**Configuração e Uso:**
Configure o arquivo `tailwind.config.js` e adicione as diretivas do Tailwind ao seu CSS:

```css
/* ./src/index.css */
@tailwind base;
@tailwind components;
@tailwind utilities;
```

```tsx
import React from 'react';
import './index.css';

const App: React.FC = () => {
    return (
        <div className="App">
            <button className="bg-blue-500 text-white font-bold py-2 px-4 rounded">
                Tailwind Button
            </button>
        </div>
    );
};

export default App;
```

### 5. Ant Design

**Ant Design** é uma biblioteca de componentes React com um estilo limpo e moderno, muito popular em aplicações empresariais.

**Documentação:** [Ant Design](https://ant.design/)

**Instalação:**

```bash
npm install antd
```

**Uso:**
Adicione o CSS do Ant Design no arquivo `index.tsx`:

```tsx
import 'antd/dist/antd.css';
```

```tsx
import React from 'react';
import { Button } from 'antd';

const App: React.FC = () => {
    return (
        <div className="App">
            <Button type="primary">Ant Design Button</Button>
        </div>
    );
};

export default App;
```