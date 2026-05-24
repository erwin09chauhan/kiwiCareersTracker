import { apiClient } from "@/api/client"
import type { Contact, ContactRequest } from "@/types/contact"

export const contactsApi = {
  getAll: (applicationId: string) =>
    apiClient
      .get<Contact[]>(`/applications/${applicationId}/contacts`)
      .then((res) => res.data),

  create: (applicationId: string, data: ContactRequest) =>
    apiClient.post(`/applications/${applicationId}/contacts`, data),

  update: (applicationId: string, contactId: string, data: ContactRequest) =>
    apiClient.put(`/applications/${applicationId}/contacts/${contactId}`, data),

  delete: (applicationId: string, contactId: string) =>
    apiClient.delete(`/applications/${applicationId}/contacts/${contactId}`),
}
