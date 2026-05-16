import { create } from "zustand"
import { persist } from "zustand/middleware"
import type { AuthResult } from "@/types/auth"

interface AuthState {
  accessToken: string | null
  refreshToken: string | null
  email: string | null
  userId: string | null
  setAuth: (result: AuthResult) => void
  clearAuth: () => void
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      accessToken: null,
      refreshToken: null,
      email: null,
      userId: null,
      setAuth: (result) =>
        set({
          accessToken: result.accessToken,
          refreshToken: result.refreshToken,
          email: result.email,
          userId: result.userId,
        }),
      clearAuth: () =>
        set({ accessToken: null, refreshToken: null, email: null, userId: null }),
    }),
    {
      name: "auth-storage",
      partialize: (state) => ({
        refreshToken: state.refreshToken,
        email: state.email,
        userId: state.userId,
      }),
    },
  ),
)
