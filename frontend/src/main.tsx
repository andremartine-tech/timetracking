import { createRoot } from 'react-dom/client'
import './index.css'
import './styles/custom.scss';
import AppRoutes from './routes/AppRoutes.tsx';
import { AuthProvider } from './context/AuthContext';

createRoot(document.getElementById('root')!).render(
  <AuthProvider>
    <AppRoutes />
  </AuthProvider>
)
