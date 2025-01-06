import http from 'k6/http';
import { check } from 'k6';

export const options = {
  stages: [
    { duration: '30s', target: 100 }, // Sobe para 100 usuários
    { duration: '1m', target: 100 },  // Mantém 100 usuários
    { duration: '10s', target: 0 },   // Reduz a carga
  ],
  thresholds: {
    'http_req_duration': ['p(95)<200'], // 95% das requisições devem ser menores que 200ms
    'checks': ['rate>0.95'],            // 95% das validações devem ser bem-sucedidas
  },
};

//export let out = 'influxdb=http://Vf88BPNvbZos6ttxXCJqY17Q6CbUuEWCY2gE45zYe1U-06_upOD1jjZRjaXwr5Wys6pVvMmkIwkNnFHYH2XuEg==@localhost:8086/k6';
export let out = 'influxdb=http://localhost:8086/k6';

export const userId = '5a460324-716c-4186-b5b3-25ba055e174b';

const user = { Username: 'test', Password: 'test' }; // Usuário único para login

// Setup: Executa antes de todos os VUs
export function setup() {
  const res = http.post('http://localhost:5000/api/auth/login', JSON.stringify(user), {
    headers: { 'Content-Type': 'application/json' },
  });

  check(res, {
    'login successful': (r) => r.status === 200,
  });

  const token = res.json('token');
  console.log(`Token recebido: ${token}`);
  return token; // Retorna o token para uso nos VUs
}

export default function (token) {
  // Calcula o timestamp atual com base na iteração
  const iteration = __ITER; // Número da iteração atual (variável automática do K6)
  const initialTimestampIn = new Date('2024-01-01T00:00:00Z').getTime();
  const initialTimestampOut = new Date('2024-01-01T00:01:00Z').getTime();
  const oneMinute = 60 * 1000;
  const currentTimestampIn = (initialTimestampIn + iteration * oneMinute) + 10;
  const currentTimestampOut = initialTimestampOut + iteration * oneMinute;

  // Converte o timestamp para ISO 8601
  const formattedTimestampIn = new Date(currentTimestampIn).toISOString();
  const formattedTimestampOut = new Date(currentTimestampOut).toISOString();

  const url = 'http://localhost:5000/api/TimeLogs';
  const payload = JSON.stringify({
    userId: userId,
    timestampIn: formattedTimestampIn,
    timestampOut: formattedTimestampOut,
  });

  const params = {
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`
    },
  };

  const res = http.post(url, payload, params);
  check(res, {
    'status is 200': (r) => r.status === 200,
    'Timestamp enviado': () => true,
    'response time < 200ms': (r) => r.timings.duration < 200,
  });

  // Exibe o timestamp atual no console (para depuração)
  console.log(`Add: Iteração ${iteration + 1}, TimestampIn enviado - ${formattedTimestampIn}, TimestampOut enviado - ${formattedTimestampOut}`);
}