import { useState, useEffect } from 'react'
import axios from 'axios'
import Navbar from '../components/Navbar'
import UrlTable from '../components/UrlTable'

const API_BASE_URL = 'http://localhost:5121/api/urls';

export default function Home() {
    const [url, setUrl] = useState('')
    const [links, setLinks] = useState([])
    const [isLoading, setIsLoading] = useState(false)

    const token = localStorage.getItem('token');

    useEffect(() => {
        const fetchLinks = async () => {
            try {
                const response = await axios.get(API_BASE_URL, {
                    headers: { Authorization: `Bearer ${token}` }
                });
                setLinks(response.data);
            } catch (error) {
                console.error("Не вдалося завантажити історію:", error);
            }
        };
        if (token) fetchLinks();
    }, [token]);

    const handleShorten = async (e) => {
        e.preventDefault();
        if (!url) return;
        if (!token) return alert("Спочатку увійдіть у систему!");

        setIsLoading(true);
        try {
            const response = await axios.post(`${API_BASE_URL}/shorten`,
                { originalUrl: url },
                { headers: { Authorization: `Bearer ${token}` } }
            );

            const newLink = {
                shortCode: response.data.shortCode,
                longUrl: url,
                createdAt: new Date().toISOString(),
                clicks: 0
            };

            setLinks([newLink, ...links]);
            setUrl('');
        } catch (error) {
            if (error.response?.status === 401) alert("Сесія закінчилася, увійдіть знову.");
            else alert("Помилка запиту! Перевір консоль.");
        } finally {
            setIsLoading(false);
        }
    };

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
                            disabled={isLoading}
                            className="bg-[#144EE3] px-10 py-4 rounded-full font-bold shadow-lg active:scale-95 disabled:opacity-50"
                        >
                            {isLoading ? 'Processing...' : 'Shorten Now!'}
                        </button>
                    </div>
                </form>

                <div className="space-y-6 text-left">
                    <h2 className="text-2xl font-bold text-[#C9CED6]">History ({links.length})</h2>
                    <UrlTable links={links} />
                </div>
            </main>
        </div>
    )
}