export interface Notification {
  id: string
  title: string
  message: string
  isRead: boolean
  readAtUtc: string | null
  relatedApplicationId: string | null
  createdAtUtc: string
}
