import { useQuery } from "@tanstack/react-query"
import { Link } from "react-router-dom"
import { remindersApi } from "@/api/reminders"
import { Card, CardContent } from "@/components/ui/card"
import { formatDateTime } from "@/lib/date"

export function RemindersPage() {
  const query = useQuery({
    queryKey: ["reminders", "upcoming"],
    queryFn: () => remindersApi.getUpcoming(),
  })

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-2xl font-semibold tracking-tight">Upcoming Reminders</h2>
        <p className="text-muted-foreground">Reminders due across all your applications.</p>
      </div>

      {query.isLoading && <p className="text-sm text-muted-foreground">Loading...</p>}
      {!query.isLoading && (query.data?.length ?? 0) === 0 && (
        <p className="text-sm text-muted-foreground">No upcoming reminders.</p>
      )}

      <div className="space-y-3">
        {query.data?.map((reminder) => (
          <Link key={reminder.id} to={`/applications/${reminder.applicationId}`}>
            <Card className="transition-colors hover:bg-accent">
              <CardContent className="space-y-1 pt-4">
                <p className="text-sm font-medium">{reminder.title}</p>
                {reminder.description && (
                  <p className="text-sm text-muted-foreground whitespace-pre-wrap">
                    {reminder.description}
                  </p>
                )}
                <p className="text-xs text-muted-foreground">
                  Due {formatDateTime(reminder.dueDateUtc)}
                </p>
              </CardContent>
            </Card>
          </Link>
        ))}
      </div>
    </div>
  )
}
