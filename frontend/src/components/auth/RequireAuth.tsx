import { useEffect, useRef } from "react";
import { Navigate, Outlet } from "react-router-dom";
import axios from "axios";
import { useAuthStore } from "@/store/authStore";
import type { AuthResult } from "@/types/auth";
import { LoadingState } from "@/components/common/LoadingState";

export function RequireAuth() {
  const accessToken = useAuthStore((s) => s.accessToken);
  const refreshToken = useAuthStore((s) => s.refreshToken);
  const setAuth = useAuthStore((s) => s.setAuth);
  const clearAuth = useAuthStore((s) => s.clearAuth);
  const hasHydrated = useAuthStore((s) => s.hasHydrated);
  const refreshStarted = useRef(false);

  const needsRefresh = hasHydrated && !accessToken && !!refreshToken;

  useEffect(() => {
    if (!needsRefresh || refreshStarted.current) return;
    refreshStarted.current = true;

    axios
      .post<AuthResult>(`${import.meta.env.VITE_API_URL}/auth/refresh`, {
        refreshToken,
      })
      .then((res) => setAuth(res.data))
      .catch(() => clearAuth());
  }, [needsRefresh, refreshToken, setAuth, clearAuth]);

  if (!hasHydrated || needsRefresh) {
    return (
      <div className="flex min-h-svh items-center justify-center">
        <LoadingState />
      </div>
    );
  }

  if (!accessToken) {
    return <Navigate to="/login" replace />;
  }

  return <Outlet />;
}
