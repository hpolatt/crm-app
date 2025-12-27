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
  Autocomplete,
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
  productionDurationMinutes?: number;
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

const statusOptions = ['Planned', 'InProgress', 'ProductionCompleted', 'Washing', 'WashingCompleted', 'Completed', 'Cancelled'];

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
  const [delayReasonRequired, setDelayReasonRequired] = useState(false);
  const [selectedDelayReason, setSelectedDelayReason] = useState<string>('');

  const getUserRole = () => {
    const userStr = localStorage.getItem('user');
    if (userStr) {
      try {
        const user = JSON.parse(userStr);
        return user.role;
      } catch {
        return null;
      }
    }
    return null;
  };

  const isForeman = () => getUserRole() === 'Foreman';

  const canEdit = (transaction: PktTransaction) => {
    // Foreman başlamış transactionları düzenleyemez
    if (isForeman() && transaction.startOfWork) {
      return false;
    }
    return true;
  };

  const canDelete = (transaction: PktTransaction) => {
    // Foreman başlamış transactionları silemez
    if (isForeman() && transaction.startOfWork) {
      return false;
    }
    return true;
  };

  useEffect(() => {
    fetchTransactions();
    fetchReactors();
    fetchProducts();
    fetchDelayReasons();
  }, []);

  // Auto-refresh detail dialog every 20 seconds
  useEffect(() => {
    if (!detailOpen || !selectedTransaction) return;

    const interval = setInterval(async () => {
      try {
        const response = await api.get(`/pkttransactions/${selectedTransaction.id}`);
        setSelectedTransaction(response.data.data);
      } catch (error) {
        console.error('Error refreshing transaction:', error);
      }
    }, 20000); // 20 seconds

    return () => clearInterval(interval);
  }, [detailOpen, selectedTransaction?.id]);

  const fetchTransactions = async () => {
    try {
      const response = await api.get('/pkttransactions');
      // Filter out Completed transactions from main list
      const allTransactions = response.data.data || [];
      const activeTransactions = allTransactions.filter((t: PktTransaction) => t.status !== 'Completed');
      setTransactions(activeTransactions);
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
      'InProgress': 'ProductionCompleted',
      'ProductionCompleted': 'Washing', // Yıkama Başlat butonu için
      'Washing': 'WashingCompleted',
      'WashingCompleted': 'Completed',
    };
    return statusMap[currentStatus] || null;
  };

  const getStatusButtonText = (status: string): string => {
    const buttonTextMap: Record<string, string> = {
      'Planned': 'Başla',
      'InProgress': 'Üretimi Bitir',
      'ProductionCompleted': 'Yıkama Başlat',
      'Washing': 'Yıkama Bitir',
      'WashingCompleted': 'Tamamla',
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
      
      // Eğer Completed'a geçtiyse dialog'u kapat (listeden filtreleniyor)
      if (newStatus === 'Completed') {
        setDetailOpen(false);
        setSelectedTransaction(null);
        return;
      }
      
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

    // Üretimi Bitir butonuna basıldığında - not dialog'u aç (zorunlu değil ama gecikme varsa gecikme nedeni zorunlu)
    if (selectedTransaction.status === 'InProgress' && nextStatus === 'ProductionCompleted') {
      const product = products.find(p => p.id === selectedTransaction.productId);
      if (product && selectedTransaction.startOfWork) {
        const startTime = new Date(selectedTransaction.startOfWork).getTime();
        const currentTime = new Date().getTime();
        const elapsedMinutes = (currentTime - startTime) / (1000 * 60);
        const expectedMinutes = (product as any).productionDurationMinutes || 0;

        // Süre aşıldıysa gecikme nedeni zorunlu
        setDelayReasonRequired(elapsedMinutes > expectedMinutes);
      } else {
        setDelayReasonRequired(false);
      }
      
      setPendingStatus(nextStatus);
      setNoteDialogOpen(true);
      return;
    }

    await handleStatusUpdate(nextStatus);
  };

  const handleCompleteWithoutWashing = async () => {
    if (!selectedTransaction || selectedTransaction.status !== 'ProductionCompleted') return;
    
    // Yıkama yapmadan direkt Completed'a geç
    await handleStatusUpdate('Completed');
  };

  const handleNoteSubmit = async () => {
    if (pendingStatus) {
      // Gecikme nedeni zorunluysa ve seçilmemişse hata ver
      if (delayReasonRequired && !selectedDelayReason) {
        alert('Üretim beklenen süreyi aştı. Lütfen gecikme nedeni seçin.');
        return;
      }

      // Update transaction with delay reason if selected
      if (selectedDelayReason) {
        try {
          await api.put(`/pkttransactions/${selectedTransaction?.id}`, {
            ...selectedTransaction,
            delayReasonId: selectedDelayReason,
            description: noteText || selectedTransaction?.description,
          });
        } catch (error) {
          console.error('Error updating delay reason:', error);
        }
      }

      await handleStatusUpdate(pendingStatus, noteText);
      setNoteDialogOpen(false);
      setNoteText('');
      setSelectedDelayReason('');
      setDelayReasonRequired(false);
      setPendingStatus(null);
    }
  };

  const calculateProgress = (transaction: PktTransaction) => {
    if (!transaction.startOfWork || transaction.status === 'Planned' || transaction.status === 'Washing' || transaction.status === 'WashingCompleted' || transaction.status === 'Completed') {
      return { percentage: 0, color: 'grey', showWarning: false, elapsedMinutes: 0, expectedMinutes: 0 };
    }

    const product = products.find(p => p.id === transaction.productId);
    if (!product) return { percentage: 0, color: 'grey', showWarning: false, elapsedMinutes: 0, expectedMinutes: 0 };

    const expectedMinutes = (product as any).productionDurationMinutes || 0;
    if (expectedMinutes === 0) return { percentage: 0, color: 'grey', showWarning: false, elapsedMinutes: 0, expectedMinutes: 0 };

    const startTime = new Date(transaction.startOfWork).getTime();
    // For InProgress: use current time, for ProductionCompleted: use end time
    const currentTime = (transaction.status === 'ProductionCompleted' && transaction.end) 
      ? new Date(transaction.end).getTime() 
      : new Date().getTime();
    const elapsedMinutes = (currentTime - startTime) / (1000 * 60);

    const percentage = Math.min((elapsedMinutes / expectedMinutes) * 100, 100);
    const threshold110 = expectedMinutes * 1.1;

    let color = 'success';
    let showWarning = false;

    if (elapsedMinutes > threshold110) {
      color = 'error';
      showWarning = true;
    } else if (elapsedMinutes > expectedMinutes) {
      color = 'warning';
    }

    return { percentage, color, showWarning, elapsedMinutes, expectedMinutes };
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
                  {canEdit(transaction) && (
                    <IconButton onClick={() => handleOpen(transaction)} color="primary">
                      <Edit />
                    </IconButton>
                  )}
                  {canDelete(transaction) && (
                    <IconButton onClick={() => handleDelete(transaction.id)} color="error">
                      <Delete />
                    </IconButton>
                  )}
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
              <Autocomplete
                options={reactors}
                getOptionLabel={(option) => option.name}
                value={reactors.find(r => r.id === formData.reactorId) || null}
                onChange={(_, newValue) => setFormData({ ...formData, reactorId: newValue?.id || '' })}
                renderInput={(params) => (
                  <TextField
                    {...params}
                    label="Reaktör"
                    required
                    placeholder="Reaktör ara..."
                  />
                )}
                fullWidth
              />
            </Grid>
            <Grid item xs={12} sm={6}>
              <Autocomplete
                options={products}
                getOptionLabel={(option) => `${option.productCode} - ${option.productName}`}
                value={products.find(p => p.id === formData.productId) || null}
                onChange={(_, newValue) => setFormData({ ...formData, productId: newValue?.id || '' })}
                renderInput={(params) => (
                  <TextField
                    {...params}
                    label="Ürün"
                    required
                    placeholder="Ürün kodu veya adı ara..."
                  />
                )}
                fullWidth
              />
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
                              {Math.round(progress.elapsedMinutes ?? 0)} dk / {progress.expectedMinutes ?? 0} dk
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
                            0 dk
                          </Typography>
                          <Typography variant="caption" color="text.secondary">
                            {progress.expectedMinutes ?? 0} dk (Hedef)
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

              {/* Status Update Buttons */}
              <Box sx={{ mb: 3, display: 'flex', gap: 2 }}>
                {getNextStatus(selectedTransaction.status) && (
                  <Button
                    fullWidth
                    variant="contained"
                    size="large"
                    sx={{ py: 2, fontSize: '1.1rem' }}
                    onClick={handleStatusButtonClick}
                  >
                    {getStatusButtonText(selectedTransaction.status)}
                  </Button>
                )}

                {/* Complete Without Washing Button */}
                {selectedTransaction.status === 'ProductionCompleted' && (
                  <Button
                    fullWidth
                    variant="outlined"
                    size="large"
                    color="success"
                    sx={{ py: 2, fontSize: '1.1rem' }}
                    onClick={handleCompleteWithoutWashing}
                  >
                    Yıkama Olmadan Tamamla
                  </Button>
                )}
              </Box>

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
        <DialogTitle>Üretim Tamamlama</DialogTitle>
        <DialogContent>
          {delayReasonRequired && (
            <Alert severity="warning" sx={{ mb: 2 }}>
              Üretim beklenen süreyi aştı. Lütfen gecikme nedeni seçin.
            </Alert>
          )}
          
          {delayReasonRequired && (
            <TextField
              select
              fullWidth
              label="Gecikme Nedeni *"
              value={selectedDelayReason}
              onChange={(e) => setSelectedDelayReason(e.target.value)}
              sx={{ mb: 2, mt: 1 }}
              required
            >
              <MenuItem value="">Seçiniz</MenuItem>
              {delayReasons.map((reason) => (
                <MenuItem key={reason.id} value={reason.id}>
                  {reason.name}
                </MenuItem>
              ))}
            </TextField>
          )}

          <TextField
            fullWidth
            multiline
            rows={4}
            label="Not (Opsiyonel)"
            value={noteText}
            onChange={(e) => setNoteText(e.target.value)}
            sx={{ mt: 1 }}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => {
            setNoteDialogOpen(false);
            setNoteText('');
            setSelectedDelayReason('');
            setDelayReasonRequired(false);
            setPendingStatus(null);
          }}>İptal</Button>
          <Button 
            onClick={handleNoteSubmit} 
            variant="contained"
            disabled={delayReasonRequired && !selectedDelayReason}
          >
            Üretimi Bitir
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}
