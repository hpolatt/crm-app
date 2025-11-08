export interface Company {
  id: string
  name: string
  industry?: string
  website?: string
  phone?: string
  email?: string
  address?: string
  city?: string
  country?: string
  postalCode?: string
  source?: string
  employeeCount?: number
  annualRevenue?: number
  notes?: string
  isActive: boolean
  createdAt: string
  updatedAt: string
}

export interface CreateCompanyDto {
  name: string
  industry?: string
  website?: string
  phone?: string
  email?: string
  address?: string
  city?: string
  country?: string
  postalCode?: string
  source?: string
  employeeCount?: number
  annualRevenue?: number
  notes?: string
  isActive?: boolean
}

export interface UpdateCompanyDto {
  name?: string
  industry?: string
  website?: string
  phone?: string
  email?: string
  address?: string
  city?: string
  country?: string
  postalCode?: string
  source?: string
  employeeCount?: number
  annualRevenue?: number
  notes?: string
  isActive?: boolean
}

export interface CompaniesResponse {
  items: Company[]
  totalCount: number
  pageNumber: number
  pageSize: number
  totalPages: number
}
