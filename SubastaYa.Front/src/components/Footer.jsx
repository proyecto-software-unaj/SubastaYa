export default function Footer() {
  const year = new Date().getFullYear();

  return (
    <footer className="bg-slate-800 border-t border-slate-700 mt-12">
      <div className="max-w-7xl mx-auto px-4 py-5 text-center">
        <p className="text-sm text-slate-400">
          © {year} SubastaYa. Todos los derechos reservados. Desarrollado por{" "}
          <a
            href="https://github.com/proyecto-software-unaj/SubastaYa"
            target="_blank"
            rel="noopener noreferrer"
            className="text-emerald-400 hover:text-emerald-300 hover:underline transition-colors"
          >
            Cristian Ponce
          </a>
          .
        </p>
      </div>
    </footer>
  );
}
