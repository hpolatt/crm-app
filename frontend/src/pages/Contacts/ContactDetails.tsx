import { useEffect } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { useDispatch, useSelector } from 'react-redux'
import {
  Container,
  Typography,
  Paper,
  Box,
  Grid,
  Chip,
  Button,
  Divider,
  CircularProgress,
  Alert,
} from '@mui/material'
import {
  Edit as EditIcon,
  ArrowBack as ArrowBackIcon,
  Email as EmailIcon,
  Phone as PhoneIcon,
  Business as BusinessIcon,
  Work as WorkIcon,
  Person as PersonIcon,
} from '@mui/icons-material'
import { AppDispatch, RootState } from '../../store'
import { fetchContactById, clearSelectedContact } from '../../store/slices/contactSlice'
import { format } from 'date-fns'
import { tr } from 'date-fns/locale'

function ContactDetails() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const dispatch = useDispatch<AppDispatch>()
  const { selectedContact, loading, error } = useSelector((state: RootState) => state.contacts)

  useEffect(() => {
    if (id) {
      dispatch(fetchContactById(id))
    }
    return () => {
      dispatch(clearSelectedContact())
    }
  }, [dispatch, id])

  if (loading || !selectedContact) {
    return (
      <Container maxWidth="md">
        <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}>
          <CircularProgress />
        </Box>
      </Container>
    )
  }

  if (error) {
    return (
      <Container maxWidth="md">
        <Alert severity="error" sx={{ mt: 2 }}>
          {error}
        </Alert>
      </Container>
    )
  }

  return (
    <Container maxWidth="md">
      <Box sx={{ mb: 4, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
          <Button startIcon={<ArrowBackIcon />} onClick={() => navigate('/contacts')}>
            Geri
          </Button>
          <Typography variant="h4" component="h1">
            Kişi Detayları
          </Typography>
        </Box>
        <Button
          variant="contained"
          color="primary"
          startIcon={<EditIcon />}
          onClick={() => navigate(`/contacts/${id}/edit`)}
        >
          Düzenle
        </Button>
      </Box>

      <Paper sx={{ p: 3 }}>
        {/* Header */}
        <Box sx={{ mb: 3, display: 'flex', alignItems: 'center', gap: 2 }}>
          <PersonIcon sx={{ fontSize: 48, color: 'primary.main' }} />
          <Box sx={{ flex: 1 }}>
            <Typography variant="h5">
              {selectedContact.firstName} {selectedContact.lastName}
            </Typography>
            <Chip
              label={selectedContact.isActive ? 'Aktif' : 'Pasif'}
              color={selectedContact.isActive ? 'success' : 'default'}
              size="small"
              sx={{ mt: 1 }}
            />
          </Box>
        </Box>

        <Divider sx={{ mb: 3 }} />

        {/* Contact Information */}
        <Grid container spacing={3}>
          <Grid item xs={12}>
            <Typography variant="h6" gutterBottom>
              İletişim Bilgileri
            </Typography>
          </Grid>

          <Grid item xs={12} sm={6}>
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 2 }}>
              <EmailIcon color="action" />
              <Box>
                <Typography variant="caption" color="text.secondary">
                  Email
                </Typography>
                <Typography variant="body1">{selectedContact.email || '-'}</Typography>
              </Box>
            </Box>
          </Grid>

          <Grid item xs={12} sm={6}>
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 2 }}>
              <PhoneIcon color="action" />
              <Box>
                <Typography variant="caption" color="text.secondary">
                  Telefon
                </Typography>
                <Typography variant="body1">{selectedContact.phone || '-'}</Typography>
              </Box>
            </Box>
          </Grid>

          <Grid item xs={12} sm={6}>
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 2 }}>
              <PhoneIcon color="action" />
              <Box>
                <Typography variant="caption" color="text.secondary">
                  Mobil Telefon
                </Typography>
                <Typography variant="body1">{selectedContact.mobile || '-'}</Typography>
              </Box>
            </Box>
          </Grid>

          {selectedContact.birthDate && (
            <Grid item xs={12} sm={6}>
              <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 2 }}>
                <PersonIcon color="action" />
                <Box>
                  <Typography variant="caption" color="text.secondary">
                    Doğum Tarihi
                  </Typography>
                  <Typography variant="body1">
                    {format(new Date(selectedContact.birthDate), 'dd MMMM yyyy', { locale: tr })}
                  </Typography>
                </Box>
              </Box>
            </Grid>
          )}
        </Grid>

        <Divider sx={{ my: 3 }} />

        {/* Company Information */}
        <Grid container spacing={3}>
          <Grid item xs={12}>
            <Typography variant="h6" gutterBottom>
              Şirket Bilgileri
            </Typography>
          </Grid>

          <Grid item xs={12} sm={6}>
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 2 }}>
              <BusinessIcon color="action" />
              <Box>
                <Typography variant="caption" color="text.secondary">
                  Şirket
                </Typography>
                <Typography variant="body1">{selectedContact.companyName || '-'}</Typography>
              </Box>
            </Box>
          </Grid>

          <Grid item xs={12} sm={6}>
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 2 }}>
              <WorkIcon color="action" />
              <Box>
                <Typography variant="caption" color="text.secondary">
                  Pozisyon
                </Typography>
                <Typography variant="body1">{selectedContact.position || '-'}</Typography>
              </Box>
            </Box>
          </Grid>

          {selectedContact.department && (
            <Grid item xs={12} sm={6}>
              <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 2 }}>
                <WorkIcon color="action" />
                <Box>
                  <Typography variant="caption" color="text.secondary">
                    Departman
                  </Typography>
                  <Typography variant="body1">{selectedContact.department}</Typography>
                </Box>
              </Box>
            </Grid>
          )}

          <Grid item xs={12} sm={6}>
            <Box sx={{ mb: 2 }}>
              <Typography variant="caption" color="text.secondary">
                Birincil İletişim
              </Typography>
              <Typography variant="body1">
                <Chip
                  label={selectedContact.isPrimary ? 'Evet' : 'Hayır'}
                  color={selectedContact.isPrimary ? 'primary' : 'default'}
                  size="small"
                />
              </Typography>
            </Box>
          </Grid>
        </Grid>

        {(selectedContact.address || selectedContact.city || selectedContact.country || selectedContact.postalCode) && (
          <>
            <Divider sx={{ my: 3 }} />
            <Grid container spacing={3}>
              <Grid item xs={12}>
                <Typography variant="h6" gutterBottom>
                  Adres Bilgileri
                </Typography>
              </Grid>

              {selectedContact.address && (
                <Grid item xs={12}>
                  <Typography variant="caption" color="text.secondary">
                    Adres
                  </Typography>
                  <Typography variant="body1">{selectedContact.address}</Typography>
                </Grid>
              )}

              {selectedContact.city && (
                <Grid item xs={12} sm={4}>
                  <Typography variant="caption" color="text.secondary">
                    Şehir
                  </Typography>
                  <Typography variant="body1">{selectedContact.city}</Typography>
                </Grid>
              )}

              {selectedContact.country && (
                <Grid item xs={12} sm={4}>
                  <Typography variant="caption" color="text.secondary">
                    Ülke
                  </Typography>
                  <Typography variant="body1">{selectedContact.country}</Typography>
                </Grid>
              )}

              {selectedContact.postalCode && (
                <Grid item xs={12} sm={4}>
                  <Typography variant="caption" color="text.secondary">
                    Posta Kodu
                  </Typography>
                  <Typography variant="body1">{selectedContact.postalCode}</Typography>
                </Grid>
              )}
            </Grid>
          </>
        )}

        {selectedContact.notes && (
          <>
            <Divider sx={{ my: 3 }} />
            <Grid container spacing={3}>
              <Grid item xs={12}>
                <Typography variant="h6" gutterBottom>
                  Notlar
                </Typography>
                <Typography variant="body1">{selectedContact.notes}</Typography>
              </Grid>
            </Grid>
          </>
        )}

        <Divider sx={{ my: 3 }} />

        {/* System Information */}
        <Grid container spacing={3}>
          <Grid item xs={12}>
            <Typography variant="h6" gutterBottom>
              Sistem Bilgileri
            </Typography>
          </Grid>

          <Grid item xs={12} sm={6}>
            <Typography variant="caption" color="text.secondary">
              Oluşturulma Tarihi
            </Typography>
            <Typography variant="body2">
              {format(new Date(selectedContact.createdAt), 'dd MMMM yyyy HH:mm', { locale: tr })}
            </Typography>
          </Grid>

          {selectedContact.updatedAt && (
            <Grid item xs={12} sm={6}>
              <Typography variant="caption" color="text.secondary">
                Son Güncelleme
              </Typography>
              <Typography variant="body2">
                {format(new Date(selectedContact.updatedAt), 'dd MMMM yyyy HH:mm', { locale: tr })}
              </Typography>
            </Grid>
          )}
        </Grid>
      </Paper>
    </Container>
  )
}

export default ContactDetails
