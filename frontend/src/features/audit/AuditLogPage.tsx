import { useInfiniteQuery } from "@tanstack/react-query"
import { Link } from "react-router-dom"
import { auditLogApi } from "@/api/auditLog"
import { statusLabel } from "@/lib/status"
import { formatDateTime } from "@/lib/date"
import { Button } from "@/components/ui/button"
import { Card, CardContent } from "@/components/ui/card"

export function AuditLogPage() {
  const query = useInfiniteQuery({
    queryKey: ["audit"],
    queryFn: ({ pageParam }) => auditLogApi.getAll(pageParam, 20),
    initialPageParam: undefined as string | undefined,
    getNextPageParam: (lastPage) => lastPage.nextCursor ?? undefined,
  })

  const entries = query.data?.pages.flatMap((p) => p.items) ?? []

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-2xl font-semibold tracking-tight">Audit Log</h2>
        <p className="text-muted-foreground">History of status changes across all applications.</p>
      </div>

      {query.isLoading && <p className="text-sm text-muted-foreground">Loading...</p>}
      {!query.isLoading && entries.length === 0 && (
        <p className="text-sm text-muted-foreground">No history yet.</p>
      )}

      <div className="space-y-3">
        {entries.map((entry) => (
          <Link key={entry.id} to={`/applications/${entry.applicationId}`}>
            <Card className="transition-colors hover:bg-accent">
              <CardContent className="space-y-1 pt-4">
                <p className="text-sm">
                  {entry.fromStatus ? (
                    <>
                      Status changed from{" "}
                      <span className="font-medium">{statusLabel(entry.fromStatus)}</span> to{" "}
                      <span className="font-medium">{statusLabel(entry.toStatus)}</span>
                    </>
                  ) : (
                    <>
                      Application created with status{" "}
                      <span className="font-medium">{statusLabel(entry.toStatus)}</span>
                    </>
                  )}
                </p>
                {entry.notes && (
                  <p className="text-sm text-muted-foreground whitespace-pre-wrap">{entry.notes}</p>
                )}
                <p className="text-xs text-muted-foreground">
                  {formatDateTime(entry.createdAtUtc)}
                </p>
              </CardContent>
            </Card>
          </Link>
        ))}
      </div>

      {query.hasNextPage && (
        <div className="flex justify-center">
          <Button
            variant="outline"
            onClick={() => query.fetchNextPage()}
            disabled={query.isFetchingNextPage}
          >
            {query.isFetchingNextPage ? "Loading..." : "Load more"}
          </Button>
        </div>
      )}
    </div>
  )
}
