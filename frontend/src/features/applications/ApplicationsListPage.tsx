import { useState } from "react"
import { useNavigate } from "react-router-dom"
import { useInfiniteQuery, useMutation, useQueryClient } from "@tanstack/react-query"
import { toast } from "sonner"
import { applicationsApi } from "@/api/applications"
import { APPLICATION_STATUSES, type ApplicationStatus } from "@/types/application"
import { statusLabel, statusBadgeClass } from "@/lib/status"
import { formatDate } from "@/lib/date"
import { AddApplicationDialog } from "@/features/applications/AddApplicationDialog"
import { Input } from "@/components/ui/input"
import { Button } from "@/components/ui/button"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import {
  Table,
  TableHeader,
  TableBody,
  TableRow,
  TableHead,
  TableCell,
} from "@/components/ui/table"

const ALL_STATUSES = "all"

export function ApplicationsListPage() {
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const [search, setSearch] = useState("")
  const [status, setStatus] = useState<ApplicationStatus | typeof ALL_STATUSES>(ALL_STATUSES)
  const [sortBy, setSortBy] = useState("appliedDate")
  const [sortOrder, setSortOrder] = useState<"asc" | "desc">("desc")

  const query = useInfiniteQuery({
    queryKey: ["applications", { search, status, sortBy, sortOrder }],
    queryFn: ({ pageParam }) =>
      applicationsApi.getAll({
        search: search || undefined,
        status: status === ALL_STATUSES ? undefined : status,
        sortBy,
        sortOrder,
        cursor: pageParam,
        limit: 20,
      }),
    initialPageParam: undefined as string | undefined,
    getNextPageParam: (lastPage) => lastPage.nextCursor ?? undefined,
  })

  const statusMutation = useMutation({
    mutationFn: ({ id, newStatus }: { id: string; newStatus: ApplicationStatus }) =>
      applicationsApi.updateStatus(id, newStatus),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["applications"] })
      queryClient.invalidateQueries({ queryKey: ["dashboard"] })
    },
    onError: () => toast.error("Failed to update status"),
  })

  const applications = query.data?.pages.flatMap((p) => p.items) ?? []

  const toggleSort = (field: string) => {
    if (sortBy === field) {
      setSortOrder((o) => (o === "asc" ? "desc" : "asc"))
    } else {
      setSortBy(field)
      setSortOrder("asc")
    }
  }

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <h2 className="text-2xl font-semibold tracking-tight">Applications</h2>
        <AddApplicationDialog />
      </div>

      <div className="flex flex-col gap-2 sm:flex-row sm:items-center">
        <Input
          placeholder="Search company or role..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          className="sm:max-w-xs"
        />
        <Select
          value={status}
          onValueChange={(v) => setStatus(v as ApplicationStatus | typeof ALL_STATUSES)}
        >
          <SelectTrigger className="sm:w-48">
            <SelectValue placeholder="All statuses" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value={ALL_STATUSES}>All statuses</SelectItem>
            {APPLICATION_STATUSES.map((s) => (
              <SelectItem key={s} value={s}>
                {statusLabel(s)}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
      </div>

      <div className="rounded-md border overflow-x-auto">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead className="cursor-pointer" onClick={() => toggleSort("company")}>
                Company
              </TableHead>
              <TableHead>Role</TableHead>
              <TableHead className="cursor-pointer" onClick={() => toggleSort("status")}>
                Status
              </TableHead>
              <TableHead className="cursor-pointer" onClick={() => toggleSort("appliedDate")}>
                Applied Date
              </TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {applications.map((app) => (
              <TableRow
                key={app.id}
                className="cursor-pointer"
                onClick={() => navigate(`/applications/${app.id}`)}
              >
                <TableCell className="font-medium">{app.company}</TableCell>
                <TableCell>{app.role}</TableCell>
                <TableCell onClick={(e) => e.stopPropagation()}>
                  <Select
                    value={app.status}
                    onValueChange={(v) =>
                      statusMutation.mutate({ id: app.id, newStatus: v as ApplicationStatus })
                    }
                  >
                    <SelectTrigger className={`h-8 w-40 border-0 ${statusBadgeClass(app.status)}`}>
                      <SelectValue />
                    </SelectTrigger>
                    <SelectContent>
                      {APPLICATION_STATUSES.map((s) => (
                        <SelectItem key={s} value={s}>
                          {statusLabel(s)}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </TableCell>
                <TableCell>{formatDate(app.appliedDate)}</TableCell>
              </TableRow>
            ))}
            {!query.isLoading && applications.length === 0 && (
              <TableRow>
                <TableCell colSpan={4} className="text-center text-muted-foreground">
                  No applications found
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
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
