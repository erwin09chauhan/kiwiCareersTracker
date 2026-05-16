import { apiClient } from "@/api/client"
import type { AuthResult, CurrentUser, LoginRequest, RegisterRequest } from "@/types/auth"

export const authApi = {
  register: (data: RegisterRequest) =>
    apiClient.post<AuthResult>("/auth/register", data).then((res) => res.data),

  login: (data: LoginRequest) =>
    apiClient.post<AuthResult>("/auth/login", data).then((res) => res.data),

  logout: () => apiClient.post("/auth/logout"),

  me: () => apiClient.get<CurrentUser>("/auth/me").then((res) => res.data),
}
