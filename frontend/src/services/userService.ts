import api from './api';

export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  fullName?: string;
}

interface ApiResponse<T> {
  data: T;
  success: boolean;
  message?: string;
}

const userService = {
  // Get all users (Admin/SuperAdmin only)
  async getAll(): Promise<User[]> {
    try {
      const response = await api.get<ApiResponse<User[]>>('/users');
      return response.data.data || [];
    } catch (error: any) {
      console.error('Error fetching users:', error);
      throw error;
    }
  },

  // Get basic user list (All authenticated users) - id, email, name
  async getBasicList(): Promise<User[]> {
    try {
      const response = await api.get<ApiResponse<User[]>>('/users/basic');
      return response.data.data || [];
    } catch (error: any) {
      console.error('Error fetching basic user list:', error);
      throw error;
    }
  },

  // Get user by ID
  async getById(id: string): Promise<User> {
    try {
      const response = await api.get<ApiResponse<User>>(`/users/${id}`);
      return response.data.data;
    } catch (error: any) {
      console.error(`Error fetching user ${id}:`, error);
      throw error;
    }
  },
};

export default userService;
