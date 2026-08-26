

export function toDateTimeLocalInputValue(isoUtc: string): string {
  const date = new Date(isoUtc);
  const pad = (n: number) => n.toString().padStart(2, '0');
  return `${date.getUTCFullYear()}-${pad(date.getUTCMonth() + 1)}-${pad(date.getUTCDate())}T${pad(date.getUTCHours())}:${pad(date.getUTCMinutes())}`;
}

export function fromDateTimeLocalInputValue(value: string): string {
  return `${value}:00Z`;
}

export function formatWorkoutDateTime(isoUtc: string): string {
  const date = new Date(isoUtc);
  const pad = (n: number) => n.toString().padStart(2, '0');
  return `${pad(date.getUTCDate())}.${pad(date.getUTCMonth() + 1)}.${date.getUTCFullYear()} ${pad(date.getUTCHours())}:${pad(date.getUTCMinutes())}`;
}
