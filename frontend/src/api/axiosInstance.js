import axios from 'axios';
import toast from 'react-hot-toast';

const axiosInstance = axios.create({
    baseURL: import.meta.env.VITE_API_URL || 'http://localhost:5121/api',
});

// Request Interceptor: add token
axiosInstance.interceptors.request.use((config) => {
    const token = localStorage.getItem('token');
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
}, (error) => Promise.reject(error));

// Response Interceptor: global error handling
axiosInstance.interceptors.response.use(
    (response) => response,
    (error) => {
        if (!error.response) {
            toast.error("Network Error. Is the API running?");
        } else {
            const { status } = error.response;
            if (status === 401) {
                toast.error("Session expired, please login again.", { id: 'unauth' });
                localStorage.removeItem('token');
                window.location.href = '/login';
            } else if (status === 403) {
                toast.error("You don't have permission to do this.");
            } else if (status >= 500) {
                toast.error("Server Error. Please try again later.");
            }
        }
        return Promise.reject(error);
    }
);

export default axiosInstance;
