import { useEffect, useState } from "react";
import { getBalance, deposit, getTransactions } from "../api/wallet";
import { useUser } from "../context/UserContext";
import toast from "react-hot-toast";

const typeInfo = {
  Deposit: { label: "Depósito", color: "text-emerald-400", sign: "+" },
  Hold: { label: "Retención", color: "text-amber-400", sign: "−" },
  Release: { label: "Liberación", color: "text-sky-400", sign: "+" },
  Payment: { label: "Pago (subasta ganada)", color: "text-rose-400", sign: "−" },
  Collection: { label: "Cobro (venta)", color: "text-emerald-400", sign: "+" },
};

export default function Wallet() {
  const { userId } = useUser();
  const [balance, setBalance] = useState(null);
  const [transactions, setTransactions] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const [amount, setAmount] = useState("");
  const [depositing, setDepositing] = useState(false);

  
  const loadData = () => {
    setLoading(true);
    setError(null);
    Promise.all([getBalance(), getTransactions()])
      .then(([bal, txs]) => {
        setBalance(bal);
        setTransactions(txs);
      })
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  };

  useEffect(() => {
    loadData();
  }, [userId]);

  const handleDeposit = async (e) => {
    e.preventDefault();

    const value = Number(amount);
    if (!value || value <= 0) {
      toast.error("Ingresá un monto positivo.");
      return;
    }

    setDepositing(true);
    try {
      await deposit(value);
      setAmount("");
      toast.success(`Se acreditaron $${value.toLocaleString("es-AR")}.`);
      loadData();
    } catch (err) {
      toast.error(err.message);
    } finally {
      setDepositing(false);
    }
  };

  return (
    <div className="max-w-4xl mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold text-white mb-6">Mi billetera</h1>

      {loading && <p className="text-slate-400">Cargando...</p>}
      {error && <p className="text-rose-400">Error: {error}</p>}

      {balance && (
        <>
          {/* Panel de las 3 metricas */}
          <div className="grid grid-cols-1 sm:grid-cols-3 gap-4 mb-8">
            <MetricCard label="Saldo total" value={balance.totalBalance} color="text-white" />
            <MetricCard label="Retenido (garantía)" value={balance.heldBalance} color="text-amber-400" />
            <MetricCard label="Disponible" value={balance.availableBalance} color="text-emerald-400" />
          </div>

          {/* Formulario de carga */}
          <div className="bg-slate-800 rounded-xl p-6 mb-8">
            <h2 className="text-lg font-semibold text-white mb-4">Cargar saldo</h2>
            <form onSubmit={handleDeposit} className="flex gap-3">
              <input
                type="number"
                step="0.01"
                value={amount}
                onChange={(e) => setAmount(e.target.value)}
                placeholder="Monto a acreditar"
                className="flex-1 bg-slate-700 text-white rounded-lg px-3 py-2"
                disabled={depositing}
              />
              <button
                type="submit"
                disabled={depositing}
                className="bg-emerald-500 hover:bg-emerald-600 disabled:opacity-50 text-white font-semibold rounded-lg px-5 py-2 transition-colors"
              >
                {depositing ? "Cargando..." : "Depositar"}
              </button>
            </form>
          </div>

          {/* Historial de movimientos */}
          <div className="bg-slate-800 rounded-xl p-6">
            <h2 className="text-lg font-semibold text-white mb-4">Historial de movimientos</h2>
            {transactions.length === 0 ? (
              <p className="text-slate-400">Todavía no hay movimientos.</p>
            ) : (
              <div className="divide-y divide-slate-700">
                {transactions.map((tx) => {
                  const info = typeInfo[tx.type] ?? { label: tx.type, color: "text-slate-300", sign: "" };
                  return (
                    <div key={tx.id} className="flex items-center justify-between py-3">
                      <div>
                        <p className="text-slate-200">{info.label}</p>
                        <p className="text-xs text-slate-500">
                          {new Date(tx.createdAt).toLocaleString("es-AR", {
                            day: "2-digit",
                            month: "2-digit",
                            year: "numeric",
                            hour: "2-digit",
                            minute: "2-digit",
                            hour12: false,
                          })}
                          {tx.auctionId ? ` · Subasta #${tx.auctionId}` : ""}
                        </p>
                      </div>
                      <span className={`font-semibold ${info.color}`}>
                        {info.sign}${tx.amount.toLocaleString("es-AR")}
                      </span>
                    </div>
                  );
                })}
              </div>
            )}
          </div>
        </>
      )}
    </div>
  );
}

function MetricCard({ label, value, color }) {
  return (
    <div className="bg-slate-800 rounded-xl p-5">
      <p className="text-sm text-slate-400 mb-1">{label}</p>
      <p className={`text-2xl font-bold ${color}`}>
        ${value.toLocaleString("es-AR")}
      </p>
    </div>
  );
}
