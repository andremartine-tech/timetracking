import http from 'k6/http';
import { check } from 'k6';
import { SharedArray } from 'k6/data';

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

// Função para gerar uma senha aleatória de 8 dígitos
function generateRandomUsernameOrPassword() {
  let charset = '0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz';
  let password = '';
  for (let i = 0; i < 8; i++) {
      password += charset[Math.floor(Math.random() * charset.length)];
  }
  return password;
}

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
  const iteration = __ITER; // Número da iteração atual (variável automática do K6)
  const username = generateRandomUsernameOrPassword();
  const password = generateRandomUsernameOrPassword();
  
  const url = 'http://localhost:5000/api/Users';
  const payload = JSON.stringify({
      username: username,
      password: password
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
    'User enviado': () => true,
    'response time < 200ms': (r) => r.timings.duration < 200,
  });
  
  // Mensagem de sucesso
  console.log(`Add: Token: ${token} Iteração ${iteration + 1}, Username enviado - ${username}, Password enviado - ${password}`);
}