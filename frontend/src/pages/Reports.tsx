import { useState, useEffect } from 'react';
import {
  Box,
  Paper,
  Typography,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TextField,
  Grid,
  Button,
  Chip,
  MenuItem,
  Autocomplete,
  IconButton,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
} from '@mui/material';
import { Search, GetApp, Edit, Delete } from '@mui/icons-material';
import { api } from '../services/api';

interface PktTransaction {
  id: string;
  status: string;
  reactorId: string;
  reactorName: string;
  productId: string;
  productCode: string;
  productName: string;
  workOrderNo: string;
  lotNo: string;
  startOfWork: string | null;
  end: string | null;
  causticAmountKg: number | null;
  delayReasonId: string | null;
  delayReasonName: string | null;
  description: string | null;
  createdAt: string;
  updatedAt: string;
}

interface Reactor {
  id: string;
  name: string;
}

interface Product {
  id: string;
  productCode: string;
  productName: string;
}

const statusOptions = ['Planned', 'InProgress', 'ProductionCompleted', 'Washing', 'WashingCompleted', 'Completed', 'Cancelled'];

export default function Reports() {
  const [transactions, setTransactions] = useState<PktTransaction[]>([]);
  const [reactors, setReactors] = useState<Reactor[]>([]);
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(false);
  const [editDialogOpen, setEditDialogOpen] = useState(false);
  const [editingTransaction, setEditingTransaction] = useState<PktTransaction | null>(null);
  const [editFormData, setEditFormData] = useState<any>({});

  const [filters, setFilters] = useState({
    startDateFrom: '',
    startDateTo: '',
    reactorId: '',
    productId: '',
    statuses: [] as string[],
    workOrderNo: '',
    lotNo: '',
  });

  useEffect(() => {
    fetchMetadata();
  }, []);

  const fetchMetadata = async () => {
    try {
      const [reactorsRes, productsRes] = await Promise.all([
        api.get('/reactors'),
        api.get('/products'),
      ]);

      setReactors(reactorsRes.data.data || []);
      setProducts(productsRes.data.data || []);
    } catch (error) {
      console.error('Error fetching metadata:', error);
    }
  };

  const fetchReport = async () => {
    setLoading(true);
    try {
      const params = new URLSearchParams();
      
      if (filters.startDateFrom) params.append('startDateFrom', filters.startDateFrom);
      if (filters.startDateTo) params.append('startDateTo', filters.startDateTo);
      if (filters.reactorId) params.append('reactorId', filters.reactorId);
      if (filters.productId) params.append('productId', filters.productId);
      if (filters.statuses.length > 0) {
        filters.statuses.forEach(status => params.append('statuses', status));
      }
      if (filters.workOrderNo) params.append('workOrderNo', filters.workOrderNo);
      if (filters.lotNo) params.append('lotNo', filters.lotNo);

      const response = await api.get(`/pkttransactions?${params.toString()}`);
      setTransactions(response.data.data || []);
    } catch (error) {
      console.error('Error fetching report:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleClearFilters = () => {
    setFilters({
      startDateFrom: '',
      startDateTo: '',
      reactorId: '',
      productId: '',
      statuses: [],
      workOrderNo: '',
      lotNo: '',
    });
    setTransactions([]);
  };

  const handleEdit = (transaction: PktTransaction) => {
    setEditingTransaction(transaction);
    setEditFormData({
      status: transaction.status,
      reactorId: transaction.reactorId,
      productId: transaction.productId,
      workOrderNo: transaction.workOrderNo,
      lotNo: transaction.lotNo,
      causticAmountKg: transaction.causticAmountKg || 0,
      delayReasonId: transaction.delayReasonId || '',
      description: transaction.description || '',
    });
    setEditDialogOpen(true);
  };

  const handleDelete = async (id: string) => {
    if (!window.confirm('Bu kaydı silmek istediğinizden emin misiniz?')) return;
    
    try {
      await api.delete(`/pkttransactions/${id}`);
      fetchReport();
    } catch (error) {
      console.error('Error deleting transaction:', error);
      alert('Silme işlemi başarısız oldu');
    }
  };

  const handleEditSubmit = async () => {
    if (!editingTransaction) return;

    try {
      await api.put(`/pkttransactions/${editingTransaction.id}`, editFormData);
      setEditDialogOpen(false);
      setEditingTransaction(null);
      fetchReport();
    } catch (error) {
      console.error('Error updating transaction:', error);
      alert('Güncelleme işlemi başarısız oldu');
    }
  };

  const getStatusColor = (status: string) => {
    const colorMap: Record<string, 'default' | 'primary' | 'secondary' | 'error' | 'info' | 'success' | 'warning'> = {
      'Planned': 'default',
      'InProgress': 'primary',
      'ProductionCompleted': 'info',
      'Washing': 'secondary',
      'WashingCompleted': 'info',
      'Completed': 'success',
      'Cancelled': 'error',
    };
    return colorMap[status] || 'default';
  };

  const calculateDuration = (startOfWork: string | null, end: string | null): string => {
    if (!startOfWork || !end) return '-';
    
    const start = new Date(startOfWork).getTime();
    const endTime = new Date(end).getTime();
    const durationMinutes = Math.round((endTime - start) / (1000 * 60));
    
    const hours = Math.floor(durationMinutes / 60);
    const minutes = durationMinutes % 60;
    
    return `${hours}s ${minutes}dk`;
  };

  return (
    <Box sx={{ p: 3 }}>
      <Typography variant="h4" gutterBottom>
        Raporlar
      </Typography>

      {/* Filters */}
      <Paper sx={{ p: 3, mb: 3 }}>
        <Typography variant="h6" gutterBottom>
          Filtreler
        </Typography>
        <Grid container spacing={2}>
          <Grid item xs={12} sm={6} md={3}>
            <TextField
              fullWidth
              type="date"
              label="Başlangıç Tarihi (Den)"
              value={filters.startDateFrom}
              onChange={(e) => setFilters({ ...filters, startDateFrom: e.target.value })}
              InputLabelProps={{ shrink: true }}
            />
          </Grid>
          <Grid item xs={12} sm={6} md={3}>
            <TextField
              fullWidth
              type="date"
              label="Başlangıç Tarihi (e)"
              value={filters.startDateTo}
              onChange={(e) => setFilters({ ...filters, startDateTo: e.target.value })}
              InputLabelProps={{ shrink: true }}
            />
          </Grid>
          <Grid item xs={12} sm={6} md={3}>
            <Autocomplete
              options={reactors}
              getOptionLabel={(option) => option.name}
              value={reactors.find(r => r.id === filters.reactorId) || null}
              onChange={(_, newValue) => setFilters({ ...filters, reactorId: newValue?.id || '' })}
              renderInput={(params) => (
                <TextField
                  {...params}
                  label="Reaktör"
                  placeholder="Reaktör ara..."
                />
              )}
              fullWidth
            />
          </Grid>
          <Grid item xs={12} sm={6} md={3}>
            <Autocomplete
              options={products}
              getOptionLabel={(option) => `${option.productCode} - ${option.productName}`}
              value={products.find(p => p.id === filters.productId) || null}
              onChange={(_, newValue) => setFilters({ ...filters, productId: newValue?.id || '' })}
              renderInput={(params) => (
                <TextField
                  {...params}
                  label="Ürün"
                  placeholder="Ürün ara..."
                />
              )}
              fullWidth
            />
          </Grid>
          <Grid item xs={12} sm={6} md={3}>
            <Autocomplete
              multiple
              options={statusOptions}
              value={filters.statuses}
              onChange={(_, newValue) => setFilters({ ...filters, statuses: newValue })}
              renderInput={(params) => (
                <TextField
                  {...params}
                  label="Durum"
                  placeholder="Durum seçin..."
                />
              )}
              fullWidth
            />
          </Grid>
          <Grid item xs={12} sm={6} md={3}>
            <TextField
              fullWidth
              label="İş Emri No"
              value={filters.workOrderNo}
              onChange={(e) => setFilters({ ...filters, workOrderNo: e.target.value })}
            />
          </Grid>
          <Grid item xs={12} sm={6} md={3}>
            <TextField
              fullWidth
              label="Lot No"
              value={filters.lotNo}
              onChange={(e) => setFilters({ ...filters, lotNo: e.target.value })}
            />
          </Grid>
          <Grid item xs={12} sm={6} md={3}>
            <Button
              fullWidth
              variant="outlined"
              onClick={handleClearFilters}
              sx={{ height: '56px' }}
            >
              Filtreleri Temizle
            </Button>
          </Grid>
          <Grid item xs={12} sm={6} md={3}>
            <Button
              fullWidth
              variant="contained"
              onClick={fetchReport}
              disabled={loading}
              sx={{ height: '56px' }}
            >
              {loading ? 'Yükleniyor...' : 'Raporla'}
            </Button>
          </Grid>
        </Grid>
      </Paper>

      {/* Results */}
      <Paper>
        <Box sx={{ p: 2, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <Typography variant="h6">
            Sonuçlar ({transactions.length})
          </Typography>
        </Box>

        <TableContainer>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Durum</TableCell>
                <TableCell>İş Emri No</TableCell>
                <TableCell>Lot No</TableCell>
                <TableCell>Reaktör</TableCell>
                <TableCell>Ürün Kodu</TableCell>
                <TableCell>Ürün Adı</TableCell>
                <TableCell>Başlangıç</TableCell>
                <TableCell>Bitiş</TableCell>
                <TableCell>Süre</TableCell>
                <TableCell>Gecikme Nedeni</TableCell>
                <TableCell align="right">İşlemler</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {transactions.length === 0 ? (
                <TableRow>
                  <TableCell colSpan={11} align="center">
                    <Typography variant="body2" color="text.secondary" sx={{ py: 3 }}>
                      Filtre seçip "Raporla" butonuna basın
                    </Typography>
                  </TableCell>
                </TableRow>
              ) : (
                transactions.map((transaction) => (
                  <TableRow key={transaction.id} hover>
                    <TableCell>
                      <Chip label={transaction.status} color={getStatusColor(transaction.status)} size="small" />
                    </TableCell>
                    <TableCell>{transaction.workOrderNo}</TableCell>
                    <TableCell>{transaction.lotNo}</TableCell>
                    <TableCell>{transaction.reactorName}</TableCell>
                    <TableCell>{transaction.productCode}</TableCell>
                    <TableCell>{transaction.productName}</TableCell>
                    <TableCell>
                      {transaction.startOfWork
                        ? new Date(transaction.startOfWork).toLocaleString('tr-TR')
                        : '-'}
                    </TableCell>
                    <TableCell>
                      {transaction.end
                        ? new Date(transaction.end).toLocaleString('tr-TR')
                        : '-'}
                    </TableCell>
                    <TableCell>{calculateDuration(transaction.startOfWork, transaction.end)}</TableCell>
                    <TableCell>{transaction.delayReasonName || '-'}</TableCell>
                    <TableCell align="right">
                      <IconButton size="small" onClick={() => handleEdit(transaction)} color="primary">
                        <Edit fontSize="small" />
                      </IconButton>
                      <IconButton size="small" onClick={() => handleDelete(transaction.id)} color="error">
                        <Delete fontSize="small" />
                      </IconButton>
                    </TableCell>
                  </TableRow>
                ))
              )}
            </TableBody>
          </Table>
        </TableContainer>
      </Paper>

      {/* Edit Dialog */}
      <Dialog open={editDialogOpen} onClose={() => setEditDialogOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>İşlem Düzenle</DialogTitle>
        <DialogContent>
          <Grid container spacing={2} sx={{ mt: 1 }}>
            <Grid item xs={12}>
              <TextField
                select
                fullWidth
                label="Durum"
                value={editFormData.status || ''}
                onChange={(e) => setEditFormData({ ...editFormData, status: e.target.value })}
              >
                {statusOptions.map((status) => (
                  <MenuItem key={status} value={status}>
                    {status}
                  </MenuItem>
                ))}
              </TextField>
            </Grid>
            <Grid item xs={12}>
              <Autocomplete
                options={reactors}
                getOptionLabel={(option) => option.name}
                value={reactors.find(r => r.id === editFormData.reactorId) || null}
                onChange={(_, newValue) => setEditFormData({ ...editFormData, reactorId: newValue?.id || '' })}
                renderInput={(params) => (
                  <TextField {...params} label="Reaktör" required />
                )}
              />
            </Grid>
            <Grid item xs={12}>
              <Autocomplete
                options={products}
                getOptionLabel={(option) => `${option.productCode} - ${option.productName}`}
                value={products.find(p => p.id === editFormData.productId) || null}
                onChange={(_, newValue) => setEditFormData({ ...editFormData, productId: newValue?.id || '' })}
                renderInput={(params) => (
                  <TextField {...params} label="Ürün" required />
                )}
              />
            </Grid>
            <Grid item xs={12} sm={6}>
              <TextField
                fullWidth
                label="İş Emri No"
                value={editFormData.workOrderNo || ''}
                onChange={(e) => setEditFormData({ ...editFormData, workOrderNo: e.target.value })}
                required
              />
            </Grid>
            <Grid item xs={12} sm={6}>
              <TextField
                fullWidth
                label="Lot No"
                value={editFormData.lotNo || ''}
                onChange={(e) => setEditFormData({ ...editFormData, lotNo: e.target.value })}
                required
              />
            </Grid>
            <Grid item xs={12}>
              <TextField
                fullWidth
                type="number"
                label="Kostik Miktarı (kg)"
                value={editFormData.causticAmountKg || 0}
                onChange={(e) => setEditFormData({ ...editFormData, causticAmountKg: parseFloat(e.target.value) })}
              />
            </Grid>
            <Grid item xs={12}>
              <TextField
                fullWidth
                multiline
                rows={3}
                label="Açıklama"
                value={editFormData.description || ''}
                onChange={(e) => setEditFormData({ ...editFormData, description: e.target.value })}
              />
            </Grid>
          </Grid>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setEditDialogOpen(false)}>İptal</Button>
          <Button onClick={handleEditSubmit} variant="contained" color="primary">
            Kaydet
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}
