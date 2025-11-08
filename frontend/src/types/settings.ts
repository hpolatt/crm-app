export interface SystemSetting {
  id: string
  key: string
  value: string
  description?: string
  isActive: boolean
  createdAt: string
  updatedAt: string
}

export interface UpdateSystemSettingDto {
  value: string
  description?: string
  isActive?: boolean
}
