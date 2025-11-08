// Backend uses string types, not enums
export enum ActivityType {
  Call = 'Call',
  Meeting = 'Meeting',
  Email = 'Email',
  Task = 'Task',
  Deadline = 'Deadline'
}

export enum ActivityStatus {
  Planned = 'planned',
  InProgress = 'in-progress',
  Completed = 'completed',
  Cancelled = 'cancelled'
}

export enum ActivityPriority {
  Low = 'low',
  Medium = 'medium',
  High = 'high',
  Urgent = 'urgent'
}

export interface Activity {
  id: string
  type: string  // Backend uses string
  subject: string
  description?: string
  status: string  // Backend uses string
  priority: string
  dueDate?: string
  completedDate?: string
  companyId?: string
  companyName?: string
  contactId?: string
  contactName?: string
  leadId?: string
  opportunityId?: string
  opportunityTitle?: string
  assignedUserId?: string
  assignedUserIdName?: string
  isActive: boolean
  createdAt: string
  updatedAt?: string
}

export interface CreateActivityDto {
  type: string
  subject: string
  description?: string
  status?: string
  priority?: string
  dueDate?: string
  companyId?: string
  contactId?: string
  leadId?: string
  opportunityId?: string
  assignedUserId?: string
}

export interface UpdateActivityDto {
  type?: string
  subject?: string
  description?: string
  status?: string
  priority?: string
  dueDate?: string
  completedDate?: string
  companyId?: string
  contactId?: string
  leadId?: string
  opportunityId?: string
  assignedUserId?: string
  isActive?: boolean
}
