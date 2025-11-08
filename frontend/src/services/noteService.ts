import api from './api'
import { Note, CreateNoteDto, UpdateNoteDto } from '../types/note'


interface NotesListResponse {
  items: Note[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
  errors?: any;
}

export const noteService = {
  getAll: async (params?: { contactId?: string; companyId?: string; opportunityId?: string; pageNumber?: number; pageSize?: number }) => {
    const response = await api.get<ApiResponse<NotesListResponse>>('/notes', { params })
    return response.data;
  },

  getById: async (id: string) => {
    const response = await api.get<ApiResponse<Note>>(`/notes/${id}`)
    return response.data
  },

  create: async (data: CreateNoteDto) => {
    const response = await api.post<ApiResponse<Note>>('/notes', data)
    return response.data
  },

  update: async (id: string, data: UpdateNoteDto) => {
    const response = await api.put<ApiResponse<Note>>(`/notes/${id}`, data)
    return response.data
  },

  delete: async (id: string) => {
    await api.delete(`/notes/${id}`)
  },
}
