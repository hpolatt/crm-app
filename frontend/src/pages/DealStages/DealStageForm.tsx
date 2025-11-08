import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Box,
  Paper,
  Typography,
  TextField,
  Button,
  Grid,
  CircularProgress,
  Alert,
  FormControlLabel,
  Checkbox,
} from '@mui/material';
import { Save as SaveIcon, Cancel as CancelIcon } from '@mui/icons-material';
import { CreateDealStageDto, UpdateDealStageDto } from '../../types/dealStage';
import { dealStageService } from '../../services/dealStageService';

const DealStageForm: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const isEditMode = Boolean(id);

  const [formData, setFormData] = useState<CreateDealStageDto>({
    name: '',
    description: '',
    order: 0,
    color: '#1976d2',
    isDefault: false,
  });

  const [loading, setLoading] = useState(false);
  const [fetchLoading, setFetchLoading] = useState(isEditMode);
  const [error, setError] = useState<string | null>(null);
  const [errors, setErrors] = useState<Record<string, string>>({});

  useEffect(() => {
    if (id) {
      fetchDealStage(id);
    } else {
      // Yeni aşama için order'ı otomatik belirle
      loadMaxOrder();
    }
  }, [id]);

  const loadMaxOrder = async () => {
    try {
      const stages = await dealStageService.getAll();
      const maxOrder = Math.max(...stages.map(s => s.order), 0);
      setFormData(prev => ({ ...prev, order: maxOrder + 1 }));
    } catch (err) {
      console.error('Order belirlenirken hata:', err);
    }
  };

  const fetchDealStage = async (stageId: string) => {
    try {
      setFetchLoading(true);
      const stages = await dealStageService.getAll();
      const stage = stages.find(s => s.id === stageId);
      
      if (!stage) {
        setError('Aşama bulunamadı');
        return;
      }

      setFormData({
        name: stage.name,
        description: stage.description || '',
        order: stage.order,
        color: stage.color || '#1976d2',
        isDefault: stage.isDefault,
      });
    } catch (err: any) {
      setError(err.response?.data?.message || 'Aşama yüklenirken bir hata oluştu');
    } finally {
      setFetchLoading(false);
    }
  };

  const validateForm = (): boolean => {
    const newErrors: Record<string, string> = {};

    if (!formData.name.trim()) {
      newErrors.name = 'Aşama adı gereklidir';
    }

    if (formData.order < 1) {
      newErrors.order = 'Sıra numarası 1 veya daha büyük olmalıdır';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!validateForm()) {
      return;
    }

    try {
      setLoading(true);
      setError(null);

      const submitData: CreateDealStageDto | UpdateDealStageDto = {
        name: formData.name,
        description: formData.description || undefined,
        order: formData.order,
        color: formData.color,
        isDefault: formData.isDefault,
      };

      if (isEditMode && id) {
        await dealStageService.update(id, submitData as UpdateDealStageDto);
      } else {
        await dealStageService.create(submitData as CreateDealStageDto);
      }

      navigate('/deal-stages');
    } catch (err: any) {
      setError(err.response?.data?.message || 'Kaydetme sırasında bir hata oluştu');
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (field: keyof CreateDealStageDto) => (
    e: React.ChangeEvent<HTMLInputElement>
  ) => {
    const value = e.target.value;
    setFormData((prev) => ({
      ...prev,
      [field]: field === 'order' ? (value ? parseInt(value) : 0) : value,
    }));

    // Clear error for this field
    if (errors[field]) {
      setErrors((prev) => {
        const newErrors = { ...prev };
        delete newErrors[field];
        return newErrors;
      });
    }
  };

  const handleCheckboxChange = (field: keyof CreateDealStageDto) => (
    e: React.ChangeEvent<HTMLInputElement>
  ) => {
    setFormData((prev) => ({
      ...prev,
      [field]: e.target.checked,
    }));
  };

  if (fetchLoading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="400px">
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box>
      <Typography variant="h5" gutterBottom>
        {isEditMode ? 'Aşamayı Düzenle' : 'Yeni Aşama'}
      </Typography>

      {error && (
        <Alert severity="error" onClose={() => setError(null)} sx={{ mb: 2 }}>
          {error}
        </Alert>
      )}

      <Paper sx={{ p: 3 }}>
        <form onSubmit={handleSubmit}>
          <Grid container spacing={3}>
            <Grid item xs={12}>
              <TextField
                label="Aşama Adı"
                fullWidth
                required
                value={formData.name}
                onChange={handleChange('name')}
                error={Boolean(errors.name)}
                helperText={errors.name}
                placeholder="örn: Keşif, Teklif, Kazanıldı"
              />
            </Grid>

            <Grid item xs={12}>
              <TextField
                label="Açıklama"
                fullWidth
                multiline
                rows={4}
                value={formData.description}
                onChange={handleChange('description')}
                placeholder="Bu aşamanın açıklamasını giriniz"
              />
            </Grid>

            <Grid item xs={12} sm={6}>
              <TextField
                label="Sıra Numarası"
                type="number"
                fullWidth
                required
                value={formData.order}
                onChange={handleChange('order')}
                error={Boolean(errors.order)}
                helperText={errors.order || 'Aşamaların gösterilme sırası'}
                InputProps={{
                  inputProps: { min: 1 },
                }}
              />
            </Grid>

            <Grid item xs={12} sm={6}>
              <TextField
                label="Renk"
                type="color"
                fullWidth
                value={formData.color}
                onChange={handleChange('color')}
                InputLabelProps={{ shrink: true }}
                sx={{
                  '& input': {
                    cursor: 'pointer',
                  },
                }}
              />
            </Grid>

            <Grid item xs={12}>
              <FormControlLabel
                control={
                  <Checkbox
                    checked={formData.isDefault}
                    onChange={handleCheckboxChange('isDefault')}
                  />
                }
                label="Varsayılan Aşama (Yeni fırsatlar bu aşamada başlayacak)"
              />
            </Grid>

            <Grid item xs={12}>
              <Box display="flex" gap={2} justifyContent="flex-end">
                <Button
                  variant="outlined"
                  startIcon={<CancelIcon />}
                  onClick={() => navigate('/deal-stages')}
                  disabled={loading}
                >
                  İptal
                </Button>
                <Button
                  variant="contained"
                  startIcon={<SaveIcon />}
                  onClick={handleSubmit}
                  disabled={loading}
                >
                  {loading ? <CircularProgress size={24} /> : 'Kaydet'}
                </Button>
              </Box>
            </Grid>
          </Grid>
        </form>
      </Paper>
    </Box>
  );
};

export default DealStageForm;
