import api from './api'
import { Lead, LeadsResponse, CreateLeadDto, UpdateLeadDto } from '../types/lead'

interface ApiResponse<T> {
  success: boolean
  message: string
  data: T
  errors: string[] | null
}

export const leadService = {
  // Get all leads with pagination
  getLeads: async (
    pageNumber: number = 1,
    pageSize: number = 10,
    status?: string,
    source?: string,
    isActive?: boolean
  ): Promise<LeadsResponse> => {
    const params: any = { pageNumber, pageSize }
    if (status !== undefined) params.status = status
    if (source !== undefined) params.source = source
    if (isActive !== undefined) params.isActive = isActive
    const response = await api.get<ApiResponse<LeadsResponse>>('/leads', { params })
    return response.data.data
  },

  // Get lead by ID
  getLeadById: async (id: string): Promise<Lead> => {
    const response = await api.get<ApiResponse<Lead>>(`/leads/${id}`)
    return response.data.data
  },

  // Create lead
  createLead: async (data: CreateLeadDto): Promise<Lead> => {
    const response = await api.post<ApiResponse<Lead>>('/leads', data)
    return response.data.data
  },

  // Update lead
  updateLead: async (id: string, data: UpdateLeadDto): Promise<Lead> => {
    const response = await api.put<ApiResponse<Lead>>(`/leads/${id}`, data)
    return response.data.data
  },

  // Delete lead
  deleteLead: async (id: string): Promise<void> => {
    await api.delete(`/leads/${id}`)
  },

  // Convert lead to opportunity
  convertLead: async (id: string, opportunityData: any): Promise<any> => {
    const response = await api.post<ApiResponse<any>>(`/leads/${id}/convert`, opportunityData)
    return response.data.data
  },
}

export default leadService
