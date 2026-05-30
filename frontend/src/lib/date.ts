function ordinal(day: number): string {
  if (day % 10 === 1 && day !== 11) return `${day}st`;
  if (day % 10 === 2 && day !== 12) return `${day}nd`;
  if (day % 10 === 3 && day !== 13) return `${day}rd`;
  return `${day}th`;
}

export function formatDate(value: string | Date): string {
  const date = new Date(value);
  const day = ordinal(date.getDate());
  const month = date.toLocaleString("en-US", { month: "long" });
  return `${day} ${month} ${date.getFullYear()}`;
}

export function formatDateTime(value: string | Date): string {
  const date = new Date(value);
  const time = date.toLocaleTimeString("en-US", {
    hour: "2-digit",
    minute: "2-digit",
    hour12: false,
  });
  return `${formatDate(date)}, ${time}`;
}
