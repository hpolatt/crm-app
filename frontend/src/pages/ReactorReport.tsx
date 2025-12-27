import { useState, useEffect } from 'react';
import {
  Box,
  Paper,
  Typography,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TextField,
  Button,
  Grid,
  CircularProgress,
} from '@mui/material';
import { Assessment } from '@mui/icons-material';
import { api } from '../services/api';

interface Reactor {
  id: string;
  name: string;
  capacity: number;
}

interface PktTransaction {
  id: string;
  status: string;
  reactorId: string;
  startOfWork: string | null;
  end: string | null;
  actualProductionDuration: string | null;
  delayDuration: string | null;
  washingDuration: string | null;
  createdAt: string;
}

interface ReactorAnalysis {
  reactorName: string;
  totalProductionMinutes: number;
  totalWashingMinutes: number;
  totalDelayMinutes: number;
  totalIdleMinutes: number;
  usagePercentage: number;
  idealUsagePercentage: number;
  difference: number;
}

export default function ReactorReport() {
  const [reactors, setReactors] = useState<Reactor[]>([]);
  const [loading, setLoading] = useState(false);
  const [analysis, setAnalysis] = useState<ReactorAnalysis[]>([]);
  const [dateFrom, setDateFrom] = useState('');
  const [dateTo, setDateTo] = useState('');

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

  const parseTimeString = (timeStr: string | null): number => {
    if (!timeStr) return 0;
    
    // Format: "HH:mm:ss" or "H:mm:ss" or "d.HH:mm:ss"
    const parts = timeStr.split('.');
    let hours = 0;
    let minutes = 0;
    let seconds = 0;

    if (parts.length === 2) {
      // Has days: "1.02:30:45"
      const days = parseInt(parts[0]);
      const timeParts = parts[1].split(':');
      hours = parseInt(timeParts[0]) + (days * 24);
      minutes = parseInt(timeParts[1]);
      seconds = parseInt(timeParts[2] || '0');
    } else {
      // No days: "02:30:45"
      const timeParts = timeStr.split(':');
      hours = parseInt(timeParts[0]);
      minutes = parseInt(timeParts[1]);
      seconds = parseInt(timeParts[2] || '0');
    }

    return (hours * 60) + minutes + (seconds / 60);
  };

  const formatMinutes = (totalMinutes: number): string => {
    const days = Math.floor(totalMinutes / (24 * 60));
    const hours = Math.floor((totalMinutes % (24 * 60)) / 60);
    const minutes = Math.round(totalMinutes % 60);

    if (days > 0) {
      return `${days}g ${hours}s ${minutes}dk`;
    } else if (hours > 0) {
      return `${hours}s ${minutes}dk`;
    } else {
      return `${minutes}dk`;
    }
  };

  const generateReport = async () => {
    if (!dateFrom || !dateTo) {
      alert('Lütfen tarih aralığı seçin');
      return;
    }

    setLoading(true);
    try {
      // Fetch all transactions
      const response = await api.get('/pkttransactions');
      const allTransactions: PktTransaction[] = response.data.data || [];

      // Filter transactions by date range
      const fromDate = new Date(dateFrom);
      const toDate = new Date(dateTo);
      toDate.setHours(23, 59, 59, 999);

      const filteredTransactions = allTransactions.filter(t => {
        const createdDate = new Date(t.createdAt);
        return createdDate >= fromDate && createdDate <= toDate;
      });

      // Calculate total minutes in date range
      const totalRangeMinutes = (toDate.getTime() - fromDate.getTime()) / (1000 * 60);

      // Analyze each reactor
      const analyses: ReactorAnalysis[] = reactors.map(reactor => {
        const reactorTransactions = filteredTransactions.filter(t => t.reactorId === reactor.id);

        let totalProductionMinutes = 0;
        let totalWashingMinutes = 0;
        let totalDelayMinutes = 0;

        reactorTransactions.forEach(t => {
          totalProductionMinutes += parseTimeString(t.actualProductionDuration);
          totalWashingMinutes += parseTimeString(t.washingDuration);
          totalDelayMinutes += parseTimeString(t.delayDuration);
        });

        // Calculate total active time (production + washing)
        const totalActiveMinutes = totalProductionMinutes + totalWashingMinutes;

        // Calculate idle time (total range - active time)
        const totalIdleMinutes = totalRangeMinutes - totalActiveMinutes;

        // Calculate usage percentage: (production + washing) / total range
        const usagePercentage = (totalActiveMinutes / totalRangeMinutes) * 100;

        // Calculate ideal usage percentage: production / total range
        const idealUsagePercentage = (totalProductionMinutes / totalRangeMinutes) * 100;

        // Calculate difference
        const difference = usagePercentage - idealUsagePercentage;

        return {
          reactorName: reactor.name,
          totalProductionMinutes,
          totalWashingMinutes,
          totalDelayMinutes,
          totalIdleMinutes: Math.max(0, totalIdleMinutes),
          usagePercentage,
          idealUsagePercentage,
          difference,
        };
      });

      setAnalysis(analyses);
    } catch (error) {
      console.error('Error generating report:', error);
    } finally {
      setLoading(false);
    }
  };

  const calculateTotals = () => {
    if (analysis.length === 0) return null;

    const totals = analysis.reduce((acc, curr) => ({
      totalProductionMinutes: acc.totalProductionMinutes + curr.totalProductionMinutes,
      totalWashingMinutes: acc.totalWashingMinutes + curr.totalWashingMinutes,
      totalDelayMinutes: acc.totalDelayMinutes + curr.totalDelayMinutes,
      totalIdleMinutes: acc.totalIdleMinutes + curr.totalIdleMinutes,
      usagePercentage: acc.usagePercentage + curr.usagePercentage,
      idealUsagePercentage: acc.idealUsagePercentage + curr.idealUsagePercentage,
    }), {
      totalProductionMinutes: 0,
      totalWashingMinutes: 0,
      totalDelayMinutes: 0,
      totalIdleMinutes: 0,
      usagePercentage: 0,
      idealUsagePercentage: 0,
    });

    return {
      ...totals,
      usagePercentage: totals.usagePercentage / analysis.length,
      idealUsagePercentage: totals.idealUsagePercentage / analysis.length,
    };
  };

  const totals = calculateTotals();

  return (
    <Box sx={{ p: 3 }}>
      <Typography variant="h4" gutterBottom>
        Reaktör Kullanım Analizi
      </Typography>

      {/* Date Range Filter */}
      <Paper sx={{ p: 3, mb: 3 }}>
        <Typography variant="h6" gutterBottom>
          Tarih Aralığı
        </Typography>
        <Grid container spacing={2} alignItems="center">
          <Grid item xs={12} sm={4}>
            <TextField
              fullWidth
              type="date"
              label="Başlangıç Tarihi"
              value={dateFrom}
              onChange={(e) => setDateFrom(e.target.value)}
              InputLabelProps={{ shrink: true }}
            />
          </Grid>
          <Grid item xs={12} sm={4}>
            <TextField
              fullWidth
              type="date"
              label="Bitiş Tarihi"
              value={dateTo}
              onChange={(e) => setDateTo(e.target.value)}
              InputLabelProps={{ shrink: true }}
            />
          </Grid>
          <Grid item xs={12} sm={4}>
            <Button
              fullWidth
              variant="contained"
              startIcon={<Assessment />}
              onClick={generateReport}
              disabled={loading}
              sx={{ height: '56px' }}
            >
              {loading ? 'Hesaplanıyor...' : 'Rapor Oluştur'}
            </Button>
          </Grid>
        </Grid>
      </Paper>

      {/* Results */}
      {analysis.length > 0 && (
        <Paper>
          <Box sx={{ p: 2 }}>
            <Typography variant="h6">
              Analiz Sonuçları
            </Typography>
            <Typography variant="body2" color="text.secondary">
              {new Date(dateFrom).toLocaleDateString('tr-TR')} - {new Date(dateTo).toLocaleDateString('tr-TR')} arası
            </Typography>
          </Box>

          <TableContainer>
            <Table size="small">
              <TableHead>
                <TableRow>
                  <TableCell><strong>Reaktör</strong></TableCell>
                  <TableCell align="right"><strong>Üretim Süresi</strong></TableCell>
                  <TableCell align="right"><strong>Yıkama Süresi</strong></TableCell>
                  <TableCell align="right"><strong>Gecikme Süresi</strong></TableCell>
                  <TableCell align="right"><strong>Boşta Süre</strong></TableCell>
                  <TableCell align="right"><strong>Kullanım %</strong></TableCell>
                  <TableCell align="right"><strong>İdeal Kullanım %</strong></TableCell>
                  <TableCell align="right"><strong>Fark</strong></TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {analysis.map((item, index) => (
                  <TableRow key={index} hover>
                    <TableCell>{item.reactorName}</TableCell>
                    <TableCell align="right">{formatMinutes(item.totalProductionMinutes)}</TableCell>
                    <TableCell align="right">{formatMinutes(item.totalWashingMinutes)}</TableCell>
                    <TableCell align="right">{formatMinutes(item.totalDelayMinutes)}</TableCell>
                    <TableCell align="right">{formatMinutes(item.totalIdleMinutes)}</TableCell>
                    <TableCell align="right">{item.usagePercentage.toFixed(1)}%</TableCell>
                    <TableCell align="right">{item.idealUsagePercentage.toFixed(1)}%</TableCell>
                    <TableCell align="right">
                      <Typography
                        color={item.difference >= 0 ? 'success.main' : 'error.main'}
                        fontWeight="bold"
                      >
                        {item.difference > 0 ? '+' : ''}{item.difference.toFixed(1)}%
                      </Typography>
                    </TableCell>
                  </TableRow>
                ))}
                
                {/* Totals Row */}
                {totals && (
                  <TableRow sx={{ backgroundColor: 'action.hover' }}>
                    <TableCell><strong>GENEL TOPLAM</strong></TableCell>
                    <TableCell align="right"><strong>{formatMinutes(totals.totalProductionMinutes)}</strong></TableCell>
                    <TableCell align="right"><strong>{formatMinutes(totals.totalWashingMinutes)}</strong></TableCell>
                    <TableCell align="right"><strong>{formatMinutes(totals.totalDelayMinutes)}</strong></TableCell>
                    <TableCell align="right"><strong>{formatMinutes(totals.totalIdleMinutes)}</strong></TableCell>
                    <TableCell align="right"><strong>{totals.usagePercentage.toFixed(1)}%</strong></TableCell>
                    <TableCell align="right"><strong>{totals.idealUsagePercentage.toFixed(1)}%</strong></TableCell>
                    <TableCell align="right"><strong>-</strong></TableCell>
                  </TableRow>
                )}
              </TableBody>
            </Table>
          </TableContainer>
        </Paper>
      )}

      {/* Empty State */}
      {analysis.length === 0 && !loading && (
        <Paper sx={{ p: 4, textAlign: 'center' }}>
          <Typography variant="body1" color="text.secondary">
            Tarih aralığı seçip "Rapor Oluştur" butonuna basın
          </Typography>
        </Paper>
      )}

      {/* Loading State */}
      {loading && (
        <Paper sx={{ p: 4, textAlign: 'center' }}>
          <CircularProgress />
        </Paper>
      )}
    </Box>
  );
}
