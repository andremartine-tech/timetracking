import React, { createContext, useState, useEffect, ReactNode } from 'react';
import api from '../api/axios';
import { User, LoginResponse } from '../types';
import process from 'process';
window.process = process;


interface AuthContextProps {
  user: User | null;
  login: (credentials: { username: string; password: string }) => Promise<void>;
  logout: () => void;
}

export const AuthContext = createContext<AuthContextProps | null>(null);

export const AuthProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  const [user, setUser] = useState<User | null>(null);
  const [token, setToken] = useState<string | null>(localStorage.getItem('token'));
  const [refreshToken, setRefreshToken] = useState<string | null>(localStorage.getItem('refreshToken'));

  const login = async (credentials: { username: string; password: string }) => {
    try {
        const response = await api.post<LoginResponse>('/api/auth/login', {
        username: credentials.username,
        password: credentials.password
      });
      
      localStorage.setItem('token', response.data.token);
      localStorage.setItem('refreshToken', response.data.refreshToken);
      localStorage.setItem('user', JSON.stringify(response.data.user));
      setUser(response.data.user);
      setToken(response.data.token);
      setRefreshToken(response.data.refreshToken);
    } catch (error) {
      console.error('Erro ao fazer login:', error);
      throw error;
    }
  };

  const runRefreshToken = async () => {
    try {
      const savedToken = localStorage.getItem('refreshToken');
      console.log(`SAVED REFRESH TOKEN: ${JSON.stringify(savedToken)}`);
      if (!savedToken) return;
  
      const response = await api.post<LoginResponse>('/api/auth/refresh-token', {
        savedToken
      });

      console.log(`RESPONSE: ${JSON.stringify(response)}`);
  
      localStorage.setItem('token', response.data.token);
      setToken(response.data.token);
    } catch (error) {
      console.error('Erro ao atualizar o token:', error);
      logout();
    }
  };

  const logout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    setUser(null);
  };

  useEffect(() => {
    const verifyToken = async () => {
      const savedToken = localStorage.getItem('token');
      console.log(`SAVED TOKEN: ${savedToken}`);
      if (savedToken) {
        try {
          const { data } = await api.get('/api/auth/verify');
          console.log(`DATA: ${JSON.stringify(data)}`);
          setUser(data.user);
          setToken(data.token);
        } catch {
          await runRefreshToken(); // Tenta atualizar o token
        }
      }
    };
    verifyToken();
  }, []);
  

  useEffect(() => {
    const interval = setInterval(async () => {
      await runRefreshToken();
    }, 10 * 60 * 1000); // Atualiza a cada 10 minutos
  
    return () => clearInterval(interval); // Limpa o intervalo ao desmontar o componente
  }, []);

  return (
    <AuthContext.Provider value={{ user, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};
