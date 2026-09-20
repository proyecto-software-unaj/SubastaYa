import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { createAuction, getCategories } from "../api/auctions";

function nowForInput() {
  const now = new Date();
  now.setMinutes(now.getMinutes() - now.getTimezoneOffset());
  return now.toISOString().slice(0, 16);
}

export default function Publish() {
  const navigate = useNavigate();
  const [categories, setCategories] = useState([]);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState(null);

  const [form, setForm] = useState({
    title: "",
    description: "",
    imageUrl: "",
    categoryId: "",
    basePrice: "",
    minimumIncrement: "",
    startDate: "",
    endDate: "",
  });

  useEffect(() => {
    getCategories().then(setCategories).catch(() => {});
  }, []);

  const update = (field) => (e) => {
    setForm({ ...form, [field]: e.target.value });
  };

  
  const validate = () => {
    if (!form.title.trim()) return "El título es obligatorio.";
    if (!form.categoryId) return "Elegí una categoría.";
    if (!form.imageUrl.trim()) return "La URL de imagen es obligatoria.";

    const basePrice = Number(form.basePrice);
    const increment = Number(form.minimumIncrement);
    if (!basePrice || basePrice <= 0) return "El precio base debe ser positivo.";
    if (!increment || increment <= 0) return "El incremento mínimo debe ser positivo.";

    if (!form.startDate || !form.endDate) return "Completá las fechas de inicio y fin.";

    const now = new Date();
    now.setMinutes(now.getMinutes() - 1);
    if (new Date(form.startDate) < now)
      return "La fecha de inicio no puede ser en el pasado.";

    if (new Date(form.endDate) <= new Date(form.startDate))
      return "La fecha de fin debe ser posterior a la de inicio.";

    return null;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);

    const validationError = validate();
    if (validationError) {
      setError(validationError);
      return;
    }

    setSubmitting(true);
    try {
      const payload = {
        title: form.title.trim(),
        description: form.description.trim(),
        imageUrl: form.imageUrl.trim(),
        categoryId: Number(form.categoryId),
        basePrice: Number(form.basePrice),
        minimumIncrement: Number(form.minimumIncrement),
        startDate: new Date(form.startDate).toISOString(),
        endDate: new Date(form.endDate).toISOString(),
      };

      const created = await createAuction(payload);
      navigate(`/auctions/${created.id}`);
    } catch (err) {
      setError(err.message);
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="max-w-2xl mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold text-white mb-6">Publicar subasta</h1>

      <form onSubmit={handleSubmit} className="bg-slate-800 rounded-xl p-6 space-y-4">
        <Field label="Título">
          <input type="text" value={form.title} onChange={update("title")}
            className="w-full bg-slate-700 text-white rounded-lg px-3 py-2" />
        </Field>

        <Field label="Descripción">
          <textarea value={form.description} onChange={update("description")} rows={3}
            className="w-full bg-slate-700 text-white rounded-lg px-3 py-2" />
        </Field>

        <Field label="URL de imagen">
          <input type="url" value={form.imageUrl} onChange={update("imageUrl")}
            placeholder="https://..."
            className="w-full bg-slate-700 text-white rounded-lg px-3 py-2" />
        </Field>

        <Field label="Categoría">
          <select value={form.categoryId} onChange={update("categoryId")}
            className="w-full bg-slate-700 text-white rounded-lg px-3 py-2">
            <option value="">Elegí una categoría</option>
            {categories.map((c) => (
              <option key={c.id} value={c.id}>{c.name}</option>
            ))}
          </select>
        </Field>

        <div className="grid grid-cols-2 gap-4">
          <Field label="Precio base">
            <input type="number" min="0" step="0.01" value={form.basePrice} onChange={update("basePrice")}
              className="w-full bg-slate-700 text-white rounded-lg px-3 py-2" />
          </Field>
          <Field label="Incremento mínimo">
            <input type="number" min="0" step="0.01" value={form.minimumIncrement} onChange={update("minimumIncrement")}
              className="w-full bg-slate-700 text-white rounded-lg px-3 py-2" />
          </Field>
        </div>

        <div className="grid grid-cols-2 gap-4">
          <Field label="Inicio">
            <input type="datetime-local" value={form.startDate} onChange={update("startDate")}
              min={nowForInput()}
              className="w-full bg-slate-700 text-white rounded-lg px-3 py-2" />
          </Field>
          <Field label="Fin">
            <input type="datetime-local" value={form.endDate} onChange={update("endDate")}
              min={form.startDate || nowForInput()}
              className="w-full bg-slate-700 text-white rounded-lg px-3 py-2" />
          </Field>
        </div>

        {error && <p className="text-rose-400 text-sm">{error}</p>}

        <button type="submit" disabled={submitting}
          className="w-full bg-emerald-500 hover:bg-emerald-600 disabled:opacity-50 text-white font-semibold rounded-lg py-2 transition-colors">
          {submitting ? "Publicando..." : "Publicar subasta"}
        </button>
      </form>
    </div>
  );
}

function Field({ label, children }) {
  return (
    <div>
      <label className="block text-sm text-slate-400 mb-1">{label}</label>
      {children}
    </div>
  );
}
