import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import {
  Box,
  Paper,
  TextField,
  Button,
  Typography,
  Container,
  Alert,
  Stack,
  Divider,
  Chip
} from '@mui/material'
import { LoginOutlined } from '@mui/icons-material'
import { api } from '../services/api'

interface LoginForm {
  email: string
  password: string
}

const Login = () => {
  const navigate = useNavigate()
  const [form, setForm] = useState<LoginForm>({ email: '', password: '' })
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setForm({ ...form, [e.target.name]: e.target.value })
    setError('')
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setLoading(true)
    setError('')

    try {
      const response = await api.post('/auth/login', form)
      
      // Store token and user info
      localStorage.setItem('token', response.data.data.token)
      localStorage.setItem('user', JSON.stringify(response.data.data.user))
      
      // Redirect to dashboard
      navigate('/')
    } catch (err) {
      setError('Kullanıcı adı veya şifre hatalı')
    } finally {
      setLoading(false)
    }
  }

  const quickLogin = (email: string, password: string) => {
    setForm({ email, password })
  }

  return (
    <Container component="main" maxWidth="xs">
      <Box
        sx={{
          marginTop: 8,
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
        }}
      >
        <Paper elevation={3} sx={{ p: 4, width: '100%' }}>
          <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center', mb: 3 }}>
            <LoginOutlined sx={{ fontSize: 48, color: 'primary.main', mb: 2 }} />
            <Typography component="h1" variant="h5">
              PKT App Giriş
            </Typography>
          </Box>

          {error && (
            <Alert severity="error" sx={{ mb: 2 }}>
              {error}
            </Alert>
          )}

          <Box component="form" onSubmit={handleSubmit} noValidate>
            <TextField
              margin="normal"
              required
              fullWidth
              id="email"
              label="Kullanıcı Adı"
              name="email"
              autoComplete="email"
              autoFocus
              value={form.email}
              onChange={handleChange}
            />
            <TextField
              margin="normal"
              required
              fullWidth
              name="password"
              label="Şifre"
              type="password"
              id="password"
              autoComplete="current-password"
              value={form.password}
              onChange={handleChange}
            />
            <Button
              type="submit"
              fullWidth
              variant="contained"
              sx={{ mt: 3, mb: 2 }}
              disabled={loading}
            >
              {loading ? 'Giriş yapılıyor...' : 'Giriş Yap'}
            </Button>
          </Box>

          <Divider sx={{ my: 2 }}>
            <Chip label="Hızlı Giriş" size="small" />
          </Divider>

          <Stack spacing={1}>
            <Button
              variant="outlined"
              size="small"
              onClick={() => quickLogin('anonymous', 'anonymous123')}
            >
              Anonymous Kullanıcı
            </Button>
            <Button
              variant="outlined"
              size="small"
              onClick={() => quickLogin('admin', 'admin123')}
            >
              Admin
            </Button>
            <Button
              variant="outlined"
              size="small"
              onClick={() => quickLogin('ekinkirmizitoprak', 'ekin123')}
            >
              Ekin Kırmızıtoprak
            </Button>
          </Stack>

          <Typography variant="body2" color="text.secondary" align="center" sx={{ mt: 3 }}>
            Demo kullanıcıları ile hızlı giriş yapabilirsiniz
          </Typography>
        </Paper>
      </Box>
    </Container>
  )
}

export default Login
