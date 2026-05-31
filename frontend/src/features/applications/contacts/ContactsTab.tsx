import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";
import { Plus, Pencil, Trash2, Mail, Phone, Link2 } from "lucide-react";
import { contactsApi } from "@/api/contacts";
import { ContactDialog } from "@/features/applications/contacts/ContactDialog";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
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

export function ContactsTab({ applicationId }: { applicationId: string }) {
  const queryClient = useQueryClient();

  const query = useQuery({
    queryKey: ["applications", applicationId, "contacts"],
    queryFn: () => contactsApi.getAll(applicationId),
  });

  const deleteMutation = useMutation({
    mutationFn: (contactId: string) =>
      contactsApi.delete(applicationId, contactId),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["applications", applicationId, "contacts"],
      });
      toast.success("Contact deleted");
    },
    onError: () => toast.error("Failed to delete contact"),
  });

  return (
    <div className="space-y-4">
      <ContactDialog
        applicationId={applicationId}
        trigger={
          <Button size="sm">
            <Plus className="size-4" />
            Add Contact
          </Button>
        }
      />

      {query.isLoading && (
        <p className="text-sm text-muted-foreground">Loading...</p>
      )}
      {query.data?.length === 0 && (
        <p className="text-sm text-muted-foreground">No contacts yet.</p>
      )}

      <div className="grid gap-3 sm:grid-cols-2">
        {query.data?.map((contact) => (
          <Card key={contact.id}>
            <CardContent className="space-y-2 pt-4">
              <div className="flex items-start justify-between">
                <div>
                  <p className="font-medium">{contact.name}</p>
                  {contact.role && (
                    <p className="text-sm text-muted-foreground">
                      {contact.role}
                    </p>
                  )}
                </div>
                <div className="flex gap-1">
                  <ContactDialog
                    applicationId={applicationId}
                    contact={contact}
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
                        <AlertDialogTitle>
                          Delete this contact?
                        </AlertDialogTitle>
                        <AlertDialogDescription>
                          This will permanently delete {contact.name}. This
                          action cannot be undone.
                        </AlertDialogDescription>
                      </AlertDialogHeader>
                      <AlertDialogFooter>
                        <AlertDialogCancel>Cancel</AlertDialogCancel>
                        <AlertDialogAction
                          onClick={() => deleteMutation.mutate(contact.id)}
                        >
                          Delete
                        </AlertDialogAction>
                      </AlertDialogFooter>
                    </AlertDialogContent>
                  </AlertDialog>
                </div>
              </div>
              <div className="space-y-1 text-sm text-muted-foreground">
                {contact.email && (
                  <div className="flex items-center gap-2">
                    <Mail className="size-3.5" />
                    {contact.email}
                  </div>
                )}
                {contact.phone && (
                  <div className="flex items-center gap-2">
                    <Phone className="size-3.5" />
                    {contact.phone}
                  </div>
                )}
                {contact.linkedInUrl && (
                  <div className="flex items-center gap-2">
                    <Link2 className="size-3.5" />{" "}
                    <a
                      href={contact.linkedInUrl}
                      target="_blank"
                      rel="noreferrer"
                      className="underline underline-offset-4"
                    >
                      LinkedIn
                    </a>
                  </div>
                )}
              </div>
            </CardContent>
          </Card>
        ))}
      </div>
    </div>
  );
}
