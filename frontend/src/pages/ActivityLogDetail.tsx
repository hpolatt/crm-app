import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Container,
  Box,
  Paper,
  Typography,
  Button,
  Grid,
  Chip,
  CircularProgress,
  Alert,
  Divider,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
} from '@mui/material';
import {
  ArrowBack as ArrowBackIcon,
  Error as ErrorIcon,
  CheckCircle as SuccessIcon,
  Warning as WarningIcon,
  Info as InfoIcon,
} from '@mui/icons-material';
import { ActivityLog } from '../types/activityLog';
import { activityLogService } from '../services/activityLogService';
import { useToast } from '../components/Toast';

const ActivityLogDetail: React.FC = () => {
  const { requestId } = useParams<{ requestId: string }>();
  const navigate = useNavigate();
  const { showToast } = useToast();
  const [log, setLog] = useState<ActivityLog | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    loadLog();
  }, [requestId]);

  const loadLog = async () => {
    if (!requestId) {
      setError('Request ID not found');
      setLoading(false);
      return;
    }

    try {
      setLoading(true);
      setError(null);
      // Get all logs and find the one with matching requestId
      const response = await activityLogService.getAll({ pageNumber: 1, pageSize: 1000 });
      const foundLog = response.items?.find((l: ActivityLog) => l.requestId === requestId);
      
      if (foundLog) {
        setLog(foundLog);
      } else {
        setError('Activity log not found');
      }
    } catch (err) {
      console.error('Failed to load activity log:', err);
      setError('Failed to load activity log');
      showToast('Failed to load activity log', 'error');
    } finally {
      setLoading(false);
    }
  };

  const getStatusColor = (statusCode: number): 'success' | 'warning' | 'error' | 'info' => {
    if (statusCode < 300) return 'success';
    if (statusCode < 400) return 'info';
    if (statusCode < 500) return 'warning';
    return 'error';
  };

  const getStatusIcon = (statusCode: number) => {
    if (statusCode < 300) return <SuccessIcon />;
    if (statusCode < 400) return <InfoIcon />;
    if (statusCode < 500) return <WarningIcon />;
    return <ErrorIcon />;
  };

  const getMethodColor = (method: string): 'primary' | 'success' | 'warning' | 'error' | 'info' => {
    switch (method.toUpperCase()) {
      case 'GET':
        return 'info';
      case 'POST':
        return 'success';
      case 'PUT':
        return 'primary';
      case 'DELETE':
        return 'error';
      case 'PATCH':
        return 'warning';
      default:
        return 'info';
    }
  };

  if (loading) {
    return (
      <Container maxWidth="lg" sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '400px' }}>
        <CircularProgress />
      </Container>
    );
  }

  if (error || !log) {
    return (
      <Container maxWidth="lg" sx={{ mt: 4 }}>
        <Button
          startIcon={<ArrowBackIcon />}
          onClick={() => navigate('/activity-logs')}
          sx={{ mb: 2 }}
        >
          Geri
        </Button>
        <Alert severity="error">{error || 'Activity log not found'}</Alert>
      </Container>
    );
  }

  return (
    <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
      {/* Header */}
      <Box sx={{ mb: 3 }}>
        <Button
          startIcon={<ArrowBackIcon />}
          onClick={() => navigate('/activity-logs')}
          sx={{ mb: 2 }}
        >
          Geri
        </Button>
        <Typography variant="h4" component="h1">
          Activity Log Detayı
        </Typography>
      </Box>

      {/* Request ID and Timestamp */}
      <Paper sx={{ p: 3, mb: 3 }}>
        <Grid container spacing={3}>
          <Grid item xs={12} md={6}>
            <Typography variant="subtitle2" color="textSecondary" gutterBottom>
              Request ID
            </Typography>
            <Typography variant="body2" sx={{ fontFamily: 'monospace', wordBreak: 'break-all' }}>
              {log.requestId}
            </Typography>
          </Grid>
          <Grid item xs={12} md={6}>
            <Typography variant="subtitle2" color="textSecondary" gutterBottom>
              Timestamp
            </Typography>
            <Typography variant="body2">
              {new Date(log.timestamp).toLocaleString('tr-TR')}
            </Typography>
          </Grid>
        </Grid>
      </Paper>

      {/* HTTP Request Details */}
      <Paper sx={{ p: 3, mb: 3 }}>
        <Typography variant="h6" gutterBottom>
          HTTP İsteği
        </Typography>
        <Divider sx={{ mb: 2 }} />
        
        <Grid container spacing={2}>
          <Grid item xs={12} sm={6} md={3}>
            <Typography variant="subtitle2" color="textSecondary" gutterBottom>
              Method
            </Typography>
            <Chip
              label={log.method}
              color={getMethodColor(log.method)}
              variant="outlined"
              size="small"
            />
          </Grid>
          <Grid item xs={12} sm={6} md={3}>
            <Typography variant="subtitle2" color="textSecondary" gutterBottom>
              Status Code
            </Typography>
            <Chip
              label={log.statusCode}
              icon={getStatusIcon(log.statusCode)}
              color={getStatusColor(log.statusCode)}
              variant="outlined"
              size="small"
            />
          </Grid>
          <Grid item xs={12} sm={6} md={3}>
            <Typography variant="subtitle2" color="textSecondary" gutterBottom>
              Duration
            </Typography>
            <Typography variant="body2">
              {log.durationMs}ms
            </Typography>
          </Grid>
          <Grid item xs={12} sm={6} md={3}>
            <Typography variant="subtitle2" color="textSecondary" gutterBottom>
              Path
            </Typography>
            <Typography variant="body2" sx={{ fontFamily: 'monospace', fontSize: '0.85rem', wordBreak: 'break-all' }}>
              {log.path}
            </Typography>
          </Grid>
        </Grid>
      </Paper>

      {/* User Information */}
      <Paper sx={{ p: 3, mb: 3 }}>
        <Typography variant="h6" gutterBottom>
          Kullanıcı Bilgileri
        </Typography>
        <Divider sx={{ mb: 2 }} />

        <Grid container spacing={2}>
          <Grid item xs={12} sm={6}>
            <Typography variant="subtitle2" color="textSecondary" gutterBottom>
              Email
            </Typography>
            <Typography variant="body2">
              {log.userEmail || '-'}
            </Typography>
          </Grid>
          <Grid item xs={12} sm={6}>
            <Typography variant="subtitle2" color="textSecondary" gutterBottom>
              User ID
            </Typography>
            <Typography variant="body2">
              {log.userId || '-'}
            </Typography>
          </Grid>
          <Grid item xs={12}>
            <Typography variant="subtitle2" color="textSecondary" gutterBottom>
              User Agent
            </Typography>
            <Typography 
              variant="body2" 
              sx={{ 
                fontFamily: 'monospace', 
                fontSize: '0.85rem',
                wordBreak: 'break-word',
                backgroundColor: '#f5f5f5',
                p: 1,
                borderRadius: 1
              }}
            >
              {log.userAgent || '-'}
            </Typography>
          </Grid>
        </Grid>
      </Paper>

      {/* Network Information */}
      <Paper sx={{ p: 3, mb: 3 }}>
        <Typography variant="h6" gutterBottom>
          Ağ Bilgileri
        </Typography>
        <Divider sx={{ mb: 2 }} />

        <Grid container spacing={2}>
          <Grid item xs={12}>
            <Typography variant="subtitle2" color="textSecondary" gutterBottom>
              IP Adresi
            </Typography>
            <Typography variant="body2" sx={{ fontFamily: 'monospace' }}>
              {log.ipAddress || '-'}
            </Typography>
          </Grid>
        </Grid>
      </Paper>

      {/* Error Details (if any) */}
      {log.errorMessage && (
        <Paper sx={{ p: 3, mb: 3, backgroundColor: '#ffebee', borderLeft: '4px solid #f44336' }}>
          <Box sx={{ display: 'flex', alignItems: 'flex-start', gap: 2 }}>
            <ErrorIcon sx={{ color: '#f44336', mt: 0.5 }} />
            <Box sx={{ flex: 1 }}>
              <Typography variant="h6" sx={{ color: '#c62828', mb: 1 }}>
                Hata Mesajı
              </Typography>
              <Typography 
                variant="body2" 
                sx={{ 
                  fontFamily: 'monospace',
                  whiteSpace: 'pre-wrap',
                  wordBreak: 'break-word',
                  backgroundColor: '#fff3e0',
                  p: 2,
                  borderRadius: 1,
                  color: '#bf360c'
                }}
              >
                {log.errorMessage}
              </Typography>
            </Box>
          </Box>
        </Paper>
      )}

      {!log.errorMessage && (
        <Paper sx={{ p: 3, mb: 3, backgroundColor: '#e8f5e9', borderLeft: '4px solid #4caf50' }}>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
            <SuccessIcon sx={{ color: '#4caf50' }} />
            <Typography sx={{ color: '#2e7d32' }}>
              İstek başarıyla tamamlandı
            </Typography>
          </Box>
        </Paper>
      )}

      {/* Summary Table */}
      <Paper sx={{ mb: 3 }}>
        <TableContainer>
          <Table>
            <TableHead>
              <TableRow sx={{ backgroundColor: '#f5f5f5' }}>
                <TableCell>Alan</TableCell>
                <TableCell>Değer</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              <TableRow>
                <TableCell><strong>Request ID</strong></TableCell>
                <TableCell sx={{ fontFamily: 'monospace', fontSize: '0.85rem' }}>
                  {log.requestId}
                </TableCell>
              </TableRow>
              <TableRow>
                <TableCell><strong>Timestamp</strong></TableCell>
                <TableCell>{new Date(log.timestamp).toLocaleString('tr-TR')}</TableCell>
              </TableRow>
              <TableRow>
                <TableCell><strong>Method</strong></TableCell>
                <TableCell>{log.method}</TableCell>
              </TableRow>
              <TableRow>
                <TableCell><strong>Path</strong></TableCell>
                <TableCell sx={{ fontFamily: 'monospace', fontSize: '0.85rem', wordBreak: 'break-all' }}>
                  {log.path}
                </TableCell>
              </TableRow>
              <TableRow>
                <TableCell><strong>Status Code</strong></TableCell>
                <TableCell>
                  <Chip
                    label={log.statusCode}
                    color={getStatusColor(log.statusCode)}
                    size="small"
                  />
                </TableCell>
              </TableRow>
              <TableRow>
                <TableCell><strong>Duration</strong></TableCell>
                <TableCell>{log.durationMs}ms</TableCell>
              </TableRow>
              <TableRow>
                <TableCell><strong>User Email</strong></TableCell>
                <TableCell>{log.userEmail || '-'}</TableCell>
              </TableRow>
              <TableRow>
                <TableCell><strong>IP Address</strong></TableCell>
                <TableCell>{log.ipAddress || '-'}</TableCell>
              </TableRow>
              <TableRow>
                <TableCell><strong>Error Message</strong></TableCell>
                <TableCell>{log.errorMessage || 'Yok'}</TableCell>
              </TableRow>
            </TableBody>
          </Table>
        </TableContainer>
      </Paper>
    </Container>
  );
};

export default ActivityLogDetail;
