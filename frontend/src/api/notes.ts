import { apiClient } from "@/api/client"
import type { Note } from "@/types/note"

export const notesApi = {
  getAll: (applicationId: string) =>
    apiClient.get<Note[]>(`/applications/${applicationId}/notes`).then((res) => res.data),

  create: (applicationId: string, content: string) =>
    apiClient.post(`/applications/${applicationId}/notes`, { content }),

  update: (applicationId: string, noteId: string, content: string) =>
    apiClient.put(`/applications/${applicationId}/notes/${noteId}`, { content }),

  delete: (applicationId: string, noteId: string) =>
    apiClient.delete(`/applications/${applicationId}/notes/${noteId}`),
}
