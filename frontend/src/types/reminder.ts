export interface Reminder {
  id: string
  applicationId: string
  title: string
  description: string | null
  dueDateUtc: string
  isCompleted: boolean
  completedAtUtc: string | null
}

export interface ReminderRequest {
  title: string
  description?: string
  dueDateUtc: string
}
