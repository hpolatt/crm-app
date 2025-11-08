import api from './api'
import { Company, CompaniesResponse, CreateCompanyDto, UpdateCompanyDto } from '../types/company'

interface ApiResponse<T> {
  success: boolean
  message: string
  data: T
  errors: string[] | null
}

export const companyService = {
  // Get all companies with pagination
  getCompanies: async (
    pageNumber: number = 1,
    pageSize: number = 100,
    isActive?: boolean
  ): Promise<CompaniesResponse> => {
    const params: any = { pageNumber, pageSize }
    if (isActive !== undefined) params.isActive = isActive
    const response = await api.get<ApiResponse<CompaniesResponse>>('/companies', { params })
    return response.data.data
  },

  // Get company by ID
  getCompanyById: async (id: string): Promise<Company> => {
    const response = await api.get<ApiResponse<Company>>(`/companies/${id}`)
    return response.data.data
  },

  // Create company
  createCompany: async (data: CreateCompanyDto): Promise<Company> => {
    const response = await api.post<ApiResponse<Company>>('/companies', data)
    return response.data.data
  },

  // Update company
  updateCompany: async (id: string, data: UpdateCompanyDto): Promise<Company> => {
    const response = await api.put<ApiResponse<Company>>(`/companies/${id}`, data)
    return response.data.data
  },

  // Delete company
  deleteCompany: async (id: string): Promise<void> => {
    await api.delete(`/companies/${id}`)
  },
}

export default companyService
