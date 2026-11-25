export interface Grupo {
  id: number;
  nome: string;
}

export interface Tarefa {
  id: number;
  titulo: string;
  descricao?: string | null;
  status: number;
  prioridade: number;
  categoriaId: number | null;
  projetoId: number | null;
  criacao?: string;
  vencimento?: string | null;
}

export interface Resumo {
  projetoId: number;
  projetoNome: string;
  totalTarefas: number;
  pendentes: number;
  emAndamento: number;
  concluidas: number;
  altaPrioridade: number;
  percentualConclusao: number;
}

export type Visao = "categorias" | "projetos";
export type Cadastro = Visao | "tarefas";

export const colunas = [
  { id: 0, nome: "Pendente" },
  { id: 1, nome: "Em andamento" },
  { id: 2, nome: "Concluída" },
];

export async function consultar<T>(rota: string, opcoes?: RequestInit): Promise<T> {
  const resposta = await fetch("http://localhost:5000/api/" + rota, opcoes);
  if (!resposta.ok) {
    const detalhe: unknown = await resposta.json().catch(() => null);
    throw new Error(typeof detalhe === "string" ? detalhe : "Falha na API (" + resposta.status + ").");
  }
  return resposta.json();
}

export function enviar<T>(rota: string, metodo: string, dados: unknown) {
  return consultar<T>(rota, {
    method: metodo,
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(dados),
  });
}
