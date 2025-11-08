export interface ActivityLog {
  requestId: string
  timestamp: string
  method: string
  path: string
  statusCode: number
  durationMs: number
  userId?: string
  userEmail?: string
  ipAddress?: string
  userAgent?: string
  errorMessage?: string | null
}
