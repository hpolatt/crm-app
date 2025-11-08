import api from './api'
import { Activity, CreateActivityDto, UpdateActivityDto, ActivityStatus, ActivityType } from '../types/activity'

interface ApiResponse<T> {
  success: boolean
  message: string
  data: T
  errors: string[] | null
}

export const activityService = {
  getAll: async (params?: { 
    status?: ActivityStatus; 
    type?: ActivityType; 
    contactId?: string; 
    companyId?: string; 
    opportunityId?: string;
    pageNumber?: number;
    pageSize?: number;
  }) => {
    const response = await api.get<ApiResponse<{ items: Activity[]; totalCount: number; pageNumber: number; pageSize: number }>>('/activities', { params })
    return response.data.data
  },

  getById: async (id: string) => {
    const response = await api.get<ApiResponse<Activity>>(`/activities/${id}`)
    return response.data.data
  },

  create: async (data: CreateActivityDto) => {
    const response = await api.post<ApiResponse<Activity>>('/activities', data)
    return response.data.data
  },

  update: async (id: string, data: UpdateActivityDto) => {
    const response = await api.put<ApiResponse<Activity>>(`/activities/${id}`, data)
    return response.data.data
  },

  delete: async (id: string) => {
    await api.delete(`/activities/${id}`)
  },
}
