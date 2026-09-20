import { useEffect, useState } from "react";
import { getBalance, deposit } from "../api/wallet";
import { useUser } from "../context/UserContext";

export default function Wallet() {
  const { userId } = useUser();
  const [balance, setBalance] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const [amount, setAmount] = useState("");
  const [depositing, setDepositing] = useState(false);
  const [message, setMessage] = useState(null);


  const loadBalance = () => {
    setLoading(true);
    setError(null);
    getBalance()
      .then(setBalance)
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  };

  
  useEffect(() => {
    loadBalance();
  }, [userId]);

  const handleDeposit = async (e) => {
    e.preventDefault();
    setMessage(null);

    const value = Number(amount);
    if (!value || value <= 0) {
      setMessage({ type: "error", text: "Ingresá un monto positivo." });
      return;
    }

    setDepositing(true);
    try {
      const updated = await deposit(value);
      setBalance(updated);
      setAmount("");
      setMessage({ type: "success", text: `Se acreditaron $${value.toLocaleString("es-AR")}.` });
    } catch (err) {
      setMessage({ type: "error", text: err.message });
    } finally {
      setDepositing(false);
    }
  };

  return (
    <div className="max-w-3xl mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold text-white mb-6">Mi billetera</h1>

      {loading && <p className="text-slate-400">Cargando saldo...</p>}
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
          <div className="bg-slate-800 rounded-xl p-6">
            <h2 className="text-lg font-semibold text-white mb-4">Cargar saldo</h2>
            <form onSubmit={handleDeposit} className="flex gap-3">
              <input
                type="number"
                min="1"
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

            {message && (
              <p className={`mt-3 text-sm ${message.type === "success" ? "text-emerald-400" : "text-rose-400"}`}>
                {message.text}
              </p>
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
