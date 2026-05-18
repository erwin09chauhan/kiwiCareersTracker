import { apiClient } from "@/api/client"
import type { ActivityItem, DashboardSummary } from "@/types/dashboard"

export const dashboardApi = {
  getSummary: () =>
    apiClient.get<DashboardSummary>("/dashboard/summary").then((res) => res.data),

  getActivity: (limit = 20) =>
    apiClient
      .get<ActivityItem[]>("/dashboard/activity", { params: { limit } })
      .then((res) => res.data),
}
