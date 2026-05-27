import { useState } from "react";
import { Outlet, NavLink, useNavigate } from "react-router-dom";
import { LogOut, Menu, X } from "lucide-react";
import { cn } from "@/lib/utils";
import { useAuthStore } from "@/store/authStore";
import { authApi } from "@/api/auth";
import { Button } from "@/components/ui/button";
import { NotificationsDropdown } from "./NotificationsDropdown";

const navItems = [
  { to: "/", label: "Dashboard", end: true },
  { to: "/applications", label: "Applications" },
  { to: "/reminders", label: "Reminders" },
  { to: "/audit", label: "Audit Log" },
];

export function AppLayout() {
  const [open, setOpen] = useState(false);
  const navigate = useNavigate();
  const email = useAuthStore((s) => s.email);
  const clearAuth = useAuthStore((s) => s.clearAuth);

  const handleLogout = async () => {
    try {
      await authApi.logout();
    } catch {
      // ignore
    }
    clearAuth();
    navigate("/login");
  };

  const navContent = (
    <>
      <div className="border-b px-6 py-5">
        <h1 className="text-sm font-semibold tracking-wide uppercase">
          Kiwi Careers
        </h1>
        {email && (
          <p className="mt-0.5 truncate text-xs text-muted-foreground">
            {email}
          </p>
        )}
      </div>

      <nav className="flex-1 space-y-0.5 px-3 py-4">
        {navItems.map((item) => (
          <NavLink
            key={item.to}
            to={item.to}
            end={item.end}
            onClick={() => setOpen(false)}
            className={({ isActive }) =>
              cn(
                "block rounded-md px-3 py-2 text-sm transition-colors",
                isActive
                  ? "bg-secondary font-medium text-foreground"
                  : "text-muted-foreground hover:bg-secondary hover:text-foreground",
              )
            }
          >
            {item.label}
          </NavLink>
        ))}
      </nav>

      <div className="border-t px-3 py-4">
        <Button
          variant="ghost"
          className="w-full justify-start gap-2 text-muted-foreground"
          onClick={handleLogout}
        >
          <LogOut className="size-4" />
          Sign out
        </Button>
      </div>
    </>
  );

  return (
    <div className="flex min-h-svh bg-background">
      {/* Mobile hamburger */}
      <button
        onClick={() => setOpen(true)}
        className="fixed top-4 left-4 z-50 rounded-md border bg-background p-2 shadow-sm md:hidden"
      >
        <Menu className="size-4" />
      </button>

      {/* Mobile overlay */}
      {open && (
        <div
          className="fixed inset-0 z-40 bg-black/40 md:hidden"
          onClick={() => setOpen(false)}
        />
      )}

      {/* Mobile drawer */}
      <aside
        className={cn(
          "fixed top-0 left-0 z-50 flex h-full w-64 flex-col border-r bg-background transition-transform duration-200 md:hidden",
          open ? "translate-x-0" : "-translate-x-full",
        )}
      >
        <button
          onClick={() => setOpen(false)}
          className="absolute top-4 right-4 rounded-md p-1 hover:bg-secondary"
        >
          <X className="size-4" />
        </button>
        {navContent}
      </aside>

      {/* Desktop sidebar */}
      <aside className="hidden w-64 flex-col border-r md:flex">
        {navContent}
      </aside>

      {/* Main content */}
      <div className="flex-1">
        <header className="flex h-14 items-center justify-end border-b px-4 md:px-6">
          <NotificationsDropdown />
        </header>
        <main className="p-4 md:p-6">
          <Outlet />
        </main>
      </div>
    </div>
  );
}
