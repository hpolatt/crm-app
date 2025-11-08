import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import {
  Box,
  Paper,
  Typography,
  TextField,
  Button,
  Grid,
  MenuItem,
  CircularProgress,
  Alert,
} from '@mui/material';
import { Save as SaveIcon, ArrowBack as ArrowBackIcon } from '@mui/icons-material';
import { leadService } from '../../services/leadService';
import { CreateLeadDto } from '../../types/lead';

const LeadForm: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const isEdit = Boolean(id);

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [formData, setFormData] = useState<CreateLeadDto>({
    title: '',
    description: '',
    source: 'Website',
    status: 'new',
    value: undefined,
    probability: undefined,
    expectedCloseDate: undefined,
    companyId: undefined,
    contactId: undefined,
    assignedUserId: undefined,
    notes: '',
  });

  useEffect(() => {
    if (isEdit && id) {
      loadLead(id);
    }
  }, [id, isEdit]);

  const loadLead = async (leadId: string) => {
    try {
      setLoading(true);
      const data = await leadService.getLeadById(leadId);
      setFormData({
        title: data.title,
        description: data.description || '',
        source: data.source,
        status: data.status,
        value: data.value,
        probability: data.probability,
        expectedCloseDate: data.expectedCloseDate,
        companyId: data.companyId,
        contactId: data.contactId,
        assignedUserId: data.assignedUserId,
        notes: data.notes || '',
      });
    } catch (err: any) {
      setError(err.message || 'Lead yüklenirken bir hata oluştu');
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    if (!formData.title.trim()) {
      setError('Başlık zorunludur');
      return;
    }

    try {
      setLoading(true);
      if (isEdit && id) {
        await leadService.updateLead(id, formData);
      } else {
        await leadService.createLead(formData);
      }
      navigate('/leads');
    } catch (err: any) {
      setError(err.message || 'Lead kaydedilirken bir hata oluştu');
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (field: keyof CreateLeadDto) => (
    e: React.ChangeEvent<HTMLInputElement>
  ) => {
    const value = e.target.value;
    setFormData((prev) => ({
      ...prev,
      [field]: field === 'value' || field === 'probability' ? (value ? Number(value) : undefined) : value,
    }));
  };

  if (loading && isEdit) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="400px">
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box p={3}>
      <Box display="flex" alignItems="center" gap={2} mb={3}>
        <Button
          startIcon={<ArrowBackIcon />}
          onClick={() => navigate('/leads')}
        >
          Geri
        </Button>
        <Typography variant="h4">
          {isEdit ? 'Lead Düzenle' : 'Yeni Lead'}
        </Typography>
      </Box>

      {error && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {error}
        </Alert>
      )}

      <Paper sx={{ p: 3 }}>
        <form onSubmit={handleSubmit}>
          <Grid container spacing={3}>
            <Grid item xs={12} md={6}>
              <TextField
                fullWidth
                required
                label="Başlık"
                value={formData.title}
                onChange={handleChange('title')}
              />
            </Grid>

            <Grid item xs={12} md={6}>
              <TextField
                fullWidth
                select
                label="Durum"
                value={formData.status}
                onChange={handleChange('status')}
              >
                <MenuItem value="new">Yeni</MenuItem>
                <MenuItem value="contacted">İletişimde</MenuItem>
                <MenuItem value="qualified">Nitelikli</MenuItem>
                <MenuItem value="unqualified">Niteliksiz</MenuItem>
                <MenuItem value="converted">Dönüştürüldü</MenuItem>
              </TextField>
            </Grid>

            <Grid item xs={12}>
              <TextField
                fullWidth
                multiline
                rows={4}
                label="Açıklama"
                value={formData.description}
                onChange={handleChange('description')}
              />
            </Grid>

            <Grid item xs={12} md={6}>
              <TextField
                fullWidth
                label="Kaynak"
                value={formData.source}
                onChange={handleChange('source')}
              />
            </Grid>

            <Grid item xs={12} md={6}>
              <TextField
                fullWidth
                type="number"
                label="Değer"
                value={formData.value || ''}
                onChange={handleChange('value')}
                inputProps={{ min: 0, step: 0.01 }}
              />
            </Grid>

            <Grid item xs={12} md={6}>
              <TextField
                fullWidth
                type="number"
                label="Olasılık (%)"
                value={formData.probability || ''}
                onChange={handleChange('probability')}
                inputProps={{ min: 0, max: 100 }}
              />
            </Grid>

            <Grid item xs={12} md={6}>
              <TextField
                fullWidth
                type="date"
                label="Tahmini Kapanış Tarihi"
                value={formData.expectedCloseDate || ''}
                onChange={handleChange('expectedCloseDate')}
                InputLabelProps={{ shrink: true }}
              />
            </Grid>

            <Grid item xs={12}>
              <TextField
                fullWidth
                multiline
                rows={3}
                label="Notlar"
                value={formData.notes}
                onChange={handleChange('notes')}
              />
            </Grid>

            <Grid item xs={12}>
              <Box display="flex" gap={2} justifyContent="flex-end">
                <Button
                  variant="outlined"
                  onClick={() => navigate('/leads')}
                  disabled={loading}
                >
                  İptal
                </Button>
                <Button
                  type="submit"
                  variant="contained"
                  startIcon={<SaveIcon />}
                  disabled={loading}
                >
                  {loading ? 'Kaydediliyor...' : 'Kaydet'}
                </Button>
              </Box>
            </Grid>
          </Grid>
        </form>
      </Paper>
    </Box>
  );
};

export default LeadForm;
