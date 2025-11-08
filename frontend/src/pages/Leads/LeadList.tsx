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
  TrendingUp as TrendingUpIcon,
  Refresh as RefreshIcon,
} from '@mui/icons-material'
import leadService from '../../services/leadService'
import { Lead } from '../../types/lead'
import ConfirmDialog from '../../components/ConfirmDialog'

function LeadList() {
  const navigate = useNavigate()
  const [leads, setLeads] = useState<Lead[]>([])
  const [totalCount, setTotalCount] = useState(0)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  
  const [filterStatus, setFilterStatus] = useState<string>('all')
  const [filterSource, setFilterSource] = useState<string>('all')
  const [searchTerm, setSearchTerm] = useState<string>('')
  const [page, setPage] = useState(0)
  const [rowsPerPage, setRowsPerPage] = useState(10)
  const [openConfirm, setOpenConfirm] = useState(false)
  const [selectedLeadId, setSelectedLeadId] = useState<string | null>(null)
  const [deletingLead, setDeletingLead] = useState<string>('')

  useEffect(() => {
    loadLeads()
  }, [page, rowsPerPage, filterStatus, filterSource])

  const loadLeads = async () => {
    try {
      setLoading(true)
      setError(null)
      const status = filterStatus === 'all' ? undefined : filterStatus
      const source = filterSource === 'all' ? undefined : filterSource
      const response = await leadService.getLeads(page + 1, rowsPerPage, status, source, true)
      setLeads(response.items || [])
      setTotalCount(response.totalCount || 0)
    } catch (err: any) {
      setError(err.message || 'Potansiyel müşteriler yüklenirken bir hata oluştu')
      setLeads([])
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
    const lead = leads.find(l => l.id === id)
    const leadTitle = lead ? lead.title : 'Potansiyel Müşteri'
    
    setSelectedLeadId(id)
    setDeletingLead(leadTitle)
    setOpenConfirm(true)
  }

  const handleConfirmDelete = async () => {
    if (selectedLeadId) {
      try {
        await leadService.deleteLead(selectedLeadId)
        setOpenConfirm(false)
        setSelectedLeadId(null)
        loadLeads()
      } catch (err: any) {
        alert(err.message || 'Potansiyel müşteri silinirken bir hata oluştu')
      }
    }
  }

  const handleCancelDelete = () => {
    setOpenConfirm(false)
    setSelectedLeadId(null)
    setDeletingLead('')
  }

  const handleFilterReset = () => {
    setFilterStatus('all')
    setFilterSource('all')
    setSearchTerm('')
    setPage(0)
  }

  const getStatusLabel = (status: string) => {
    const labels: any = {
      'new': 'Yeni',
      'contacted': 'İletişimde',
      'qualified': 'Nitelikli',
      'unqualified': 'Niteliksiz',
      'converted': 'Dönüştürüldü'
    }
    return labels[status] || status
  }

  const getStatusColor = (status: string) => {
    const colors: any = {
      'new': 'info',
      'contacted': 'warning',
      'qualified': 'success',
      'unqualified': 'error',
      'converted': 'success',
    }
    return colors[status] || 'default'
  }

  const getSourceLabel = (source: string) => {
    return source || 'Bilinmiyor'
  }

  const filteredLeads = leads.filter(lead =>
    lead.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
    lead.description?.toLowerCase().includes(searchTerm.toLowerCase()) ||
    lead.source?.toLowerCase().includes(searchTerm.toLowerCase())
  )

  return (
    <Container maxWidth="xl">
      <Box sx={{ mb: 4, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <Typography variant="h4" component="h1">
          Potansiyel Müşteri Yönetimi
        </Typography>
        <Box sx={{ display: 'flex', gap: 1 }}>
          <Button
            variant="outlined"
            color="primary"
            startIcon={<RefreshIcon />}
            onClick={loadLeads}
            disabled={loading}
          >
            Yenile
          </Button>
          <Button
            variant="contained"
            color="primary"
            startIcon={<AddIcon />}
            onClick={() => navigate('/leads/new')}
          >
            Yeni Potansiyel Müşteri
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
              fullWidth
              label="Ara"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              placeholder="Ad, email, şirket..."
            />
          </Grid>
          <Grid item xs={12} sm={3}>
            <TextField
              select
              fullWidth
              label="Durum"
              value={filterStatus}
              onChange={(e) => setFilterStatus(e.target.value)}
            >
              <MenuItem value="all">Tümü</MenuItem>
              <MenuItem value="new">Yeni</MenuItem>
              <MenuItem value="contacted">İletişimde</MenuItem>
              <MenuItem value="qualified">Nitelikli</MenuItem>
              <MenuItem value="unqualified">Niteliksiz</MenuItem>
              <MenuItem value="converted">Dönüştürüldü</MenuItem>
            </TextField>
          </Grid>
          <Grid item xs={12} sm={3}>
            <TextField
              select
              fullWidth
              label="Kaynak"
              value={filterSource}
              onChange={(e) => setFilterSource(e.target.value)}
            >
              <MenuItem value="all">Tümü</MenuItem>
              <MenuItem value="Website">Website</MenuItem>
              <MenuItem value="Referral">Referans</MenuItem>
              <MenuItem value="Campaign">Kampanya</MenuItem>
              <MenuItem value="SocialMedia">Sosyal Medya</MenuItem>
              <MenuItem value="Event">Etkinlik</MenuItem>
              <MenuItem value="Other">Diğer</MenuItem>
            </TextField>
          </Grid>
          <Grid item xs={12} sm={2}>
            <Button variant="outlined" onClick={handleFilterReset} fullWidth>
              Temizle
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
                  <TableCell>Başlık</TableCell>
                  <TableCell>Açıklama</TableCell>
                  <TableCell>Durum</TableCell>
                  <TableCell>Kaynak</TableCell>
                  <TableCell>Değer</TableCell>
                  <TableCell>Olasılık</TableCell>
                  <TableCell align="right">İşlemler</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {!filteredLeads || filteredLeads.length === 0 ? (
                  <TableRow>
                    <TableCell colSpan={7} align="center">
                      Kayıt bulunamadı
                    </TableCell>
                  </TableRow>
                ) : (
                  filteredLeads.map((lead) => (
                    <TableRow key={lead.id} hover>
                      <TableCell>
                        <Typography variant="body2" fontWeight="medium">
                          {lead.title}
                        </Typography>
                      </TableCell>
                      <TableCell>
                        <Typography variant="body2" color="text.secondary" noWrap sx={{ maxWidth: 200 }}>
                          {lead.description || '-'}
                        </Typography>
                      </TableCell>
                      <TableCell>
                        <Chip
                          label={getStatusLabel(lead.status)}
                          color={getStatusColor(lead.status)}
                          size="small"
                        />
                      </TableCell>
                      <TableCell>{getSourceLabel(lead.source)}</TableCell>
                      <TableCell>
                        {lead.value
                          ? `${lead.value.toLocaleString('tr-TR', { minimumFractionDigits: 2 })} ₺`
                          : '-'}
                      </TableCell>
                      <TableCell>
                        {lead.probability ? `%${lead.probability}` : '-'}
                      </TableCell>
                      <TableCell align="right">
                        <IconButton
                          size="small"
                          onClick={() => navigate(`/leads/${lead.id}`)}
                          title="Görüntüle"
                        >
                          <ViewIcon />
                        </IconButton>
                        <IconButton
                          size="small"
                          onClick={() => navigate(`/leads/${lead.id}/edit`)}
                          title="Düzenle"
                        >
                          <EditIcon />
                        </IconButton>
                        {lead.status === 'qualified' && (
                          <IconButton
                            size="small"
                            onClick={() => navigate(`/leads/${lead.id}/convert`)}
                            title="Fırsata Dönüştür"
                            color="success"
                          >
                            <TrendingUpIcon />
                          </IconButton>
                        )}
                        <IconButton
                          size="small"
                          onClick={() => handleDelete(lead.id)}
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
        title="Potansiyel Müşteriyi Sil"
        message={`"${deletingLead}" potansiyel müşterisini silmek istediğinizden emin misiniz? Bu işlem geri alınamaz.`}
        onConfirm={handleConfirmDelete}
        onCancel={handleCancelDelete}
        confirmText="Sil"
        cancelText="İptal"
        confirmColor="error"
      />
    </Container>
  )
}

export default LeadList
