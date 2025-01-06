// Representa os dados do usuário autenticado
  export interface User {
    id: string;       // ID único do usuário
    username: string;     // Nome do usuário
  }
  
  // Resposta esperada ao fazer login
  export interface LoginResponse {
    token: string; // JWT retornado pela API
    refreshToken: string; // Refresh token retornado pela API
    user: User;    // Dados do usuário autenticado
  }
  
  // Registro de ponto (time clock)
  export interface TimeLog {
    id: string;       // ID único do registro
    timestampIn: number; // Data/hora do registro de entrada de ponto
    timestampOut: number; // Data/hora do registro de saída de ponto
    user: User;       // Usuário associado ao registro
  }

  export enum Month {
    'Janeiro' = '1',
    'Fevereiro' = '2',
    'Março' = '3',
    'Abril' = '4',
    'Maio' = '5',
    'Junho' = '6',
    'Julho' = '7',
    'Agosto' = '8',
    'Setembro' = '9',
    'Outubro' = '10',
    'Novembro' = '11',
    'Dezembro' = '12'
  }
  