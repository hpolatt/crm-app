import api from './api'
import { Contact, CreateContactDto, UpdateContactDto, ContactsResponse } from '../types/contact'

interface ApiResponse<T> {
  success: boolean
  message?: string
  data: T
  errors?: string[]
}

export const contactService = {
  // Get all contacts with pagination and filters
  getContacts: async (
    pageNumber: number = 1,
    pageSize: number = 10,
    companyId?: string,
    isActive?: boolean
  ): Promise<ContactsResponse> => {
    const params: any = { pageNumber, pageSize }
    if (companyId) params.companyId = companyId
    if (isActive !== undefined) params.isActive = isActive

    const response = await api.get<ApiResponse<ContactsResponse>>('/contacts', { params })
    // Backend returns nested data: response.data.data contains the actual ContactsResponse
    const data = response.data.data
    
    // Ensure we have a valid response structure
    return {
      items: data?.items || [],
      totalCount: data?.totalCount || 0,
      pageNumber: data?.pageNumber || 1,
      pageSize: data?.pageSize || 10,
      totalPages: data?.totalPages || 0
    }
  },

  // Get contact by ID
  getContactById: async (id: string): Promise<Contact> => {
    const response = await api.get<ApiResponse<Contact>>(`/contacts/${id}`)
    return response.data.data
  },

  // Get contacts by company ID
  getContactsByCompany: async (companyId: string): Promise<Contact[]> => {
    const response = await api.get<ApiResponse<Contact[]>>(`/contacts/company/${companyId}`)
    return response.data.data || []
  },

  // Create new contact
  createContact: async (contact: CreateContactDto): Promise<Contact> => {
    const response = await api.post<ApiResponse<Contact>>('/contacts', contact)
    return response.data.data
  },

  // Update contact
  updateContact: async (id: string, contact: UpdateContactDto): Promise<Contact> => {
    const response = await api.put<ApiResponse<Contact>>(`/contacts/${id}`, contact)
    return response.data.data
  },

  // Delete contact (soft delete)
  deleteContact: async (id: string): Promise<void> => {
    await api.delete(`/contacts/${id}`)
  },
}

export default contactService
