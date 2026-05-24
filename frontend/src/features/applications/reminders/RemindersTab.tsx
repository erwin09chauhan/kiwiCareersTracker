import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";
import { Plus, Pencil, Trash2, Check } from "lucide-react";
import { remindersApi } from "@/api/reminders";
import type { Reminder } from "@/types/reminder";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { ReminderDialog } from "./ReminderDialog";
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

function ReminderItem({
  applicationId,
  reminder,
}: {
  applicationId: string;
  reminder: Reminder;
}) {
  const queryClient = useQueryClient();

  const invalidate = () => {
    queryClient.invalidateQueries({
      queryKey: ["applications", applicationId, "reminders"],
    });
    queryClient.invalidateQueries({ queryKey: ["reminders", "upcoming"] });
  };

  const completeMutation = useMutation({
    mutationFn: () => remindersApi.complete(applicationId, reminder.id),
    onSuccess: () => {
      invalidate();
      toast.success("Reminder completed");
    },
    onError: () => toast.error("Failed to complete reminder"),
  });

  const deleteMutation = useMutation({
    mutationFn: () => remindersApi.delete(applicationId, reminder.id),
    onSuccess: () => {
      invalidate();
      toast.success("Reminder deleted");
    },
    onError: () => toast.error("Failed to delete reminder"),
  });

  return (
    <Card>
      <CardContent className="flex items-start justify-between gap-4 pt-4">
        <div className="space-y-1">
          <p
            className={`text-sm font-medium ${reminder.isCompleted ? "line-through text-muted-foreground" : ""}`}
          >
            {reminder.title}
          </p>
          {reminder.description && (
            <p className="text-sm text-muted-foreground whitespace-pre-wrap">
              {reminder.description}
            </p>
          )}
          <p className="text-xs text-muted-foreground">
            Due {new Date(reminder.dueDateUtc).toLocaleString()}
            {reminder.isCompleted &&
              reminder.completedAtUtc &&
              ` · Completed ${new Date(reminder.completedAtUtc).toLocaleString()}`}
          </p>
        </div>
        <div className="flex shrink-0 gap-1">
          {!reminder.isCompleted && (
            <Button
              variant="ghost"
              size="icon"
              className="size-8"
              onClick={() => completeMutation.mutate()}
              disabled={completeMutation.isPending}
            >
              <Check className="size-4" />
            </Button>
          )}
          <ReminderDialog
            applicationId={applicationId}
            reminder={reminder}
            trigger={
              <Button variant="ghost" size="icon" className="size-8">
                <Pencil className="size-4" />
              </Button>
            }
          />
          <AlertDialog>
            <AlertDialogTrigger asChild>
              <Button
                variant="ghost"
                size="icon"
                className="size-8 text-destructive"
              >
                <Trash2 className="size-4" />
              </Button>
            </AlertDialogTrigger>
            <AlertDialogContent>
              <AlertDialogHeader>
                <AlertDialogTitle>Delete this reminder?</AlertDialogTitle>
                <AlertDialogDescription>
                  This action cannot be undone.
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
      </CardContent>
    </Card>
  );
}

export function RemindersTab({ applicationId }: { applicationId: string }) {
  const query = useQuery({
    queryKey: ["applications", applicationId, "reminders"],
    queryFn: () => remindersApi.getAll(applicationId),
  });

  const sorted = [...(query.data ?? [])].sort(
    (a, b) =>
      new Date(a.dueDateUtc).getTime() - new Date(b.dueDateUtc).getTime(),
  );

  return (
    <div className="space-y-4">
      <ReminderDialog
        applicationId={applicationId}
        trigger={
          <Button size="sm">
            <Plus className="size-4" />
            Add Reminder
          </Button>
        }
      />

      {query.isLoading && (
        <p className="text-sm text-muted-foreground">Loading...</p>
      )}
      {sorted.length === 0 && !query.isLoading && (
        <p className="text-sm text-muted-foreground">No reminders yet.</p>
      )}

      <div className="space-y-3">
        {sorted.map((reminder) => (
          <ReminderItem
            key={reminder.id}
            applicationId={applicationId}
            reminder={reminder}
          />
        ))}
      </div>
    </div>
  );
}
