export const APPLICATION_STATUSES = [
  "Applied",
  "PhoneScreen",
  "Interview",
  "Technical",
  "Offer",
  "Rejected",
  "Accepted",
] as const;

export interface JobApplication {
  id: string;
  company: string;
  role: string;
  status: ApplicationStatus;
  appliedDate: string;
  createdAtUtc: string;
  updatedAtUtc: string | null;
}

export interface PaginatedList<T> {
  items: T[];
  hasMore: boolean;
  nextCursor: string | null;
}

export interface CreateApplicationRequest {
  company: string;
  role: string;
  appliedDate: string;
}

export type ApplicationStatus = (typeof APPLICATION_STATUSES)[number];
