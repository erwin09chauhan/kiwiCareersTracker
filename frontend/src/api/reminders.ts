import { apiClient } from "@/api/client"
import type { Reminder, ReminderRequest } from "@/types/reminder"

export const remindersApi = {
  getAll: (applicationId: string) =>
    apiClient
      .get<Reminder[]>(`/applications/${applicationId}/reminders`)
      .then((res) => res.data),

  getUpcoming: () =>
    apiClient.get<Reminder[]>("/reminders/upcoming").then((res) => res.data),

  create: (applicationId: string, data: ReminderRequest) =>
    apiClient.post(`/applications/${applicationId}/reminders`, data),

  update: (applicationId: string, reminderId: string, data: ReminderRequest) =>
    apiClient.put(`/applications/${applicationId}/reminders/${reminderId}`, data),

  delete: (applicationId: string, reminderId: string) =>
    apiClient.delete(`/applications/${applicationId}/reminders/${reminderId}`),

  complete: (applicationId: string, reminderId: string) =>
    apiClient.patch(`/applications/${applicationId}/reminders/${reminderId}/complete`),
}
