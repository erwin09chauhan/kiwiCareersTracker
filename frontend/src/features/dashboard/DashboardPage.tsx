import { useQuery } from "@tanstack/react-query";
import { dashboardApi } from "@/api/dashboard";
import { Card, CardHeader, CardTitle, CardContent } from "@/components/ui/card";
import { APPLICATION_STATUSES } from "@/types/application";
import { statusLabel } from "@/lib/status";
import { formatDateTime } from "@/lib/date";

export function DashboardPage() {
  const summaryQuery = useQuery({
    queryKey: ["dashboard", "summary"],
    queryFn: dashboardApi.getSummary,
  });

  const activityQuery = useQuery({
    queryKey: ["dashboard", "activity"],
    queryFn: () => dashboardApi.getActivity(20),
  });

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-2xl font-semibold tracking-tight">Dashboard</h2>
        <p className="text-sm text-muted-foreground">
          {summaryQuery.data
            ? `${summaryQuery.data.total} total application${summaryQuery.data.total === 1 ? "" : "s"}`
            : "Overview of your job applications"}
        </p>
      </div>

      <div className="grid grid-cols-2 gap-4 sm:grid-cols-3 lg:grid-cols-4">
        {APPLICATION_STATUSES.map((status) => (
          <Card key={status}>
            <CardHeader className="pb-2">
              <CardTitle className="text-sm font-medium text-muted-foreground">
                {statusLabel(status)}
              </CardTitle>
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-semibold">
                {summaryQuery.isLoading
                  ? "-"
                  : (summaryQuery.data?.countsByStatus[status] ?? 0)}
              </div>
            </CardContent>
          </Card>
        ))}
      </div>

      <Card>
        <CardHeader>
          <CardTitle className="text-base">Recent Activity</CardTitle>
        </CardHeader>
        <CardContent>
          {activityQuery.isLoading && (
            <p className="text-sm text-muted-foreground">Loading...</p>
          )}
          {activityQuery.data?.length === 0 && (
            <p className="text-sm text-muted-foreground">No recent activity</p>
          )}
          <ul className="divide-y">
            {activityQuery.data?.map((item, idx) => (
              <li
                key={idx}
                className="flex items-start justify-between gap-4 py-3"
              >
                <div>
                  <p className="text-sm font-medium">{item.title}</p>
                  {item.detail && (
                    <p className="text-sm text-muted-foreground">
                      {item.detail}
                    </p>
                  )}
                </div>
                <span className="shrink-0 text-xs text-muted-foreground">
                  {formatDateTime(item.createdAtUtc)}
                </span>
              </li>
            ))}
          </ul>
        </CardContent>
      </Card>
    </div>
  );
}
