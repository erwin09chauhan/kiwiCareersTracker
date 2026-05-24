import { useState, useEffect } from "react"
import { useMutation, useQueryClient } from "@tanstack/react-query"
import { toast } from "sonner"
import { contactsApi } from "@/api/contacts"
import type { Contact, ContactRequest } from "@/types/contact"
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

interface ContactDialogProps {
  applicationId: string
  contact?: Contact
  trigger: React.ReactNode
}

const emptyForm: ContactRequest = { name: "", role: "", email: "", phone: "", linkedInUrl: "" }

export function ContactDialog({ applicationId, contact, trigger }: ContactDialogProps) {
  const queryClient = useQueryClient()
  const [open, setOpen] = useState(false)
  const [form, setForm] = useState<ContactRequest>(emptyForm)
  const isEdit = !!contact

  useEffect(() => {
    if (open) {
      setForm(
        contact
          ? {
              name: contact.name,
              role: contact.role ?? "",
              email: contact.email ?? "",
              phone: contact.phone ?? "",
              linkedInUrl: contact.linkedInUrl ?? "",
            }
          : emptyForm,
      )
    }
  }, [open, contact])

  const mutation = useMutation({
    mutationFn: () =>
      isEdit
        ? contactsApi.update(applicationId, contact!.id, form)
        : contactsApi.create(applicationId, form),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["applications", applicationId, "contacts"] })
      toast.success(isEdit ? "Contact updated" : "Contact added")
      setOpen(false)
    },
    onError: () => toast.error(isEdit ? "Failed to update contact" : "Failed to add contact"),
  })

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    mutation.mutate()
  }

  const update = (field: keyof ContactRequest) => (e: React.ChangeEvent<HTMLInputElement>) =>
    setForm((f) => ({ ...f, [field]: e.target.value }))

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>{trigger}</DialogTrigger>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>{isEdit ? "Edit Contact" : "Add Contact"}</DialogTitle>
        </DialogHeader>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="space-y-2">
            <Label htmlFor="contact-name">Name</Label>
            <Input id="contact-name" required value={form.name} onChange={update("name")} />
          </div>
          <div className="space-y-2">
            <Label htmlFor="contact-role">Role</Label>
            <Input id="contact-role" value={form.role} onChange={update("role")} />
          </div>
          <div className="space-y-2">
            <Label htmlFor="contact-email">Email</Label>
            <Input id="contact-email" type="email" value={form.email} onChange={update("email")} />
          </div>
          <div className="space-y-2">
            <Label htmlFor="contact-phone">Phone</Label>
            <Input id="contact-phone" value={form.phone} onChange={update("phone")} />
          </div>
          <div className="space-y-2">
            <Label htmlFor="contact-linkedin">LinkedIn URL</Label>
            <Input id="contact-linkedin" value={form.linkedInUrl} onChange={update("linkedInUrl")} />
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
