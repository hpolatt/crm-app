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
} from '@mui/material';
import { Add, Edit, Delete } from '@mui/icons-material';
import { api } from '../services/api'

interface Product {
  id: string;
  sbu: string;
  productCode: string;
  productName: string;
  minProductionQuantity: number;
  maxProductionQuantity: number;
  productionDurationHours: number;
  notes?: string;
  createdAt: string;
  updatedAt?: string;
}

const initialFormData = {
  sbu: '',
  productCode: '',
  productName: '',
  minProductionQuantity: 0,
  maxProductionQuantity: 0,
  productionDurationHours: 0,
  notes: '',
};

export default function Products() {
  const [products, setProducts] = useState<Product[]>([]);
  const [open, setOpen] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [formData, setFormData] = useState(initialFormData);

  useEffect(() => {
    fetchProducts();
  }, []);

  const fetchProducts = async () => {
    try {
      const response = await api.get('/products');
      setProducts(response.data.data || []);
    } catch (error) {
      console.error('Error fetching products:', error);
    }
  };

  const handleOpen = (product?: Product) => {
    if (product) {
      setEditingId(product.id);
      setFormData({
        sbu: product.sbu,
        productCode: product.productCode,
        productName: product.productName,
        minProductionQuantity: product.minProductionQuantity,
        maxProductionQuantity: product.maxProductionQuantity,
        productionDurationHours: product.productionDurationHours,
        notes: product.notes || '',
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

  const handleSubmit = async () => {
    try {
      if (editingId) {
        await api.put(`/products/${editingId}`, formData);
      } else {
        await api.post('/products', formData);
      }
      fetchProducts();
      handleClose();
    } catch (error) {
      console.error('Error saving product:', error);
    }
  };

  const handleDelete = async (id: string) => {
    if (window.confirm('Bu ürünü silmek istediğinize emin misiniz?')) {
      try {
        await api.delete(`/products/${id}`);
        fetchProducts();
      } catch (error) {
        console.error('Error deleting product:', error);
      }
    }
  };

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 3 }}>
        <Typography variant="h4">Ürünler</Typography>
        <Button variant="contained" startIcon={<Add />} onClick={() => handleOpen()}>
          Yeni Ekle
        </Button>
      </Box>

      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>SBU</TableCell>
              <TableCell>Ürün Kodu</TableCell>
              <TableCell>Ürün Adı</TableCell>
              <TableCell align="right">Min Miktar</TableCell>
              <TableCell align="right">Max Miktar</TableCell>
              <TableCell align="right">Üretim Süresi (Saat)</TableCell>
              <TableCell align="right">İşlemler</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {products.map((product) => (
              <TableRow key={product.id}>
                <TableCell>{product.sbu}</TableCell>
                <TableCell>{product.productCode}</TableCell>
                <TableCell>{product.productName}</TableCell>
                <TableCell align="right">{product.minProductionQuantity}</TableCell>
                <TableCell align="right">{product.maxProductionQuantity}</TableCell>
                <TableCell align="right">{product.productionDurationHours}</TableCell>
                <TableCell align="right">
                  <IconButton onClick={() => handleOpen(product)} color="primary">
                    <Edit />
                  </IconButton>
                  <IconButton onClick={() => handleDelete(product.id)} color="error">
                    <Delete />
                  </IconButton>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>

      <Dialog open={open} onClose={handleClose} maxWidth="md" fullWidth>
        <DialogTitle>{editingId ? 'Ürünü Düzenle' : 'Yeni Ürün'}</DialogTitle>
        <DialogContent>
          <Grid container spacing={2} sx={{ mt: 1 }}>
            <Grid item xs={12} sm={6}>
              <TextField
                label="SBU"
                fullWidth
                value={formData.sbu}
                onChange={(e) => setFormData({ ...formData, sbu: e.target.value })}
              />
            </Grid>
            <Grid item xs={12} sm={6}>
              <TextField
                label="Ürün Kodu"
                fullWidth
                value={formData.productCode}
                onChange={(e) => setFormData({ ...formData, productCode: e.target.value })}
              />
            </Grid>
            <Grid item xs={12}>
              <TextField
                label="Ürün Adı"
                fullWidth
                value={formData.productName}
                onChange={(e) => setFormData({ ...formData, productName: e.target.value })}
              />
            </Grid>
            <Grid item xs={12} sm={4}>
              <TextField
                label="Min Üretim Miktarı"
                type="number"
                fullWidth
                value={formData.minProductionQuantity}
                onChange={(e) => setFormData({ ...formData, minProductionQuantity: Number(e.target.value) })}
              />
            </Grid>
            <Grid item xs={12} sm={4}>
              <TextField
                label="Max Üretim Miktarı"
                type="number"
                fullWidth
                value={formData.maxProductionQuantity}
                onChange={(e) => setFormData({ ...formData, maxProductionQuantity: Number(e.target.value) })}
              />
            </Grid>
            <Grid item xs={12} sm={4}>
              <TextField
                label="Üretim Süresi (Saat)"
                type="number"
                fullWidth
                value={formData.productionDurationHours}
                onChange={(e) => setFormData({ ...formData, productionDurationHours: Number(e.target.value) })}
              />
            </Grid>
            <Grid item xs={12}>
              <TextField
                label="Notlar"
                multiline
                rows={3}
                fullWidth
                value={formData.notes}
                onChange={(e) => setFormData({ ...formData, notes: e.target.value })}
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
    </Box>
  );
}
