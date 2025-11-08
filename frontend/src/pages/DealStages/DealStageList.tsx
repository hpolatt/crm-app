import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Box,
  Button,
  Chip,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  IconButton,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Typography,
  Alert,
  CircularProgress,
} from '@mui/material';
import { Add as AddIcon, Edit as EditIcon, Delete as DeleteIcon, Refresh as RefreshIcon } from '@mui/icons-material';
import { DealStage } from '../../types/dealStage';
import { dealStageService } from '../../services/dealStageService';

const DealStageList: React.FC = () => {
  const navigate = useNavigate();
  const [dealStages, setDealStages] = useState<DealStage[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [openConfirm, setOpenConfirm] = useState(false);
  const [selectedStage, setSelectedStage] = useState<DealStage | null>(null);
  const [draggedStage, setDraggedStage] = useState<DealStage | null>(null);

  useEffect(() => {
    loadDealStages();
  }, []);

  const loadDealStages = async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await dealStageService.getAll();
      setDealStages(data.sort((a, b) => a.order - b.order));
    } catch (err: any) {
      setError(err.response?.data?.message || 'DealStages yüklenirken hata oluştu');
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async () => {
    if (!selectedStage) return;

    try {
      await dealStageService.delete(selectedStage.id);
      setDealStages(dealStages.filter((s) => s.id !== selectedStage.id));
      setOpenConfirm(false);
      setSelectedStage(null);
    } catch (err: any) {
      setError(err.response?.data?.message || 'Silme işlemi başarısız');
    }
  };

  const handleDragStart = (stage: DealStage) => {
    setDraggedStage(stage);
  };

  const handleDragOver = (e: React.DragEvent) => {
    e.preventDefault();
  };

  const handleDrop = async (targetStage: DealStage) => {
    if (!draggedStage || draggedStage.id === targetStage.id) {
      setDraggedStage(null);
      return;
    }

    const draggedIndex = dealStages.findIndex((s) => s.id === draggedStage.id);
    const targetIndex = dealStages.findIndex((s) => s.id === targetStage.id);

    const newStages = [...dealStages];
    [newStages[draggedIndex], newStages[targetIndex]] = [newStages[targetIndex], newStages[draggedIndex]];

    // Sırayı güncelle
    const updatedStages = newStages.map((stage, index) => ({
      ...stage,
      order: index + 1,
    }));

    setDealStages(updatedStages);

    // Backend'e gönder
    try {
      await dealStageService.update(draggedStage.id, { order: targetIndex + 1 });
      await dealStageService.update(targetStage.id, { order: draggedIndex + 1 });
    } catch (err: any) {
      setError('Sıralama güncellenirken hata oluştu');
      loadDealStages(); // Hata durumunda yeniden yükle
    }

    setDraggedStage(null);
  };

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="400px">
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <Typography variant="h5">İşlem Aşamaları (Deal Stages)</Typography>
        <Box gap={1} display="flex">
          <Button
            variant="outlined"
            startIcon={<RefreshIcon />}
            onClick={loadDealStages}
            disabled={loading}
          >
            Yenile
          </Button>
          <Button
            variant="contained"
            startIcon={<AddIcon />}
            onClick={() => navigate('/deal-stages/new')}
          >
            Yeni Aşama
          </Button>
        </Box>
      </Box>

      {error && (
        <Alert severity="error" onClose={() => setError(null)} sx={{ mb: 2 }}>
          {error}
        </Alert>
      )}

      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow sx={{ backgroundColor: '#f5f5f5' }}>
              <TableCell width={40} />
              <TableCell>Aşama Adı</TableCell>
              <TableCell>Açıklama</TableCell>
              <TableCell width={100}>Renk</TableCell>
              <TableCell width={80} align="center">
                Varsayılan
              </TableCell>
              <TableCell width={80} align="center">
                Aktif
              </TableCell>
              <TableCell width={120} align="right">
                İşlemler
              </TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {dealStages.map((stage, index) => (
              <TableRow
                key={stage.id}
                draggable
                onDragStart={() => handleDragStart(stage)}
                onDragOver={handleDragOver}
                onDrop={() => handleDrop(stage)}
                sx={{
                  cursor: 'grab',
                  backgroundColor: draggedStage?.id === stage.id ? '#f0f0f0' : 'inherit',
                  '&:hover': {
                    backgroundColor: '#f9f9f9',
                  },
                }}
              >
                <TableCell align="center" sx={{ color: '#999' }}>
                  ⋮⋮
                </TableCell>
                <TableCell>
                  <Typography variant="body2" fontWeight={600}>
                    {index + 1}. {stage.name}
                  </Typography>
                </TableCell>
                <TableCell>
                  <Typography variant="body2" color="text.secondary">
                    {stage.description || '-'}
                  </Typography>
                </TableCell>
                <TableCell>
                  {stage.color && (
                    <Box
                      sx={{
                        width: 32,
                        height: 32,
                        backgroundColor: stage.color,
                        borderRadius: 1,
                        border: '1px solid #ddd',
                      }}
                    />
                  )}
                </TableCell>
                <TableCell align="center">
                  <Chip
                    label={stage.isDefault ? 'Evet' : 'Hayır'}
                    size="small"
                    color={stage.isDefault ? 'primary' : 'default'}
                    variant="outlined"
                  />
                </TableCell>
                <TableCell align="center">
                  <Chip
                    label={stage.isActive ? 'Aktif' : 'Pasif'}
                    size="small"
                    color={stage.isActive ? 'success' : 'default'}
                    variant="outlined"
                  />
                </TableCell>
                <TableCell align="right">
                  <IconButton
                    size="small"
                    onClick={() => navigate(`/deal-stages/${stage.id}`)}
                    color="primary"
                  >
                    <EditIcon />
                  </IconButton>
                  <IconButton
                    size="small"
                    onClick={() => {
                      setSelectedStage(stage);
                      setOpenConfirm(true);
                    }}
                    color="error"
                  >
                    <DeleteIcon />
                  </IconButton>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>

      {dealStages.length === 0 && !loading && (
        <Paper sx={{ p: 3, textAlign: 'center', mt: 2 }}>
          <Typography color="text.secondary">Henüz aşama oluşturulmadı</Typography>
        </Paper>
      )}

      {/* Delete Confirmation Dialog */}
      <Dialog open={openConfirm} onClose={() => setOpenConfirm(false)}>
        <DialogTitle>Aşamayı Sil</DialogTitle>
        <DialogContent>
          <Typography>
            "{selectedStage?.name}" aşamasını silmek istediğinize emin misiniz?
          </Typography>
          <Typography variant="caption" color="error" sx={{ display: 'block', mt: 2 }}>
            Bu işlem geri alınamaz.
          </Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenConfirm(false)}>İptal</Button>
          <Button onClick={handleDelete} color="error" variant="contained">
            Sil
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};

export default DealStageList;
