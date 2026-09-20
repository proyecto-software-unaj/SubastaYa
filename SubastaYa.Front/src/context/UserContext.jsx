import { createContext, useContext, useState } from "react";
import { setCurrentUserId, getCurrentUserId } from "../api/client";

const UserContext = createContext(null);

export const SEED_USERS = [
  { id: 1, name: "Vendedor", email: "vendedor@test.com" },
  { id: 2, name: "Comprador 1", email: "comprador1@test.com" },
  { id: 3, name: "Comprador 2", email: "comprador2@test.com" },
  { id: 4, name: "Sin Fondos", email: "sinfondos@test.com" },
];

export function UserProvider({ children }) {
  const [userId, setUserId] = useState(getCurrentUserId());

  const changeUser = (id) => {
    const numericId = Number(id);
    setCurrentUserId(numericId); 
    setUserId(numericId);
  };

  return (
    <UserContext.Provider value={{ userId, changeUser, users: SEED_USERS }}>
      {children}
    </UserContext.Provider>
  );
}

export function useUser() {
  return useContext(UserContext);
}
