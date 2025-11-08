export interface DealStage {
  id: string
  name: string
  description?: string
  order: number
  color?: string
  isDefault: boolean
  isActive: boolean
  createdAt: string
  updatedAt: string
}

export interface CreateDealStageDto {
  name: string
  description?: string
  order: number
  color?: string
  isDefault?: boolean
}

export interface UpdateDealStageDto {
  name?: string
  description?: string
  order?: number
  color?: string
  isDefault?: boolean
  isActive?: boolean
}
