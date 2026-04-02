import toast from 'react-hot-toast';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

export default function UrlTable({ links, onDelete, currentUserId, isAdmin }) {
    const navigate = useNavigate();
    const { isAuthenticated } = useAuth();

    const apiBase =
        import.meta.env.VITE_API_URL || 'http://localhost:5121/api';

    const copyToClipboard = (text, e) => {
        e.stopPropagation();
        navigator.clipboard.writeText(text);
        toast.success('Скопійовано!');
    };

    const handleRowClick = (shortCode) => {
        if (!isAuthenticated) {
            toast.error('Авторизуйтеся, щоб переглянути деталі!');
            return;
        }
        navigate(`/info/${shortCode}`);
    };

    if (links.length === 0) {
        return (
            <div className="text-center py-20 border-2 border-dashed border-[#1E1E20] rounded-3xl opacity-50">
                <p>База даних порожня.</p>
            </div>
        );
    }

    return (
        <div className="overflow-hidden rounded-3xl border border-[#1E1E20] bg-[#1E1E20]/20 backdrop-blur-xl shadow-2xl">
            <table className="w-full text-left">
                <thead className="bg-[#1E1E20]/60 text-[#C9CED6] text-xs uppercase tracking-[0.2em]">
                <tr>
                    <th className="px-8 py-5 font-bold">Short Link</th>
                    <th className="px-8 py-5 font-bold">Original Link</th>
                    <th className="px-8 py-5 font-bold text-center">Status</th>
                    <th className="px-8 py-5 font-bold text-right">Date</th>
                    <th className="px-8 py-5 font-bold text-center">Actions</th>
                </tr>
                </thead>
                <tbody className="divide-y divide-[#1E1E20]">
                {links.map((link) => {
                    const canDelete = isAdmin || link.createdByUserId === currentUserId;
                    
                    return (
                        <tr 
                            key={link.id || link.shortCode} 
                            onClick={() => handleRowClick(link.shortCode)}
                            className="hover:bg-white/5 transition-all group cursor-pointer"
                        >
                            <td className="px-8 py-6 text-[#C9CED6] font-medium flex items-center gap-3">
                                <span className="text-[#144EE3]">linkly.com/{link.shortCode}</span>
                                <button
                                    onClick={(e) => copyToClipboard(`${apiBase}/urls/${link.shortCode}`, e)}
                                    className="p-2 hover:bg-[#144EE3]/20 rounded-lg transition-colors"
                                    title="Copy to clipboard"
                                >
                                    📋
                                </button>
                            </td>
                            <td className="px-8 py-6 text-sm max-w-[200px] truncate text-[#C9CED6]/60" title={link.originalUrl || link.longUrl}>
                                {link.originalUrl || link.longUrl}
                            </td>
                            <td className="px-8 py-6 text-center">
                                <span className="px-3 py-1 rounded-full text-[10px] font-black bg-green-500/10 text-green-400 border border-green-500/20 uppercase">
                                  Active
                                </span>
                            </td>
                            <td className="px-8 py-6 text-right text-[#C9CED6]/40 text-xs font-medium">
                                {new Date(link.createdAt).toLocaleDateString('uk-UA')}
                            </td>
                            <td className="px-8 py-6 text-center">
                                {isAuthenticated ? (
                                    canDelete ? (
                                        <button
                                            onClick={(e) => onDelete(link.shortCode, e)}
                                            className="p-2 text-red-500 hover:text-red-300 hover:bg-red-500/20 rounded-lg transition-colors opacity-50 group-hover:opacity-100"
                                            title="Delete"
                                        >
                                            🗑️
                                        </button>
                                    ) : (
                                        <span className="text-[10px] text-[#C9CED6]/30 uppercase">No Access</span>
                                    )
                                ) : (
                                    <span className="text-[10px] text-[#C9CED6]/10 uppercase italic">Read Only</span>
                                )}
                            </td>
                        </tr>
                    );
                })}
                </tbody>
            </table>
        </div>
    );
}