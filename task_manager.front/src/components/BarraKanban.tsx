import { Cadastro, Visao } from "./KanbanDados";

interface Props {
  visao: Visao;
  mudarVisao: (visao: Visao) => void;
  abrirCadastro: (tipo: Cadastro) => void;
  cadastroBloqueado: boolean;
}

function BarraKanban({ visao, mudarVisao, abrirCadastro, cadastroBloqueado }: Props) {
  return (
    <header className="kanban-barra">
      <div>
        <h1>Task Manager</h1>
        <p>Quadro de tarefas</p>
      </div>
      <div className="kanban-acoes">
        <button
          disabled = {cadastroBloqueado}
          className="kanban-primario"
          onClick = {() => abrirCadastro("tarefas")}
        >
          + Incluir tarefa
        </button>
        <button disabled = {cadastroBloqueado} onClick = {() => abrirCadastro("projetos")}>
          + Incluir projeto
        </button>
        <button disabled = {cadastroBloqueado} onClick = {() => abrirCadastro("categorias")}>
          + Categoria
        </button>
      </div>
      <div className="kanban-visoes" role="group" aria-label="Agrupar tarefas">
        <button aria-pressed = {visao === "categorias"} onClick = {() => mudarVisao("categorias")}>
          Categorias
        </button>
        <button aria-pressed = {visao === "projetos"} onClick = {() => mudarVisao("projetos")}>
          Projetos
        </button>
      </div>
      <style>{`
        .kanban-barra {
          display: flex;
          align-items: center;
          flex-wrap: wrap;
          gap: 1.2rem;
          padding-bottom: 1.5rem;
        }

        .kanban-barra h1 {
          margin: 0;
          font-size: clamp(1.75rem, 4vw, 2.6rem);
          letter-spacing: -.06em;
          line-height: 1.05;
        }

        .kanban-barra p {
          margin: .45rem 0 0;
          color: var(--kanban-muted);
        }

        .kanban-acoes, .kanban-visoes {
          display: flex;
          flex-wrap: wrap;
          gap: .5rem;
        }

        .kanban-acoes {
          margin-left: auto;
        }

        .kanban-visoes {
          flex-basis: 100%;
        }

        .kanban-visoes button[aria-pressed="true"] {
          background: #d2f3e4;
          border-color: #91c7ab;
        }

        @media (max-width: 780px) {
          .kanban-acoes {
            margin-left: 0;
          }
        }
      `}</style>
    </header>
  );
}

export default BarraKanban;
