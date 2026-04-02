import { useState } from 'react';
import axios from 'axios';
import { useNavigate, Link } from 'react-router-dom';

export default function AuthPage({ type }) {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const navigate = useNavigate();
    const isLogin = type === 'login';

    const handleSubmit = async (e) => {
        e.preventDefault();
        const endpoint = isLogin ? 'login' : 'register';
        try {
            const response = await axios.post(`http://localhost:5121/api/auth/${endpoint}`, { email, password });
            if (isLogin) {
                localStorage.setItem('token', response.data.token);
                navigate('/');
            } else {
                alert("Реєстрація успішна! Тепер увійдіть.");
                navigate('/login');
            }
        } catch (err) {
            console.error(err);
            alert("Помилка авторизації!");
        }
    };

    return (
        <div className="min-h-screen bg-[#0B101B] text-white flex items-center justify-center p-6">
            <div className="relative w-full max-w-md bg-[#1E1E20]/40 backdrop-blur-2xl border border-white/5 p-10 rounded-[40px] shadow-2xl">
                <div className="text-center mb-10">
                    <h2 className="text-3xl font-bold mb-2">{isLogin ? 'Welcome Back!' : 'Join Linkly'}</h2>
                    <p className="text-[#C9CED6]/60">{isLogin ? 'Login to manage links' : 'Create an account to start'}</p>
                </div>
                <form onSubmit={handleSubmit} className="space-y-6">
                    <input type="email" placeholder="Email" value={email} onChange={(e)=>setEmail(e.target.value)} required className="w-full bg-[#0B101B] border border-[#1E1E20] rounded-full px-6 py-4 outline-none focus:ring-2 focus:ring-[#144EE3]" />
                    <input type="password" placeholder="Password" value={password} onChange={(e)=>setPassword(e.target.value)} required className="w-full bg-[#0B101B] border border-[#1E1E20] rounded-full px-6 py-4 outline-none focus:ring-2 focus:ring-[#144EE3]" />
                    <button type="submit" className={`w-full py-4 rounded-full font-bold text-lg transition-all active:scale-95 ${isLogin ? 'bg-[#144EE3]' : 'bg-gradient-to-r from-[#EB568E] to-[#144EE3]'}`}>
                        {isLogin ? 'Login' : 'Register'}
                    </button>
                </form>
                <p className="mt-8 text-center text-sm opacity-60">
                    {isLogin ? "No account?" : "Have an account?"} <Link to={isLogin ? "/register" : "/login"} className="underline ml-1 font-bold">{isLogin ? "Register" : "Login"}</Link>
                </p>
            </div>
        </div>
    );
}