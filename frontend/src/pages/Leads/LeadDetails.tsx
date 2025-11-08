import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Box,
  Paper,
  Typography,
  Button,
  Grid,
  Chip,
  Divider,
  CircularProgress,
  Alert,
  Card,
  CardContent,
} from '@mui/material';
import {
  Edit as EditIcon,
  Delete as DeleteIcon,
  ArrowBack as ArrowBackIcon,
  TrendingUp as TrendingUpIcon,
} from '@mui/icons-material';
import { Lead } from '../../types/lead';
import { leadService } from '../../services/leadService';

const getStatusLabel = (status: string) => {
  const labels: any = {
    'new': 'Yeni',
    'contacted': 'İletişimde',
    'qualified': 'Nitelikli',
    'unqualified': 'Niteliksiz',
    'converted': 'Dönüştürüldü'
  }
  return labels[status] || status
}

const getStatusColor = (status: string): 'info' | 'warning' | 'success' | 'error' | 'default' => {
  const colors: any = {
    'new': 'info',
    'contacted': 'warning',
    'qualified': 'success',
    'unqualified': 'error',
    'converted': 'success',
  }
  return colors[status] || 'default'
}

const LeadDetails: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [lead, setLead] = useState<Lead | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    loadLead();
  }, [id]);

  const loadLead = async () => {
    if (!id) return;
    
    try {
      setLoading(true);
      const data = await leadService.getLeadById(id);
      setLead(data);
    } catch (err: any) {
      setError(err.message || 'Lead yüklenirken bir hata oluştu');
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async () => {
    if (!id) return;
    if (window.confirm('Bu lead\'i silmek istediğinizden emin misiniz?')) {
      try {
        await leadService.deleteLead(id);
        navigate('/leads');
      } catch (err: any) {
        alert(err.message || 'Lead silinirken bir hata oluştu');
      }
    }
  };

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="400px">
        <CircularProgress />
      </Box>
    );
  }

  if (error || !lead) {
    return (
      <Box p={3}>
        <Alert severity="error">{error || 'Lead bulunamadı'}</Alert>
        <Button
          startIcon={<ArrowBackIcon />}
          onClick={() => navigate('/leads')}
          sx={{ mt: 2 }}
        >
          Listeye Dön
        </Button>
      </Box>
    );
  }

  return (
    <Box p={3}>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <Box display="flex" alignItems="center" gap={2}>
          <Button
            startIcon={<ArrowBackIcon />}
            onClick={() => navigate('/leads')}
          >
            Geri
          </Button>
          <Typography variant="h4">{lead.title}</Typography>
          <Chip
            label={getStatusLabel(lead.status)}
            color={getStatusColor(lead.status)}
            size="medium"
          />
        </Box>
        <Box display="flex" gap={1}>
          <Button
            variant="outlined"
            startIcon={<EditIcon />}
            onClick={() => navigate(`/leads/${id}/edit`)}
          >
            Düzenle
          </Button>
          <Button
            variant="outlined"
            color="error"
            startIcon={<DeleteIcon />}
            onClick={handleDelete}
          >
            Sil
          </Button>
          {lead.status === 'qualified' && (
            <Button
              variant="contained"
              color="success"
              startIcon={<TrendingUpIcon />}
              onClick={() => navigate(`/leads/${id}/convert`)}
            >
              Fırsata Dönüştür
            </Button>
          )}
        </Box>
      </Box>

      <Grid container spacing={3}>
        <Grid item xs={12} md={8}>
          <Paper sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>
              Lead Bilgileri
            </Typography>
            <Divider sx={{ mb: 2 }} />
            
            <Grid container spacing={2}>
              <Grid item xs={12} sm={6}>
                <Typography variant="subtitle2" color="text.secondary">
                  Başlık
                </Typography>
                <Typography variant="body1">{lead.title}</Typography>
              </Grid>

              <Grid item xs={12} sm={6}>
                <Typography variant="subtitle2" color="text.secondary">
                  Kaynak
                </Typography>
                <Typography variant="body1">{lead.source || '-'}</Typography>
              </Grid>

              <Grid item xs={12}>
                <Typography variant="subtitle2" color="text.secondary">
                  Açıklama
                </Typography>
                <Typography variant="body1">{lead.description || '-'}</Typography>
              </Grid>

              {lead.value && (
                <Grid item xs={12} sm={6}>
                  <Typography variant="subtitle2" color="text.secondary">
                    Değer
                  </Typography>
                  <Typography variant="body1">
                    {lead.value.toLocaleString('tr-TR', {
                      minimumFractionDigits: 2,
                      style: 'currency',
                      currency: 'TRY'
                    })}
                  </Typography>
                </Grid>
              )}

              {lead.probability !== undefined && lead.probability !== null && (
                <Grid item xs={12} sm={6}>
                  <Typography variant="subtitle2" color="text.secondary">
                    Olasılık
                  </Typography>
                  <Typography variant="body1">%{lead.probability}</Typography>
                </Grid>
              )}

              {lead.expectedCloseDate && (
                <Grid item xs={12} sm={6}>
                  <Typography variant="subtitle2" color="text.secondary">
                    Tahmini Kapanış Tarihi
                  </Typography>
                  <Typography variant="body1">
                    {new Date(lead.expectedCloseDate).toLocaleDateString('tr-TR')}
                  </Typography>
                </Grid>
              )}

              {lead.notes && (
                <Grid item xs={12}>
                  <Typography variant="subtitle2" color="text.secondary">
                    Notlar
                  </Typography>
                  <Typography variant="body1">{lead.notes}</Typography>
                </Grid>
              )}
            </Grid>
          </Paper>
        </Grid>

        <Grid item xs={12} md={4}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Durum Bilgisi
              </Typography>
              <Divider sx={{ mb: 2 }} />
              
              <Box mb={2}>
                <Typography variant="subtitle2" color="text.secondary">
                  Mevcut Durum
                </Typography>
                <Chip
                  label={getStatusLabel(lead.status)}
                  color={getStatusColor(lead.status)}
                  size="small"
                  sx={{ mt: 0.5 }}
                />
              </Box>

              <Box mb={2}>
                <Typography variant="subtitle2" color="text.secondary">
                  Oluşturulma Tarihi
                </Typography>
                <Typography variant="body2">
                  {new Date(lead.createdAt).toLocaleDateString('tr-TR')}
                </Typography>
              </Box>

              {lead.updatedAt && (
                <Box mb={2}>
                  <Typography variant="subtitle2" color="text.secondary">
                    Son Güncelleme
                  </Typography>
                  <Typography variant="body2">
                    {new Date(lead.updatedAt).toLocaleDateString('tr-TR')}
                  </Typography>
                </Box>
              )}

              <Box>
                <Typography variant="subtitle2" color="text.secondary">
                  Aktif
                </Typography>
                <Typography variant="body2">
                  {lead.isActive ? 'Evet' : 'Hayır'}
                </Typography>
              </Box>
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Box>
  );
};

export default LeadDetails;
