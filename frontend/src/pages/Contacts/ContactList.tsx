import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import { useDispatch, useSelector } from 'react-redux'
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
  Email as EmailIcon,
  Phone as PhoneIcon,
  Refresh as RefreshIcon,
} from '@mui/icons-material'
import { AppDispatch, RootState } from '../../store'
import { fetchContacts, deleteContact } from '../../store/slices/contactSlice'
import companyService from '../../services/companyService'
import { Company } from '../../types/company'
import ConfirmDialog from '../../components/ConfirmDialog'

function ContactList() {
  const navigate = useNavigate()
  const dispatch = useDispatch<AppDispatch>()
  const { contacts, totalCount, loading, error } = useSelector(
    (state: RootState) => state.contacts
  )

  const [companies, setCompanies] = useState<Company[]>([])
  const [filterCompanyId, setFilterCompanyId] = useState<string>('')
  const [filterIsActive, setFilterIsActive] = useState<string>('all')
  const [page, setPage] = useState(0)
  const [rowsPerPage, setRowsPerPage] = useState(10)
  const [openConfirm, setOpenConfirm] = useState(false)
  const [selectedContactId, setSelectedContactId] = useState<string | null>(null)
  const [deletingContact, setDeletingContact] = useState<string>('')

  // Load companies for filter
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

  // Load contacts
  useEffect(() => {
    const params: any = {
      pageNumber: page + 1,
      pageSize: rowsPerPage,
    }
    if (filterCompanyId) params.companyId = filterCompanyId
    if (filterIsActive !== 'all') params.isActive = filterIsActive === 'true'

    dispatch(fetchContacts(params))
  }, [dispatch, page, rowsPerPage, filterCompanyId, filterIsActive])

  const handleChangePage = (_event: unknown, newPage: number) => {
    setPage(newPage)
  }

  const handleChangeRowsPerPage = (event: React.ChangeEvent<HTMLInputElement>) => {
    setRowsPerPage(parseInt(event.target.value, 10))
    setPage(0)
  }

  const handleDelete = async (id: string) => {
    const contact = contacts.find(c => c.id === id)
    const contactName = contact ? `${contact.firstName} ${contact.lastName}` : 'Kişi'
    
    setSelectedContactId(id)
    setDeletingContact(contactName)
    setOpenConfirm(true)
  }

  const handleConfirmDelete = async () => {
    if (selectedContactId) {
      try {
        await dispatch(deleteContact(selectedContactId)).unwrap()
        setOpenConfirm(false)
        setSelectedContactId(null)
      } catch (err) {
        console.error('Failed to delete contact:', err)
      }
    }
  }

  const handleCancelDelete = () => {
    setOpenConfirm(false)
    setSelectedContactId(null)
    setDeletingContact('')
  }

  const handleFilterReset = () => {
    setFilterCompanyId('')
    setFilterIsActive('all')
    setPage(0)
  }

  const handleRefresh = () => {
    const params: any = {
      pageNumber: page + 1,
      pageSize: rowsPerPage,
    }
    if (filterCompanyId) params.companyId = filterCompanyId
    if (filterIsActive !== 'all') params.isActive = filterIsActive === 'true'
    dispatch(fetchContacts(params))
  }

  return (
    <Container maxWidth="xl">
      <Box sx={{ mb: 4, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <Typography variant="h4" component="h1">
          İletişim Yönetimi
        </Typography>
        <Box sx={{ display: 'flex', gap: 1 }}>
          <Button
            variant="outlined"
            color="primary"
            startIcon={<RefreshIcon />}
            onClick={handleRefresh}
            disabled={loading}
          >
            Yenile
          </Button>
          <Button
            variant="contained"
            color="primary"
            startIcon={<AddIcon />}
            onClick={() => navigate('/contacts/new')}
          >
            Yeni Kişi Ekle
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
          <Grid item xs={12} sm={4}>
            <TextField
              select
              fullWidth
              label="Şirket"
              value={filterCompanyId}
              onChange={(e) => setFilterCompanyId(e.target.value)}
            >
              <MenuItem value="">Tümü</MenuItem>
              {companies && Array.isArray(companies) && companies.map((company) => (
                <MenuItem key={company.id} value={company.id}>
                  {company.name}
                </MenuItem>
              ))}
            </TextField>
          </Grid>
          <Grid item xs={12} sm={4}>
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
          <Grid item xs={12} sm={4}>
            <Button variant="outlined" onClick={handleFilterReset}>
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
                  <TableCell>Ad Soyad</TableCell>
                  <TableCell>Email</TableCell>
                  <TableCell>Telefon</TableCell>
                  <TableCell>Pozisyon</TableCell>
                  <TableCell>Şirket</TableCell>
                  <TableCell>Durum</TableCell>
                  <TableCell align="right">İşlemler</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {!contacts || contacts.length === 0 ? (
                  <TableRow>
                    <TableCell colSpan={7} align="center">
                      Kayıt bulunamadı
                    </TableCell>
                  </TableRow>
                ) : (
                  contacts.map((contact) => (
                    <TableRow key={contact.id} hover>
                      <TableCell>
                        {contact.firstName} {contact.lastName}
                      </TableCell>
                      <TableCell>
                        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                          <EmailIcon fontSize="small" color="action" />
                          {contact.email}
                        </Box>
                      </TableCell>
                      <TableCell>
                        {contact.phone ? (
                          <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                            <PhoneIcon fontSize="small" color="action" />
                            {contact.phone}
                          </Box>
                        ) : (
                          '-'
                        )}
                      </TableCell>
                      <TableCell>{contact.position || '-'}</TableCell>
                      <TableCell>{contact.companyName || '-'}</TableCell>
                      <TableCell>
                        <Chip
                          label={contact.isActive ? 'Aktif' : 'Pasif'}
                          color={contact.isActive ? 'success' : 'default'}
                          size="small"
                        />
                      </TableCell>
                      <TableCell align="right">
                        <IconButton
                          size="small"
                          onClick={() => navigate(`/contacts/${contact.id}`)}
                          title="Görüntüle"
                        >
                          <ViewIcon />
                        </IconButton>
                        <IconButton
                          size="small"
                          onClick={() => navigate(`/contacts/${contact.id}/edit`)}
                          title="Düzenle"
                        >
                          <EditIcon />
                        </IconButton>
                        <IconButton
                          size="small"
                          onClick={() => handleDelete(contact.id)}
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
        title="Kişi Sil"
        message={`"${deletingContact}" kişisini silmek istediğinizden emin misiniz? Bu işlem geri alınamaz.`}
        onConfirm={handleConfirmDelete}
        onCancel={handleCancelDelete}
        confirmText="Sil"
        cancelText="İptal"
        confirmColor="error"
      />
    </Container>
  )
}

export default ContactList
