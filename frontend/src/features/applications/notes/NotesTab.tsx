import { useState } from "react"
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query"
import { toast } from "sonner"
import { Pencil, Trash2, X, Check } from "lucide-react"
import { notesApi } from "@/api/notes"
import type { Note } from "@/types/note"
import { Button } from "@/components/ui/button"
import { Textarea } from "@/components/ui/textarea"
import { Card, CardContent } from "@/components/ui/card"
import { formatDateTime } from "@/lib/date"
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
} from "@/components/ui/alert-dialog"

function NoteItem({ applicationId, note }: { applicationId: string; note: Note }) {
  const queryClient = useQueryClient()
  const [editing, setEditing] = useState(false)
  const [content, setContent] = useState(note.content)

  const invalidate = () =>
    queryClient.invalidateQueries({ queryKey: ["applications", applicationId, "notes"] })

  const updateMutation = useMutation({
    mutationFn: () => notesApi.update(applicationId, note.id, content),
    onSuccess: () => {
      invalidate()
      setEditing(false)
      toast.success("Note updated")
    },
    onError: () => toast.error("Failed to update note"),
  })

  const deleteMutation = useMutation({
    mutationFn: () => notesApi.delete(applicationId, note.id),
    onSuccess: () => {
      invalidate()
      toast.success("Note deleted")
    },
    onError: () => toast.error("Failed to delete note"),
  })

  return (
    <Card>
      <CardContent className="space-y-2 pt-4">
        {editing ? (
          <div className="space-y-2">
            <Textarea value={content} onChange={(e) => setContent(e.target.value)} />
            <div className="flex gap-2">
              <Button
                size="sm"
                onClick={() => updateMutation.mutate()}
                disabled={updateMutation.isPending}
              >
                <Check className="size-4" />
                Save
              </Button>
              <Button
                size="sm"
                variant="outline"
                onClick={() => {
                  setContent(note.content)
                  setEditing(false)
                }}
              >
                <X className="size-4" />
                Cancel
              </Button>
            </div>
          </div>
        ) : (
          <>
            <p className="text-sm whitespace-pre-wrap">{note.content}</p>
            <div className="flex items-center justify-between">
              <p className="text-xs text-muted-foreground">
                {formatDateTime(note.createdAtUtc)}
                {note.updatedAtUtc && " (edited)"}
              </p>
              <div className="flex gap-1">
                <Button variant="ghost" size="icon" className="size-8" onClick={() => setEditing(true)}>
                  <Pencil className="size-4" />
                </Button>
                <AlertDialog>
                  <AlertDialogTrigger asChild>
                    <Button variant="ghost" size="icon" className="size-8 text-destructive">
                      <Trash2 className="size-4" />
                    </Button>
                  </AlertDialogTrigger>
                  <AlertDialogContent>
                    <AlertDialogHeader>
                      <AlertDialogTitle>Delete this note?</AlertDialogTitle>
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
            </div>
          </>
        )}
      </CardContent>
    </Card>
  )
}

export function NotesTab({ applicationId }: { applicationId: string }) {
  const queryClient = useQueryClient()
  const [content, setContent] = useState("")

  const query = useQuery({
    queryKey: ["applications", applicationId, "notes"],
    queryFn: () => notesApi.getAll(applicationId),
  })

  const createMutation = useMutation({
    mutationFn: () => notesApi.create(applicationId, content),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["applications", applicationId, "notes"] })
      setContent("")
      toast.success("Note added")
    },
    onError: () => toast.error("Failed to add note"),
  })

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    if (!content.trim()) return
    createMutation.mutate()
  }

  const sorted = [...(query.data ?? [])].sort(
    (a, b) => new Date(b.createdAtUtc).getTime() - new Date(a.createdAtUtc).getTime(),
  )

  return (
    <div className="space-y-4">
      <form onSubmit={handleSubmit} className="space-y-2">
        <Textarea
          placeholder="Add a note..."
          value={content}
          onChange={(e) => setContent(e.target.value)}
        />
        <Button type="submit" disabled={createMutation.isPending || !content.trim()}>
          {createMutation.isPending ? "Adding..." : "Add Note"}
        </Button>
      </form>

      {query.isLoading && <p className="text-sm text-muted-foreground">Loading...</p>}
      {sorted.length === 0 && !query.isLoading && (
        <p className="text-sm text-muted-foreground">No notes yet.</p>
      )}

      <div className="space-y-3">
        {sorted.map((note) => (
          <NoteItem key={note.id} applicationId={applicationId} note={note} />
        ))}
      </div>
    </div>
  )
}
