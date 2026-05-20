import { apiClient } from "@/api/client"
import type {
  ApplicationStatus,
  CreateApplicationRequest,
  JobApplication,
  PaginatedList,
} from "@/types/application"

export interface GetApplicationsParams {
  status?: ApplicationStatus
  search?: string
  sortBy?: string
  sortOrder?: string
  cursor?: string
  limit?: number
}

export const applicationsApi = {
  getAll: (params: GetApplicationsParams) =>
    apiClient
      .get<PaginatedList<JobApplication>>("/applications", { params })
      .then((res) => res.data),

  getById: (id: string) =>
    apiClient.get<JobApplication>(`/applications/${id}`).then((res) => res.data),

  create: (data: CreateApplicationRequest) =>
    apiClient.post<{ id: string }>("/applications", data).then((res) => res.data),

  update: (id: string, data: { company: string; role: string; appliedDate: string }) =>
    apiClient.put(`/applications/${id}`, data),

  delete: (id: string) => apiClient.delete(`/applications/${id}`),

  updateStatus: (id: string, newStatus: ApplicationStatus, notes?: string) =>
    apiClient.patch(`/applications/${id}/status`, { newStatus, notes }),
}
