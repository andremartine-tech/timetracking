import React, { useState, useContext } from 'react';
import { AuthContext } from '../context/AuthContext';
import { useNavigate } from 'react-router-dom';
import styles from './Login.module.css';

const Login: React.FC = () => {
  const navigate = useNavigate();
  const auth = useContext(AuthContext);

  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null); // Limpa erros anteriores

    try {
      await auth?.login({ username, password });
      navigate('/time-mirror'); // Redireciona após login bem-sucedido
    } catch {
      setError('Credenciais inválidas. Por favor, tente novamente.');
    }
  };

  return (
    <div className={styles.container}>
      <div className={styles.formWrapper}>
        <img
          src="/logo.png"
          alt="Logotipo"
          className={styles.logo}
        />
        <h2 className={styles.title}>Bem-vindo ao Sistema</h2>

        {error && <p className={styles.error}>{error}</p>}

        <form className={styles.form} onSubmit={handleSubmit}>
          <input
            type="text"
            placeholder="Usuário"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            className={styles.input}
            required
          />
          <input
            type="password"
            placeholder="Senha"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            className={styles.input}
            required
          />
          <button type="submit" className={styles.button}>
            Entrar
          </button>
        </form>
      </div>
    </div>
  );
};

export default Login;
