import axios from "axios"
import { useAuthStore } from "@/store/authStore"
import type { AuthResult } from "@/types/auth"

export const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
})

apiClient.interceptors.request.use((config) => {
  const token = useAuthStore.getState().accessToken
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

let refreshPromise: Promise<string> | null = null

async function refreshAccessToken(): Promise<string> {
  const { refreshToken, setAuth, clearAuth } = useAuthStore.getState()
  if (!refreshToken) {
    clearAuth()
    throw new Error("No refresh token")
  }

  try {
    const response = await axios.post<AuthResult>(
      `${import.meta.env.VITE_API_URL}/auth/refresh`,
      { refreshToken },
    )
    setAuth(response.data)
    return response.data.accessToken
  } catch (err) {
    clearAuth()
    throw err
  }
}

apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config

    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true

      try {
        refreshPromise ??= refreshAccessToken()
        const newToken = await refreshPromise
        refreshPromise = null

        originalRequest.headers.Authorization = `Bearer ${newToken}`
        return apiClient(originalRequest)
      } catch (refreshError) {
        refreshPromise = null
        window.location.href = "/login"
        return Promise.reject(refreshError)
      }
    }

    return Promise.reject(error)
  },
)
