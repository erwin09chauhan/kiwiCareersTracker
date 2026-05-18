import { Routes, Route, Navigate } from "react-router-dom";
import { AuthLayout } from "@/components/layout/AuthLayout";
import { AppLayout } from "@/components/layout/AppLayout";
import { LoginPage } from "@/features/auth/LoginPage";
import { RegisterPage } from "@/features/auth/RegisterPage";
import { DashboardPage } from "@/features/dashboard/DashboardPage";
import { ApplicationsListPage } from "@/features/applications/ApplicationsListPage";
import { ApplicationDetailPage } from "@/features/applications/ApplicationDetailPage";
import { RemindersPage } from "@/features/reminders/RemindersPage";
import { AuditLogPage } from "@/features/audit/AuditLogPage";
import { RequireAuth } from "./components/auth/RequireAuth";

function App() {
  return (
    <Routes>
      <Route element={<AuthLayout />}>
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
      </Route>
      <Route element={<RequireAuth />}>
        <Route element={<AppLayout />}>
          <Route path="/" element={<DashboardPage />} />
          <Route path="/applications" element={<ApplicationsListPage />} />
          <Route path="/applications/:id" element={<ApplicationDetailPage />} />
          <Route path="/reminders" element={<RemindersPage />} />
          <Route path="/audit" element={<AuditLogPage />} />
        </Route>
      </Route>
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}

export default App;
