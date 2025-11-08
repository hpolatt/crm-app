export enum LeadStatus {
  New = 0,
  Contacted = 1,
  Qualified = 2,
  Unqualified = 3,
  Converted = 4
}

export enum LeadSource {
  Website = 0,
  Referral = 1,
  Campaign = 2,
  SocialMedia = 3,
  Event = 4,
  Other = 5
}

export interface Lead {
  id: string
  companyId?: string
  contactId?: string
  title: string
  description?: string
  source: string
  status: string
  value?: number
  probability?: number
  expectedCloseDate?: string
  assignedUserId?: string
  notes?: string
  isActive: boolean
  createdAt: string
  updatedAt?: string
  // Navigation properties (optional, may be populated by backend)
  company?: any
  contact?: any
  assignedUser?: any
}

export interface CreateLeadDto {
  companyId?: string
  contactId?: string
  title: string
  description?: string
  source: string
  status?: string
  value?: number
  probability?: number
  expectedCloseDate?: string
  assignedUserId?: string
  notes?: string
  isActive?: boolean
}

export interface UpdateLeadDto {
  companyId?: string
  contactId?: string
  title?: string
  description?: string
  source?: string
  status?: string
  value?: number
  probability?: number
  expectedCloseDate?: string
  assignedUserId?: string
  notes?: string
  isActive?: boolean
}

export interface LeadsResponse {
  items: Lead[]
  totalCount: number
  pageNumber: number
  pageSize: number
  totalPages: number
}
