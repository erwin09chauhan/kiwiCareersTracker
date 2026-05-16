export interface AuthResult {
  userId: string
  email: string
  accessToken: string
  refreshToken: string
}

export interface LoginRequest {
  email: string
  password: string
}

export interface RegisterRequest {
  email: string
  password: string
}

export interface CurrentUser {
  userId: string
  email: string
}
