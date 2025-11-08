import { createSlice, PayloadAction } from '@reduxjs/toolkit'

interface AuthState {
  token: string | null
  user: {
    id: string
    email: string
    firstName: string
    lastName: string
  } | null
}

const loadUserFromStorage = (): AuthState['user'] => {
  const userStr = localStorage.getItem('user')
  if (userStr) {
    try {
      return JSON.parse(userStr)
    } catch (e) {
      console.error('Failed to parse user from localStorage:', e)
      return null
    }
  }
  return null
}

const initialState: AuthState = {
  token: localStorage.getItem('token'),
  user: loadUserFromStorage(),
}

const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    setCredentials: (
      state,
      action: PayloadAction<{ token: string; user: AuthState['user'] }>
    ) => {
      console.log('authSlice - setCredentials called with:', action.payload)
      state.token = action.payload.token
      state.user = action.payload.user
      localStorage.setItem('token', action.payload.token)
      if (action.payload.user) {
        localStorage.setItem('user', JSON.stringify(action.payload.user))
      }
      console.log('authSlice - state updated:', { token: state.token, user: state.user })
    },
    logout: (state) => {
      state.token = null
      state.user = null
      localStorage.removeItem('token')
      localStorage.removeItem('user')
    },
  },
})

export const { setCredentials, logout } = authSlice.actions
export default authSlice.reducer
