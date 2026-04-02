import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

export default function Navbar() {
    const navigate = useNavigate();
    const { isAuthenticated, logout } = useAuth();

    const handleLogout = () => {
        logout();
        navigate('/login');
    };

    return (
        <nav className="relative z-20 flex items-center justify-between py-8 px-12 max-w-7xl mx-auto">
            <Link to="/" className="text-3xl font-black tracking-tighter bg-gradient-to-r from-[#EB568E] to-[#144EE3] bg-clip-text text-transparent hover:opacity-80 transition-opacity">
                Linkly
            </Link>

            <div className="flex items-center gap-6">
                {!isAuthenticated ? (
                    <>
                        <Link
                            to="/login"
                            className="text-[#C9CED6] hover:text-white font-bold transition-all flex items-center gap-2 px-6 py-3 rounded-full border border-[#1E1E20] bg-[#1E1E20]/40 hover:bg-[#1E1E20]"
                        >
                            Login <span className="text-sm">→</span>
                        </Link>

                        <Link
                            to="/register"
                            className="bg-[#144EE3] hover:bg-blue-600 text-white px-8 py-3 rounded-full font-bold shadow-[0_0_20px_rgba(20,78,227,0.4)] transition-all active:scale-95"
                        >
                            Register Now
                        </Link>
                    </>
                ) : (
                    <button
                        onClick={handleLogout}
                        className="text-[#EB568E] border border-[#EB568E]/20 hover:bg-[#EB568E]/10 px-8 py-3 rounded-full font-bold transition-all active:scale-95"
                    >
                        Logout
                    </button>
                )}
            </div>
        </nav>
    );
}