import { apiClient } from "@/api/client"
import type { Notification } from "@/types/notification"
import type { PaginatedList } from "@/types/application"

export const notificationsApi = {
  getAll: (cursor?: string, limit = 20) =>
    apiClient
      .get<PaginatedList<Notification>>("/notifications", { params: { cursor, limit } })
      .then((res) => res.data),

  markRead: (id: string) => apiClient.patch(`/notifications/${id}/read`),

  markAllRead: () => apiClient.patch("/notifications/read-all"),

  delete: (id: string) => apiClient.delete(`/notifications/${id}`),
}
