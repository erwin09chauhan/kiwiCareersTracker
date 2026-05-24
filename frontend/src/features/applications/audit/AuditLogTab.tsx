import { useQuery } from "@tanstack/react-query";
import { auditLogApi } from "@/api/auditLog";
import { statusLabel } from "@/lib/status";
import { Card, CardContent } from "@/components/ui/card";

export function AuditLogTab({ applicationId }: { applicationId: string }) {
  const query = useQuery({
    queryKey: ["applications", applicationId, "audit"],
    queryFn: () => auditLogApi.getForApplication(applicationId),
  });

  if (query.isLoading) {
    return <p className="text-sm text-muted-foreground">Loading...</p>;
  }

  if (!query.data || query.data.length === 0) {
    return <p className="text-sm text-muted-foreground">No history yet.</p>;
  }

  return (
    <div className="space-y-3">
      {query.data.map((entry) => (
        <Card key={entry.id}>
          <CardContent className="space-y-1 pt-4">
            <p className="text-sm">
              {entry.fromStatus ? (
                <>
                  Status changed from{" "}
                  <span className="font-medium">
                    {statusLabel(entry.fromStatus)}
                  </span>{" "}
                  to{" "}
                  <span className="font-medium">
                    {statusLabel(entry.toStatus)}
                  </span>
                </>
              ) : (
                <>
                  Application created with status{" "}
                  <span className="font-medium">
                    {statusLabel(entry.toStatus)}
                  </span>
                </>
              )}
            </p>
            {entry.notes && (
              <p className="text-sm text-muted-foreground whitespace-pre-wrap">
                {entry.notes}
              </p>
            )}
            <p className="text-xs text-muted-foreground">
              {new Date(entry.createdAtUtc).toLocaleString()}
            </p>
          </CardContent>
        </Card>
      ))}
    </div>
  );
}
