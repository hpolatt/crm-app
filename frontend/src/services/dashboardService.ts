import api from './api'
import { DashboardSummary } from '../types/dashboard'

interface ApiResponse<T> {
  success: boolean
  message: string
  data: T
  errors: string[] | null
}

export const dashboardService = {
  getSummary: async () => {
    const response = await api.get<ApiResponse<DashboardSummary>>('/dashboard/summary')
    return response.data.data
  },
}
