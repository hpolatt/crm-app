import { useState, useEffect } from 'react';
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
  IconButton,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Grid,
  MenuItem,
  Chip,
  LinearProgress,
  Alert,
} from '@mui/material';
import { Add, Edit, Delete, Visibility, Warning } from '@mui/icons-material';
import { api } from '../services/api';

interface PktTransaction {
  id: string;
  status: string;
  reactorId: string;
  reactorName: string;
  productId: string;
  productName: string;
  workOrderNo: string;
  lotNo: string;
  startOfWork?: string;
  end?: string;
  actualProductionDuration?: string;
  delayDuration?: string;
  washingDuration?: string;
  causticAmountKg?: number;
  delayReasonId?: string;
  delayReasonName?: string;
  description?: string;
  createdAt: string;
}

interface Reactor {
  id: string;
  name: string;
}

interface Product {
  id: string;
  productName: string;
  productionDurationHours?: number;
}

interface DelayReason {
  id: string;
  name: string;
}

const initialFormData = {
  status: 'Planlanan',
  reactorId: '',
  productId: '',
  workOrderNo: '',
  lotNo: '',
  startOfWork: '',
  end: '',
  causticAmountKg: 0,
  delayReasonId: '',
  description: '',
};

const statusOptions = ['Planned', 'InProgress', 'Completed', 'Washing', 'WashingCompleted', 'Cancelled'];

