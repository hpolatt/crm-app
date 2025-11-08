import api from './api'
import { Opportunity, OpportunityStats, CreateOpportunityDto, UpdateOpportunityDto } from '../types/opportunity'

interface ApiResponse<T> {
  success: boolean
  message: string
  data: T
  errors: string[] | null
}

export const opportunityService = {
  getAll: async (params?: { stage?: string; contactId?: string; companyId?: string; pageNumber?: number; pageSize?: number }) => {
    const response = await api.get<ApiResponse<{ items: Opportunity[]; totalCount: number; pageNumber: number; pageSize: number }>>('/opportunities', { params })
    return response.data.data
  },

  getById: async (id: string) => {
    const response = await api.get<ApiResponse<Opportunity>>(`/opportunities/${id}`)
    return response.data.data
  },

  create: async (data: CreateOpportunityDto) => {
    const response = await api.post<ApiResponse<Opportunity>>('/opportunities', data)
    return response.data.data
  },

  update: async (id: string, data: UpdateOpportunityDto) => {
    const response = await api.put<ApiResponse<Opportunity>>(`/opportunities/${id}`, data)
    return response.data.data
  },

  delete: async (id: string) => {
    await api.delete(`/opportunities/${id}`)
  },

  getStats: async () => {
    const response = await api.get<ApiResponse<OpportunityStats>>('/opportunities/stats')
    return response.data.data
  },
}
