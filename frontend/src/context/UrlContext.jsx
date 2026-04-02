import { createContext, useContext, useState, useCallback, useEffect } from 'react';
import axiosInstance from '../api/axiosInstance';
import { useAuth } from './AuthContext';

const UrlContext = createContext(null);

export function UrlProvider({ children }) {
    const [links, setLinks] = useState([]);
    const [isLoading, setIsLoading] = useState(false);
    const { isAuthenticated } = useAuth();

    const fetchLinks = useCallback(async () => {
        setIsLoading(true);
        try {
            const response = await axiosInstance.get('/urls/all');
            setLinks(response.data);
        } catch (error) {
            console.error('Failed to fetch links', error);
        } finally {
            setIsLoading(false);
        }
    }, []);

    const clearLinks = useCallback(() => {
        setLinks([]);
    }, []);

    // Clear links immediately when user logs out
    useEffect(() => {
        if (!isAuthenticated) {
            clearLinks();
        }
        fetchLinks();
    }, [isAuthenticated, clearLinks, fetchLinks]);

    const addLink = useCallback((newLink) => {
        setLinks(prev => [newLink, ...prev]);
    }, []);

    const removeLink = useCallback((shortCode) => {
        setLinks(prev => prev.filter(l => l.shortCode !== shortCode));
    }, []);

    return (
        <UrlContext.Provider value={{ links, isLoading, fetchLinks, clearLinks, addLink, removeLink }}>
            {children}
        </UrlContext.Provider>
    );
}

export function useUrl() {
    const ctx = useContext(UrlContext);
    if (!ctx) throw new Error('useUrl must be used within UrlProvider');
    return ctx;
}
