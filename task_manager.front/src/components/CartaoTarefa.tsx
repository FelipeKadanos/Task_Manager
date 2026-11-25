import { Tarefa } from "./KanbanDados";

interface Props {
  tarefa: Tarefa;
  ocupada: boolean;
  mudarStatus: (id: number, status: number) => void;
  editar: (tarefa: Tarefa) => void;
  excluir: (id: number) => void;
}

const prioridades = ["Baixa", "Média", "Alta"];

function formatarData(data: string) {
  const [ano, mes, dia] = data.slice(0, 10).split("-");
  return dia + "/" + mes + "/" + ano;
}

function CartaoTarefa({ tarefa, ocupada, mudarStatus, editar, excluir }: Props) {
  return (
    <article
      className="kanban-cartao"
      data-status = {tarefa.status}
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
      {tarefa.vencimento && <small className="kanban-vencimento">Vence em {formatarData(tarefa.vencimento)}</small>}
      <div className="kanban-cartao-acoes">
        {tarefa.status === 2 ? (
          <button disabled = {ocupada} onClick = {() => mudarStatus(tarefa.id, 0)}>Reabrir</button>
        ) : (
          <>
            <button disabled = {ocupada} onClick = {() => mudarStatus(tarefa.id, tarefa.status === 0 ? 1 : 0)}>
              {tarefa.status === 0 ? "Iniciar" : "Voltar para pendente"}
            </button>
            <button disabled = {ocupada} onClick = {() => mudarStatus(tarefa.id, 2)}>Concluir</button>
          </>
        )}
        <button disabled = {ocupada} onClick = {() => editar(tarefa)}>Editar</button>
        <button className="kanban-perigo" disabled = {ocupada} onClick = {() => excluir(tarefa.id)}>Excluir</button>
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

        .kanban-cartao[data-status="2"] h4 {
          text-decoration: line-through;
          opacity: .7;
        }

        .kanban-cartao h4 {
          margin: .45rem 0;
        }

        .kanban-cartao p {
          margin: 0;
          color: var(--kanban-muted);
          white-space: pre-wrap;
        }

        .kanban-vencimento {
          display: block;
          margin-top: .5rem;
          color: var(--kanban-muted);
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

        .kanban-cartao-acoes button.kanban-perigo {
          color: #742f2f;
        }
      `}</style>
    </article>
  );
}

export default CartaoTarefa;
