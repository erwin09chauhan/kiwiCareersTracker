import { useEffect, useState } from "react";

interface LoadingStateProps {
  slowMessage?: string;
  slowThresholdMs?: number;
}

export function LoadingState({
  slowMessage = "Waking up the server, this can take up to a minute...",
  slowThresholdMs = 3000,
}: LoadingStateProps) {
  const [showSlowMessage, setShowSlowMessage] = useState(false);

  useEffect(() => {
    const timeout = setTimeout(() => setShowSlowMessage(true), slowThresholdMs);
    return () => clearTimeout(timeout);
  }, [slowThresholdMs]);

  return (
    <div className="flex flex-col items-center justify-center gap-3 py-12">
      <div className="h-8 w-8 animate-spin rounded-full border-4 border-muted border-t-primary" />
      {showSlowMessage && (
        <p className="text-sm text-muted-foreground">{slowMessage}</p>
      )}
    </div>
  );
}
