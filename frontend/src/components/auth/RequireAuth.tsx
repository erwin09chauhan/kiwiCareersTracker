import { useEffect, useState } from "react"
import { Navigate, Outlet } from "react-router-dom"
import axios from "axios"
import { useAuthStore } from "@/store/authStore"
import type { AuthResult } from "@/types/auth"

export function RequireAuth() {
  const accessToken = useAuthStore((s) => s.accessToken)
  const refreshToken = useAuthStore((s) => s.refreshToken)
  const setAuth = useAuthStore((s) => s.setAuth)
  const clearAuth = useAuthStore((s) => s.clearAuth)
  const [checking, setChecking] = useState(!accessToken && !!refreshToken)

  useEffect(() => {
    if (accessToken || !refreshToken) return

    axios
      .post<AuthResult>(`${import.meta.env.VITE_API_URL}/auth/refresh`, { refreshToken })
      .then((res) => setAuth(res.data))
      .catch(() => clearAuth())
      .finally(() => setChecking(false))
  }, [accessToken, refreshToken, setAuth, clearAuth])

  if (checking) {
    return <div className="flex min-h-svh items-center justify-center text-sm text-muted-foreground">Loading...</div>
  }

  if (!accessToken) {
    return <Navigate to="/login" replace />
  }

  return <Outlet />
}
