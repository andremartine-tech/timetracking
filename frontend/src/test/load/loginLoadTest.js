import http from 'k6/http';
import { check, sleep } from 'k6';

export let options = {
    stages: [
        { duration: '1m', target: 100 },  // Aumenta para 100 usuários simultâneos em 1 minuto
        { duration: '3m', target: 100 },  // Mantém 100 usuários simultâneos por 3 minutos
        { duration: '1m', target: 0 },   // Reduz para 0 usuários simultâneos em 1 minuto
    ],
    thresholds: {
        http_req_duration: ['p(95)<500'], // 95% das requisições devem ser menores que 500ms
        http_req_failed: ['rate<0.01'],   // Menos de 1% das requisições devem falhar
    },
};

export default function () {
    let url = 'https://localhost:5001/api/Auth/login';
    let payload = JSON.stringify({
        username: 'test',
        password: 'test',
    });

    let params = {
        headers: {
            'Content-Type': 'application/json',
        },
    };

    // Faz a requisição POST para login
    let res = http.post(url, payload, params);

    // Valida o resultado
    check(res, {
        'status is 200': (r) => r.status === 200,
        'response time < 500ms': (r) => r.timings.duration < 500,
    });

    sleep(1); // Pausa de 1 segundo por iteração
}
