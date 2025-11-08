import { useState, useEffect } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import {
  Container,
  Typography,
  Button,
  Paper,
  Box,
  Grid,
  Chip,
  CircularProgress,
  Alert,
  Divider,
  List,
  ListItem,
  ListItemText,
} from '@mui/material'
import {
  ArrowBack as ArrowBackIcon,
  Edit as EditIcon,
  Business as BusinessIcon,
  Email as EmailIcon,
  Phone as PhoneIcon,
  Language as LanguageIcon,
  LocationOn as LocationOnIcon,
} from '@mui/icons-material'
import companyService from '../../services/companyService'
import { Company } from '../../types/company'

function CompanyDetails() {
  const navigate = useNavigate()
  const { id } = useParams<{ id: string }>()
  
  const [company, setCompany] = useState<Company | null>(null)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    if (id) {
      loadCompany(id)
    }
  }, [id])

  const loadCompany = async (companyId: string) => {
    try {
      setLoading(true)
      const data = await companyService.getCompanyById(companyId)
      setCompany(data)
    } catch (err: any) {
      setError(err.message || 'Şirket yüklenirken bir hata oluştu')
    } finally {
      setLoading(false)
    }
  }

  if (loading) {
    return (
      <Container maxWidth="md">
        <Box sx={{ display: 'flex', justifyContent: 'center', mt: 4 }}>
          <CircularProgress />
        </Box>
      </Container>
    )
  }

  if (error) {
    return (
      <Container maxWidth="md">
        <Alert severity="error" sx={{ mt: 4 }}>
          {error}
        </Alert>
      </Container>
    )
  }

  if (!company) {
    return (
      <Container maxWidth="md">
        <Alert severity="warning" sx={{ mt: 4 }}>
          Şirket bulunamadı
        </Alert>
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
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <Typography variant="h4" component="h1">
            Şirket Detayları
          </Typography>
          <Button
            variant="contained"
            startIcon={<EditIcon />}
            onClick={() => navigate(`/companies/${id}/edit`)}
          >
            Düzenle
          </Button>
        </Box>
      </Box>

      <Paper sx={{ p: 3 }}>
        <Grid container spacing={3}>
          <Grid item xs={12}>
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 2, mb: 2 }}>
              <BusinessIcon fontSize="large" color="primary" />
              <Box>
                <Typography variant="h5" component="h2">
                  {company.name}
                </Typography>
                <Chip
                  label={company.isActive ? 'Aktif' : 'Pasif'}
                  color={company.isActive ? 'success' : 'default'}
                  size="small"
                  sx={{ mt: 1 }}
                />
              </Box>
            </Box>
          </Grid>

          <Grid item xs={12}>
            <Divider />
          </Grid>

          <Grid item xs={12}>
            <Typography variant="h6" gutterBottom>
              İletişim Bilgileri
            </Typography>
            <List>
              {company.email && (
                <ListItem>
                  <EmailIcon sx={{ mr: 2 }} color="action" />
                  <ListItemText
                    primary="Email"
                    secondary={
                      <a href={`mailto:${company.email}`}>{company.email}</a>
                    }
                  />
                </ListItem>
              )}
              {company.phone && (
                <ListItem>
                  <PhoneIcon sx={{ mr: 2 }} color="action" />
                  <ListItemText
                    primary="Telefon"
                    secondary={
                      <a href={`tel:${company.phone}`}>{company.phone}</a>
                    }
                  />
                </ListItem>
              )}
              {company.website && (
                <ListItem>
                  <LanguageIcon sx={{ mr: 2 }} color="action" />
                  <ListItemText
                    primary="Website"
                    secondary={
                      <a href={company.website} target="_blank" rel="noopener noreferrer">
                        {company.website}
                      </a>
                    }
                  />
                </ListItem>
              )}
              {company.address && (
                <ListItem>
                  <LocationOnIcon sx={{ mr: 2 }} color="action" />
                  <ListItemText primary="Adres" secondary={company.address} />
                </ListItem>
              )}
            </List>
          </Grid>

          <Grid item xs={12}>
            <Divider />
          </Grid>

          <Grid item xs={12}>
            <Typography variant="h6" gutterBottom>
              Kayıt Bilgileri
            </Typography>
            <List>
              <ListItem>
                <ListItemText
                  primary="Oluşturulma Tarihi"
                  secondary={new Date(company.createdAt).toLocaleString('tr-TR')}
                />
              </ListItem>
              <ListItem>
                <ListItemText
                  primary="Son Güncelleme"
                  secondary={new Date(company.updatedAt).toLocaleString('tr-TR')}
                />
              </ListItem>
            </List>
          </Grid>
        </Grid>
      </Paper>
    </Container>
  )
}

export default CompanyDetails
