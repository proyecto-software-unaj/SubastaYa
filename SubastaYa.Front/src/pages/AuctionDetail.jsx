import { useParams } from "react-router-dom";

export default function AuctionDetail() {
  const { id } = useParams();
  return <div className="max-w-7xl mx-auto px-4 py-8 text-slate-300">Sala de subasta #{id} (próximamente)</div>;
}
