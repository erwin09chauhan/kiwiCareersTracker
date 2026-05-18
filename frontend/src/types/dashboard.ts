import type { ApplicationStatus } from "@/types/application"

export interface DashboardSummary {
  countsByStatus: Record<ApplicationStatus, number>
  total: number
}

export interface ActivityItem {
  type: "StatusChange" | "Notification"
  applicationId: string | null
  title: string
  detail: string | null
  createdAtUtc: string
}
