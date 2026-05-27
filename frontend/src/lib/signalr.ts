import * as signalR from "@microsoft/signalr"
import { useAuthStore } from "@/store/authStore"

let connection: signalR.HubConnection | null = null

export function getNotificationsConnection() {
  if (connection) return connection

  connection = new signalR.HubConnectionBuilder()
    .withUrl(`${import.meta.env.VITE_API_URL.replace("/api/v1", "")}/hubs/notifications`, {
      accessTokenFactory: () => useAuthStore.getState().accessToken ?? "",
    })
    .withAutomaticReconnect()
    .build()

  return connection
}
