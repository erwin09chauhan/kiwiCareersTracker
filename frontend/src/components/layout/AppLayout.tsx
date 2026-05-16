import { Outlet, Link } from "react-router-dom"

export function AppLayout() {
  return (
    <div className="min-h-svh bg-background">
      <header className="border-b">
        <div className="mx-auto flex h-14 max-w-6xl items-center justify-between px-4">
          <nav className="flex items-center gap-6 text-sm font-medium">
            <Link to="/" className="font-semibold">Kiwi Careers</Link>
            <Link to="/applications">Applications</Link>
            <Link to="/reminders">Reminders</Link>
            <Link to="/audit">Audit Log</Link>
          </nav>
        </div>
      </header>
      <main className="mx-auto max-w-6xl p-4">
        <Outlet />
      </main>
    </div>
  )
}
