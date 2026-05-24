import { useState, useEffect } from "react"
import { useMutation, useQueryClient } from "@tanstack/react-query"
import { toast } from "sonner"
import { remindersApi } from "@/api/reminders"
import type { Reminder, ReminderRequest } from "@/types/reminder"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Textarea } from "@/components/ui/textarea"
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
  DialogTrigger,
} from "@/components/ui/dialog"

interface ReminderDialogProps {
  applicationId: string
  reminder?: Reminder
  trigger: React.ReactNode
}

const emptyForm = { title: "", description: "", dueDateUtc: "" }

function toLocalInput(iso: string) {
  const d = new Date(iso)
  const offset = d.getTimezoneOffset()
  const local = new Date(d.getTime() - offset * 60000)
  return local.toISOString().slice(0, 16)
}

export function ReminderDialog({ applicationId, reminder, trigger }: ReminderDialogProps) {
  const queryClient = useQueryClient()
  const [open, setOpen] = useState(false)
  const [form, setForm] = useState(emptyForm)
  const isEdit = !!reminder

  useEffect(() => {
    if (open) {
      setForm(
        reminder
          ? {
              title: reminder.title,
              description: reminder.description ?? "",
              dueDateUtc: toLocalInput(reminder.dueDateUtc),
            }
          : emptyForm,
      )
    }
  }, [open, reminder])

  const mutation = useMutation({
    mutationFn: () => {
      const payload: ReminderRequest = {
        title: form.title,
        description: form.description || undefined,
        dueDateUtc: new Date(form.dueDateUtc).toISOString(),
      }
      return isEdit
        ? remindersApi.update(applicationId, reminder!.id, payload)
        : remindersApi.create(applicationId, payload)
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["applications", applicationId, "reminders"] })
      queryClient.invalidateQueries({ queryKey: ["reminders", "upcoming"] })
      toast.success(isEdit ? "Reminder updated" : "Reminder added")
      setOpen(false)
    },
    onError: () => toast.error(isEdit ? "Failed to update reminder" : "Failed to add reminder"),
  })

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    mutation.mutate()
  }

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>{trigger}</DialogTrigger>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>{isEdit ? "Edit Reminder" : "Add Reminder"}</DialogTitle>
        </DialogHeader>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="space-y-2">
            <Label htmlFor="reminder-title">Title</Label>
            <Input
              id="reminder-title"
              required
              value={form.title}
              onChange={(e) => setForm((f) => ({ ...f, title: e.target.value }))}
            />
          </div>
          <div className="space-y-2">
            <Label htmlFor="reminder-due">Due date</Label>
            <Input
              id="reminder-due"
              type="datetime-local"
              required
              value={form.dueDateUtc}
              onChange={(e) => setForm((f) => ({ ...f, dueDateUtc: e.target.value }))}
            />
          </div>
          <div className="space-y-2">
            <Label htmlFor="reminder-description">Description</Label>
            <Textarea
              id="reminder-description"
              value={form.description}
              onChange={(e) => setForm((f) => ({ ...f, description: e.target.value }))}
            />
          </div>
          <DialogFooter>
            <Button type="submit" disabled={mutation.isPending}>
              {mutation.isPending ? "Saving..." : "Save"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  )
}
