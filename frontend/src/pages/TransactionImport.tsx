import { useState } from 'react';
import {
  Box,
  Button,
  Card,
  CardContent,
  Typography,
  Alert,
  LinearProgress,
  List,
  ListItem,
  ListItemText,
  Paper,
  Divider,
  Grid,
} from '@mui/material';
import { CloudUpload, CheckCircle, Error, Warning } from '@mui/icons-material';
import { api } from '../services/api';

interface ImportResult {
  totalRows: number;
  successCount: number;
  failureCount: number;
  errors: string[];
  warnings: string[];
}

export default function TransactionImport() {
  const [file, setFile] = useState<File | null>(null);
  const [loading, setLoading] = useState(false);
  const [result, setResult] = useState<ImportResult | null>(null);

  const handleFileSelect = (event: React.ChangeEvent<HTMLInputElement>) => {
    const selectedFile = event.target.files?.[0];
    if (selectedFile) {
      setFile(selectedFile);
      setResult(null);
    }
  };

  const handleUpload = async () => {
    if (!file) return;

    setLoading(true);
    const formData = new FormData();
    formData.append('file', file);

    try {
      const response = await api.post('/pkttransactions/import', formData, {
        headers: {
          'Content-Type': 'multipart/form-data',
        },
      });

      setResult(response.data.data);
      setFile(null);
      
      // Reset file input
      const fileInput = document.getElementById('file-input') as HTMLInputElement;
      if (fileInput) fileInput.value = '';
    } catch (error: any) {
      console.error('Import error:', error);
      setResult({
        totalRows: 0,
        successCount: 0,
        failureCount: 0,
        errors: [error.response?.data?.message || 'Dosya yüklenirken hata oluştu'],
        warnings: [],
      });
    } finally {
      setLoading(false);
    }
  };

  return (
    <Box sx={{ p: 3 }}>
      <Typography variant="h4" gutterBottom>
        Excel'den Transaction İçe Aktarma
      </Typography>
      <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
        Excel dosyanızdan toplu olarak transaction verilerini sisteme yükleyebilirsiniz.
      </Typography>

      {/* Beklenen Format Bilgisi */}
      <Paper sx={{ p: 3, mb: 3, bgcolor: 'info.lighter' }}>
        <Typography variant="h6" gutterBottom>
          📋 Excel Formatı
        </Typography>
        <Typography variant="body2" paragraph>
          Excel dosyanızın ilk satırında şu başlıklar olmalıdır:
        </Typography>
        <Box sx={{ pl: 2 }}>
          <Typography variant="body2" component="div" sx={{ fontFamily: 'monospace', fontSize: '0.85rem' }}>
            1. REAKTÖR<br />
            2. Ürün/İşlem<br />
            3. İş Emri No<br />
            4. Lot Numarası<br />
            5. Başlangıç Tarih<br />
            6. Başlangıç Saati<br />
            7. Bitiş Tarih<br />
            8. Bitiş Saati<br />
            9. Yıkama İçin Geçen Süre<br />
            10. Yıkamada kullanılan kostik miktarı (kg)<br />
            11. Reaktör Bekleme / Gecikme Nedeni<br />
            12. Açıklama
          </Typography>
        </Box>
        <Alert severity="info" sx={{ mt: 2 }}>
          <strong>Not:</strong> Reaktör adı ve Ürün/İşlem adı mutlaka sistemde kayıtlı olmalıdır.
        </Alert>
      </Paper>

      {/* File Upload */}
      <Card sx={{ mb: 3 }}>
        <CardContent>
          <Grid container spacing={2} alignItems="center">
            <Grid item xs={12} sm={8}>
              <input
                id="file-input"
                type="file"
                accept=".xlsx,.xls"
                style={{ display: 'none' }}
                onChange={handleFileSelect}
              />
              <label htmlFor="file-input">
                <Button
                  variant="outlined"
                  component="span"
                  startIcon={<CloudUpload />}
                  fullWidth
                >
                  Excel Dosyası Seç
                </Button>
              </label>
              {file && (
                <Typography variant="body2" sx={{ mt: 1 }}>
                  Seçili dosya: <strong>{file.name}</strong>
                </Typography>
              )}
            </Grid>
            <Grid item xs={12} sm={4}>
              <Button
                variant="contained"
                color="primary"
                onClick={handleUpload}
                disabled={!file || loading}
                fullWidth
                size="large"
              >
                {loading ? 'Yükleniyor...' : 'Yükle'}
              </Button>
            </Grid>
          </Grid>
          {loading && <LinearProgress sx={{ mt: 2 }} />}
        </CardContent>
      </Card>

      {/* Results */}
      {result && (
        <Paper sx={{ p: 3 }}>
          <Typography variant="h6" gutterBottom>
            İçe Aktarma Sonuçları
          </Typography>
          
          <Grid container spacing={2} sx={{ mb: 3 }}>
            <Grid item xs={12} sm={4}>
              <Card>
                <CardContent>
                  <Typography color="text.secondary" variant="body2">
                    Toplam Satır
                  </Typography>
                  <Typography variant="h4">{result.totalRows}</Typography>
                </CardContent>
              </Card>
            </Grid>
            <Grid item xs={12} sm={4}>
              <Card sx={{ bgcolor: 'success.lighter' }}>
                <CardContent>
                  <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                    <CheckCircle color="success" />
                    <Typography color="success.dark" variant="body2">
                      Başarılı
                    </Typography>
                  </Box>
                  <Typography variant="h4" color="success.dark">
                    {result.successCount}
                  </Typography>
                </CardContent>
              </Card>
            </Grid>
            <Grid item xs={12} sm={4}>
              <Card sx={{ bgcolor: 'error.lighter' }}>
                <CardContent>
                  <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                    <Error color="error" />
                    <Typography color="error.dark" variant="body2">
                      Hatalı
                    </Typography>
                  </Box>
                  <Typography variant="h4" color="error.dark">
                    {result.failureCount}
                  </Typography>
                </CardContent>
              </Card>
            </Grid>
          </Grid>

          {/* Errors */}
          {result.errors.length > 0 && (
            <Box sx={{ mb: 2 }}>
              <Alert severity="error" sx={{ mb: 1 }}>
                <strong>Hatalar ({result.errors.length})</strong>
              </Alert>
              <Paper variant="outlined" sx={{ maxHeight: 300, overflow: 'auto' }}>
                <List dense>
                  {result.errors.map((error, index) => (
                    <ListItem key={index}>
                      <ListItemText primary={error} />
                    </ListItem>
                  ))}
                </List>
              </Paper>
            </Box>
          )}

          {/* Warnings */}
          {result.warnings.length > 0 && (
            <Box>
              <Alert severity="warning" sx={{ mb: 1 }}>
                <strong>Uyarılar ({result.warnings.length})</strong>
              </Alert>
              <Paper variant="outlined" sx={{ maxHeight: 200, overflow: 'auto' }}>
                <List dense>
                  {result.warnings.map((warning, index) => (
                    <ListItem key={index}>
                      <ListItemText primary={warning} />
                    </ListItem>
                  ))}
                </List>
              </Paper>
            </Box>
          )}

          {result.errors.length === 0 && result.warnings.length === 0 && result.successCount > 0 && (
            <Alert severity="success">
              <strong>Tüm kayıtlar başarıyla içe aktarıldı! 🎉</strong>
            </Alert>
          )}
        </Paper>
      )}
    </Box>
  );
}
