import { Tarefa } from "./KanbanDados";

interface Props {
  tarefa: Tarefa;
  ocupada: boolean;
  mudarStatus: (id: number, status: number) => void;
}

const prioridades = ["Baixa", "Média", "Alta"];

function CartaoTarefa({ tarefa, ocupada, mudarStatus }: Props) {
  return (
    <article
      className="kanban-cartao"
      draggable = {!ocupada}
      onDragStart = {evento => {
        evento.dataTransfer.setData("application/x-task-manager", String(tarefa.id));
        evento.dataTransfer.effectAllowed = "move";
      }}
    >
      <span className="kanban-prioridade" data-prioridade = {tarefa.prioridade}>
        Prioridade {prioridades[tarefa.prioridade]?.toLowerCase()}
      </span>
      <h4>{tarefa.titulo}</h4>
      {tarefa.descricao && <p>{tarefa.descricao}</p>}
      <div className="kanban-cartao-acoes">
        <button disabled = {ocupada} onClick = {() => mudarStatus(tarefa.id, tarefa.status === 0 ? 1 : 0)}>
          {tarefa.status === 0 ? "Iniciar" : "Voltar para pendente"}
        </button>
        <button disabled = {ocupada} onClick = {() => mudarStatus(tarefa.id, 2)}>Concluir</button>
      </div>
      <style>{`
        .kanban-cartao {
          padding: .9rem;
          border: 1px solid #fff;
          border-radius: 1rem;
          background: #ffffffa8;
          backdrop-filter: blur(12px);
          box-shadow: 0 5px 15px #244d4010;
          overflow-wrap: anywhere;
        }

        .kanban-cartao[draggable="true"] {
          cursor: grab;
        }

        .kanban-cartao h4 {
          margin: .45rem 0;
        }

        .kanban-cartao p {
          margin: 0;
          color: var(--kanban-muted);
          white-space: pre-wrap;
        }

        .kanban-prioridade {
          font-size: .75rem;
          background: #e3ebe8;
          padding: .2rem .45rem;
          border-radius: 1rem;
        }

        .kanban-prioridade[data-prioridade="2"] {
          background: #f7dfd6;
        }

        .kanban-cartao-acoes {
          display: flex;
          flex-wrap: wrap;
          gap: .35rem;
          margin-top: .7rem;
        }

        .kanban-cartao-acoes button {
          font-size: .78rem;
          padding: .35rem .55rem;
        }
      `}</style>
    </article>
  );
}

export default CartaoTarefa;
