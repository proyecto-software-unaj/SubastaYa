import { useEffect, useState } from "react";
import { getMyAuctions, getParticipatingAuctions } from "../api/auctions";
import { useUser } from "../context/UserContext";
import AuctionCard from "../components/AuctionCard";

export default function MyActivity() {
  const { userId } = useUser();
  const [tab, setTab] = useState("bids"); 
  const [auctions, setAuctions] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    setLoading(true);
    setError(null);

    const fetcher = tab === "bids" ? getParticipatingAuctions : getMyAuctions;

    fetcher()
      .then(setAuctions)
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }, [tab, userId]); 

  return (
    <div className="max-w-7xl mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold text-white mb-6">Mis actividades</h1>

      {/* Tabs */}
      <div className="flex gap-2 mb-6 border-b border-slate-700">
        <TabButton active={tab === "bids"} onClick={() => setTab("bids")}>
          Mis compras / pujas
        </TabButton>
        <TabButton active={tab === "publications"} onClick={() => setTab("publications")}>
          Mis publicaciones
        </TabButton>
      </div>

      {loading && <p className="text-slate-400">Cargando...</p>}
      {error && <p className="text-rose-400">Error: {error}</p>}
      {!loading && !error && auctions.length === 0 && (
        <p className="text-slate-400">
          {tab === "bids"
            ? "Todavía no participaste en ninguna subasta."
            : "Todavía no publicaste ninguna subasta."}
        </p>
      )}

      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
        {auctions.map((auction) => (
          <AuctionCard key={auction.id} auction={auction} />
        ))}
      </div>
    </div>
  );
}

function TabButton({ active, onClick, children }) {
  return (
    <button
      onClick={onClick}
      className={`px-4 py-2 text-sm font-medium border-b-2 transition-colors ${
        active
          ? "border-emerald-400 text-emerald-400"
          : "border-transparent text-slate-400 hover:text-white"
      }`}
    >
      {children}
    </button>
  );
}
