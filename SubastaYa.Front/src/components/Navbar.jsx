import { Link } from "react-router-dom";
import { useUser } from "../context/UserContext";

export default function Navbar() {
  const { userId, changeUser, users } = useUser();

  return (
    <nav className="bg-slate-800 border-b border-slate-700">
      <div className="max-w-7xl mx-auto px-4 py-3 flex items-center justify-between">
        <div className="flex items-center gap-6">
          <Link to="/" className="text-xl font-bold text-emerald-400">
            SubastaYa
          </Link>
          <div className="flex gap-4 text-sm">
            <Link to="/" className="text-slate-300 hover:text-white">
              Catálogo
            </Link>
            <Link to="/wallet" className="text-slate-300 hover:text-white">
              Billetera
            </Link>
            <Link to="/publish" className="text-slate-300 hover:text-white">
              Publicar
            </Link>
            <Link to="/my-activity" className="text-slate-300 hover:text-white">
              Mis actividades
            </Link>
          </div>
        </div>

        {/* Selector de usuario (demo) */}
        <div className="flex items-center gap-2">
          <span className="text-xs text-slate-400">Usuario:</span>
          <select
            value={userId}
            onChange={(e) => changeUser(e.target.value)}
            className="bg-slate-700 text-white rounded-lg px-2 py-1 text-sm"
          >
            {users.map((u) => (
              <option key={u.id} value={u.id}>
                {u.name}
              </option>
            ))}
          </select>
        </div>
      </div>
    </nav>
  );
}
