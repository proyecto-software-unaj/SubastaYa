import { api } from "./client";

export function getBalance() {
  return api.get("/api/wallet/balance");
}

export function deposit(amount) {
  return api.post("/api/wallet/deposit", { amount });
}

export function getTransactions() {
  return api.get("/api/wallet/transactions");
}
