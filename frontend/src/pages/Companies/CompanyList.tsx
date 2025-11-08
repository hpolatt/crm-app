import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import {
  Container,
  Typography,
  Button,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TablePagination,
  IconButton,
  Chip,
  Box,
  TextField,
  MenuItem,
  Grid,
  CircularProgress,
  Alert,
} from '@mui/material'
import {
  Add as AddIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  Visibility as ViewIcon,
  Business as BusinessIcon,
  Language as LanguageIcon,
  Email as EmailIcon,
  Phone as PhoneIcon,
  Refresh as RefreshIcon,
} from '@mui/icons-material'
import companyService from '../../services/companyService'
import { Company } from '../../types/company'
import ConfirmDialog from '../../components/ConfirmDialog'

function CompanyList() {
  const navigate = useNavigate()
  const [companies, setCompanies] = useState<Company[]>([])
  const [totalCount, setTotalCount] = useState(0)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  
  const [filterIsActive, setFilterIsActive] = useState<string>('all')
  const [searchTerm, setSearchTerm] = useState<string>('')
  const [page, setPage] = useState(0)
  const [rowsPerPage, setRowsPerPage] = useState(10)
  const [openConfirm, setOpenConfirm] = useState(false)
  const [selectedCompanyId, setSelectedCompanyId] = useState<string | null>(null)
  const [deletingCompany, setDeletingCompany] = useState<string>('')

  // Load companies
  useEffect(() => {
    loadCompanies()
  }, [page, rowsPerPage, filterIsActive])

  const loadCompanies = async () => {
    try {
      setLoading(true)
      setError(null)
      const isActive = filterIsActive === 'all' ? undefined : filterIsActive === 'true'
      const response = await companyService.getCompanies(page + 1, rowsPerPage, isActive)
      setCompanies(response.items || [])
      setTotalCount(response.totalCount || 0)
    } catch (err: any) {
      setError(err.message || 'Şirketler yüklenirken bir hata oluştu')
      setCompanies([])
    } finally {
      setLoading(false)
    }
  }

  const handleChangePage = (_event: unknown, newPage: number) => {
    setPage(newPage)
  }

  const handleChangeRowsPerPage = (event: React.ChangeEvent<HTMLInputElement>) => {
    setRowsPerPage(parseInt(event.target.value, 10))
    setPage(0)
  }

  const handleDelete = async (id: string) => {
    const company = companies.find(c => c.id === id)
    const companyName = company ? company.name : 'Şirket'
    
    setSelectedCompanyId(id)
    setDeletingCompany(companyName)
    setOpenConfirm(true)
  }

  const handleConfirmDelete = async () => {
    if (selectedCompanyId) {
      try {
        await companyService.deleteCompany(selectedCompanyId)
        setOpenConfirm(false)
        setSelectedCompanyId(null)
        loadCompanies()
      } catch (err: any) {
        alert(err.message || 'Şirket silinirken bir hata oluştu')
      }
    }
  }

  const handleCancelDelete = () => {
    setOpenConfirm(false)
    setSelectedCompanyId(null)
    setDeletingCompany('')
  }

  const handleFilterReset = () => {
    setFilterIsActive('all')
    setSearchTerm('')
    setPage(0)
  }

  // Filter companies by search term
  const filteredCompanies = companies.filter(company =>
    company.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
    company.email?.toLowerCase().includes(searchTerm.toLowerCase()) ||
    company.phone?.toLowerCase().includes(searchTerm.toLowerCase())
  )

  return (
    <Container maxWidth="xl">
      <Box sx={{ mb: 4, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <Typography variant="h4" component="h1">
          Şirket Yönetimi
        </Typography>
        <Box sx={{ display: 'flex', gap: 1 }}>
          <Button
            variant="outlined"
            color="primary"
            startIcon={<RefreshIcon />}
            onClick={loadCompanies}
            disabled={loading}
          >
            Yenile
          </Button>
          <Button
            variant="contained"
            color="primary"
            startIcon={<AddIcon />}
            onClick={() => navigate('/companies/new')}
          >
            Yeni Şirket Ekle
          </Button>
        </Box>
      </Box>

      {error && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {error}
        </Alert>
      )}

      {/* Filters */}
      <Paper sx={{ p: 2, mb: 2 }}>
        <Grid container spacing={2} alignItems="center">
          <Grid item xs={12} sm={6}>
            <TextField
              fullWidth
              label="Ara (Şirket adı, email, telefon)"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              placeholder="Ara..."
            />
          </Grid>
          <Grid item xs={12} sm={3}>
            <TextField
              select
              fullWidth
              label="Durum"
              value={filterIsActive}
              onChange={(e) => setFilterIsActive(e.target.value)}
            >
              <MenuItem value="all">Tümü</MenuItem>
              <MenuItem value="true">Aktif</MenuItem>
              <MenuItem value="false">Pasif</MenuItem>
            </TextField>
          </Grid>
          <Grid item xs={12} sm={3}>
            <Button variant="outlined" onClick={handleFilterReset} fullWidth>
              Filtreleri Temizle
            </Button>
          </Grid>
        </Grid>
      </Paper>

      {/* Table */}
      <TableContainer component={Paper}>
        {loading ? (
          <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}>
            <CircularProgress />
          </Box>
        ) : (
          <>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell>Şirket Adı</TableCell>
                  <TableCell>Email</TableCell>
                  <TableCell>Telefon</TableCell>
                  <TableCell>Website</TableCell>
                  <TableCell>Adres</TableCell>
                  <TableCell>Durum</TableCell>
                  <TableCell align="right">İşlemler</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {!filteredCompanies || filteredCompanies.length === 0 ? (
                  <TableRow>
                    <TableCell colSpan={7} align="center">
                      Kayıt bulunamadı
                    </TableCell>
                  </TableRow>
                ) : (
                  filteredCompanies.map((company) => (
                    <TableRow key={company.id} hover>
                      <TableCell>
                        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                          <BusinessIcon fontSize="small" color="action" />
                          {company.name}
                        </Box>
                      </TableCell>
                      <TableCell>
                        {company.email ? (
                          <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                            <EmailIcon fontSize="small" color="action" />
                            {company.email}
                          </Box>
                        ) : (
                          '-'
                        )}
                      </TableCell>
                      <TableCell>
                        {company.phone ? (
                          <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                            <PhoneIcon fontSize="small" color="action" />
                            {company.phone}
                          </Box>
                        ) : (
                          '-'
                        )}
                      </TableCell>
                      <TableCell>
                        {company.website ? (
                          <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                            <LanguageIcon fontSize="small" color="action" />
                            <a href={company.website} target="_blank" rel="noopener noreferrer">
                              {company.website}
                            </a>
                          </Box>
                        ) : (
                          '-'
                        )}
                      </TableCell>
                      <TableCell>{company.address || '-'}</TableCell>
                      <TableCell>
                        <Chip
                          label={company.isActive ? 'Aktif' : 'Pasif'}
                          color={company.isActive ? 'success' : 'default'}
                          size="small"
                        />
                      </TableCell>
                      <TableCell align="right">
                        <IconButton
                          size="small"
                          onClick={() => navigate(`/companies/${company.id}`)}
                          title="Görüntüle"
                        >
                          <ViewIcon />
                        </IconButton>
                        <IconButton
                          size="small"
                          onClick={() => navigate(`/companies/${company.id}/edit`)}
                          title="Düzenle"
                        >
                          <EditIcon />
                        </IconButton>
                        <IconButton
                          size="small"
                          onClick={() => handleDelete(company.id)}
                          title="Sil"
                          color="error"
                        >
                          <DeleteIcon />
                        </IconButton>
                      </TableCell>
                    </TableRow>
                  ))
                )}
              </TableBody>
            </Table>
            <TablePagination
              rowsPerPageOptions={[5, 10, 25, 50]}
              component="div"
              count={totalCount}
              rowsPerPage={rowsPerPage}
              page={page}
              onPageChange={handleChangePage}
              onRowsPerPageChange={handleChangeRowsPerPage}
              labelRowsPerPage="Sayfa başına kayıt:"
              labelDisplayedRows={({ from, to, count }) => `${from}-${to} / ${count}`}
            />
          </>
        )}
      </TableContainer>

      {/* Delete Confirmation Dialog */}
      <ConfirmDialog
        open={openConfirm}
        title="Şirketi Sil"
        message={`"${deletingCompany}" şirketini silmek istediğinizden emin misiniz? Bu işlem geri alınamaz.`}
        onConfirm={handleConfirmDelete}
        onCancel={handleCancelDelete}
        confirmText="Sil"
        cancelText="İptal"
        confirmColor="error"
      />
    </Container>
  )
}

export default CompanyList
