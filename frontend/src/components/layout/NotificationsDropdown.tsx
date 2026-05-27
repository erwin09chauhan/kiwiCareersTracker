import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";
import { Bell, X } from "lucide-react";
import { notificationsApi } from "@/api/notifications";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover";
import { useEffect } from "react";
import { getNotificationsConnection } from "@/lib/signalr";

export function NotificationsDropdown() {
  const queryClient = useQueryClient();
  const navigate = useNavigate();

  const query = useQuery({
    queryKey: ["notifications"],
    queryFn: () => notificationsApi.getAll(undefined, 10),
  });

  const notifications = query.data?.items ?? [];
  const unreadCount = notifications.filter((n) => !n.isRead).length;

  const invalidate = () =>
    queryClient.invalidateQueries({ queryKey: ["notifications"] });

  useEffect(() => {
    const connection = getNotificationsConnection();

    connection.on("notification", () => {
      queryClient.invalidateQueries({ queryKey: ["notifications"] });
    });

    if (connection.state === "Disconnected") {
      connection.start().catch(() => {});
    }

    return () => {
      connection.off("notification");
    };
  }, [queryClient]);

  const markReadMutation = useMutation({
    mutationFn: (id: string) => notificationsApi.markRead(id),
    onSuccess: invalidate,
  });

  const markAllReadMutation = useMutation({
    mutationFn: () => notificationsApi.markAllRead(),
    onSuccess: invalidate,
  });

  const deleteMutation = useMutation({
    mutationFn: (id: string) => notificationsApi.delete(id),
    onSuccess: invalidate,
  });

  const handleClick = (notification: (typeof notifications)[number]) => {
    if (!notification.isRead) {
      markReadMutation.mutate(notification.id);
    }
    if (notification.relatedApplicationId) {
      navigate(`/applications/${notification.relatedApplicationId}`);
    }
  };

  return (
    <Popover>
      <PopoverTrigger asChild>
        <Button variant="ghost" size="icon" className="relative">
          <Bell className="size-4" />
          {unreadCount > 0 && (
            <Badge className="absolute -right-1 -top-1 h-4 min-w-4 justify-center rounded-full px-1 text-[10px]">
              {unreadCount}
            </Badge>
          )}
        </Button>
      </PopoverTrigger>
      <PopoverContent className="w-80 p-0" align="end">
        <div className="flex items-center justify-between border-b p-3">
          <p className="text-sm font-medium">Notifications</p>
          {unreadCount > 0 && (
            <Button
              variant="ghost"
              size="sm"
              className="h-auto p-0 text-xs"
              onClick={() => markAllReadMutation.mutate()}
            >
              Mark all read
            </Button>
          )}
        </div>
        <div className="max-h-80 overflow-y-auto">
          {query.isLoading && (
            <p className="p-3 text-sm text-muted-foreground">Loading...</p>
          )}
          {!query.isLoading && notifications.length === 0 && (
            <p className="p-3 text-sm text-muted-foreground">
              No notifications.
            </p>
          )}
          {notifications.map((notification) => (
            <div
              key={notification.id}
              className={`flex cursor-pointer items-start justify-between gap-2 border-b p-3 last:border-b-0 hover:bg-accent ${
                notification.isRead ? "" : "bg-accent/50"
              }`}
              onClick={() => handleClick(notification)}
            >
              <div className="space-y-1">
                <p className="text-sm font-medium">{notification.title}</p>
                <p className="text-sm text-muted-foreground">
                  {notification.message}
                </p>
                <p className="text-xs text-muted-foreground">
                  {new Date(notification.createdAtUtc).toLocaleString()}
                </p>
              </div>
              <Button
                variant="ghost"
                size="icon"
                className="size-6 shrink-0"
                onClick={(e) => {
                  e.stopPropagation();
                  deleteMutation.mutate(notification.id);
                }}
              >
                <X className="size-3" />
              </Button>
            </div>
          ))}
        </div>
      </PopoverContent>
    </Popover>
  );
}
