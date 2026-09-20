import { Link } from "react-router-dom";
import Countdown from "./Countdown";

const statusLabels = {
  Active: { text: "Activa", className: "bg-emerald-500/20 text-emerald-400" },
  Scheduled: { text: "Próxima", className: "bg-sky-500/20 text-sky-400" },
  Finished: { text: "Finalizada", className: "bg-slate-500/20 text-slate-400" },
  Deserted: { text: "Desierta", className: "bg-rose-500/20 text-rose-400" },
};

export default function AuctionCard({ auction }) {
  const status = statusLabels[auction.status] ?? statusLabels.Finished;

  return (
    <Link to={`/auctions/${auction.id}`} className="block">
      <div className="bg-slate-800 rounded-xl overflow-hidden shadow-lg hover:shadow-emerald-500/10 hover:ring-1 hover:ring-emerald-500/30 transition-all">
        <img
          src={auction.imageUrl}
          alt={auction.title}
          className="w-full h-44 object-cover"
        />
        <div className="p-4 space-y-2">
          <div className="flex items-center justify-between">
            <span className="text-xs text-slate-400">{auction.categoryName}</span>
            <span className={`text-xs px-2 py-0.5 rounded-full ${status.className}`}>
              {status.text}
            </span>
          </div>

          <h3 className="text-lg font-semibold text-white truncate">{auction.title}</h3>

          <div className="flex items-end justify-between pt-2">
            <div>
              <p className="text-xs text-slate-400">Puja actual</p>
              <p className="text-xl font-bold text-emerald-400">
                ${auction.currentHighestBid.toLocaleString("es-AR")}
              </p>
            </div>
            <div className="text-right">
              <p className="text-xs text-slate-400">{auction.bidCount} ofertas</p>
              <Countdown endDate={auction.endDate} />
            </div>
          </div>
        </div>
      </div>
    </Link>
  );
}
