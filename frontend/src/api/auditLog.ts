import { apiClient } from "@/api/client";
import type { AuditLogEntry } from "@/types/auditLog";
import type { PaginatedList } from "@/types/application";

export const auditLogApi = {
  getForApplication: (applicationId: string) =>
    apiClient
      .get<AuditLogEntry[]>(`/applications/${applicationId}/audit`)
      .then((res) => res.data),

  getAll: (cursor?: string, limit = 20) =>
    apiClient
      .get<
        PaginatedList<AuditLogEntry>
      >("/audit", { params: { cursor, limit } })
      .then((res) => res.data),
};
