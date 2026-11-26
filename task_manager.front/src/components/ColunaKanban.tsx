import { Fragment } from "react";
import CartaoTarefa from "./CartaoTarefa";
import { Grupo, Tarefa, Visao } from "./KanbanDados";

interface Props {
  nome: string;
  status: number;
  tarefas: Tarefa[];
  categorias: Grupo[];
  projetos: Grupo[];
  visao: Visao;
  carregando: boolean;
  dadosCarregados: boolean;
  ocupadas: number[];
  moverTarefa: (id: number, status: number) => void;
  editarTarefa: (tarefa: Tarefa) => void;
  excluirTarefa: (id: number) => void;
  abrirResumo: (projetoId: number) => void;
}

function ColunaKanban({
  nome,
  status,
  tarefas,
  categorias,
  projetos,
  visao,
  carregando,
  dadosCarregados,
  ocupadas,
  moverTarefa,
  editarTarefa,
  excluirTarefa,
  abrirResumo,
}: Props) {
  const porProjeto = visao === "projetos";
  const grupos = porProjeto ? [...projetos, { id: null, nome: "Sem projeto" }] : [{ id: null, nome: "" }];
  const gruposCategorias = [...categorias, { id: null, nome: "Sem categoria" }];
  const temGrupos = tarefas.length > 0 || (porProjeto ? projetos.length > 0 : categorias.length > 0);

  return (
    <section
      className="kanban-coluna"
      onDragOver = {evento => {
        if (evento.dataTransfer.types.includes("application/x-task-manager")) evento.preventDefault();
      }}
      onDrop = {evento => {
        const id = Number(evento.dataTransfer.getData("application/x-task-manager"));
        if (!Number.isInteger(id) || id <= 0) return;
        evento.preventDefault();
        moverTarefa(id, status);
      }}
    >
      <header>
        <span className="kanban-indicador" data-status = {status} aria-hidden="true" />
        <h2>{nome}</h2>
        <span className="kanban-contagem" aria-label = {tarefas.length + " tarefas"}>{tarefas.length}</span>
      </header>
      <div className="kanban-coluna-conteudo">
        {grupos.map(grupo => {
          const tarefasDoGrupo = porProjeto ? tarefas.filter(tarefa => tarefa.projetoId === grupo.id) : tarefas;
          const projetoId = grupo.id;
          if (porProjeto && grupo.id === null && !tarefasDoGrupo.length) return null;

          const conteudo = gruposCategorias.map(categoria => {
            const itens = tarefasDoGrupo.filter(tarefa => tarefa.categoriaId === categoria.id);
            if (!itens.length && (porProjeto || categoria.id === null)) return null;

            return (
              <section className="kanban-grupo" key = {categoria.id ?? "sem-categoria"}>
                <h3>{categoria.nome}</h3>
                <div className="kanban-grupo-conteudo">
                  {itens.length ? itens.map(tarefa => (
                    <CartaoTarefa
                      key = {tarefa.id}
                      tarefa = {tarefa}
                      ocupada = {carregando || ocupadas.includes(tarefa.id)}
                      mudarStatus = {moverTarefa}
                      editar = {editarTarefa}
                      excluir = {excluirTarefa}
                    />
                  )) : <p>Sem tarefas neste status.</p>}
                </div>
              </section>
            );
          });

          if (!porProjeto) return <Fragment key="categorias">{conteudo}</Fragment>;

          return (
            <section className="kanban-grupo kanban-projeto" key = {grupo.id ?? "sem-projeto"}>
              <div className="kanban-projeto-titulo">
                <h3>{grupo.nome}</h3>
                {projetoId !== null && status === 0 && (
                  <button onClick = {() => abrirResumo(projetoId)}>Resumo</button>
                )}
              </div>
              <div className="kanban-grupo-conteudo">
                {tarefasDoGrupo.length ? conteudo : <p>Sem tarefas neste status.</p>}
              </div>
            </section>
          );
        })}
        {!temGrupos && !carregando && dadosCarregados && <p>Nenhuma tarefa neste status.</p>}
      </div>
      <style>{`
        .kanban-coluna {
          min-width: 0;
          min-height: 25rem;
          padding: 1rem;
          background: #ffffff48;
          border: 1px solid #ffffffd9;
          border-radius: 1.4rem;
          backdrop-filter: blur(16px);
          box-shadow: 0 10px 28px #244d400c;
        }

        .kanban-coluna > header {
          display: flex;
          align-items: center;
          gap: .55rem;
          margin-bottom: 1rem;
        }

        .kanban-coluna h2 {
          margin: 0;
          font-size: 1.1rem;
        }

        .kanban-indicador {
          width: .65rem;
          height: .65rem;
          border-radius: 50%;
          background: #b68a36;
        }

        .kanban-indicador[data-status="1"] {
          background: #387e62;
        }

        .kanban-indicador[data-status="2"] {
          background: #3b6ea5;
        }

        .kanban-contagem {
          margin-left: auto;
          padding: .2rem .5rem;
          background: #ffffff9c;
          border-radius: 1rem;
          font-size: .85rem;
        }

        .kanban-coluna-conteudo {
          display: grid;
          gap: .7rem;
        }

        .kanban-coluna-conteudo > p {
          color: var(--kanban-muted);
          margin: .25rem 0;
        }

        .kanban-grupo {
          min-width: 0;
          padding: .75rem;
          border: 1px solid #ffffffb8;
          border-radius: 1.1rem;
          background: #ffffff52;
          overflow-wrap: anywhere;
        }

        .kanban-projeto {
          background: #bbecd49e;
        }

        .kanban-projeto-titulo {
          display: flex;
          align-items: center;
          justify-content: space-between;
          gap: .5rem;
          margin: 0 0 .55rem;
        }

        .kanban-projeto-titulo h3 {
          margin: 0;
          font-size: .95rem;
        }

        .kanban-projeto-titulo button {
          font-size: .78rem;
          padding: .3rem .55rem;
        }

        .kanban-grupo > h3 {
          font-size: .95rem;
          margin: 0 0 .55rem;
        }

        .kanban-grupo-conteudo {
          display: grid;
          gap: .65rem;
        }

        .kanban-projeto > .kanban-grupo-conteudo {
          grid-template-columns: repeat(auto-fit, minmax(min(100%, 13rem), 1fr));
          align-items: start;
        }

        .kanban-grupo-conteudo > p {
          margin: 0;
          color: var(--kanban-muted);
          font-size: .85rem;
        }
      `}</style>
    </section>
  );
}

export default ColunaKanban;
