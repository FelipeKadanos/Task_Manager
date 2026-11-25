import { useEffect, useState } from "react";
import BarraKanban from "./BarraKanban";
import ColunaKanban from "./ColunaKanban";
import FormularioKanban from "./FormularioKanban";
import ResumoProjeto from "./ResumoProjeto";
import { Cadastro, Grupo, Tarefa, Visao, colunas, consultar, enviar } from "./KanbanDados";

interface Dados {
  tarefas: Tarefa[];
  projetos: Grupo[];
  categorias: Grupo[];
}

const dadosIniciais: Dados = { tarefas: [], projetos: [], categorias: [] };

function QuadroKanban() {
  const [dados, setDados] = useState(dadosIniciais);
  const [rota, setRota] = useState(window.location.pathname);
  const [erro, setErro] = useState("");
  const [carregando, setCarregando] = useState(true);
  const [ocupadas, setOcupadas] = useState<number[]>([]);
  const [edicao, setEdicao] = useState<Tarefa | null>(null);
  const [resumoId, setResumoId] = useState<number | null>(null);
  const bloqueado = carregando || ocupadas.length > 0;
  const [, grupo, acao, tipo] = rota.split("/");
  const visao: Visao = grupo === "projetos" ? "projetos" : "categorias";
  const cadastro = acao === "incluir" && (tipo === "tarefas" || tipo === "projetos" || tipo === "categorias") ? tipo : null;

  useEffect(() => {
    const atualizarRota = () => setRota(window.location.pathname);
    window.addEventListener("popstate", atualizarRota);
    return () => window.removeEventListener("popstate", atualizarRota);
  }, []);

  const navegar = (destino: string, substituir = false) => {
    if (destino === window.location.pathname) return;

    if (substituir) window.history.replaceState(null, "", destino);
    else window.history.pushState(null, "", destino);
    setRota(destino);
  };

  useEffect(() => {
    if (!carregando) return;

    const controle = new AbortController();
    const { signal } = controle;
    setErro("");

    Promise.all([
      consultar<Tarefa[]>("tarefas", { signal }),
      consultar<Grupo[]>("projetos", { signal }),
      consultar<Grupo[]>("categorias", { signal }),
    ])
      .then(([tarefas, projetos, categorias]) => {
        if (!signal.aborted) setDados({ tarefas, projetos, categorias });
      })
      .catch(() => {
        if (!signal.aborted) setErro("Não foi possível carregar o quadro. Verifique se a API está rodando na porta 5000.");
      })
      .finally(() => {
        if (!signal.aborted) setCarregando(false);
      });

    return () => controle.abort();
  }, [carregando]);

  const mudarStatus = async (id: number, status: number) => {
    const tarefa = dados.tarefas.find(item => item.id === id);
    if (carregando || !tarefa || tarefa.status === status || ocupadas.includes(id)) return;

    setOcupadas(ids => [...ids, id]);
    setErro("");
    try {
      await enviar("tarefas/" + id + "/status", "PATCH", status);
      setDados(atual => ({
        ...atual,
        tarefas: atual.tarefas.map(item => item.id === id ? { ...item, status } : item),
      }));
    } catch (falha) {
      setErro(falha instanceof Error ? falha.message : "Não foi possível mover a tarefa.");
    } finally {
      setOcupadas(ids => ids.filter(item => item !== id));
    }
  };

  const excluirTarefa = async (id: number) => {
    const tarefa = dados.tarefas.find(item => item.id === id);
    if (carregando || !tarefa || ocupadas.includes(id)) return;
    if (!window.confirm("Excluir a tarefa \"" + tarefa.titulo + "\"?")) return;

    setOcupadas(ids => [...ids, id]);
    setErro("");
    try {
      await consultar<string>("tarefas/" + id, { method: "DELETE" });
      setDados(atual => ({
        ...atual,
        tarefas: atual.tarefas.filter(item => item.id !== id),
      }));
    } catch (falha) {
      setErro(falha instanceof Error ? falha.message : "Não foi possível excluir a tarefa.");
    } finally {
      setOcupadas(ids => ids.filter(item => item !== id));
    }
  };

  return (
    <main className="kanban-fundo">
      <div className="kanban-quadro">
        <BarraKanban
          visao = {visao}
          mudarVisao = {destino => navegar("/" + destino)}
          abrirCadastro = {(tipo: Cadastro) => navegar("/" + visao + "/incluir/" + tipo)}
          cadastroBloqueado = {bloqueado}
        />

        {erro && (
          <p className="kanban-erro" role="alert">
            {erro}{" "}
            {!bloqueado && <button onClick = {() => setCarregando(true)}>Recarregar</button>}
          </p>
        )}
        {carregando && <p role="status">Carregando quadro...</p>}

        <div className="kanban-colunas">
          {colunas.map(coluna => (
            <ColunaKanban
              key = {coluna.id}
              nome = {coluna.nome}
              status = {coluna.id}
              tarefas = {dados.tarefas.filter(tarefa => tarefa.status === coluna.id)}
              categorias = {dados.categorias}
              projetos = {dados.projetos}
              visao = {visao}
              carregando = {carregando}
              dadosCarregados = {dados !== dadosIniciais}
              ocupadas = {ocupadas}
              moverTarefa = {mudarStatus}
              editarTarefa = {setEdicao}
              excluirTarefa = {excluirTarefa}
              abrirResumo = {setResumoId}
            />
          ))}
        </div>

        {cadastro && (
          <FormularioKanban
            key = {rota}
            tipo = {cadastro}
            categorias = {dados.categorias}
            projetos = {dados.projetos}
            fechar = {() => navegar("/" + visao, true)}
            aoSalvar = {() => {
              if (window.location.pathname === rota) {
                navegar("/" + (cadastro === "tarefas" ? visao : cadastro), true);
              }
              setCarregando(true);
            }}
          />
        )}

        {edicao && (
          <FormularioKanban
            key = {"editar-" + edicao.id}
            tipo = "tarefas"
            edicao = {edicao}
            categorias = {dados.categorias}
            projetos = {dados.projetos}
            fechar = {() => setEdicao(null)}
            aoSalvar = {() => {
              setEdicao(null);
              setCarregando(true);
            }}
          />
        )}

        {resumoId !== null && (
          <ResumoProjeto
            key = {resumoId}
            projetoId = {resumoId}
            fechar = {() => setResumoId(null)}
          />
        )}
      </div>
      <style>{`
        body {
          margin: 0;
        }

        .kanban-fundo {
          --kanban-muted: #4a655e;
          min-height: 100vh;
          box-sizing: border-box;
          padding: clamp(.75rem, 4vw, 3.5rem);
          color: #173833;
          font-family: system-ui, sans-serif;
          background: radial-gradient(ellipse at 90% 16%, #b9d6f6 0%, transparent 53%), radial-gradient(ellipse at 14% 42%, #aee5cc 0%, transparent 57%), linear-gradient(145deg, #f9f7ee, #d4eceb);
        }

        .kanban-quadro {
          max-width: 82rem;
          min-height: 70vh;
          margin: auto;
          padding: clamp(.8rem, 3vw, 2rem);
          background: #ffffff52;
          backdrop-filter: blur(24px);
          border: 1px solid #ffffffe8;
          border-radius: 1.8rem;
          box-shadow: 0 24px 70px #234c4120, inset 0 1px 0 #fff;
        }

        .kanban-colunas {
          display: grid;
          grid-template-columns: repeat(3, minmax(0, 1fr));
          gap: 1rem;
        }

        .kanban-quadro button {
          font: inherit;
          font-weight: 600;
          color: inherit;
          background: #ffffff9c;
          border: 1px solid #ffffffed;
          border-radius: .7rem;
          padding: .55rem .8rem;
          cursor: pointer;
        }

        .kanban-quadro button:hover:not(:disabled) {
          background: #fff;
        }

        .kanban-quadro button.kanban-primario {
          background: #235d4f;
          color: white;
          border-color: #235d4f;
        }

        .kanban-quadro button.kanban-primario:hover:not(:disabled) {
          background: #19483d;
        }

        .kanban-quadro button:disabled {
          opacity: .55;
          cursor: wait;
        }

        .kanban-quadro :focus-visible {
          outline: 2px solid #246555;
          outline-offset: 2px;
        }

        .kanban-erro {
          color: #742f2f;
        }

        @media (max-width: 980px) {
          .kanban-colunas {
            grid-template-columns: minmax(0, 1fr);
          }
          .kanban-coluna {
            min-height: 12rem;
          }
        }
      `}</style>
    </main>
  );
}

export default QuadroKanban;
