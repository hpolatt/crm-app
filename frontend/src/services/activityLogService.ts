import api from './api'
import { ActivityLog } from '../types/activityLog'

interface ActivityLogsListResponse {
  items: ActivityLog[]
  totalCount: number
  pageNumber: number
  pageSize: number
  totalPages: number
}

interface ApiResponse<T> {
  success: boolean
  message: string
  data: T
  errors?: any
}

export const activityLogService = {
  getAll: async (params?: { 
    userId?: string
    action?: string
    path?: string
    startDate?: string
    endDate?: string
    minStatusCode?: number
    maxStatusCode?: number
    pageNumber?: number
    pageSize?: number 
  }) => {
    const response = await api.get<ApiResponse<ActivityLogsListResponse>>('/activitylogs', { params })
    return response.data.data
  },
}
