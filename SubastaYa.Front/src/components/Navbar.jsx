import { Link, NavLink } from "react-router-dom";
import { useUser } from "../context/UserContext";
import logo from "../assets/logo.png";

const navLinkClass = ({ isActive }) =>
  isActive
    ? "text-white font-semibold"
    : "text-slate-300 hover:text-white";

export default function Navbar() {
  const { userId, changeUser, users } = useUser();

  return (
    <nav className="bg-slate-800 border-b border-slate-700 sticky top-0 z-40 shadow-lg">
      <div className="max-w-7xl mx-auto px-4 py-3 flex items-center justify-between">
        <div className="flex items-center gap-6">
          <Link to="/" className="flex items-center">
            <img src={logo} alt="SubastaYa" className="h-9 w-auto" />
          </Link>
            <div className="flex gap-4 text-sm">
              <NavLink to="/" end className={navLinkClass}>
                Catálogo
              </NavLink>
              <NavLink to="/wallet" className={navLinkClass}>
                Billetera
              </NavLink>
              <NavLink to="/publish" className={navLinkClass}>
                Publicar
              </NavLink>
              <NavLink to="/my-activity" className={navLinkClass}>
                Mis actividades
              </NavLink>
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
