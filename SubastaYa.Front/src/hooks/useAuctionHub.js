import { useEffect, useRef } from "react";
import * as signalR from "@microsoft/signalr";

const HUB_URL = (import.meta.env.VITE_API_URL ?? "http://localhost:5269") + "/hubs/auctions";

/**
 * Conecta al hub de SignalR y se une al grupo de una subasta.
 * @param auctionId  id de la subasta (grupo al que unirse)
 * @param handlers   { onBidPlaced, onAuctionExtended } callbacks para los eventos
 */
export function useAuctionHub(auctionId, handlers) {
  
  const handlersRef = useRef(handlers);
  handlersRef.current = handlers;

  useEffect(() => {
    if (!auctionId) return;

    const connection = new signalR.HubConnectionBuilder()
      .withUrl(HUB_URL)
      .withAutomaticReconnect()
      .build();

    
    connection.on("BidPlaced", (data) => {
      handlersRef.current.onBidPlaced?.(data);
    });

    connection.on("AuctionExtended", (data) => {
      handlersRef.current.onAuctionExtended?.(data);
    });

    
    connection
      .start()
      .then(() => connection.invoke("JoinAuction", Number(auctionId)))
      .catch((err) => console.error("Error conectando al hub:", err));

    
    return () => {
      connection.stop();
    };
  }, [auctionId]);
}
