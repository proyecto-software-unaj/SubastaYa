import { useEffect, useState } from "react";

function getRemaining(endDate) {
  return new Date(endDate).getTime() - Date.now();
}

function format(ms) {
  if (ms <= 0) return "Finalizada";
  const totalSeconds = Math.floor(ms / 1000);
  const days = Math.floor(totalSeconds / 86400);
  const hours = Math.floor((totalSeconds % 86400) / 3600);
  const minutes = Math.floor((totalSeconds % 3600) / 60);
  const seconds = totalSeconds % 60;

  if (days > 0) return `${days}d ${hours}h`;
  if (hours > 0) return `${hours}h ${minutes}m`;
  return `${minutes}m ${seconds}s`;
}

export default function Countdown({ endDate }) {
  const [remaining, setRemaining] = useState(() => getRemaining(endDate));

  useEffect(() => {
    const interval = setInterval(() => {
      setRemaining(getRemaining(endDate));
    }, 1000);
    return () => clearInterval(interval);
  }, [endDate]);

  
  const isCritical = remaining > 0 && remaining <= 60_000;
  const isWarning = remaining > 60_000 && remaining <= 300_000;

  const color = isCritical
    ? "text-red-500"
    : isWarning
    ? "text-amber-400"
    : "text-slate-300";

  return <span className={`font-mono font-semibold ${color}`}>{format(remaining)}</span>;
}
