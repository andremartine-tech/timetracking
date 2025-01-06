import React, { useContext } from 'react';
import { BrowserRouter as Router, Route, Routes, Navigate } from 'react-router-dom';
import Login from '../pages/Login';
import TimeMirror from '../pages/TimeMirror';
import { AuthContext } from '../context/AuthContext';

// Componente para proteger rotas privadas
const PrivateRoute: React.FC<{ children: React.ReactElement }> = ({ children }) => {
  const auth = useContext(AuthContext);

  if (!auth?.user) {
    // Se o usuário não estiver autenticado, redireciona para o Login
    return <Navigate to="/" replace />;
  } 

  return children;
};

// Configuração de rotas principais
const AppRoutes: React.FC = () => {
  return (
    <Router>
      <Routes>
        {/* Rota pública para Login */}
        <Route path="/" element={<Login />} />

        <Route
          path="/time-mirror"
          element={
            <PrivateRoute>
              <TimeMirror />
            </PrivateRoute>
          }
        />

        {/* Redirecionar para Login caso a rota não exista */}
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </Router>
  );
};

export default AppRoutes;
