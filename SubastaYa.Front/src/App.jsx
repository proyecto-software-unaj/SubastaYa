import { BrowserRouter, Routes, Route } from "react-router-dom";
import { Toaster } from "react-hot-toast";
import { UserProvider } from "./context/UserContext";
import Layout from "./components/Layout";
import Catalog from "./pages/Catalog";
import Wallet from "./pages/Wallet";
import Publish from "./pages/Publish";
import MyActivity from "./pages/MyActivity";
import AuctionDetail from "./pages/AuctionDetail";

function App() {
  return (
    <UserProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<Layout />}>
            <Route index element={<Catalog />} />
            <Route path="auctions/:id" element={<AuctionDetail />} />
            <Route path="wallet" element={<Wallet />} />
            <Route path="publish" element={<Publish />} />
            <Route path="my-activity" element={<MyActivity />} />
          </Route>
        </Routes>
      </BrowserRouter>

      {/* Contenedor global de notificaciones */}
      <Toaster
        position="bottom-right"
        toastOptions={{
          style: { background: "#1e293b", color: "#fff" },
          success: { iconTheme: { primary: "#10b981", secondary: "#fff" } },
          error: { iconTheme: { primary: "#f43f5e", secondary: "#fff" } },
        }}
      />
    </UserProvider>
  );
}

export default App;
