import React, { useEffect, useState } from 'react';
import api from '../api/axios'; // Instância do Axios configurada
import './TimeMirror.css'; // Estilos responsivos
import { User, TimeLog, Month } from '../types';
import { format } from "date-fns";
import swal from 'sweetalert';
import Swal from "sweetalert2";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faTrash, faEdit } from "@fortawesome/free-solid-svg-icons";

const TimeMirror: React.FC = () => {
  const [user, setUser] = useState<User>(() => {
    const savedUser = localStorage.getItem("user");
    return savedUser ? JSON.parse(savedUser) : null;
  });
  const [timeLogs, setTimeLogs] = useState<TimeLog[]>([]);
  const [selectedMonth, setSelectedMonth] = useState<Month>(Month.Janeiro);
  const [selectedYear, setSelectedYear] = useState<string>(new Date().getFullYear().toString());

  const [timeLogIn, setTimeLogIn] = useState<string>('');
  const [timeLogOut, setTimeLogOut] = useState<string>('');
  const [editId, setEditId] = useState<string | null>(null);

  // Busca os registros do backend com filtro de mês e ano
  const fetchTimeLogs = async () => {
    console.log(`Buscando...`);
    try {
      const response = await api.get(`/api/TimeLogs/${user.id}/${selectedMonth}/${selectedYear}`);
      setTimeLogs(response.data);
    } catch (error: any) {
      setTimeLogs([]);
    }
  };

  useEffect(() => {
    const interval = setInterval(() => {
      fetchTimeLogs();
    }, 3000); // Atualiza a cada 3 segundos
  
    return () => clearInterval(interval);
  }, [selectedMonth, selectedYear]);


  const isTimeLogOK = (timestampIn: string, timestampOut: string) => {
    // Verifica se a data de início é anterior à de término
    if (new Date(timestampIn) >= new Date(timestampOut)) {
      swal("Erro de Validação", "O horário de início deve ser anterior ao de término.", "error");
      return false;
    } else {
      return true;
    }
  };

  const hasIntersection = (timestampIn: string, timestampOut: string) => {
    const hasConflict = timeLogs.some((log: TimeLog) => {
      const logStart = new Date(log.timestampIn);
      const logEnd = new Date(log.timestampOut);

      // Verifica se há sobreposição entre os intervalos
      // Se for edição
      if (editId != null) {
        return (
          (new Date(timestampIn) < logEnd && new Date(timestampOut) > logStart && (log.id != editId)) // Interseção
        );
      } else {
        return (
          (new Date(timestampIn) < logEnd && new Date(timestampOut) > logStart) // Interseção
        );
      }
    });

    if (hasConflict) {
      swal("Erro de Validação", "Os horários fornecidos entram em conflito com horários existentes.", "error");
      return true;
    } else {
      return false;
    }
  }

  // Adiciona ou edita um registro
  const handleSave = async () => {
    try {
      // Converte os horários fornecidos
      const timestampIn = new Date(timeLogIn).toISOString();
      const timestampOut = new Date(timeLogOut).toISOString();
      if (editId) {
        try {
          // Verifica se a data de início é anterior à de término
          if (!isTimeLogOK(timestampIn, timestampOut)) return;

          // Verifica interseções com horários existentes
          if (hasIntersection(timestampIn, timestampOut)) return;

          const log = { id: editId, userId: user.id, timestampIn: new Date(timeLogIn).toISOString(), timestampOut: new Date(timeLogOut).toISOString() };

          console.log(`Editando: ${JSON.stringify(log)}`);
          await api.put('/api/TimeLogs', log);
        } catch (error: any) {
          swal("Erro ao registrar o ponto", `${JSON.stringify(error.message)}`, "error");
        }
      } else {

        // Verifica se a data de início é anterior à de término
        if (!isTimeLogOK(timestampIn, timestampOut)) return;

        // Verifica interseções com horários existentes
        if (hasIntersection(timestampIn, timestampOut)) return;

        const log = { userId: user.id, timestampIn: new Date(timeLogIn).toISOString(), timestampOut: new Date(timeLogOut).toISOString() };

        try {
          await api.post('/api/TimeLogs', log);
        } catch (error: any) {
          swal("Erro ao registrar o ponto", `${JSON.stringify(error.message)}`, "error");
        }
      }
      fetchTimeLogs();
      clearForm();
    } catch (error: any) {
      swal("Erro ao registrar o ponto", `${JSON.stringify(error.message)}`, "error");
    }
  };

  // Deleta um registro
  const handleDelete = async (id: string) => {
    try {
      Swal.fire({
        title: "Você confirma a exclusão do registro?",
        showDenyButton: true,
        showCancelButton: false,
        confirmButtonText: "Excluir",
        denyButtonText: `Cancelar`
      }).then(async (result) => {
        if (result.isConfirmed) {
          await api.delete(`/api/TimeLogs/${id}`);
          fetchTimeLogs();
          Swal.fire("Registro excluído!", "", "success");
        } else if (result.isDenied) {
          Swal.fire("O registro não foi excluído", "", "info");
        }
      });
    } catch (error: any) {
      swal("Erro ao excluir o registro", `${JSON.stringify(error.message)}`, "error");
    }
  };

  // Edita um registro existente
  const handleEdit = (log: TimeLog) => {
    setEditId(log.id);
    setTimeLogIn(format(log.timestampIn, "yyyy-MM-dd HH:mm"));
    setTimeLogOut(format(log.timestampOut, "yyyy-MM-dd HH:mm"));
  };

  // Limpa o formulário
  const clearForm = () => {
    setEditId(null);
    setTimeLogIn('');
    setTimeLogOut('');
  };

  return (
    <div className="time-mirror-container">
      <div className="time-mirror-card">
        <div>
          <h2>Espelho de Ponto</h2>
          <h3>{user.username}</h3>

          {/* Filtros de mês e ano */}
          <div className="filter-container">
            <select id="role" value={selectedMonth || ""} onChange={(e) => setSelectedMonth(e.target.value as Month)}>
              <option value="" disabled>
                -- Selecione --
              </option>
              {Object.entries(Month).map(([key, value]) => (
                <option key={key} value={value}>
                  {key}
                </option>
              ))}
            </select>

            <select value={selectedYear} onChange={(e) => setSelectedYear(e.target.value)}>
              {Array.from({ length: 3 }, (_, i) => {
                const year = (new Date().getFullYear() - i).toString();
                return <option key={year} value={year}>{year}</option>;
              })}
            </select>
          </div>

          {/* Formulário de entrada */}
          <div className="form-container">
            <input
              type="datetime-local"
              value={timeLogIn}
              onChange={(e) => setTimeLogIn(e.target.value)}
              placeholder="Data e Hora de Entrada"
            />
            <input
              type="datetime-local"
              value={timeLogOut}
              onChange={(e) => setTimeLogOut(e.target.value)}
              placeholder="Data e Hora Saída"
            />
            <button onClick={handleSave}>{editId ? 'Atualizar' : 'Adicionar'}</button>
            {editId && <button onClick={clearForm}>Cancelar</button>}
          </div>

          {/* Lista de registros */}
          {timeLogs.length === 0 ? (
            <p>0 resultados encontrados</p>
          ) : (
            <div>
              <table className="time-log-table">
                <thead>
                  <tr>
                    <th>Data</th>
                    <th>Entrada</th>
                    <th>Saída</th>
                    <th>Ações</th>
                  </tr>
                </thead>
                <tbody>
                  {timeLogs.map((log) => (
                    <tr key={log.id}>
                      <td>{format(log.timestampIn, "dd/MM")}</td>
                      <td>{format(log.timestampIn, "HH:mm")}</td>
                      <td>{format(log.timestampOut, "HH:mm")}</td>
                      <td>
                        <FontAwesomeIcon
                          icon={faEdit}
                          size="1x"
                          style={{ cursor: "pointer", margin: "5px" }}
                          onClick={() => handleEdit(log)}
                        />
                        <FontAwesomeIcon
                          icon={faTrash}
                          size="1x"
                          style={{ cursor: "pointer", color: "red", margin: "5px" }}
                          onClick={() => handleDelete(log.id)}
                        />
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default TimeMirror;
