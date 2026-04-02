import { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import axiosInstance from '../api/axiosInstance';
import Navbar from '../components/Navbar';

export default function UrlInfoPage() {
    const { code } = useParams();
    const [info, setInfo] = useState(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchInfo = async () => {
            try {
                const response = await axiosInstance.get(`/urls/info/${code}`);
                setInfo(response.data);
            } catch (error) {
            } finally {
                setLoading(false);
            }
        };
        fetchInfo();
    }, [code]);

    return (
        <div className="min-h-screen bg-[#0B101B] text-white font-sans relative overflow-x-hidden">
            <div className="absolute top-[-10%] left-1/2 -translate-x-1/2 w-[1000px] h-[600px] bg-[#144EE3]/10 blur-[120px] rounded-full pointer-events-none"></div>

            <Navbar />

            <main className="relative z-10 max-w-3xl mx-auto pt-20 px-6 pb-24">
                <Link to="/" className="text-[#EB568E] mb-8 inline-block hover:underline">&larr; Back to Dashboard</Link>
                
                <div className="bg-[#1E1E20]/40 backdrop-blur-2xl border border-white/5 p-10 rounded-[40px] shadow-2xl">
                    <h2 className="text-3xl font-bold mb-6 text-[#144EE3]">URL Details</h2>
                    
                    {loading ? (
                        <div className="flex justify-center py-10">
                            <svg className="animate-spin h-10 w-10 text-[#144EE3]" viewBox="0 0 24 24" fill="none">
                                <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                                <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v8H4z"></path>
                            </svg>
                        </div>
                    ) : info ? (
                        <div className="space-y-6 text-[#C9CED6]">
                            <div className="flex flex-col border-b border-[#1E1E20] pb-4">
                                <span className="text-sm opacity-50 uppercase tracking-widest font-bold mb-1">Short Code</span>
                                <span className="text-2xl font-mono text-white">{info.shortCode}</span>
                            </div>
                            
                            <div className="flex flex-col border-b border-[#1E1E20] pb-4">
                                <span className="text-sm opacity-50 uppercase tracking-widest font-bold mb-1">Original URL</span>
                                <a href={info.originalUrl} target="_blank" rel="noopener noreferrer" className="text-lg text-[#144EE3] hover:underline break-all">
                                    {info.originalUrl}
                                </a>
                            </div>

                            <div className="flex flex-col border-b border-[#1E1E20] pb-4">
                                <span className="text-sm opacity-50 uppercase tracking-widest font-bold mb-1">Created By (User ID)</span>
                                <span className="text-lg font-mono">{info.createdByUserId}</span>
                            </div>

                            <div className="flex flex-col pb-4">
                                <span className="text-sm opacity-50 uppercase tracking-widest font-bold mb-1">Creation Date</span>
                                <span className="text-lg">{new Date(info.createdAt).toLocaleString()}</span>
                            </div>
                        </div>
                    ) : (
                        <p className="text-red-400">Information could not be loaded.</p>
                    )}
                </div>
            </main>
        </div>
    );
}
