import axios, { AxiosInstance } from 'axios';

// Base URL da API (definida no .env)
const BASE_URL = import.meta.env.VITE_API_URL || 'https://localhost:9001';

// Criação da instância do Axios
const api: AxiosInstance = axios.create({
  baseURL: BASE_URL,
  timeout: 10000, // Tempo máximo para requisições (10 segundos)
  headers: {
    'Content-Type': 'application/json',
    Accept: 'application/json',
  },
});

// Interceptador de requisição
api.interceptors.request.use(
  (config) => {
    // Adiciona o token JWT ao cabeçalho Authorization, se disponível
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    // Log ou tratamento de erros antes de enviar a requisição
    console.error('Erro na requisição:', error);
    return Promise.reject(error);
  }
);

// Interceptador de resposta
api.interceptors.response.use(
  (response) => {
    // Processa a resposta se for bem-sucedida
    return response;
  },
  (error) => {
    // Tratamento global de erros na resposta
    if (error.response) {
      console.error('Erro na resposta:', error.response.status, error.response.data);

      // Exemplos de tratamento por status HTTP
      if (error.response.status === 401) {
        console.warn('Usuário não autorizado. Redirecionando para login...');
        // Redirecionar para o login ou limpar dados do usuário
        localStorage.removeItem('token');
        window.location.href = '/';
      }
    } else {
      console.error('Erro na comunicação com a API:', error.message);
    }
    return Promise.reject(error);
  }
);

// Interceptor para tratar erros
api.interceptors.response.use(
  (response) => response, // Retorna a resposta normalmente
  (error) => {
    if (error.response) {
      // Erros retornados pelo servidor (4xx, 5xx)
      return Promise.reject({
        status: error.response.status,
        message: error.response.data?.message || "Erro desconhecido no servidor.",
      });
    } else if (error.request) {
      // Erros relacionados à rede ou sem resposta do servidor
      return Promise.reject({
        status: null,
        message: "Falha na conexão. Verifique sua internet.",
      });
    } else {
      // Outros erros (ex.: erro na configuração do Axios)
      return Promise.reject({
        status: null,
        message: error.message || "Erro inesperado.",
      });
    }
  }
);

export default api;
