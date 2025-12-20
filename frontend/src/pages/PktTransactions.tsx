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
} from '@mui/material';
import { Add, Edit, Delete, Visibility } from '@mui/icons-material';
import axios from 'axios';

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

const statusOptions = ['Planlanan', 'Devam Ediyor', 'Tamamlandı', 'İptal Edildi', 'Gecikmiş'];

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

  useEffect(() => {
    fetchTransactions();
    fetchReactors();
    fetchProducts();
    fetchDelayReasons();
  }, []);

  const fetchTransactions = async () => {
    try {
      const response = await axios.get('http://localhost:5000/api/pkttransactions');
      setTransactions(response.data.data || []);
    } catch (error) {
      console.error('Error fetching transactions:', error);
    }
  };

  const fetchReactors = async () => {
    try {
      const response = await axios.get('http://localhost:5000/api/reactors');
      setReactors(response.data.data || []);
    } catch (error) {
      console.error('Error fetching reactors:', error);
    }
  };

  const fetchProducts = async () => {
    try {
      const response = await axios.get('http://localhost:5000/api/products');
      setProducts(response.data.data || []);
    } catch (error) {
      console.error('Error fetching products:', error);
    }
  };

  const fetchDelayReasons = async () => {
    try {
      const response = await axios.get('http://localhost:5000/api/delayreasons');
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
      const submitData = {
        ...formData,
        startOfWork: formData.startOfWork ? new Date(formData.startOfWork).toISOString() : null,
        end: formData.end ? new Date(formData.end).toISOString() : null,
        delayReasonId: formData.delayReasonId || null,
      };

      if (editingId) {
        await axios.put(`http://localhost:5000/api/pkttransactions/${editingId}`, submitData);
      } else {
        await axios.post('http://localhost:5000/api/pkttransactions', submitData);
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
        await axios.delete(`http://localhost:5000/api/pkttransactions/${id}`);
        fetchTransactions();
      } catch (error) {
        console.error('Error deleting transaction:', error);
      }
    }
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Tamamlandı':
        return 'success';
      case 'Devam Ediyor':
        return 'info';
      case 'Gecikmiş':
        return 'error';
      case 'İptal Edildi':
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
            <Grid item xs={12} sm={6}>
              <TextField
                select
                label="Reaktör"
                fullWidth
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
                value={formData.workOrderNo}
                onChange={(e) => setFormData({ ...formData, workOrderNo: e.target.value })}
              />
            </Grid>
            <Grid item xs={12} sm={6}>
              <TextField
                label="Lot No"
                fullWidth
                value={formData.lotNo}
                onChange={(e) => setFormData({ ...formData, lotNo: e.target.value })}
              />
            </Grid>
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
      <Dialog open={detailOpen} onClose={() => setDetailOpen(false)} maxWidth="md" fullWidth>
        <DialogTitle>İşlem Detayı</DialogTitle>
        <DialogContent>
          {selectedTransaction && (
            <Grid container spacing={2} sx={{ mt: 1 }}>
              <Grid item xs={12} sm={6}>
                <Typography variant="subtitle2" color="text.secondary">
                  Durum
                </Typography>
                <Chip label={selectedTransaction.status} color={getStatusColor(selectedTransaction.status)} />
              </Grid>
              <Grid item xs={12} sm={6}>
                <Typography variant="subtitle2" color="text.secondary">
                  İş Emri No
                </Typography>
                <Typography>{selectedTransaction.workOrderNo}</Typography>
              </Grid>
              <Grid item xs={12} sm={6}>
                <Typography variant="subtitle2" color="text.secondary">
                  Lot No
                </Typography>
                <Typography>{selectedTransaction.lotNo}</Typography>
              </Grid>
              <Grid item xs={12} sm={6}>
                <Typography variant="subtitle2" color="text.secondary">
                  Reaktör
                </Typography>
                <Typography>{selectedTransaction.reactorName}</Typography>
              </Grid>
              <Grid item xs={12} sm={6}>
                <Typography variant="subtitle2" color="text.secondary">
                  Ürün
                </Typography>
                <Typography>{selectedTransaction.productName}</Typography>
              </Grid>
              <Grid item xs={12} sm={6}>
                <Typography variant="subtitle2" color="text.secondary">
                  Başlangıç Zamanı
                </Typography>
                <Typography>
                  {selectedTransaction.startOfWork
                    ? new Date(selectedTransaction.startOfWork).toLocaleString('tr-TR')
                    : '-'}
                </Typography>
              </Grid>
              <Grid item xs={12} sm={6}>
                <Typography variant="subtitle2" color="text.secondary">
                  Bitiş Zamanı
                </Typography>
                <Typography>
                  {selectedTransaction.end ? new Date(selectedTransaction.end).toLocaleString('tr-TR') : '-'}
                </Typography>
              </Grid>
              <Grid item xs={12} sm={6}>
                <Typography variant="subtitle2" color="text.secondary">
                  Kostik Miktarı (Kg)
                </Typography>
                <Typography>{selectedTransaction.causticAmountKg || '-'}</Typography>
              </Grid>
              {selectedTransaction.delayReasonName && (
                <Grid item xs={12}>
                  <Typography variant="subtitle2" color="text.secondary">
                    Gecikme Nedeni
                  </Typography>
                  <Typography>{selectedTransaction.delayReasonName}</Typography>
                </Grid>
              )}
              {selectedTransaction.description && (
                <Grid item xs={12}>
                  <Typography variant="subtitle2" color="text.secondary">
                    Açıklama
                  </Typography>
                  <Typography>{selectedTransaction.description}</Typography>
                </Grid>
              )}
            </Grid>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDetailOpen(false)}>Kapat</Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}
