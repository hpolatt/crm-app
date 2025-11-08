import { useState, useEffect } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { useDispatch, useSelector } from 'react-redux'
import {
  Container,
  Typography,
  Paper,
  TextField,
  Button,
  Grid,
  Box,
  MenuItem,
  FormControlLabel,
  Switch,
  CircularProgress,
  Alert,
} from '@mui/material'
import { Save as SaveIcon, Cancel as CancelIcon } from '@mui/icons-material'
import { AppDispatch, RootState } from '../../store'
import { createContact, updateContact, fetchContactById } from '../../store/slices/contactSlice'
import companyService from '../../services/companyService'
import { Company } from '../../types/company'
import { CreateContactDto, UpdateContactDto } from '../../types/contact'

function ContactForm() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const dispatch = useDispatch<AppDispatch>()
  const { selectedContact, loading, error } = useSelector((state: RootState) => state.contacts)

  const [companies, setCompanies] = useState<Company[]>([])
  const [formData, setFormData] = useState({
    firstName: '',
    lastName: '',
    email: '',
    phone: '',
    mobile: '',
    position: '',
    department: '',
    address: '',
    city: '',
    country: '',
    postalCode: '',
    birthDate: '',
    notes: '',
    companyId: '',
    isPrimary: false,
    isActive: true,
  })
  const [validationErrors, setValidationErrors] = useState<Record<string, string>>({})

  const isEditMode = Boolean(id && id !== 'new')

  // Load companies
  useEffect(() => {
    const loadCompanies = async () => {
      try {
        const response = await companyService.getCompanies(1, 100, true)
        setCompanies(response.items)
      } catch (err) {
        console.error('Failed to load companies:', err)
      }
    }
    loadCompanies()
  }, [])

  // Load contact for edit
  useEffect(() => {
    if (isEditMode && id) {
      dispatch(fetchContactById(id))
    }
  }, [dispatch, id, isEditMode])

  // Populate form with contact data
  useEffect(() => {
    if (isEditMode && selectedContact) {
      setFormData({
        firstName: selectedContact.firstName,
        lastName: selectedContact.lastName,
        email: selectedContact.email || '',
        phone: selectedContact.phone || '',
        mobile: selectedContact.mobile || '',
        position: selectedContact.position || '',
        department: selectedContact.department || '',
        address: selectedContact.address || '',
        city: selectedContact.city || '',
        country: selectedContact.country || '',
        postalCode: selectedContact.postalCode || '',
        birthDate: selectedContact.birthDate || '',
        notes: selectedContact.notes || '',
        companyId: selectedContact.companyId || '',
        isPrimary: selectedContact.isPrimary,
        isActive: selectedContact.isActive,
      })
    }
  }, [isEditMode, selectedContact])

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value, checked } = e.target
    setFormData((prev) => ({
      ...prev,
      [name]: name === 'isActive' || name === 'isPrimary' ? checked : value,
    }))
    // Clear validation error for this field
    if (validationErrors[name]) {
      setValidationErrors((prev) => {
        const newErrors = { ...prev }
        delete newErrors[name]
        return newErrors
      })
    }
  }

  const validateForm = (): boolean => {
    const errors: Record<string, string> = {}

    if (!formData.firstName.trim()) {
      errors.firstName = 'Ad zorunludur'
    }
    if (!formData.lastName.trim()) {
      errors.lastName = 'Soyad zorunludur'
    }
    if (formData.email && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email)) {
      errors.email = 'Geçerli bir email adresi giriniz'
    }

    setValidationErrors(errors)
    return Object.keys(errors).length === 0
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()

    if (!validateForm()) {
      return
    }

    try {
      const contactData: CreateContactDto | UpdateContactDto = {
        firstName: formData.firstName.trim(),
        lastName: formData.lastName.trim(),
        email: formData.email.trim() || undefined,
        phone: formData.phone.trim() || undefined,
        mobile: formData.mobile.trim() || undefined,
        position: formData.position.trim() || undefined,
        department: formData.department.trim() || undefined,
        address: formData.address.trim() || undefined,
        city: formData.city.trim() || undefined,
        country: formData.country.trim() || undefined,
        postalCode: formData.postalCode.trim() || undefined,
        birthDate: formData.birthDate || undefined,
        notes: formData.notes.trim() || undefined,
        companyId: formData.companyId || undefined,
        isPrimary: formData.isPrimary,
        isActive: formData.isActive,
      }

      if (isEditMode && id) {
        await dispatch(updateContact({ id, contact: contactData as UpdateContactDto })).unwrap()
      } else {
        await dispatch(createContact(contactData as CreateContactDto)).unwrap()
      }

      navigate('/contacts')
    } catch (err: any) {
      console.error('Failed to save contact:', err)
    }
  }

  const handleCancel = () => {
    navigate('/contacts')
  }

  if (isEditMode && loading && !selectedContact) {
    return (
      <Container maxWidth="md">
        <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}>
          <CircularProgress />
        </Box>
      </Container>
    )
  }

  return (
    <Container maxWidth="md">
      <Box sx={{ mb: 4 }}>
        <Typography variant="h4" component="h1">
          {isEditMode ? 'Kişi Düzenle' : 'Yeni Kişi Ekle'}
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
            {/* Personal Information */}
            <Grid item xs={12}>
              <Typography variant="h6" gutterBottom>
                Kişisel Bilgiler
              </Typography>
            </Grid>
            <Grid item xs={12} sm={6}>
              <TextField
                required
                fullWidth
                label="Ad"
                name="firstName"
                value={formData.firstName}
                onChange={handleChange}
                error={Boolean(validationErrors.firstName)}
                helperText={validationErrors.firstName}
              />
            </Grid>
            <Grid item xs={12} sm={6}>
              <TextField
                required
                fullWidth
                label="Soyad"
                name="lastName"
                value={formData.lastName}
                onChange={handleChange}
                error={Boolean(validationErrors.lastName)}
                helperText={validationErrors.lastName}
              />
            </Grid>
            <Grid item xs={12} sm={6}>
              <TextField
                fullWidth
                label="Email"
                name="email"
                type="email"
                value={formData.email}
                onChange={handleChange}
                error={Boolean(validationErrors.email)}
                helperText={validationErrors.email}
              />
            </Grid>
            <Grid item xs={12} sm={6}>
              <TextField
                fullWidth
                label="Telefon"
                name="phone"
                value={formData.phone}
                onChange={handleChange}
              />
            </Grid>
            <Grid item xs={12} sm={6}>
              <TextField
                fullWidth
                label="Mobil Telefon"
                name="mobile"
                value={formData.mobile}
                onChange={handleChange}
              />
            </Grid>
            <Grid item xs={12} sm={6}>
              <TextField
                fullWidth
                label="Doğum Tarihi"
                name="birthDate"
                type="date"
                value={formData.birthDate}
                onChange={handleChange}
                InputLabelProps={{ shrink: true }}
              />
            </Grid>

            {/* Company Information */}
            <Grid item xs={12}>
              <Typography variant="h6" gutterBottom sx={{ mt: 2 }}>
                Şirket Bilgileri
              </Typography>
            </Grid>
            <Grid item xs={12} sm={6}>
              <TextField
                select
                fullWidth
                label="Şirket"
                name="companyId"
                value={formData.companyId}
                onChange={handleChange}
              >
                <MenuItem value="">Seçiniz</MenuItem>
                {companies.map((company) => (
                  <MenuItem key={company.id} value={company.id}>
                    {company.name}
                  </MenuItem>
                ))}
              </TextField>
            </Grid>
            <Grid item xs={12} sm={6}>
              <TextField
                fullWidth
                label="Pozisyon"
                name="position"
                value={formData.position}
                onChange={handleChange}
              />
            </Grid>
            <Grid item xs={12} sm={6}>
              <TextField
                fullWidth
                label="Departman"
                name="department"
                value={formData.department}
                onChange={handleChange}
              />
            </Grid>

            {/* Address Information */}
            <Grid item xs={12}>
              <Typography variant="h6" gutterBottom sx={{ mt: 2 }}>
                Adres Bilgileri
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <TextField
                fullWidth
                label="Adres"
                name="address"
                value={formData.address}
                onChange={handleChange}
                multiline
                rows={2}
              />
            </Grid>
            <Grid item xs={12} sm={4}>
              <TextField
                fullWidth
                label="Şehir"
                name="city"
                value={formData.city}
                onChange={handleChange}
              />
            </Grid>
            <Grid item xs={12} sm={4}>
              <TextField
                fullWidth
                label="Ülke"
                name="country"
                value={formData.country}
                onChange={handleChange}
              />
            </Grid>
            <Grid item xs={12} sm={4}>
              <TextField
                fullWidth
                label="Posta Kodu"
                name="postalCode"
                value={formData.postalCode}
                onChange={handleChange}
              />
            </Grid>

            {/* Additional Information */}
            <Grid item xs={12}>
              <Typography variant="h6" gutterBottom sx={{ mt: 2 }}>
                Ek Bilgiler
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <TextField
                fullWidth
                label="Notlar"
                name="notes"
                value={formData.notes}
                onChange={handleChange}
                multiline
                rows={3}
              />
            </Grid>
            <Grid item xs={12} sm={6}>
              <FormControlLabel
                control={
                  <Switch
                    checked={formData.isPrimary}
                    onChange={handleChange}
                    name="isPrimary"
                    color="primary"
                  />
                }
                label="Birincil İletişim"
              />
            </Grid>
            <Grid item xs={12} sm={6}>
              <FormControlLabel
                control={
                  <Switch
                    checked={formData.isActive}
                    onChange={handleChange}
                    name="isActive"
                    color="primary"
                  />
                }
                label="Aktif"
              />
            </Grid>
            <Grid item xs={12}>
              <Box sx={{ display: 'flex', gap: 2, justifyContent: 'flex-end' }}>
                <Button variant="outlined" startIcon={<CancelIcon />} onClick={handleCancel}>
                  İptal
                </Button>
                <Button
                  type="submit"
                  variant="contained"
                  color="primary"
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

export default ContactForm
