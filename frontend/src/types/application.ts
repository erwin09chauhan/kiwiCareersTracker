export const APPLICATION_STATUSES = [
  "Applied",
  "PhoneScreen",
  "Interview",
  "Technical",
  "Offer",
  "Rejected",
  "Accepted",
] as const

export type ApplicationStatus = (typeof APPLICATION_STATUSES)[number]
