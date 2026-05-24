import type { ApplicationStatus } from "@/types/application";

export interface AuditLogEntry {
  id: string;
  applicationId: string;
  fromStatus: ApplicationStatus | null;
  toStatus: ApplicationStatus;
  notes: string | null;
  changedByUserId: string;
  createdAtUtc: string;
}
