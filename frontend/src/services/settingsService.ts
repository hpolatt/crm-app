import api from './api'
import { SystemSetting, UpdateSystemSettingDto } from '../types/settings'

interface ApiResponse<T> {
  success: boolean
  message: string
  data: T
  errors: string[] | null
}

export const settingsService = {
  getAll: async (params?: { isActive?: boolean }) => {
    const response = await api.get<ApiResponse<SystemSetting[]>>('/settings', { params })
    return response.data.data
  },

  getByKey: async (key: string) => {
    const response = await api.get<ApiResponse<SystemSetting>>(`/settings/${key}`)
    return response.data.data
  },

  update: async (key: string, data: UpdateSystemSettingDto) => {
    const response = await api.put<ApiResponse<SystemSetting>>(`/settings/${key}`, data)
    return response.data.data
  },
}
