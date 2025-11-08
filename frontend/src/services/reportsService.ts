import api from './api'
import { SalesReportDto, CustomerReportDto } from '../types/reports'

interface ApiResponse<T> {
  success: boolean
  message: string
  data: T
  errors?: any
}

export const reportsService = {
  getSalesReport: async (startDate?: string, endDate?: string) => {
    const response = await api.get<ApiResponse<SalesReportDto>>('/reports/sales', {
      params: { startDate, endDate }
    })
    return response.data.data
  },

  getCustomerReport: async () => {
    const response = await api.get<ApiResponse<CustomerReportDto>>('/reports/customers')
    return response.data.data
  },
}
