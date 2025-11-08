export interface Contact {
  id: string
  companyId?: string
  companyName?: string
  firstName: string
  lastName: string
  fullName?: string
  email?: string
  phone?: string
  mobile?: string
  position?: string
  department?: string
  address?: string
  city?: string
  country?: string
  postalCode?: string
  birthDate?: string
  notes?: string
  isPrimary: boolean
  isActive: boolean
  createdAt: string
  updatedAt?: string
}

export interface CreateContactDto {
  companyId?: string
  firstName: string
  lastName: string
  email?: string
  phone?: string
  mobile?: string
  position?: string
  department?: string
  address?: string
  city?: string
  country?: string
  postalCode?: string
  birthDate?: string
  notes?: string
  isPrimary?: boolean
  isActive?: boolean
}

export interface UpdateContactDto {
  companyId?: string
  firstName: string
  lastName: string
  email?: string
  phone?: string
  mobile?: string
  position?: string
  department?: string
  address?: string
  city?: string
  country?: string
  postalCode?: string
  birthDate?: string
  notes?: string
  isPrimary?: boolean
  isActive?: boolean
}

export interface ContactsResponse {
  items: Contact[]
  totalCount: number
  pageNumber: number
  pageSize: number
  totalPages: number
}
