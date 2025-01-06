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
  const url = 'http://localhost:5000/api/Users';
  
  const params = {
    headers: {
      Authorization: `Bearer ${token}`
    },
  };

  const res = http.get(url, params);
  check(res, {
    'status is 200': (r) => r.status === 200,
    'response time < 200ms': (r) => r.timings.duration < 200,
  });
  
  // Mensagem de sucesso
  console.log(`Iteração bem-sucedida. Tempo de resposta: ${res.timings.duration}ms`);
  //sleep(1);
}