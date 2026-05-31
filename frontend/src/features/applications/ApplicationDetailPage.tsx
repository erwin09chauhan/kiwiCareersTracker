import { useParams, useNavigate, Link } from "react-router-dom";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";
import { ArrowLeft, Trash2 } from "lucide-react";
import { applicationsApi } from "@/api/applications";
import {
  APPLICATION_STATUSES,
  type ApplicationStatus,
} from "@/types/application";
import { statusLabel, statusBadgeClass } from "@/lib/status";
import { formatDate } from "@/lib/date";
import { EditApplicationDialog } from "@/features/applications/EditApplicationDialog";
import { Button } from "@/components/ui/button";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Tabs, TabsList, TabsTrigger, TabsContent } from "@/components/ui/tabs";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogTrigger,
} from "@/components/ui/alert-dialog";
import { ContactsTab } from "./contacts/ContactsTab";
import { NotesTab } from "./notes/NotesTab";
import { RemindersTab } from "./reminders/RemindersTab";
import { AuditLogTab } from "./audit/AuditLogTab";

export function ApplicationDetailPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  const query = useQuery({
    queryKey: ["applications", id],
    queryFn: () => applicationsApi.getById(id!),
    enabled: !!id,
  });

  const statusMutation = useMutation({
    mutationFn: (newStatus: ApplicationStatus) =>
      applicationsApi.updateStatus(id!, newStatus),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["applications", id] });
      queryClient.invalidateQueries({ queryKey: ["applications"] });
      queryClient.invalidateQueries({ queryKey: ["dashboard"] });
      queryClient.invalidateQueries({
        queryKey: ["applications", id, "audit"],
      });
    },
    onError: () => toast.error("Failed to update status"),
  });

  const deleteMutation = useMutation({
    mutationFn: () => applicationsApi.delete(id!),
    onSuccess: () => {
      toast.success("Application deleted");
      queryClient.invalidateQueries({ queryKey: ["applications"] });
      queryClient.invalidateQueries({ queryKey: ["dashboard"] });
      navigate("/applications");
    },
    onError: () => toast.error("Failed to delete application"),
  });

  if (query.isLoading) {
    return <p className="text-sm text-muted-foreground">Loading...</p>;
  }

  if (query.isError || !query.data) {
    return (
      <p className="text-sm text-muted-foreground">Application not found.</p>
    );
  }

  const app = query.data;

  return (
    <div className="space-y-6">
      <div>
        <Link
          to="/applications"
          className="inline-flex items-center gap-1 text-sm text-muted-foreground hover:text-foreground"
        >
          <ArrowLeft className="size-4" />
          Back to applications
        </Link>
      </div>
      <div className="rounded-lg border bg-card p-4 sm:p-6">
        <div className="flex flex-col gap-4 sm:flex-row sm:items-start sm:justify-between">
          <div className="space-y-3">
            <div>
              <p className="text-xs font-medium uppercase tracking-wide text-muted-foreground">
                Company
              </p>
              <p className="text-lg capitalize">{app.company}</p>
            </div>
            <div>
              <p className="text-xs font-medium uppercase tracking-wide text-muted-foreground">
                Position
              </p>
              <p className="text-lg capitalize">{app.role}</p>
            </div>
            <p className="text-sm text-muted-foreground">
              Applied {formatDate(app.appliedDate)}
            </p>
          </div>

          <div className="flex flex-wrap items-center gap-2">
            <Select
              value={app.status}
              onValueChange={(v) =>
                statusMutation.mutate(v as ApplicationStatus)
              }
            >
              <SelectTrigger
                className={`w-44 border-0 ${statusBadgeClass(app.status)}`}
              >
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

            <EditApplicationDialog application={app} />

            <AlertDialog>
              <AlertDialogTrigger asChild>
                <Button
                  variant="outline"
                  size="icon"
                  className="text-destructive"
                >
                  <Trash2 className="size-4" />
                </Button>
              </AlertDialogTrigger>
              <AlertDialogContent>
                <AlertDialogHeader>
                  <AlertDialogTitle>Delete this application?</AlertDialogTitle>
                  <AlertDialogDescription>
                    This will permanently delete the application for {app.role}{" "}
                    at {app.company}. This action cannot be undone.
                  </AlertDialogDescription>
                </AlertDialogHeader>
                <AlertDialogFooter>
                  <AlertDialogCancel>Cancel</AlertDialogCancel>
                  <AlertDialogAction onClick={() => deleteMutation.mutate()}>
                    Delete
                  </AlertDialogAction>
                </AlertDialogFooter>
              </AlertDialogContent>
            </AlertDialog>
          </div>
        </div>
      </div>
      <Tabs defaultValue="contacts">
        <TabsList>
          <TabsTrigger value="contacts">Contacts</TabsTrigger>
          <TabsTrigger value="notes">Notes</TabsTrigger>
          <TabsTrigger value="reminders">Reminders</TabsTrigger>
          <TabsTrigger value="audit">Audit Log</TabsTrigger>
        </TabsList>
        <TabsContent value="contacts">
          <ContactsTab applicationId={app.id} />
        </TabsContent>
        <TabsContent value="notes">
          <NotesTab applicationId={app.id} />
        </TabsContent>
        <TabsContent value="reminders">
          <RemindersTab applicationId={app.id} />
        </TabsContent>
        <TabsContent value="audit">
          <AuditLogTab applicationId={app.id} />
        </TabsContent>
      </Tabs>{" "}
    </div>
  );
}
