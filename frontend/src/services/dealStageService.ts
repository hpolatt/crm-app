import api from './api'
import { DealStage, CreateDealStageDto, UpdateDealStageDto } from '../types/dealStage'

interface ApiResponse<T> {
  success: boolean
  message: string
  data: T
  errors: string[] | null
}

export const dealStageService = {
  getAll: async (params?: { isActive?: boolean }) => {
    const response = await api.get<ApiResponse<DealStage[]>>('/dealstages', { params })
    return response.data.data
  },

  getById: async (id: string) => {
    const response = await api.get<ApiResponse<DealStage>>(`/dealstages/${id}`)
    return response.data.data
  },

  create: async (data: CreateDealStageDto) => {
    const response = await api.post<ApiResponse<DealStage>>('/dealstages', data)
    return response.data.data
  },

  update: async (id: string, data: UpdateDealStageDto) => {
    const response = await api.put<ApiResponse<DealStage>>(`/dealstages/${id}`, data)
    return response.data.data
  },

  delete: async (id: string) => {
    await api.delete(`/dealstages/${id}`)
  },
}
