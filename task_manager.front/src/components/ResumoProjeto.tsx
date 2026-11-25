import { useEffect, useRef, useState } from "react";
import { Resumo, consultar } from "./KanbanDados";

interface Props {
  projetoId: number;
  fechar: () => void;
}

function ResumoProjeto({ projetoId, fechar }: Props) {
  const dialogo = useRef<HTMLDialogElement>(null);
  const [resumo, setResumo] = useState<Resumo | null>(null);
  const [erro, setErro] = useState("");

  useEffect(() => {
    if (!dialogo.current?.open) dialogo.current?.showModal();
  }, []);

  useEffect(() => {
    const controle = new AbortController();

    consultar<Resumo>("projetos/" + projetoId + "/resumo", { signal: controle.signal })
      .then(dados => setResumo(dados))
      .catch(() => {
        if (!controle.signal.aborted) setErro("Não foi possível carregar o resumo do projeto.");
      });

    return () => controle.abort();
  }, [projetoId]);

  const indicadores = resumo ? [
    ["Total de tarefas", resumo.totalTarefas],
    ["Pendentes", resumo.pendentes],
    ["Em andamento", resumo.emAndamento],
    ["Concluídas", resumo.concluidas],
    ["Alta prioridade", resumo.altaPrioridade],
  ] : [];

  return (
    <dialog
      ref = {dialogo}
      className="kanban-resumo"
      aria-labelledby="kanban-resumo-titulo"
      onCancel = {evento => {
        evento.preventDefault();
        fechar();
      }}
    >
      <h2 id="kanban-resumo-titulo">{resumo ? "Resumo: " + resumo.projetoNome : "Resumo do projeto"}</h2>
      {!resumo && !erro && <p role="status">Carregando resumo...</p>}
      {erro && <p role="alert">{erro}</p>}

      {resumo && (
        <>
          <dl className="kanban-resumo-lista">
            {indicadores.map(([nome, valor]) => (
              <div key = {nome}>
                <dt>{nome}</dt>
                <dd>{valor}</dd>
              </div>
            ))}
          </dl>
          <label className="kanban-resumo-progresso">
            Conclusão: {resumo.percentualConclusao}%
            <progress max = {100} value = {resumo.percentualConclusao} />
          </label>
        </>
      )}

      <div className="kanban-resumo-acoes">
        <button className="kanban-primario" onClick = {fechar}>Fechar</button>
      </div>
      <style>{`
        .kanban-resumo {
          width: min(28rem, calc(100% - 2rem));
          box-sizing: border-box;
          padding: 1.4rem;
          color: #173833;
          font-family: system-ui, sans-serif;
          background: #f2fff5e8;
          backdrop-filter: blur(24px);
          border: 1px solid white;
          border-radius: 1.4rem;
          box-shadow: 0 20px 70px #102d3940;
        }

        .kanban-resumo::backdrop {
          background: #153c4663;
          backdrop-filter: blur(4px);
        }

        .kanban-resumo h2 {
          margin: 0 0 1rem;
          overflow-wrap: anywhere;
        }

        .kanban-resumo-lista {
          display: grid;
          grid-template-columns: repeat(auto-fit, minmax(8rem, 1fr));
          gap: .6rem;
          margin: 0 0 1rem;
        }

        .kanban-resumo-lista div {
          padding: .6rem;
          background: #ffffffa8;
          border-radius: .8rem;
        }

        .kanban-resumo-lista dt {
          font-size: .8rem;
          color: #4a655e;
        }

        .kanban-resumo-lista dd {
          margin: .2rem 0 0;
          font-size: 1.4rem;
          font-weight: 700;
        }

        .kanban-resumo-progresso {
          display: grid;
          gap: .4rem;
        }

        .kanban-resumo-progresso progress {
          width: 100%;
        }

        .kanban-resumo-acoes {
          display: flex;
          justify-content: flex-end;
          margin-top: 1rem;
        }
      `}</style>
    </dialog>
  );
}

export default ResumoProjeto;
