import { useEffect, useState, useCallback } from "react";
import { useParams } from "react-router-dom";
import { getAuctionById, placeBid } from "../api/auctions";
import { useUser } from "../context/UserContext";
import Countdown from "../components/Countdown";
import { useAuctionHub } from "../hooks/useAuctionHub";

export default function AuctionDetail() {
  const { id } = useParams();
  const { userId } = useUser();

  const [auction, setAuction] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const [bidAmount, setBidAmount] = useState("");
  const [placing, setPlacing] = useState(false);
  const [feedback, setFeedback] = useState(null);

  
  const loadAuction = useCallback(() => {
    getAuctionById(id)
      .then((data) => {
        setAuction(data);
        
        setBidAmount(String(data.suggestedNextBid));
      })
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }, [id]);

  useEffect(() => {
    setLoading(true);
    loadAuction();
  }, [loadAuction]);

  useAuctionHub(id, {
    onBidPlaced: () => {
      loadAuction();
    },
    onAuctionExtended: () => {
      setFeedback({ type: "success", text: "⏱️ La subasta se extendió porque hubo una oferta en los últimos minutos." });
      loadAuction();
    },
  });

  const handleBid = async (e) => {
    e.preventDefault();
    setFeedback(null);

    const value = Number(bidAmount);
    if (!value || value <= 0) {
      setFeedback({ type: "error", text: "Ingresá un monto válido." });
      return;
    }

    setPlacing(true);
    try {
      await placeBid(id, value);
      setFeedback({ type: "success", text: "¡Puja registrada!" });
      loadAuction(); 
    } catch (err) {
      
      let text = err.message;
      if (err.status === 409) text = "Conflicto: otra puja se registró primero o la subasta cambió.";
      if (err.status === 422) text = "Saldo insuficiente para esta puja.";
      if (err.status === 400) text = err.message; 
      setFeedback({ type: "error", text });
    } finally {
      setPlacing(false);
    }
  };

  if (loading) return <div className="max-w-5xl mx-auto px-4 py-8 text-slate-400">Cargando subasta...</div>;
  if (error) return <div className="max-w-5xl mx-auto px-4 py-8 text-rose-400">Error: {error}</div>;
  if (!auction) return null;

  
  const isLeading = auction.currentWinnerId === userId;
  const isActive = auction.status === "Active";

  return (
    <div className="max-w-5xl mx-auto px-4 py-8">
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
        {/* Columna izquierda: imagen + info */}
        <div>
          <img src={auction.imageUrl} alt={auction.title} className="w-full h-64 object-cover rounded-xl" />
          <h1 className="text-2xl font-bold text-white mt-4">{auction.title}</h1>
          <p className="text-slate-400 mt-2">{auction.description}</p>
          <p className="text-xs text-slate-500 mt-2">Categoría: {auction.categoryName}</p>
        </div>

        {/* Columna derecha: temporizador + puja + consola */}
        <div className="space-y-4">
          {/* Temporizador */}
          <div className="bg-slate-800 rounded-xl p-5 text-center">
            <p className="text-sm text-slate-400 mb-1">Tiempo restante</p>
            <div className="text-3xl">
              <Countdown endDate={auction.endDate} />
            </div>
          </div>

          {/* Puja actual + estado */}
          <div className="bg-slate-800 rounded-xl p-5">
            <p className="text-sm text-slate-400">Puja actual</p>
            <p className="text-3xl font-bold text-emerald-400">
              ${auction.currentHighestBid.toLocaleString("es-AR")}
            </p>
            {isActive && (
              <p className={`mt-2 text-sm font-semibold ${isLeading ? "text-emerald-400" : "text-slate-400"}`}>
                {isLeading ? "🏆 Estás liderando" : "No estás liderando"}
              </p>
            )}
          </div>

          {/* Consola de puja */}
          {isActive ? (
            <form onSubmit={handleBid} className="bg-slate-800 rounded-xl p-5 space-y-3">
              <label className="text-sm text-slate-400">
                Tu puja (mínimo sugerido: ${auction.suggestedNextBid.toLocaleString("es-AR")})
              </label>
              <input
                type="number"
                min="0"
                step="0.01"
                value={bidAmount}
                onChange={(e) => setBidAmount(e.target.value)}
                className="w-full bg-slate-700 text-white rounded-lg px-3 py-2"
                disabled={placing || isLeading}
              />
              <button
                type="submit"
                disabled={placing || isLeading}
                className="w-full bg-emerald-500 hover:bg-emerald-600 disabled:opacity-50 text-white font-semibold rounded-lg py-2 transition-colors"
              >
                {placing ? "Enviando..." : isLeading ? "Ya sos el líder" : "Pujar"}
              </button>

              {feedback && (
                <p className={`text-sm ${feedback.type === "success" ? "text-emerald-400" : "text-rose-400"}`}>
                  {feedback.text}
                </p>
              )}
            </form>
          ) : (
            <div className="bg-slate-800 rounded-xl p-5 text-center text-slate-400">
              Esta subasta no está disponible para pujar (estado: {auction.status}).
            </div>
          )}
        </div>
      </div>

      {/* Historial de pujas */}
      <div className="mt-8">
        <h2 className="text-xl font-semibold text-white mb-4">Historial de pujas</h2>
        {auction.recentBids.length === 0 ? (
          <p className="text-slate-400">Todavía no hay pujas.</p>
        ) : (
          <div className="bg-slate-800 rounded-xl divide-y divide-slate-700">
            {auction.recentBids.map((bid) => (
              <div key={bid.id} className="flex items-center justify-between px-5 py-3">
                <span className="text-slate-300">{bid.bidderAlias}</span>
                <span className="text-emerald-400 font-semibold">
                  ${bid.amount.toLocaleString("es-AR")}
                </span>
                <span className="text-xs text-slate-500">
                  {new Date(bid.bidDate).toLocaleTimeString("es-AR")}
                </span>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}

