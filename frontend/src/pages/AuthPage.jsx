import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import toast from 'react-hot-toast';
import axiosInstance from '../api/axiosInstance';
import { useAuth } from '../context/AuthContext';

export default function AuthPage({ type }) {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [isLoading, setIsLoading] = useState(false);
    const navigate = useNavigate();
    const { login } = useAuth();
    const isLogin = type === 'login';

    const handleSubmit = async (e) => {
        e.preventDefault();
        const endpoint = isLogin ? '/auth/login' : '/auth/register';
        setIsLoading(true);
        try {
            const response = await axiosInstance.post(endpoint, { email, password });
            if (isLogin) {
                login(response.data.token, response.data.userId, response.data.roles || []);
                toast.success('Welcome back!');
                setTimeout(() => {
                    navigate('/');
                }, 500);
            } else {
                toast.success('Реєстрація успішна! Тепер увійдіть.');
                navigate('/login');
            }
        } catch (err) {
            toast.error(err.response?.data?.message || err.response?.data?.title || 'Помилка авторизації!');
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className="min-h-screen bg-[#0B101B] text-white flex items-center justify-center p-6 relative overflow-hidden">
            <div className="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-[800px] h-[500px] bg-[#144EE3]/10 blur-[120px] rounded-full pointer-events-none"></div>
            
            <div className="relative w-full max-w-md bg-[#1E1E20]/40 backdrop-blur-2xl border border-white/5 p-10 rounded-[40px] shadow-2xl">
                <div className="text-center mb-10">
                    <h2 className="text-3xl font-bold mb-2">{isLogin ? 'Welcome Back!' : 'Join Linkly'}</h2>
                    <p className="text-[#C9CED6]/60">{isLogin ? 'Login to manage links' : 'Create an account to start'}</p>
                </div>
                <form onSubmit={handleSubmit} className="space-y-6">
                    <input type="email" placeholder="Email" value={email} onChange={(e)=>setEmail(e.target.value)} required className="w-full bg-[#0B101B] border border-[#1E1E20] rounded-full px-6 py-4 outline-none focus:ring-2 focus:ring-[#144EE3]" />
                    <input type="password" placeholder="Password" value={password} onChange={(e)=>setPassword(e.target.value)} required className="w-full bg-[#0B101B] border border-[#1E1E20] rounded-full px-6 py-4 outline-none focus:ring-2 focus:ring-[#144EE3]" />
                    <button type="submit" disabled={isLoading} className={`w-full flex justify-center items-center h-[60px] rounded-full font-bold text-lg transition-all active:scale-95 disabled:opacity-50 ${isLogin ? 'bg-[#144EE3]' : 'bg-gradient-to-r from-[#EB568E] to-[#144EE3]'}`}>
                        {isLoading ? (
                            <svg className="animate-spin h-6 w-6 text-white" viewBox="0 0 24 24" fill="none">
                                <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                                <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v8H4z"></path>
                            </svg>
                        ) : isLogin ? 'Login' : 'Register'}
                    </button>
                </form>
                <p className="mt-8 text-center text-sm opacity-60">
                    {isLogin ? "No account?" : "Have an account?"} <Link to={isLogin ? "/register" : "/login"} className="underline ml-1 font-bold">{isLogin ? "Register" : "Login"}</Link>
                </p>
            </div>
        </div>
    );
}