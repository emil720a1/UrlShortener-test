import { useState } from 'react';
import toast from 'react-hot-toast';
import Navbar from '../components/Navbar';
import UrlTable from '../components/UrlTable';
import axiosInstance from '../api/axiosInstance';
import { useAuth } from '../context/AuthContext';
import { useUrl } from '../context/UrlContext';

export default function Home() {
    const [url, setUrl] = useState('');
    const [isShortening, setIsShortening] = useState(false);
    const { isAuthenticated, isAdmin, userId } = useAuth();
    const { links, isLoading: isFetchingData, addLink, removeLink } = useUrl();

    const handleShorten = async (e) => {
        e.preventDefault();
        if (!url) return;

        setIsShortening(true);
        try {
            const response = await axiosInstance.post('/urls/shorten', { originalUrl: url });
            const shortCode = response.data.shortCode;

            const infoResponse = await axiosInstance.get(`/urls/info/${shortCode}`);
            addLink(infoResponse.data);
            setUrl('');
            toast.success('Успішно скорочено!');
        } catch (error) {
            const msg =
                error.response?.data?.[0]?.message ||
                error.response?.data?.message ||
                'Failed to shorten URL';
            toast.error(msg);
        } finally {
            setIsShortening(false);
        }
    };

    const handleDelete = async (shortCode, e) => {
        e.stopPropagation();
        try {
            await axiosInstance.delete(`/urls/${shortCode}`);
            removeLink(shortCode);
            toast.success('Видалено!');
        } catch {
            toast.error('Не вдалося видалити');
        }
    };

    const apiBase =
        import.meta.env.VITE_API_URL?.replace('/api', '') || 'http://localhost:5121';

    return (
        <div className="min-h-screen bg-[#0B101B] text-white font-sans relative overflow-x-hidden">
            <div className="absolute top-[-10%] left-1/2 -translate-x-1/2 w-[1000px] h-[600px] bg-[#144EE3]/10 blur-[120px] rounded-full pointer-events-none"></div>

            <Navbar />

            <main className="relative z-10 max-w-5xl mx-auto pt-20 px-6 pb-24 text-center">
                <h1 className="text-5xl md:text-7xl font-black mb-6 bg-gradient-to-r from-[#EB568E] to-[#144EE3] bg-clip-text text-transparent leading-tight tracking-tight">
                    Shorten Your Loooong Links :)
                </h1>

                <p className="text-[#C9CED6] text-lg max-w-xl mx-auto opacity-80 mb-12">
                    Linkly is an efficient and easy-to-use URL shortening service.
                </p>

                {isAuthenticated ? (
                    <form onSubmit={handleShorten} className="relative max-w-2xl mx-auto mb-10 group">
                        <div className="relative flex items-center bg-[#1E1E20] border-4 border-[#0B101B] rounded-full p-2 pl-6 shadow-2xl">
                            <span className="text-xl">🔗</span>
                            <input
                                type="url"
                                required
                                placeholder="Enter the link here"
                                value={url}
                                onChange={(e) => setUrl(e.target.value)}
                                className="w-full bg-transparent border-none px-4 py-3 text-white focus:outline-none text-lg"
                            />
                            <button
                                type="submit"
                                disabled={isShortening}
                                className="bg-[#144EE3] w-[180px] h-[56px] flex justify-center items-center rounded-full font-bold shadow-lg active:scale-95 disabled:opacity-50"
                            >
                                {isShortening ? (
                                    <svg className="animate-spin h-6 w-6 text-white" viewBox="0 0 24 24" fill="none">
                                        <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                                        <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v8H4z"></path>
                                    </svg>
                                ) : 'Shorten Now!'}
                            </button>
                        </div>
                    </form>
                ) : (
                    <div className="mb-14 py-6 border border-[#144EE3]/20 bg-[#144EE3]/5 rounded-2xl max-w-2xl mx-auto">
                        <p className="text-[#C9CED6]">You must be logged in to shorten URLs.</p>
                        <a href="/login" className="text-[#144EE3] font-bold underline mt-2 inline-block">
                            Sign In / Register
                        </a>
                    </div>
                )}

                <div className="space-y-6 text-left">
                    <h2 className="text-2xl font-bold text-[#C9CED6]">System Links ({links.length})</h2>
                    {isFetchingData ? (
                        <div className="flex justify-center py-20">
                            <svg className="animate-spin h-10 w-10 text-[#144EE3]" viewBox="0 0 24 24" fill="none">
                                <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                                <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v8H4z"></path>
                            </svg>
                        </div>
                    ) : (
                        <UrlTable
                            links={links}
                            onDelete={handleDelete}
                            currentUserId={userId}
                            isAdmin={isAdmin}
                        />
                    )}
                </div>

                {/* About Link for Razor Page */}
                <div className="mt-20 opacity-50 hover:opacity-100 transition-opacity">
                    <a href={`${apiBase}/About`} target="_blank" rel="noreferrer" className="text-[#C9CED6] underline decoration-[#144EE3]">
                        📄 Read Algorithm Information (Razor Page)
                    </a>
                </div>
            </main>
        </div>
    );
}