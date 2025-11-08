export interface Note {
  id: string
  content: string
  companyId?: string
  companyName?: string
  contactId?: string
  contactName?: string
  leadId?: string
  leadName?: string
  opportunityId?: string
  opportunityTitle?: string
  isPinned: boolean
  isActive: boolean
  createdAt: string
  updatedAt?: string
  createdBy?: string
  updatedBy?: string
  createdByName?: string
  isDeleted: boolean
}

export interface CreateNoteDto {
  content: string
  companyId?: string
  contactId?: string
  leadId?: string
  opportunityId?: string
  isPinned?: boolean
  isActive?: boolean
}

export interface UpdateNoteDto {
  content: string
  isPinned?: boolean
  isActive?: boolean
}

export interface NotesResponse {
  data: Note[]
  total: number
  page: number
  limit: number
}