export default function PktTransactions() {
  const [transactions, setTransactions] = useState<PktTransaction[]>([]);
  const [reactors, setReactors] = useState<Reactor[]>([]);
  const [products, setProducts] = useState<Product[]>([]);
  const [delayReasons, setDelayReasons] = useState<DelayReason[]>([]);
  const [open, setOpen] = useState(false);
  const [detailOpen, setDetailOpen] = useState(false);
  const [selectedTransaction, setSelectedTransaction] = useState<PktTransaction | null>(null);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [formData, setFormData] = useState(initialFormData);
  const [noteDialogOpen, setNoteDialogOpen] = useState(false);
  const [noteText, setNoteText] = useState('');
  const [pendingStatus, setPendingStatus] = useState<string | null>(null);

  useEffect(() => {
    fetchTransactions();
    fetchReactors();
    fetchProducts();
    fetchDelayReasons();
  }, []);

  const fetchTransactions = async () => {
    try {
      const response = await api.get('/pkttransactions');
      setTransactions(response.data.data || []);
    } catch (error) {
      console.error('Error fetching transactions:', error);
    }
  };

  const fetchReactors = async () => {
    try {
      const response = await api.get('/reactors');
      setReactors(response.data.data || []);
    } catch (error) {
      console.error('Error fetching reactors:', error);
    }
  };

  const fetchProducts = async () => {
    try {
      const response = await api.get('/products');
      setProducts(response.data.data || []);
    } catch (error) {
      console.error('Error fetching products:', error);
    }
  };

  const fetchDelayReasons = async () => {
    try {
      const response = await api.get('/delayreasons');
      setDelayReasons(response.data.data || []);
    } catch (error) {
      console.error('Error fetching delay reasons:', error);
    }
  };

  const handleOpen = (transaction?: PktTransaction) => {
    if (transaction) {
      setEditingId(transaction.id);
      setFormData({
        status: transaction.status,
        reactorId: transaction.reactorId,
        productId: transaction.productId,
        workOrderNo: transaction.workOrderNo,
        lotNo: transaction.lotNo,
        startOfWork: transaction.startOfWork ? new Date(transaction.startOfWork).toISOString().slice(0, 16) : '',
        end: transaction.end ? new Date(transaction.end).toISOString().slice(0, 16) : '',
        causticAmountKg: transaction.causticAmountKg || 0,
        delayReasonId: transaction.delayReasonId || '',
        description: transaction.description || '',
      });
    } else {
      setEditingId(null);
      setFormData(initialFormData);
    }
    setOpen(true);
  };

  const handleClose = () => {
    setOpen(false);
    setEditingId(null);
    setFormData(initialFormData);
  };

  const handleViewDetail = (transaction: PktTransaction) => {
    setSelectedTransaction(transaction);
    setDetailOpen(true);
  };

  const handleSubmit = async () => {
    try {
      if (editingId) {
        // Edit - Tüm alanları gönder
        const submitData = {
          ...formData,
          startOfWork: formData.startOfWork ? new Date(formData.startOfWork).toISOString() : null,
          end: formData.end ? new Date(formData.end).toISOString() : null,
          delayReasonId: formData.delayReasonId || null,
        };
        await api.put(`/pkttransactions/${editingId}`, submitData);
      } else {
        // Create - Sadece gerekli alanları gönder (status, startOfWork, end yok)
        const submitData = {
          reactorId: formData.reactorId,
          productId: formData.productId,
          workOrderNo: formData.workOrderNo,
          lotNo: formData.lotNo,
          causticAmountKg: formData.causticAmountKg || null,
          delayReasonId: formData.delayReasonId || null,
          description: formData.description || null,
        };
        await api.post('/pkttransactions', submitData);
      }
      fetchTransactions();
      handleClose();
    } catch (error) {
      console.error('Error saving transaction:', error);
    }
  };

  const handleDelete = async (id: string) => {
    if (window.confirm('Bu işlemi silmek istediğinize emin misiniz?')) {
      try {
        await api.delete(`/pkttransactions/${id}`);
        fetchTransactions();
      } catch (error) {
        console.error('Error deleting transaction:', error);
      }
    }
  };

  const getNextStatus = (currentStatus: string): string | null => {
    const statusMap: Record<string, string> = {
      'Planned': 'InProgress',
      'InProgress': 'Completed',
      'Completed': 'Washing',
      'Washing': 'WashingCompleted',
    };
    return statusMap[currentStatus] || null;
  };

  const getStatusButtonText = (status: string): string => {
    const buttonTextMap: Record<string, string> = {
      'Planned': 'Başla',
      'InProgress': 'Bitir',
      'Completed': 'Yıkama Başlat',
      'Washing': 'Yıkama Bitir',
    };
    return buttonTextMap[status] || '';
  };

  const handleStatusUpdate = async (newStatus: string, note?: string) => {
    if (!selectedTransaction) return;

    try {
      await api.patch(`/pkttransactions/${selectedTransaction.id}/status`, {
        newStatus,
        note: note || null,
      });
      await fetchTransactions();
      // Refresh selected transaction
      const response = await api.get(`/pkttransactions/${selectedTransaction.id}`);
      setSelectedTransaction(response.data.data);
    } catch (error) {
      console.error('Error updating status:', error);
    }
  };

  const handleStatusButtonClick = async () => {
    if (!selectedTransaction) return;

    const nextStatus = getNextStatus(selectedTransaction.status);
    if (!nextStatus) return;

    // Bitir butonuna basıldığında ve süre aşıldıysa not iste
    if (selectedTransaction.status === 'InProgress' && nextStatus === 'Completed') {
      const product = products.find(p => p.id === selectedTransaction.productId);
      if (product && selectedTransaction.startOfWork) {
        const startTime = new Date(selectedTransaction.startOfWork).getTime();
        const currentTime = new Date().getTime();
        const elapsedHours = (currentTime - startTime) / (1000 * 60 * 60);
        const expectedHours = (product as any).productionDurationHours || 0;

        if (elapsedHours > expectedHours) {
          // Not iste
          setPendingStatus(nextStatus);
          setNoteDialogOpen(true);
          return;
        }
      }
    }

    await handleStatusUpdate(nextStatus);
  };

  const handleNoteSubmit = async () => {
    if (pendingStatus) {
      await handleStatusUpdate(pendingStatus, noteText);
      setNoteDialogOpen(false);
      setNoteText('');
      setPendingStatus(null);
    }
  };

  const calculateProgress = (transaction: PktTransaction) => {
    if (!transaction.startOfWork || transaction.status === 'Planned' || transaction.status === 'Washing') {
      return { percentage: 0, color: 'grey', showWarning: false };
    }

    const product = products.find(p => p.id === transaction.productId);
    if (!product) return { percentage: 0, color: 'grey', showWarning: false };

    const expectedHours = (product as any).productionDurationHours || 0;
    if (expectedHours === 0) return { percentage: 0, color: 'grey', showWarning: false };

    const startTime = new Date(transaction.startOfWork).getTime();
    const currentTime = transaction.end ? new Date(transaction.end).getTime() : new Date().getTime();
    const elapsedHours = (currentTime - startTime) / (1000 * 60 * 60);

    const percentage = Math.min((elapsedHours / expectedHours) * 100, 100);
    const threshold110 = expectedHours * 1.1;

    let color = 'success';
    let showWarning = false;

    if (elapsedHours > threshold110) {
      color = 'error';
      showWarning = true;
    } else if (elapsedHours > expectedHours) {
      color = 'warning';
    }

    return { percentage, color, showWarning, elapsedHours, expectedHours };
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Completed':
      case 'WashingCompleted':
        return 'success';
      case 'InProgress':
      case 'Washing':
        return 'info';
      case 'Cancelled':
        return 'default';
      default:
        return 'warning';
    }
  };

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 3 }}>
        <Typography variant="h4">PKT İşlemleri</Typography>
        <Button variant="contained" startIcon={<Add />} onClick={() => handleOpen()}>
          Yeni İşlem
        </Button>
      </Box>

      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Durum</TableCell>
              <TableCell>İş Emri No</TableCell>
              <TableCell>Lot No</TableCell>
              <TableCell>Reaktör</TableCell>
              <TableCell>Ürün</TableCell>
              <TableCell>Başlangıç</TableCell>
              <TableCell>Bitiş</TableCell>
              <TableCell align="right">İşlemler</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {transactions.map((transaction) => (
              <TableRow key={transaction.id}>
                <TableCell>
                  <Chip label={transaction.status} color={getStatusColor(transaction.status)} size="small" />
                </TableCell>
                <TableCell>{transaction.workOrderNo}</TableCell>
                <TableCell>{transaction.lotNo}</TableCell>
                <TableCell>{transaction.reactorName}</TableCell>
                <TableCell>{transaction.productName}</TableCell>
                <TableCell>
                  {transaction.startOfWork ? new Date(transaction.startOfWork).toLocaleString('tr-TR') : '-'}
                </TableCell>
                <TableCell>
                  {transaction.end ? new Date(transaction.end).toLocaleString('tr-TR') : '-'}
                </TableCell>
                <TableCell align="right">
                  <IconButton onClick={() => handleViewDetail(transaction)} color="info">
                    <Visibility />
                  </IconButton>
                  <IconButton onClick={() => handleOpen(transaction)} color="primary">
                    <Edit />
                  </IconButton>
                  <IconButton onClick={() => handleDelete(transaction.id)} color="error">
                    <Delete />
                  </IconButton>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>

      {/* Edit/Create Dialog */}
      <Dialog open={open} onClose={handleClose} maxWidth="md" fullWidth>
        <DialogTitle>{editingId ? 'İşlemi Düzenle' : 'Yeni İşlem'}</DialogTitle>
        <DialogContent>
          <Grid container spacing={2} sx={{ mt: 1 }}>
            {/* Status - Sadece Edit'te göster */}
            {editingId && (
              <Grid item xs={12} sm={6}>
                <TextField
                  select
                  label="Durum"
                  fullWidth
                  value={formData.status}
                  onChange={(e) => setFormData({ ...formData, status: e.target.value })}
                >
                  {statusOptions.map((option) => (
                    <MenuItem key={option} value={option}>
                      {option}
                    </MenuItem>
                  ))}
                </TextField>
              </Grid>
            )}
            <Grid item xs={12} sm={6}>
              <TextField
                select
                label="Reaktör"
                fullWidth
                required
                value={formData.reactorId}
                onChange={(e) => setFormData({ ...formData, reactorId: e.target.value })}
              >
                {reactors.map((reactor) => (
                  <MenuItem key={reactor.id} value={reactor.id}>
                    {reactor.name}
                  </MenuItem>
                ))}
              </TextField>
            </Grid>
            <Grid item xs={12} sm={6}>
              <TextField
                select
                label="Ürün"
                fullWidth
                required
                value={formData.productId}
                onChange={(e) => setFormData({ ...formData, productId: e.target.value })}
              >
                {products.map((product) => (
                  <MenuItem key={product.id} value={product.id}>
                    {product.productName}
                  </MenuItem>
                ))}
              </TextField>
            </Grid>
            <Grid item xs={12} sm={6}>
              <TextField
                label="İş Emri No"
                fullWidth
                required
                value={formData.workOrderNo}
                onChange={(e) => setFormData({ ...formData, workOrderNo: e.target.value })}
              />
            </Grid>
            <Grid item xs={12} sm={6}>
              <TextField
                label="Lot No"
                fullWidth
                required
                value={formData.lotNo}
                onChange={(e) => setFormData({ ...formData, lotNo: e.target.value })}
              />
            </Grid>
            {/* Başlangıç ve Bitiş Zamanı - Sadece Edit'te göster */}
            {editingId && (
              <>
                <Grid item xs={12} sm={6}>
                  <TextField
                    label="Başlangıç Zamanı"
                    type="datetime-local"
                    fullWidth
                    InputLabelProps={{ shrink: true }}
                    value={formData.startOfWork}
                    onChange={(e) => setFormData({ ...formData, startOfWork: e.target.value })}
                  />
                </Grid>
                <Grid item xs={12} sm={6}>
                  <TextField
                    label="Bitiş Zamanı"
                    type="datetime-local"
                    fullWidth
                    InputLabelProps={{ shrink: true }}
                    value={formData.end}
                    onChange={(e) => setFormData({ ...formData, end: e.target.value })}
                  />
                </Grid>
              </>
            )}
            <Grid item xs={12} sm={6}>
              <TextField
                label="Kostik Miktarı (Kg)"
                type="number"
                fullWidth
                value={formData.causticAmountKg}
                onChange={(e) => setFormData({ ...formData, causticAmountKg: Number(e.target.value) })}
              />
            </Grid>
            <Grid item xs={12} sm={6}>
              <TextField
                select
                label="Gecikme Nedeni"
                fullWidth
                value={formData.delayReasonId}
                onChange={(e) => setFormData({ ...formData, delayReasonId: e.target.value })}
              >
                <MenuItem value="">Yok</MenuItem>
                {delayReasons.map((reason) => (
                  <MenuItem key={reason.id} value={reason.id}>
                    {reason.name}
                  </MenuItem>
                ))}
              </TextField>
            </Grid>
            <Grid item xs={12}>
              <TextField
                label="Açıklama"
                multiline
                rows={3}
                fullWidth
                value={formData.description}
                onChange={(e) => setFormData({ ...formData, description: e.target.value })}
              />
            </Grid>
          </Grid>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose}>İptal</Button>
          <Button onClick={handleSubmit} variant="contained">
            Kaydet
          </Button>
        </DialogActions>
      </Dialog>

      {/* Detail Dialog */}
      <Dialog open={detailOpen} onClose={() => setDetailOpen(false)} fullScreen>
        <DialogTitle>
          <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
            <Typography variant="h5">İşlem Detayı</Typography>
            {selectedTransaction && (
              <Chip 
                label={selectedTransaction.status} 
                color={getStatusColor(selectedTransaction.status)} 
                size="medium"
              />
            )}
          </Box>
        </DialogTitle>
        <DialogContent>
          {selectedTransaction && (
            <Box sx={{ maxWidth: 1200, mx: 'auto', py: 2 }}>
              {/* Progress Bar */}
              {selectedTransaction.status !== 'Planned' && 
               selectedTransaction.status !== 'Washing' && 
               selectedTransaction.status !== 'WashingCompleted' && (
                <Paper elevation={3} sx={{ p: 3, mb: 3 }}>
                  {(() => {
                    const progress = calculateProgress(selectedTransaction);
                    return (
                      <>
                        <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 2 }}>
                          <Typography variant="h6">
                            Üretim Süresi
                          </Typography>
                          <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                            <Typography variant="h6" color={progress.color === 'error' ? 'error' : progress.color === 'warning' ? 'warning.main' : 'success.main'}>
                              {progress.elapsedHours?.toFixed(1) || 0}h / {progress.expectedHours || 0}h
                            </Typography>
                            {progress.showWarning && <Warning color="error" fontSize="large" />}
                          </Box>
                        </Box>
                        <LinearProgress
                          variant="determinate"
                          value={Math.min(progress.percentage, 100)}
                          color={progress.color as any}
                          sx={{ height: 20, borderRadius: 2 }}
                        />
                        <Box sx={{ mt: 1, display: 'flex', justifyContent: 'space-between' }}>
                          <Typography variant="caption" color="text.secondary">
                            0h
                          </Typography>
                          <Typography variant="caption" color="text.secondary">
                            {progress.expectedHours}h (Hedef)
                          </Typography>
                        </Box>
                      </>
                    );
                  })()}
                </Paper>
              )}

              {selectedTransaction.status === 'Planned' && (
                <Alert severity="info" sx={{ mb: 3 }}>
                  Bu işlem henüz başlatılmadı
                </Alert>
              )}

              {(selectedTransaction.status === 'Washing' || selectedTransaction.status === 'WashingCompleted') && (
                <Alert severity="info" sx={{ mb: 3 }}>
                  Yıkama aşamasında
                </Alert>
              )}

              {/* Status Update Button */}
              {getNextStatus(selectedTransaction.status) && (
                <Button
                  fullWidth
                  variant="contained"
                  size="large"
                  sx={{ mb: 3, py: 2, fontSize: '1.1rem' }}
                  onClick={handleStatusButtonClick}
                >
                  {getStatusButtonText(selectedTransaction.status)}
                </Button>
              )}

              <Paper elevation={2} sx={{ p: 3 }}>
                <Grid container spacing={3}>
                  <Grid item xs={12} sm={6} md={4}>
                    <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                      İş Emri No
                    </Typography>
                    <Typography variant="h6">{selectedTransaction.workOrderNo}</Typography>
                  </Grid>
                  <Grid item xs={12} sm={6} md={4}>
                    <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                      Lot No
                    </Typography>
                    <Typography variant="h6">{selectedTransaction.lotNo}</Typography>
                  </Grid>
                  <Grid item xs={12} sm={6} md={4}>
                    <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                      Reaktör
                    </Typography>
                    <Typography variant="h6">{selectedTransaction.reactorName}</Typography>
                  </Grid>
                  <Grid item xs={12} sm={6} md={4}>
                    <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                      Ürün
                    </Typography>
                    <Typography variant="h6">{selectedTransaction.productName}</Typography>
                  </Grid>
                  <Grid item xs={12} sm={6} md={4}>
                    <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                      Başlangıç Zamanı
                    </Typography>
                    <Typography variant="body1">
                      {selectedTransaction.startOfWork
                        ? new Date(selectedTransaction.startOfWork).toLocaleString('tr-TR')
                        : '-'}
                    </Typography>
                  </Grid>
                  <Grid item xs={12} sm={6} md={4}>
                    <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                      Bitiş Zamanı
                    </Typography>
                    <Typography variant="body1">
                      {selectedTransaction.end ? new Date(selectedTransaction.end).toLocaleString('tr-TR') : '-'}
                    </Typography>
                  </Grid>
                  <Grid item xs={12} sm={6} md={4}>
                    <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                      Kostik Miktarı (Kg)
                    </Typography>
                    <Typography variant="body1">{selectedTransaction.causticAmountKg || '-'}</Typography>
                  </Grid>
                  {selectedTransaction.delayReasonName && (
                    <Grid item xs={12}>
                      <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                        Gecikme Nedeni
                      </Typography>
                      <Typography variant="body1">{selectedTransaction.delayReasonName}</Typography>
                    </Grid>
                  )}
                  {selectedTransaction.description && (
                    <Grid item xs={12}>
                      <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                        Açıklama
                      </Typography>
                      <Typography variant="body1" sx={{ whiteSpace: 'pre-wrap' }}>{selectedTransaction.description}</Typography>
                    </Grid>
                  )}
                </Grid>
              </Paper>
            </Box>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDetailOpen(false)}>Kapat</Button>
        </DialogActions>
      </Dialog>

      {/* Note Dialog */}
      <Dialog open={noteDialogOpen} onClose={() => setNoteDialogOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>Gecikme Notu</DialogTitle>
        <DialogContent>
          <Alert severity="warning" sx={{ mb: 2 }}>
            Üretim beklenen süreyi aştı. Lütfen bir açıklama girin.
          </Alert>
          <TextField
            fullWidth
            multiline
            rows={4}
            label="Açıklama"
            value={noteText}
            onChange={(e) => setNoteText(e.target.value)}
            sx={{ mt: 1 }}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setNoteDialogOpen(false)}>İptal</Button>
          <Button onClick={handleNoteSubmit} variant="contained" disabled={!noteText.trim()}>
            Kaydet ve Bitir
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}
