import { useEffect } from "react";

function ListarTarefa() {

    // useEffect é utlizado para executar qualquer codigo no momento em que o componente estiver sendo aberto
    useEffect(() => {
        fetch("https://localhost:5000/api/tarefas")
            .then(resposta => {
                return resposta.json();
            })
            .then(dados => {
                console.log(dados);
            });
    }, []);
    
    return (
        <div>
            <h2>Listar Tarefa</h2>
            
        </div>
    )
}

export default ListarTarefa;



/* import { useEffect, useState } from "react";
import axios from "axios";

interface Tarefa {
  id: number;
  titulo: string;
  descricao?: string;
}

function ListarTarefas() {
  const [tarefas, setTarefas] = useState<Tarefa[]>([]);
  const [erro, setErro] = useState("");

  useEffect(() => {
    async function buscarTarefas() {
      try {
        const resposta = await axios.get<Tarefa[]>(
          "http://localhost:8080/tarefas"
        );

        setTarefas(resposta.data);
      } catch (error) {
        setErro("Não foi possível carregar as tarefas.");
        console.error(error);
      }
    }

    buscarTarefas();
  }, []);

  return (
    <div>
      <h2>Listar Tarefas</h2>

      {erro && <p>{erro}</p>}

      {tarefas.map((tarefa) => (
        <div key={tarefa.id}>
          <h3>{tarefa.titulo}</h3>
          <p>{tarefa.descricao}</p>
        </div>
      ))}
    </div>
  );
}

export default ListarTarefas; */