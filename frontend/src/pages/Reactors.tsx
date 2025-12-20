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
} from '@mui/material';
import { Add, Edit, Delete } from '@mui/icons-material';
import { api } from '../services/api';

interface Reactor {
  id: string;
  name: string;
  createdAt: string;
  updatedAt?: string;
}

export default function Reactors() {
  const [reactors, setReactors] = useState<Reactor[]>([]);
  const [open, setOpen] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [formData, setFormData] = useState({ name: '' });

  useEffect(() => {
    fetchReactors();
  }, []);

  const fetchReactors = async () => {
    try {
      const response = await api.get('/reactors');
      setReactors(response.data.data || []);
    } catch (error) {
      console.error('Error fetching reactors:', error);
    }
  };

  const handleOpen = (reactor?: Reactor) => {
    if (reactor) {
      setEditingId(reactor.id);
      setFormData({ name: reactor.name });
    } else {
      setEditingId(null);
      setFormData({ name: '' });
    }
    setOpen(true);
  };

  const handleClose = () => {
    setOpen(false);
    setEditingId(null);
    setFormData({ name: '' });
  };

  const handleSubmit = async () => {
    try {
      if (editingId) {
        await api.put(`/reactors/${editingId}`, formData);
      } else {
        await api.post('/reactors', formData);
      }
      fetchReactors();
      handleClose();
    } catch (error) {
      console.error('Error saving reactor:', error);
    }
  };

  const handleDelete = async (id: string) => {
    if (window.confirm('Bu reaktörü silmek istediğinize emin misiniz?')) {
      try {
        await api.delete(`/reactors/${id}`);
        fetchReactors();
      } catch (error) {
        console.error('Error deleting reactor:', error);
      }
    }
  };

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 3 }}>
        <Typography variant="h4">Reaktörler</Typography>
        <Button variant="contained" startIcon={<Add />} onClick={() => handleOpen()}>
          Yeni Ekle
        </Button>
      </Box>

      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Ad</TableCell>
              <TableCell>Oluşturulma Tarihi</TableCell>
              <TableCell align="right">İşlemler</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {reactors.map((reactor) => (
              <TableRow key={reactor.id}>
                <TableCell>{reactor.name}</TableCell>
                <TableCell>{new Date(reactor.createdAt).toLocaleDateString('tr-TR')}</TableCell>
                <TableCell align="right">
                  <IconButton onClick={() => handleOpen(reactor)} color="primary">
                    <Edit />
                  </IconButton>
                  <IconButton onClick={() => handleDelete(reactor.id)} color="error">
                    <Delete />
                  </IconButton>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>

      <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
        <DialogTitle>{editingId ? 'Reaktörü Düzenle' : 'Yeni Reaktör'}</DialogTitle>
        <DialogContent>
          <TextField
            autoFocus
            margin="dense"
            label="Ad"
            fullWidth
            value={formData.name}
            onChange={(e) => setFormData({ name: e.target.value })}
          />
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
