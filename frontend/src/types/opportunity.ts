export interface Opportunity {
  id: string
  title: string
  description?: string
  value: number
  probability?: number
  expectedCloseDate?: string
  actualCloseDate?: string
  stage: string
  dealStageId?: string
  leadId?: string
  contactId?: string
  companyId?: string
  assignedUserId?: string
  notes?: string
  isActive: boolean
  createdAt: string
  updatedAt: string
}

export interface OpportunityStats {
  totalValue: number
  averageValue: number
  totalCount: number
  wonCount: number
  lostCount: number
  activeCount: number
  winRate: number
}

export interface CreateOpportunityDto {
  title: string
  description?: string
  value: number
  probability?: number
  expectedCloseDate?: string
  dealStageId?: string
  leadId?: string
  contactId?: string
  companyId?: string
  assignedUserId?: string
  notes?: string
}

export interface UpdateOpportunityDto {
  title?: string
  description?: string
  value?: number
  probability?: number
  expectedCloseDate?: string
  actualCloseDate?: string
  stage?: string
  dealStageId?: string
  leadId?: string
  contactId?: string
  companyId?: string
  assignedUserId?: string
  notes?: string
  isActive?: boolean
}

export interface OpportunitiesResponse {
  data: Opportunity[]
  total: number
  page: number
  limit: number
}
