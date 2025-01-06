import http from 'k6/http';
import { check } from 'k6';

export const options = {
  stages: [
    { duration: '30s', target: 100 }, // Sobe para 100 usuários
    { duration: '1m', target: 100 },  // Mantém 100 usuários
    { duration: '10s', target: 0 },   // Reduz a carga
  ],
};

//export let out = 'influxdb=http://Vf88BPNvbZos6ttxXCJqY17Q6CbUuEWCY2gE45zYe1U-06_upOD1jjZRjaXwr5Wys6pVvMmkIwkNnFHYH2XuEg==@localhost:8086/k6';
export let out = 'influxdb=http://localhost:8086/k6';

export default function () {
  // Calcula o timestamp atual com base na iteração
  const iteration = __ITER; // Número da iteração atual (variável automática do K6)
  const initialTimestampIn = new Date('2024-01-01T00:00:00Z').getTime();
  const initialTimestampOut = new Date('2024-01-01T00:01:00Z').getTime();
  const oneMinute = 60 * 1000;
  const currentTimestampIn = (initialTimestampIn + iteration * oneMinute) + 10;
  const currentTimestampOut = initialTimestampOut + iteration * oneMinute;
  
  // Converte o timestamp para ISO 8601 (formato comum para APIs)
  const formattedTimestampIn = new Date(currentTimestampIn).toISOString();
  const formattedTimestampOut = new Date(currentTimestampOut).toISOString();

  const userId = '5a460324-716c-4186-b5b3-25ba055e174b';
  const recordId = 'ab26efd7-20aa-43a4-887b-7d3e84d917c7';
  
  // Exemplo de requisição
  const url = 'http://localhost:5000/api/TimeLogs';
  const payload = JSON.stringify({
      id: recordId,
      userId: userId,
      timestampIn: formattedTimestampIn,
      timestampOut: formattedTimestampOut,
  });

  const params = {
    headers: {
      'Content-Type': 'application/json',
    },
  };

  const res = http.put(`${url}`, payload, params);
  check(res, {
    'status is 200': (r) => r.status === 200,
    'Timestamp enviado': () => true,
    'response time < 200ms': (r) => r.timings.duration < 200,
  });
  
  // Exibe o timestamp atual no console (para depuração)
  console.log(`Update: Iteração ${iteration + 1}, Registro ${recordId} atualizado.`);
}