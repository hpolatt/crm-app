import { useState, useEffect } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import {
  Container,
  Typography,
  Button,
  Paper,
  Box,
  TextField,
  Grid,
  CircularProgress,
  Alert,
  FormControlLabel,
  Switch,
} from '@mui/material'
import { Save as SaveIcon, ArrowBack as ArrowBackIcon } from '@mui/icons-material'
import companyService from '../../services/companyService'
import { CreateCompanyDto, UpdateCompanyDto } from '../../types/company'

function CompanyForm() {
  const navigate = useNavigate()
  const { id } = useParams<{ id: string }>()
  const isEditMode = Boolean(id)

  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [formData, setFormData] = useState<CreateCompanyDto>({
    name: '',
    email: '',
    phone: '',
    website: '',
    address: '',
    isActive: true,
  })

  useEffect(() => {
    if (isEditMode && id) {
      loadCompany(id)
    }
  }, [id, isEditMode])

  const loadCompany = async (companyId: string) => {
    try {
      setLoading(true)
      const company = await companyService.getCompanyById(companyId)
      setFormData({
        name: company.name,
        email: company.email || '',
        phone: company.phone || '',
        website: company.website || '',
        address: company.address || '',
        isActive: company.isActive,
      })
    } catch (err: any) {
      setError(err.message || 'Şirket yüklenirken bir hata oluştu')
    } finally {
      setLoading(false)
    }
  }

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value, type, checked } = e.target
    setFormData((prev) => ({
      ...prev,
      [name]: type === 'checkbox' ? checked : value,
    }))
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    
    if (!formData.name.trim()) {
      setError('Şirket adı zorunludur')
      return
    }

    if (!formData.email || !formData.email.trim()) {
      setError('Email zorunludur')
      return
    }

    // Basic email validation
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
    if (!emailRegex.test(formData.email.trim())) {
      setError('Geçerli bir email adresi giriniz')
      return
    }

    try {
      setLoading(true)
      setError(null)
      
      if (isEditMode && id) {
        const updateData: UpdateCompanyDto = formData
        await companyService.updateCompany(id, updateData)
      } else {
        await companyService.createCompany(formData)
      }
      
      navigate('/companies')
    } catch (err: any) {
      setError(err.message || 'Şirket kaydedilirken bir hata oluştu')
    } finally {
      setLoading(false)
    }
  }

  if (loading && isEditMode) {
    return (
      <Container maxWidth="md">
        <Box sx={{ display: 'flex', justifyContent: 'center', mt: 4 }}>
          <CircularProgress />
        </Box>
      </Container>
    )
  }

  return (
    <Container maxWidth="md">
      <Box sx={{ mb: 4 }}>
        <Button
          startIcon={<ArrowBackIcon />}
          onClick={() => navigate('/companies')}
          sx={{ mb: 2 }}
        >
          Geri Dön
        </Button>
        <Typography variant="h4" component="h1">
          {isEditMode ? 'Şirket Düzenle' : 'Yeni Şirket Ekle'}
        </Typography>
      </Box>

      {error && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {error}
        </Alert>
      )}

      <Paper sx={{ p: 3 }}>
        <form onSubmit={handleSubmit}>
          <Grid container spacing={3}>
            <Grid item xs={12}>
              <TextField
                required
                fullWidth
                label="Şirket Adı"
                name="name"
                value={formData.name}
                onChange={handleChange}
                disabled={loading}
              />
            </Grid>

            <Grid item xs={12} sm={6}>
              <TextField
                required
                fullWidth
                label="Email"
                name="email"
                type="email"
                value={formData.email}
                onChange={handleChange}
                disabled={loading}
              />
            </Grid>

            <Grid item xs={12} sm={6}>
              <TextField
                fullWidth
                label="Telefon"
                name="phone"
                value={formData.phone}
                onChange={handleChange}
                disabled={loading}
              />
            </Grid>

            <Grid item xs={12}>
              <TextField
                fullWidth
                label="Website"
                name="website"
                value={formData.website}
                onChange={handleChange}
                disabled={loading}
                placeholder="https://www.example.com"
              />
            </Grid>

            <Grid item xs={12}>
              <TextField
                fullWidth
                label="Adres"
                name="address"
                value={formData.address}
                onChange={handleChange}
                disabled={loading}
                multiline
                rows={3}
              />
            </Grid>

            <Grid item xs={12}>
              <FormControlLabel
                control={
                  <Switch
                    checked={formData.isActive}
                    onChange={handleChange}
                    name="isActive"
                    disabled={loading}
                  />
                }
                label="Aktif"
              />
            </Grid>

            <Grid item xs={12}>
              <Box sx={{ display: 'flex', gap: 2, justifyContent: 'flex-end' }}>
                <Button
                  variant="outlined"
                  onClick={() => navigate('/companies')}
                  disabled={loading}
                >
                  İptal
                </Button>
                <Button
                  type="submit"
                  variant="contained"
                  startIcon={<SaveIcon />}
                  disabled={loading}
                >
                  {loading ? 'Kaydediliyor...' : 'Kaydet'}
                </Button>
              </Box>
            </Grid>
          </Grid>
        </form>
      </Paper>
    </Container>
  )
}

export default CompanyForm
