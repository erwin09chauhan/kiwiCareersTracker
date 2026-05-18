import type { ApplicationStatus } from "@/types/application"

const LABELS: Record<ApplicationStatus, string> = {
  Applied: "Applied",
  PhoneScreen: "Phone Screen",
  Interview: "Interview",
  Technical: "Technical",
  Offer: "Offer",
  Rejected: "Rejected",
  Accepted: "Accepted",
}

export function statusLabel(status: ApplicationStatus): string {
  return LABELS[status] ?? status
}

const BADGE_CLASSES: Record<ApplicationStatus, string> = {
  Applied: "bg-secondary text-secondary-foreground",
  PhoneScreen: "bg-secondary text-secondary-foreground",
  Interview: "bg-secondary text-secondary-foreground",
  Technical: "bg-secondary text-secondary-foreground",
  Offer: "bg-primary/10 text-primary",
  Accepted: "bg-primary/10 text-primary",
  Rejected: "bg-destructive/10 text-destructive",
}

export function statusBadgeClass(status: ApplicationStatus): string {
  return BADGE_CLASSES[status] ?? "bg-secondary text-secondary-foreground"
}
