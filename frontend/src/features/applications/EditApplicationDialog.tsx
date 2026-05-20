import { useState, useEffect } from "react"
import { useMutation, useQueryClient } from "@tanstack/react-query"
import { toast } from "sonner"
import { applicationsApi } from "@/api/applications"
import type { JobApplication } from "@/types/application"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
  DialogTrigger,
} from "@/components/ui/dialog"

export function EditApplicationDialog({ application }: { application: JobApplication }) {
  const queryClient = useQueryClient()
  const [open, setOpen] = useState(false)
  const [company, setCompany] = useState(application.company)
  const [role, setRole] = useState(application.role)
  const [appliedDate, setAppliedDate] = useState(application.appliedDate.slice(0, 10))

  useEffect(() => {
    if (open) {
      setCompany(application.company)
      setRole(application.role)
      setAppliedDate(application.appliedDate.slice(0, 10))
    }
  }, [open, application])

  const mutation = useMutation({
    mutationFn: () => applicationsApi.update(application.id, { company, role, appliedDate }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["applications", application.id] })
      queryClient.invalidateQueries({ queryKey: ["applications"] })
      toast.success("Application updated")
      setOpen(false)
    },
    onError: () => toast.error("Failed to update application"),
  })

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    mutation.mutate()
  }

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>
        <Button variant="outline">Edit</Button>
      </DialogTrigger>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Edit Application</DialogTitle>
        </DialogHeader>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="space-y-2">
            <Label htmlFor="edit-company">Company</Label>
            <Input
              id="edit-company"
              required
              value={company}
              onChange={(e) => setCompany(e.target.value)}
            />
          </div>
          <div className="space-y-2">
            <Label htmlFor="edit-role">Role</Label>
            <Input
              id="edit-role"
              required
              value={role}
              onChange={(e) => setRole(e.target.value)}
            />
          </div>
          <div className="space-y-2">
            <Label htmlFor="edit-appliedDate">Applied Date</Label>
            <Input
              id="edit-appliedDate"
              type="date"
              required
              value={appliedDate}
              onChange={(e) => setAppliedDate(e.target.value)}
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
