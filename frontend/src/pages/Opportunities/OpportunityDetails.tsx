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
  LinearProgress,
} from '@mui/material';
import {
  Edit as EditIcon,
  Delete as DeleteIcon,
  ArrowBack as ArrowBackIcon,
  AttachMoney as AttachMoneyIcon,
  TrendingUp as TrendingUpIcon,
  CalendarToday as CalendarIcon,
  Business as BusinessIcon,
  Person as PersonIcon,
} from '@mui/icons-material';
import { Opportunity } from '../../types/opportunity';
import { opportunityService } from '../../services/opportunityService';

const OpportunityDetails: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [opportunity, setOpportunity] = useState<Opportunity | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (id) {
      fetchOpportunity(id);
    }
  }, [id]);

  const fetchOpportunity = async (opportunityId: string) => {
    try {
      setLoading(true);
      setError(null);
      const data = await opportunityService.getById(opportunityId);
      setOpportunity(data);
    } catch (err: any) {
      setError(err.response?.data?.message || 'Fırsat yüklenirken bir hata oluştu');
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async () => {
    if (!id || !window.confirm('Bu fırsatı silmek istediğinizden emin misiniz?')) {
      return;
    }

    try {
      await opportunityService.delete(id);
      navigate('/opportunities');
    } catch (err: any) {
      setError(err.response?.data?.message || 'Fırsat silinirken bir hata oluştu');
    }
  };

  const getStageColor = (stage: string): 'default' | 'primary' | 'secondary' | 'success' | 'warning' | 'error' => {
    const stageLower = stage.toLowerCase();
    if (stageLower.includes('won') || stageLower.includes('kazanıldı')) return 'success';
    if (stageLower.includes('lost') || stageLower.includes('kaybedildi')) return 'error';
    if (stageLower.includes('proposal') || stageLower.includes('teklif')) return 'warning';
    if (stageLower.includes('negotiation') || stageLower.includes('müzakere')) return 'secondary';
    return 'primary';
  };

  const getProbabilityColor = (probability: number): string => {
    if (probability >= 75) return '#4caf50';
    if (probability >= 50) return '#ff9800';
    if (probability >= 25) return '#ff5722';
    return '#f44336';
  };

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="400px">
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return (
      <Box p={3}>
        <Alert severity="error" onClose={() => setError(null)}>
          {error}
        </Alert>
      </Box>
    );
  }

  if (!opportunity) {
    return (
      <Box p={3}>
        <Alert severity="warning">Fırsat bulunamadı</Alert>
      </Box>
    );
  }

  return (
    <Box>
      {/* Header */}
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <Box display="flex" alignItems="center" gap={2}>
          <Button
            startIcon={<ArrowBackIcon />}
            onClick={() => navigate('/opportunities')}
          >
            Geri
          </Button>
          <Typography variant="h5">{opportunity.title}</Typography>
          <Chip
            label={opportunity.stage}
            color={getStageColor(opportunity.stage)}
            size="small"
          />
        </Box>
        <Box display="flex" gap={1}>
          <Button
            variant="outlined"
            startIcon={<EditIcon />}
            onClick={() => navigate(`/opportunities/${id}/edit`)}
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
        </Box>
      </Box>

      <Grid container spacing={3}>
        {/* Main Info */}
        <Grid item xs={12} md={8}>
          <Paper sx={{ p: 3, mb: 3 }}>
            <Typography variant="h6" gutterBottom>
              Genel Bilgiler
            </Typography>
            <Divider sx={{ mb: 2 }} />

            <Grid container spacing={3}>
              <Grid item xs={12} sm={6}>
                <Box display="flex" alignItems="center" gap={1} mb={2}>
                  <AttachMoneyIcon color="action" />
                  <Box>
                    <Typography variant="caption" color="text.secondary">
                      Fırsat Değeri
                    </Typography>
                    <Typography variant="h6" color="primary">
                      {opportunity.value.toLocaleString('tr-TR', {
                        style: 'currency',
                        currency: 'TRY',
                      })}
                    </Typography>
                  </Box>
                </Box>
              </Grid>

              <Grid item xs={12} sm={6}>
                <Box display="flex" alignItems="center" gap={1} mb={2}>
                  <TrendingUpIcon color="action" />
                  <Box>
                    <Typography variant="caption" color="text.secondary">
                      Başarı Olasılığı
                    </Typography>
                    <Box display="flex" alignItems="center" gap={1} mt={0.5}>
                      <Typography variant="h6" sx={{ color: getProbabilityColor(opportunity.probability ?? 0) }}>
                        {opportunity.probability ?? 0}%
                      </Typography>
                    </Box>
                  </Box>
                </Box>
              </Grid>

              <Grid item xs={12}>
                <Box mb={2}>
                  <Typography variant="caption" color="text.secondary" gutterBottom>
                    İlerleme
                  </Typography>
                  <LinearProgress
                    variant="determinate"
                    value={opportunity.probability ?? 0}
                    sx={{
                      height: 8,
                      borderRadius: 4,
                      backgroundColor: 'rgba(0,0,0,0.1)',
                      '& .MuiLinearProgress-bar': {
                        backgroundColor: getProbabilityColor(opportunity.probability ?? 0),
                      },
                    }}
                  />
                </Box>
              </Grid>

              {opportunity.description && (
                <Grid item xs={12}>
                  <Box>
                    <Typography variant="caption" color="text.secondary">
                      Açıklama
                    </Typography>
                    <Typography variant="body1" sx={{ whiteSpace: 'pre-wrap', mt: 1 }}>
                      {opportunity.description}
                    </Typography>
                  </Box>
                </Grid>
              )}

              <Grid item xs={12} sm={6}>
                <Box display="flex" alignItems="center" gap={1}>
                  <CalendarIcon color="action" fontSize="small" />
                  <Box>
                    <Typography variant="caption" color="text.secondary">
                      Tahmini Kapanış Tarihi
                    </Typography>
                    <Typography variant="body1">
                      {opportunity.expectedCloseDate
                        ? new Date(opportunity.expectedCloseDate).toLocaleDateString('tr-TR')
                        : '-'}
                    </Typography>
                  </Box>
                </Box>
              </Grid>

              {opportunity.actualCloseDate && (
                <Grid item xs={12} sm={6}>
                  <Box display="flex" alignItems="center" gap={1}>
                    <CalendarIcon color="action" fontSize="small" />
                    <Box>
                      <Typography variant="caption" color="text.secondary">
                        Gerçek Kapanış Tarihi
                      </Typography>
                      <Typography variant="body1">
                        {new Date(opportunity.actualCloseDate).toLocaleDateString('tr-TR')}
                      </Typography>
                    </Box>
                  </Box>
                </Grid>
              )}
            </Grid>
          </Paper>
        </Grid>

        {/* Sidebar */}
        <Grid item xs={12} md={4}>
          <Card sx={{ mb: 2 }}>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Durum Bilgileri
              </Typography>
              <Divider sx={{ mb: 2 }} />

              <Box mb={2}>
                <Typography variant="caption" color="text.secondary">
                  Aşama
                </Typography>
                <Box mt={0.5}>
                  <Chip
                    label={opportunity.stage}
                    color={getStageColor(opportunity.stage)}
                    size="small"
                  />
                </Box>
              </Box>

              <Box mb={2}>
                <Typography variant="caption" color="text.secondary">
                  Durum
                </Typography>
                <Box mt={0.5}>
                  <Chip
                    label={opportunity.isActive ? 'Aktif' : 'Pasif'}
                    color={opportunity.isActive ? 'success' : 'default'}
                    size="small"
                    variant="outlined"
                  />
                </Box>
              </Box>

              <Box mb={2}>
                <Typography variant="caption" color="text.secondary">
                  Oluşturulma Tarihi
                </Typography>
                <Typography variant="body2">
                  {new Date(opportunity.createdAt).toLocaleDateString('tr-TR')}
                </Typography>
              </Box>

              <Box>
                <Typography variant="caption" color="text.secondary">
                  Son Güncelleme
                </Typography>
                <Typography variant="body2">
                  {new Date(opportunity.updatedAt).toLocaleDateString('tr-TR')}
                </Typography>
              </Box>
            </CardContent>
          </Card>

          {/* Relations */}
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                İlişkiler
              </Typography>
              <Divider sx={{ mb: 2 }} />

              {opportunity.companyId && (
                <Box display="flex" alignItems="center" gap={1} mb={2}>
                  <BusinessIcon color="action" fontSize="small" />
                  <Box>
                    <Typography variant="caption" color="text.secondary">
                      Şirket ID
                    </Typography>
                    <Typography variant="body2">{opportunity.companyId}</Typography>
                  </Box>
                </Box>
              )}

              {opportunity.contactId && (
                <Box display="flex" alignItems="center" gap={1} mb={2}>
                  <PersonIcon color="action" fontSize="small" />
                  <Box>
                    <Typography variant="caption" color="text.secondary">
                      İletişim ID
                    </Typography>
                    <Typography variant="body2">{opportunity.contactId}</Typography>
                  </Box>
                </Box>
              )}

              {opportunity.assignedUserId && (
                <Box display="flex" alignItems="center" gap={1}>
                  <PersonIcon color="action" fontSize="small" />
                  <Box>
                    <Typography variant="caption" color="text.secondary">
                      Atanan Kullanıcı
                    </Typography>
                    <Typography variant="body2">{opportunity.assignedUserId}</Typography>
                  </Box>
                </Box>
              )}
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Box>
  );
};

export default OpportunityDetails;
