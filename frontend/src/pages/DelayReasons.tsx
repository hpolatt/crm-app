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
import axios from 'axios';

interface DelayReason {
  id: string;
  name: string;
  createdAt: string;
  updatedAt?: string;
}

export default function DelayReasons() {
  const [delayReasons, setDelayReasons] = useState<DelayReason[]>([]);
  const [open, setOpen] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [formData, setFormData] = useState({ name: '' });

  useEffect(() => {
    fetchDelayReasons();
  }, []);

  const fetchDelayReasons = async () => {
    try {
      const response = await axios.get('http://localhost:5000/api/delayreasons');
      setDelayReasons(response.data.data || []);
    } catch (error) {
      console.error('Error fetching delay reasons:', error);
    }
  };

  const handleOpen = (delayReason?: DelayReason) => {
    if (delayReason) {
      setEditingId(delayReason.id);
      setFormData({ name: delayReason.name });
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
        await axios.put(`http://localhost:5000/api/delayreasons/${editingId}`, formData);
      } else {
        await axios.post('http://localhost:5000/api/delayreasons', formData);
      }
      fetchDelayReasons();
      handleClose();
    } catch (error) {
      console.error('Error saving delay reason:', error);
    }
  };

  const handleDelete = async (id: string) => {
    if (window.confirm('Bu gecikme nedenini silmek istediğinize emin misiniz?')) {
      try {
        await axios.delete(`http://localhost:5000/api/delayreasons/${id}`);
        fetchDelayReasons();
      } catch (error) {
        console.error('Error deleting delay reason:', error);
      }
    }
  };

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 3 }}>
        <Typography variant="h4">Gecikme Nedenleri</Typography>
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
            {delayReasons.map((delayReason) => (
              <TableRow key={delayReason.id}>
                <TableCell>{delayReason.name}</TableCell>
                <TableCell>{new Date(delayReason.createdAt).toLocaleDateString('tr-TR')}</TableCell>
                <TableCell align="right">
                  <IconButton onClick={() => handleOpen(delayReason)} color="primary">
                    <Edit />
                  </IconButton>
                  <IconButton onClick={() => handleDelete(delayReason.id)} color="error">
                    <Delete />
                  </IconButton>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>

      <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
        <DialogTitle>{editingId ? 'Gecikme Nedenini Düzenle' : 'Yeni Gecikme Nedeni'}</DialogTitle>
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
