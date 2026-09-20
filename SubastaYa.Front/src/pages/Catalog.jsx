import { useEffect, useState } from "react";
import { getAuctions, getCategories } from "../api/auctions";
import AuctionCard from "../components/AuctionCard";

export default function Catalog() {
  const [auctions, setAuctions] = useState([]);
  const [categories, setCategories] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);


  const [status, setStatus] = useState("");
  const [categoryId, setCategoryId] = useState("");
  const [sort, setSort] = useState("");


  useEffect(() => {
    getCategories().then(setCategories).catch(() => {});
  }, []);

  
  useEffect(() => {
    setLoading(true);
    setError(null);
    getAuctions({ status, categoryId, sort })
      .then(setAuctions)
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }, [status, categoryId, sort]);

  return (
    <div className="max-w-7xl mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold text-white mb-6">Catálogo de subastas</h1>

      {/* Filtros */}
      <div className="flex flex-wrap gap-3 mb-8">
        <select
          value={status}
          onChange={(e) => setStatus(e.target.value)}
          className="bg-slate-800 text-white rounded-lg px-3 py-2 text-sm"
        >
          <option value="">Todos los estados</option>
          <option value="Active">Activas</option>
          <option value="Scheduled">Próximas</option>
          <option value="Finished">Finalizadas</option>
          <option value="Deserted">Desiertas</option>
        </select>

        <select
          value={categoryId}
          onChange={(e) => setCategoryId(e.target.value)}
          className="bg-slate-800 text-white rounded-lg px-3 py-2 text-sm"
        >
          <option value="">Todas las categorías</option>
          {categories.map((c) => (
            <option key={c.id} value={c.id}>
              {c.name}
            </option>
          ))}
        </select>

        <select
          value={sort}
          onChange={(e) => setSort(e.target.value)}
          className="bg-slate-800 text-white rounded-lg px-3 py-2 text-sm"
        >
          <option value="">Ordenar por</option>
          <option value="endingSoon">Menor tiempo restante</option>
          <option value="highestBid">Mayor puja</option>
        </select>
      </div>

      {/* Estados de carga / error / vacio */}
      {loading && <p className="text-slate-400">Cargando subastas...</p>}
      {error && <p className="text-rose-400">Error: {error}</p>}
      {!loading && !error && auctions.length === 0 && (
        <p className="text-slate-400">No hay subastas que coincidan con los filtros.</p>
      )}

      {/* Grid de cards */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
        {auctions.map((auction) => (
          <AuctionCard key={auction.id} auction={auction} />
        ))}
      </div>
    </div>
  );
}
