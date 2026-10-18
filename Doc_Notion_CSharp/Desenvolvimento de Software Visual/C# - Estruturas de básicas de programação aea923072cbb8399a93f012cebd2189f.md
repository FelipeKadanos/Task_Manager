# C# - Estruturas de básicas de programação

<aside>
💡 Essas são apenas algumas estruturas básicas em C#. Você pode expandir seu conhecimento explorando mais sobre classes, objetos, métodos, exceções e outros conceitos importantes da linguagem.

</aside>

## Comentários

```csharp
// Este é um comentário de linha única

/*
   Este é um comentário
   de múltiplas linhas
*/
```

---

## Variáveis e Tipos de Dados

Tipagem em linguagens de programação refere-se à definição de tipos de dados para variáveis, parâmetros de função, e assim por diante. Em C#, existem dois tipos principais de tipagem:

1. **Tipagem Estática**: O tipo de uma variável é verificado em tempo de compilação e não pode ser alterado após a compilação.
2. **Tipagem Dinâmica**: O tipo de uma variável é verificado em tempo de execução e pode ser alterado durante a execução do programa.

## Tipos Primitivos

### Tipos Numéricos Integrais

- **sbyte**: 8 bits com sinal (-128 a 127)
- **byte**: 8 bits sem sinal (0 a 255)
- **short**: 16 bits com sinal (-32,768 a 32,767)
- **ushort**: 16 bits sem sinal (0 a 65,535)
- **int**: 32 bits com sinal (-2,147,483,648 a 2,147,483,647)
- **uint**: 32 bits sem sinal (0 a 4,294,967,295)
- **long**: 64 bits com sinal (-9,223,372,036,854,775,808 a 9,223,372,036,854,775,807)
- **ulong**: 64 bits sem sinal (0 a 18,446,744,073,709,551,615)

### Tipos Numéricos de Ponto Flutuante

- **float**: 32 bits (aproximadamente de -3.4 × 10^38 a 3.4 × 10^38, precisão de 7 dígitos)
- **double**: 64 bits (aproximadamente de ±5.0 × 10^−324 a ±1.7 × 10^308, precisão de 15-16 dígitos)
- **decimal**: 128 bits (aproximadamente de ±1.0 x 10^-28 a ±7.9 x 10^28, precisão de 28-29 dígitos)

### Tipos Booleanos

- **bool**: Representa valores verdadeiro ou falso (true ou false).

### Tipos de Caracteres

- **char**: Representa um único caractere Unicode.

### Tipo String

- **string**: Representa uma sequência de caracteres Unicode.

## Tipos Compostos

### Array

- **Array**: Coleção de elementos do mesmo tipo.

```csharp
int[] numeros = new int[5];

```

### Tuple

- **Tuple**: Agrupa um conjunto de elementos de diferentes tipos.

```csharp
var pessoa = (nome: "Maria", idade: 30, altura: 1.65);

```

## Tipo Enumerado (Enum)

- **Enum**: Define um tipo de valor nomeado.

```csharp
enum DiaSemana { Domingo, Segunda, Terca, Quarta, Quinta, Sexta, Sabado }
DiaSemana dia = DiaSemana.Segunda;

```

---

## Estruturas Condicionais

```csharp
if (idade >= 18)
{
    Console.WriteLine("Você é maior de idade.");
}
else
{
    Console.WriteLine("Você é menor de idade.");
}
```

---

## Estruturas de Repetição

### a) While

```csharp
int contador = 0;
while (contador < 5)
{
    Console.WriteLine(contador);
    contador++;
}
```

### b) For

```csharp
for (int i = 0; i < 5; i++)
{
    Console.WriteLine(i);
}
```

### c) Foreach

```csharp
string[] cores = { "vermelho", "azul", "verde" };
foreach (string cor in cores)
{
    Console.WriteLine(cor);
}
```

---

## Funções

```csharp
int Somar(int a, int b)
{
    return a + b;
}
```