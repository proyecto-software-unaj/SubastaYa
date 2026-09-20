import { api } from "./client";

export function getAuctions({ status, categoryId, sort } = {}) {
  const params = new URLSearchParams();
  if (status) params.append("status", status);
  if (categoryId) params.append("categoryId", categoryId);
  if (sort) params.append("sort", sort);

  const query = params.toString();
  return api.get(`/api/auctions${query ? `?${query}` : ""}`);
}

export function getAuctionById(id) {
  return api.get(`/api/auctions/${id}`);
}

export function getCategories() {
  return api.get("/api/categories");
}

export function placeBid(auctionId, amount) {
  return api.post(`/api/auctions/${auctionId}/bids`, { amount });
}
