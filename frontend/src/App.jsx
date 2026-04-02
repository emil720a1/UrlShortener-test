import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { Toaster } from 'react-hot-toast';
import { AuthProvider } from './context/AuthContext';
import { UrlProvider } from './context/UrlContext';
import Home from './pages/Home';
import AuthPage from './pages/AuthPage';
import UrlInfoPage from './pages/UrlInfoPage';
import ProtectedRoute from './components/ProtectedRoute';

export default function App() {
  return (
      <AuthProvider>
          <UrlProvider>
            <Router>
              <Toaster position="top-right" toastOptions={{
                style: {
                  background: '#1E1E20',
                  color: '#fff',
                  border: '1px solid rgba(255, 255, 255, 0.1)'
                }
              }} />
              <Routes>
                <Route path="/" element={<Home />} />
                <Route path="/info/:code" element={<ProtectedRoute><UrlInfoPage /></ProtectedRoute>} />

                <Route path="/login" element={<AuthPage type="login" />} />
                <Route path="/register" element={<AuthPage type="register" />} />
              </Routes>
            </Router>
          </UrlProvider>
      </AuthProvider>
  );
}