import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Box,
  Paper,
  Typography,
  Button,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TablePagination,
  IconButton,
  TextField,
  InputAdornment,
  Chip,
  CircularProgress,
  Alert,
  Tooltip,
} from '@mui/material';
import {
  Add as AddIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  Visibility as VisibilityIcon,
  Search as SearchIcon,
  Refresh as RefreshIcon,
} from '@mui/icons-material';
import { Opportunity } from '../../types/opportunity';
import { opportunityService } from '../../services/opportunityService';
import ConfirmDialog from '../../components/ConfirmDialog';

const OpportunityList: React.FC = () => {
  const navigate = useNavigate();
  const [opportunities, setOpportunities] = useState<Opportunity[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [page, setPage] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(10);
  const [totalCount, setTotalCount] = useState(0);
  const [searchQuery, setSearchQuery] = useState('');
  const [stageFilter] = useState<string>('');
  const [openConfirm, setOpenConfirm] = useState(false);
  const [selectedOpportunityId, setSelectedOpportunityId] = useState<string | null>(null);
  const [deletingOpportunity, setDeletingOpportunity] = useState<string>('');

  useEffect(() => {
    fetchOpportunities();
  }, [page, rowsPerPage, searchQuery, stageFilter]);

  const fetchOpportunities = async () => {
    try {
      setLoading(true);
      setError(null);
      const response = await opportunityService.getAll({
        pageNumber: page + 1,
        pageSize: rowsPerPage,
        stage: stageFilter || undefined,
      });

      // Filter by search query on the client side
      let filteredData = response.items || [];
      if (searchQuery) {
        const query = searchQuery.toLowerCase();
        filteredData = filteredData.filter((opp) =>
          opp.title.toLowerCase().includes(query) ||
          (opp.description && opp.description.toLowerCase().includes(query))
        );
      }

      setOpportunities(filteredData);
      setTotalCount(response.totalCount || 0);
    } catch (err: any) {
      setError(err.response?.data?.message || 'Fırsatlar yüklenirken bir hata oluştu');
    } finally {
      setLoading(false);
    }
  };

  const handleChangePage = (_event: unknown, newPage: number) => {
    setPage(newPage);
  };

  const handleChangeRowsPerPage = (event: React.ChangeEvent<HTMLInputElement>) => {
    setRowsPerPage(parseInt(event.target.value, 10));
    setPage(0);
  };

  const handleDelete = async (id: string) => {
    const opportunity = opportunities.find(o => o.id === id);
    const opportunityTitle = opportunity ? opportunity.title : 'Fırsat';

    setSelectedOpportunityId(id);
    setDeletingOpportunity(opportunityTitle);
    setOpenConfirm(true);
  };

  const handleConfirmDelete = async () => {
    if (selectedOpportunityId) {
      try {
        await opportunityService.delete(selectedOpportunityId);
        setOpenConfirm(false);
        setSelectedOpportunityId(null);
        fetchOpportunities();
      } catch (err: any) {
        setError(err.response?.data?.message || 'Fırsat silinirken bir hata oluştu');
      }
    }
  };

  const handleCancelDelete = () => {
    setOpenConfirm(false);
    setSelectedOpportunityId(null);
    setDeletingOpportunity('');
  };

  const getStageColor = (stage: string): 'default' | 'primary' | 'secondary' | 'success' | 'warning' | 'error' => {
    const stageLower = stage.toLowerCase();
    if (stageLower.includes('won') || stageLower.includes('kazanıldı')) return 'success';
    if (stageLower.includes('lost') || stageLower.includes('kaybedildi')) return 'error';
    if (stageLower.includes('proposal') || stageLower.includes('teklif')) return 'warning';
    if (stageLower.includes('negotiation') || stageLower.includes('müzakere')) return 'secondary';
    return 'primary';
  };

  const getProbabilityColor = (probability: number): string => {
    if (probability >= 75) return '#4caf50';
    if (probability >= 50) return '#ff9800';
    if (probability >= 25) return '#ff5722';
    return '#f44336';
  };

  if (loading && opportunities.length === 0) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="400px">
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box>
      {/* Header */}
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <Typography variant="h5">Fırsatlar</Typography>
        <Box sx={{ display: 'flex', gap: 1 }}>
          <Button
            variant="outlined"
            startIcon={<RefreshIcon />}
            onClick={fetchOpportunities}
            disabled={loading}
          >
            Yenile
          </Button>
          <Button
            variant="contained"
            startIcon={<AddIcon />}
            onClick={() => navigate('/opportunities/new')}
          >
            Yeni Fırsat
          </Button>
        </Box>
      </Box>

      {error && (
        <Alert severity="error" onClose={() => setError(null)} sx={{ mb: 2 }}>
          {error}
        </Alert>
      )}

      {/* Filters */}
      <Paper sx={{ p: 2, mb: 2 }}>
        <Box display="flex" gap={2} flexWrap="wrap">
          <TextField
            placeholder="Ara..."
            size="small"
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            InputProps={{
              startAdornment: (
                <InputAdornment position="start">
                  <SearchIcon />
                </InputAdornment>
              ),
            }}
            sx={{ minWidth: 250 }}
          />
        </Box>
      </Paper>

      {/* Table */}
      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Başlık</TableCell>
              <TableCell>Değer</TableCell>
              <TableCell>Olasılık</TableCell>
              <TableCell>Aşama</TableCell>
              <TableCell>Tahmini Kapanış</TableCell>
              <TableCell>Durum</TableCell>
              <TableCell align="right">İşlemler</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {opportunities.length === 0 ? (
              <TableRow>
                <TableCell colSpan={7} align="center">
                  <Typography color="text.secondary" py={3}>
                    Fırsat bulunamadı
                  </Typography>
                </TableCell>
              </TableRow>
            ) : (
              opportunities.map((opportunity) => (
                <TableRow key={opportunity.id} hover>
                  <TableCell>
                    <Typography variant="body2" fontWeight={500}>
                      {opportunity.title}
                    </Typography>
                    {opportunity.description && (
                      <Typography variant="caption" color="text.secondary" noWrap>
                        {opportunity.description.substring(0, 50)}
                        {opportunity.description.length > 50 ? '...' : ''}
                      </Typography>
                    )}
                  </TableCell>
                  <TableCell>
                    <Typography variant="body2" fontWeight={500}>
                      {opportunity.value.toLocaleString('tr-TR', {
                        style: 'currency',
                        currency: 'TRY',
                      })}
                    </Typography>
                  </TableCell>
                  <TableCell>
                    <Box display="flex" alignItems="center" gap={1}>
                      <Box
                        sx={{
                          width: 40,
                          height: 4,
                          bgcolor: getProbabilityColor(opportunity.probability ?? 0),
                          borderRadius: 2,
                        }}
                      />
                      <Typography variant="body2">
                        {opportunity.probability ?? 0}%
                      </Typography>
                    </Box>
                  </TableCell>
                  <TableCell>
                    <Chip
                      label={opportunity.stage}
                      color={getStageColor(opportunity.stage)}
                      size="small"
                    />
                  </TableCell>
                  <TableCell>
                    {opportunity.expectedCloseDate
                      ? new Date(opportunity.expectedCloseDate).toLocaleDateString('tr-TR')
                      : '-'}
                  </TableCell>
                  <TableCell>
                    <Chip
                      label={opportunity.isActive ? 'Aktif' : 'Pasif'}
                      color={opportunity.isActive ? 'success' : 'default'}
                      size="small"
                      variant="outlined"
                    />
                  </TableCell>
                  <TableCell align="right">
                    <Tooltip title="Detaylar">
                      <IconButton
                        size="small"
                        onClick={() => navigate(`/opportunities/${opportunity.id}`)}
                      >
                        <VisibilityIcon fontSize="small" />
                      </IconButton>
                    </Tooltip>
                    <Tooltip title="Düzenle">
                      <IconButton
                        size="small"
                        onClick={() => navigate(`/opportunities/${opportunity.id}/edit`)}
                      >
                        <EditIcon fontSize="small" />
                      </IconButton>
                    </Tooltip>
                    <Tooltip title="Sil">
                      <IconButton
                        size="small"
                        color="error"
                        onClick={() => handleDelete(opportunity.id)}
                      >
                        <DeleteIcon fontSize="small" />
                      </IconButton>
                    </Tooltip>
                  </TableCell>
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
        <TablePagination
          component="div"
          count={totalCount}
          page={page}
          onPageChange={handleChangePage}
          rowsPerPage={rowsPerPage}
          onRowsPerPageChange={handleChangeRowsPerPage}
          labelRowsPerPage="Sayfa başına satır:"
          labelDisplayedRows={({ from, to, count }) =>
            `${from}-${to} / ${count !== -1 ? count : `${to}'den fazla`}`
          }
        />
      </TableContainer>

      {/* Delete Confirmation Dialog */}
      <ConfirmDialog
        open={openConfirm}
        title="Fırsatı Sil"
        message={`"${deletingOpportunity}" fırsatını silmek istediğinizden emin misiniz? Bu işlem geri alınamaz.`}
        onConfirm={handleConfirmDelete}
        onCancel={handleCancelDelete}
        confirmText="Sil"
        cancelText="İptal"
        confirmColor="error"
      />
    </Box>
  );
};

export default OpportunityList;
