import { Outlet } from "react-router-dom"

export function AuthLayout() {
  return (
    <div className="flex min-h-svh items-center justify-center bg-background p-4">
      <div className="w-full max-w-sm">
        <h1 className="mb-6 text-center text-lg font-semibold tracking-wide uppercase">
          Kiwi Careers Tracker
        </h1>
        <Outlet />
      </div>
    </div>
  )
}
