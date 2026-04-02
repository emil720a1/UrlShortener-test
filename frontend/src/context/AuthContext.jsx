import { createContext, useContext, useState, useCallback } from 'react';

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
    const [authState, setAuthState] = useState(() => {
        const token = localStorage.getItem('token');
        const userId = localStorage.getItem('userId');
        const rolesRaw = localStorage.getItem('roles');
        const roles = rolesRaw ? JSON.parse(rolesRaw) : [];
        return { token, userId, roles };
    });

    const login = useCallback((token, userId, roles) => {
        localStorage.setItem('token', token);
        localStorage.setItem('userId', userId);
        localStorage.setItem('roles', JSON.stringify(roles));
        setAuthState({ token, userId, roles });
    }, []);

    const logout = useCallback(() => {
        localStorage.removeItem('token');
        localStorage.removeItem('userId');
        localStorage.removeItem('roles');
        setAuthState({ token: null, userId: null, roles: [] });
    }, []);

    const isAuthenticated = Boolean(authState.token);
    const isAdmin = authState.roles.includes('Admin');

    return (
        <AuthContext.Provider value={{ ...authState, isAuthenticated, isAdmin, login, logout }}>
            {children}
        </AuthContext.Provider>
    );
}

export function useAuth() {
    const ctx = useContext(AuthContext);
    if (!ctx) throw new Error('useAuth must be used within AuthProvider');
    return ctx;
}
