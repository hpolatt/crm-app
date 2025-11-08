export interface SalesReportDto {
  monthlySales: MonthlySalesDto[]
  salesByStage: SalesByStageDto[]
  salesByUser: SalesByUserDto[]
  totalRevenue: number
  averageDealSize: number
  winRate: number
}

export interface MonthlySalesDto {
  month: string
  totalValue: number
  count: number
  wonValue: number
  wonCount: number
}

export interface SalesByStageDto {
  stage: string
  totalValue: number
  count: number
  averageValue: number
}

export interface SalesByUserDto {
  userId: string
  userName: string
  totalValue: number
  count: number
  wonCount: number
}

export interface CustomerReportDto {
  totalCustomers: number
  activeCustomers: number
  newCustomersThisMonth: number
  topCustomersByRevenue: TopCustomerDto[]
  customersBySource: CustomerBySourceDto[]
  customersByIndustry: CustomerByIndustryDto[]
  customerGrowth: CustomerGrowthDto[]
}

export interface TopCustomerDto {
  companyId: string
  companyName: string
  totalRevenue: number
  opportunityCount: number
}

export interface CustomerBySourceDto {
  source: string
  count: number
  percentage: number
}

export interface CustomerByIndustryDto {
  industry: string
  count: number
  percentage: number
}

export interface CustomerGrowthDto {
  year: number
  month: number
  monthName: string
  newCustomers: number
  totalCustomers: number
}
