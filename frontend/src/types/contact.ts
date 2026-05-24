export interface Contact {
  id: string
  applicationId: string
  name: string
  role: string | null
  email: string | null
  phone: string | null
  linkedInUrl: string | null
}

export interface ContactRequest {
  name: string
  role?: string
  email?: string
  phone?: string
  linkedInUrl?: string
}
