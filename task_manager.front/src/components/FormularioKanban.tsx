import { FormEvent, useEffect, useRef, useState } from "react";
import { Cadastro, Grupo, enviar } from "./KanbanDados";

interface Props {
  tipo: Cadastro;
  categorias: Grupo[];
  projetos: Grupo[];
  fechar: () => void;
  aoSalvar: () => void;
}

const titulos = {
  tarefas: "Incluir tarefa",
  projetos: "Incluir projeto",
  categorias: "Incluir categoria",
};

function FormularioKanban({ tipo, categorias, projetos, fechar, aoSalvar }: Props) {
  const dialogo = useRef<HTMLDialogElement>(null);
  const [salvando, setSalvando] = useState(false);
  const [erro, setErro] = useState("");
  const tarefa = tipo === "tarefas";

  useEffect(() => {
    dialogo.current?.showModal();
  }, []);

  const salvar = async (evento: FormEvent<HTMLFormElement>) => {
    evento.preventDefault();
    if (salvando) return;

    const campos = new FormData(evento.currentTarget);
    const nome = String(campos.get("nome")).trim();
    if (!nome) {
      setErro("Preencha " + (tarefa ? "o título." : "o nome."));
      return;
    }

    const dados = tarefa ? {
      titulo: nome,
      descricao: String(campos.get("descricao")).trim() || null,
      prioridade: Number(campos.get("prioridade")),
      status: 0,
      categoriaId: Number(campos.get("categoria")) || null,
      projetoId: Number(campos.get("projeto")) || null,
      vencimento: campos.get("vencimento") || null,
    } : { nome };

    setErro("");
    setSalvando(true);
    try {
      await enviar(tipo, "POST", dados);
      aoSalvar();
    } catch (falha) {
      setErro(falha instanceof Error ? falha.message : "Não foi possível salvar.");
      setSalvando(false);
    }
  };

  return (
    <dialog
      ref = {dialogo}
      className="kanban-dialogo"
      aria-labelledby="kanban-cadastro-titulo"
      onCancel = {evento => {
        evento.preventDefault();
        if (!salvando) fechar();
      }}
    >
      <form onSubmit = {salvar}>
        <h2 id="kanban-cadastro-titulo">{titulos[tipo]}</h2>
        <label>
          {tarefa ? "Título" : "Nome"}
          <input name="nome" maxLength = {150} required autoFocus />
        </label>

        {tarefa && (
          <>
            <label>
              Descrição
              <textarea name="descricao" rows = {3} maxLength = {2000} />
            </label>

            <div className="kanban-campos">
              <label>
                Categoria
                <select name="categoria">
                  <option value="">Sem categoria</option>
                  {categorias.map(categoria => (
                    <option key = {categoria.id} value = {categoria.id}>{categoria.nome}</option>
                  ))}
                </select>
              </label>
              <label>
                Projeto
                <select name="projeto">
                  <option value="">Sem projeto</option>
                  {projetos.map(projeto => (
                    <option key = {projeto.id} value = {projeto.id}>{projeto.nome}</option>
                  ))}
                </select>
              </label>
              <label>
                Prioridade
                <select name="prioridade" defaultValue="1">
                  <option value="0">Baixa</option>
                  <option value="1">Média</option>
                  <option value="2">Alta</option>
                </select>
              </label>
              <label>
                Vencimento
                <input type="date" name="vencimento" />
              </label>
            </div>
          </>
        )}
        {erro && <p role="alert">{erro}</p>}
        <div className="kanban-dialogo-acoes">
          <button type="button" onClick = {fechar} disabled = {salvando}>Cancelar</button>
          <button className="kanban-primario" type="submit" disabled = {salvando}>{salvando ? "Salvando..." : "Salvar"}</button>
        </div>
      </form>
      <style>{`
        .kanban-dialogo {
          width: min(32rem, calc(100% - 2rem));
          box-sizing: border-box;
          max-height: calc(100% - 2rem);
          padding: 1.4rem;
          color: inherit;
          background: #f2fff5e8;
          backdrop-filter: blur(24px);
          border: 1px solid white;
          border-radius: 1.4rem;
          box-shadow: 0 20px 70px #102d3940;
        }

        .kanban-dialogo::backdrop {
          background: #153c4663;
          backdrop-filter: blur(4px);
        }

        .kanban-dialogo form, .kanban-dialogo label {
          display: grid;
          gap: .5rem;
        }

        .kanban-dialogo form {
          gap: .9rem;
        }

        .kanban-dialogo h2 {
          margin: 0;
        }

        .kanban-dialogo input, .kanban-dialogo select, .kanban-dialogo textarea {
          width: 100%;
          min-width: 0;
          box-sizing: border-box;
          padding: .6rem;
          border: 1px solid #adc9c1;
          border-radius: .6rem;
          background: #ffffffbd;
          color: inherit;
          font: inherit;
        }

        .kanban-campos {
          display: grid;
          grid-template-columns: repeat(auto-fit, minmax(min(100%, 11rem), 1fr));
          gap: .7rem;
        }

        .kanban-dialogo-acoes {
          display: flex;
          justify-content: flex-end;
          gap: .5rem;
        }
      `}</style>
    </dialog>
  );
}

export default FormularioKanban;
