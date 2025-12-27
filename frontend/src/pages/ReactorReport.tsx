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
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Chip,
} from '@mui/material';
import { Assessment } from '@mui/icons-material';
import { api } from '../services/api';

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

// Helper to get local date string (YYYY-MM-DD) for display
const getLocalDateString = (date: Date): string => {
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const day = String(date.getDate()).padStart(2, '0');
  return `${year}-${month}-${day}`;
};

const getLastWeek = () => {
  const today = new Date();
  const dayOfWeek = today.getDay(); // 0 = Sunday, 1 = Monday, ...
  const daysToLastMonday = dayOfWeek === 0 ? 6 : dayOfWeek + 6; // Go to last Monday
  const lastMonday = new Date(today.getTime() - daysToLastMonday * 24 * 60 * 60 * 1000);
  const lastSunday = new Date(lastMonday.getTime() + 6 * 24 * 60 * 60 * 1000);
  
  return {
    from: getLocalDateString(lastMonday),
    to: getLocalDateString(lastSunday)
  };
};

export default function ReactorReport() {
  const lastWeek = getLastWeek();
  const [loading, setLoading] = useState(false);
  const [analysis, setAnalysis] = useState<ReactorAnalysis[]>([]);
  const [dateFrom, setDateFrom] = useState(lastWeek.from);
  const [dateTo, setDateTo] = useState(lastWeek.to);
  const [reactors, setReactors] = useState<any[]>([]);
  const [selectedReactorIds, setSelectedReactorIds] = useState<string[]>([]);

  useEffect(() => {
    // Fetch reactors on mount
    const fetchReactors = async () => {
      try {
        const response = await api.get('/reactors');
        setReactors(response.data.data || []);
      } catch (error) {
        console.error('Error fetching reactors:', error);
      }
    };
    fetchReactors();
  }, []);

  useEffect(() => {
    // Clear previous analysis before generating new report
    setAnalysis([]);
    if (dateFrom && dateTo) {
      generateReport();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [dateFrom, dateTo, selectedReactorIds]);

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

  const parseTimeSpanToMinutes = (timeSpan: string | null): number => {
    if (!timeSpan) return 0;
    
    // C# TimeSpan format: "hh:mm:ss.fffffff" or "d.hh:mm:ss.fffffff"
    // Examples: "00:01:40.7464700" or "1.02:30:45.1234567"
    
    let workingString = timeSpan;
    let days = 0;
    
    // Check if it has days (format: d.hh:mm:ss)
    // Days format has a dot before the hours part (between first 1-3 digits and two-digit hours)
    const daysMatch = workingString.match(/^(\d+)\.(\d{2}:\d{2}:\d{2})/);
    if (daysMatch) {
      days = parseInt(daysMatch[1]);
      workingString = daysMatch[2] + workingString.substring(daysMatch[0].length);
    }
    
    // Now parse hh:mm:ss.fffffff
    // Remove fractional seconds (everything after the last colon's seconds)
    const timeParts = workingString.split(':');
    const hours = parseInt(timeParts[0]);
    const minutes = parseInt(timeParts[1]);
    // Handle seconds with fractional part: "40.7464700"
    const secondsStr = timeParts[2] || '0';
    const seconds = parseFloat(secondsStr);
    
    const totalMinutes = (days * 24 * 60) + (hours * 60) + minutes + (seconds / 60);
    return totalMinutes;
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
    // Clear previous data immediately
    setAnalysis([]);
    
    try {
      // Fetch reactor usage analysis from backend with date range filter
      const response = await api.get('/reactors/usage-analysis', {
        params: {
          startDateFrom: dateFrom,
          startDateTo: dateTo
        }
      });
      
      const reactorUsageData = response.data.data || [];

      // Filter by selected reactors if any selected
      const filteredData = selectedReactorIds.length > 0
        ? reactorUsageData.filter((r: any) => selectedReactorIds.includes(r.reactorId))
        : reactorUsageData;

      // Calculate total minutes in date range using local dates
      const fromDate = new Date(dateFrom);
      const toDate = new Date(dateTo);
      toDate.setHours(23, 59, 59, 999);
      const totalRangeMinutes = (toDate.getTime() - fromDate.getTime()) / (1000 * 60);

      // Convert backend data to analysis format
      const analyses: ReactorAnalysis[] = filteredData.map((r: any) => {
        const totalProductionMinutes = parseTimeSpanToMinutes(r.totalProductionDuration);
        const totalWashingMinutes = parseTimeSpanToMinutes(r.totalWashingDuration);
        const totalDelayMinutes = parseTimeSpanToMinutes(r.totalDelayDuration);

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
          reactorName: r.reactorName,
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
      setAnalysis([]); // Clear on error too
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
          <Grid item xs={12} sm={3}>
            <TextField
              fullWidth
              type="date"
              label="Başlangıç Tarihi"
              value={dateFrom}
              onChange={(e) => setDateFrom(e.target.value)}
              InputLabelProps={{ shrink: true }}
              inputProps={{
                placeholder: 'yyyy-mm-dd'
              }}
            />
          </Grid>
          <Grid item xs={12} sm={3}>
            <TextField
              fullWidth
              type="date"
              label="Bitiş Tarihi"
              value={dateTo}
              onChange={(e) => setDateTo(e.target.value)}
              InputLabelProps={{ shrink: true }}
              inputProps={{
                placeholder: 'yyyy-mm-dd'
              }}
            />
          </Grid>
          <Grid item xs={12} sm={3}>
            <FormControl fullWidth>
              <InputLabel id="reactor-select-label">Reaktörler</InputLabel>
              <Select
                labelId="reactor-select-label"
                multiple
                value={selectedReactorIds}
                label="Reaktörler"
                onChange={(e) =>
                  setSelectedReactorIds(
                    typeof e.target.value === 'string'
                      ? e.target.value.split(',')
                      : e.target.value
                  )
                }
                renderValue={(selected) => (
                  <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.5 }}>
                    {selected.length === 0 ? (
                      <em>Tüm Reaktörler</em>
                    ) : (
                      selected.map((id) => {
                        const reactor = reactors.find((r) => r.id === id);
                        return (
                          <Chip
                            key={id}
                            label={reactor?.name || id}
                            size="small"
                          />
                        );
                      })
                    )}
                  </Box>
                )}
              >
                {reactors.map((reactor) => (
                  <MenuItem key={reactor.id} value={reactor.id}>
                    {reactor.name}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          </Grid>
          <Grid item xs={12} sm={3}>
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
