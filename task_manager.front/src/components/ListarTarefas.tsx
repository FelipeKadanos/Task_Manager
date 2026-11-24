import { useEffect, useState } from "react";

interface Tarefa {
  id: number;
  titulo: string;
  descricao?: string;
}

function ListarTarefa() {

  const [tarefas, setTarefas] = useState<Tarefa[]>([]);

  // useEffect é utlizado para executar qualquer codigo no momento em que o componente estiver sendo aberto
  useEffect(() => {
    fetch("http://localhost:5000/api/tarefas")
      .then(resposta => resposta.json())
      .then(dados => setTarefas(dados));
  }, []);

  return (
    <div>
      <h2>Listar Tarefa</h2>

      {tarefas.map(tarefa => (
        <div key = {tarefa.id}>
          <h3>{tarefa.titulo}</h3>
          {tarefa.descricao && <p>{tarefa.descricao}</p>}
        </div>
      ))}

    </div>
  )
}

export default ListarTarefa;