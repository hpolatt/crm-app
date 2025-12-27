import { useEffect, useState } from 'react'
import {
  Box,
  Container,
  Grid,
  Typography,
  Card,
  CardContent,
  TextField,
  Button,
  Paper,
  MenuItem,
  FormControl,
  InputLabel,
  Select,
  Chip,
  Box as MuiBox,
} from '@mui/material'
import {
  BarChart,
  Bar,
  PieChart,
  Pie,
  Cell,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  ResponsiveContainer,
} from 'recharts'
import SettingsIcon from '@mui/icons-material/Settings'
import ScienceIcon from '@mui/icons-material/Science'
import InventoryIcon from '@mui/icons-material/Inventory'
import AssignmentIcon from '@mui/icons-material/Assignment'
import { Refresh } from '@mui/icons-material'
import { api } from '../services/api'

interface StatCardProps {
  title: string
  value: string | number
  icon: React.ReactNode
  color: string
}

function StatCard({ title, value, icon, color }: StatCardProps) {
  return (
    <Card>
      <CardContent>
        <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
          <Box>
            <Typography color="textSecondary" gutterBottom variant="body2">
              {title}
            </Typography>
            <Typography variant="h4">{value}</Typography>
          </Box>
          <Box
            sx={{
              backgroundColor: color,
              borderRadius: '50%',
              width: 56,
              height: 56,
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
            }}
          >
            {icon}
          </Box>
        </Box>
      </CardContent>
    </Card>
  )
}

const COLORS = ['#0088FE', '#00C49F', '#FFBB28', '#FF8042', '#8884D8']

// Helper to get local date string (YYYY-MM-DD) for display
const getLocalDateString = (date: Date): string => {
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const day = String(date.getDate()).padStart(2, '0');
  return `${year}-${month}-${day}`;
};

function Dashboard() {
  const [loading, setLoading] = useState(true)
  
  // Calculate last week as default
  const getLastWeek = () => {
    const today = new Date()
    const lastWeek = new Date(today.getTime() - 7 * 24 * 60 * 60 * 1000)
    return {
      from: getLocalDateString(lastWeek),
      to: getLocalDateString(today)
    }
  }
  
  const lastWeek = getLastWeek()
  const [dateFrom, setDateFrom] = useState(lastWeek.from)
  const [dateTo, setDateTo] = useState(lastWeek.to)
  const [reactors, setReactors] = useState<any[]>([])
  const [selectedReactorIds, setSelectedReactorIds] = useState<string[]>([])
  
  const [stats, setStats] = useState({
    totalTransactions: 0,
    activeReactors: 0,
    totalProducts: 0,
    delayReasons: 0,
  })
  const [statusData, setStatusData] = useState<any[]>([])
  const [reactorData, setReactorData] = useState<any[]>([])
  const [delayReasonsData, setDelayReasonsData] = useState<any[]>([])
  const [reactorUsageData, setReactorUsageData] = useState<any[]>([])

  useEffect(() => {
    loadData()
  }, [dateFrom, dateTo, selectedReactorIds])

  const loadData = async () => {
    try {
      setLoading(true)
      
      const [summary, transactions, reactorsResponse] = await Promise.all([
        api.get('/dashboard/summary'),
        api.get('/pkttransactions', {
          params: {
            startDateFrom: dateFrom,
            startDateTo: dateTo
          }
        }),
        api.get('/reactors'),
      ])

      // Store reactors for dropdown
      const allReactors = reactorsResponse.data.data || []
      setReactors(allReactors)

      // All transactions are already filtered by backend (date)
      let filteredTransactions = transactions.data.data || []

      // Apply reactor filter if specific reactors selected
      if (selectedReactorIds.length > 0) {
        filteredTransactions = filteredTransactions.filter(
          (t: any) => selectedReactorIds.includes(t.reactorId)
        )
      }

      // Set stats based on filtered transactions
      setStats({
        totalTransactions: filteredTransactions.length,
        activeReactors: new Set(filteredTransactions.map((t: any) => t.reactorId)).size,
        totalProducts: filteredTransactions.filter((t: any) => t.status === 'Completed').length,
        delayReasons: filteredTransactions.filter((t: any) => t.delayReasonName).length,
      })

      // Status distribution data - use filtered transactions
      const statusCounts: Record<string, number> = {}
      filteredTransactions.forEach((t: any) => {
        statusCounts[t.status] = (statusCounts[t.status] || 0) + 1
      })
      
      const statusArray = Object.entries(statusCounts).map(([status, count]) => ({
        name: status,
        value: count,
      }))
      
      setStatusData(statusArray)

      // Reactor analytics data - use filtered transactions
      const reactorCounts: Record<string, { name: string, count: number }> = {}
      filteredTransactions.forEach((t: any) => {
        if (!reactorCounts[t.reactorId]) {
          reactorCounts[t.reactorId] = { name: t.reactorName, count: 0 }
        }
        reactorCounts[t.reactorId].count++
      })
      
      const reactorArray = Object.values(reactorCounts)
      setReactorData(reactorArray)
      
      const delayReasonCounts: Record<string, number> = {}
      let totalDelayCount = 0

      filteredTransactions.forEach((t: any) => {
        if (t.delayReasonName) {
          delayReasonCounts[t.delayReasonName] = (delayReasonCounts[t.delayReasonName] || 0) + 1
          totalDelayCount++
        }
      })

      const delayReasonsArray = Object.entries(delayReasonCounts).map(([name, count]) => ({
        name,
        value: count,
        percentage: ((count / totalDelayCount) * 100).toFixed(1),
      }))

      setDelayReasonsData(delayReasonsArray)

      // Process reactor usage data (Bar Chart - Ideal vs Actual)
      // Filter reactors if specific ones selected
      const reactorsToShow = selectedReactorIds.length === 0
        ? allReactors 
        : allReactors.filter((r: any) => selectedReactorIds.includes(r.id))
      
      // Calculate total hours in date range using local dates
      const fromDate = new Date(dateFrom)
      const toDate = new Date(dateTo)
      toDate.setHours(23, 59, 59, 999)
      const totalRangeHours = (toDate.getTime() - fromDate.getTime()) / (1000 * 60 * 60)
      
      const reactorUsageArray = reactorsToShow.map((reactor: any) => {
        const reactorTransactions = filteredTransactions.filter(
          (t: any) => t.reactorId === reactor.id
        )

        const parseTimeString = (timeStr: string | null): number => {
          if (!timeStr) return 0
          const parts = timeStr.split('.')
          let hours = 0
          let minutes = 0

          if (parts.length === 2) {
            const days = parseInt(parts[0])
            const timeParts = parts[1].split(':')
            hours = parseInt(timeParts[0]) + (days * 24)
            minutes = parseInt(timeParts[1])
          } else {
            const timeParts = timeStr.split(':')
            hours = parseInt(timeParts[0])
            minutes = parseInt(timeParts[1])
          }

          return hours + (minutes / 60)
        }

        let totalProductionHours = 0
        let totalWashingHours = 0

        reactorTransactions.forEach((t: any) => {
          totalProductionHours += parseTimeString(t.actualProductionDuration)
          totalWashingHours += parseTimeString(t.washingDuration)
        })

        const totalActiveHours = totalProductionHours + totalWashingHours
        const actualUsagePercent = (totalActiveHours / totalRangeHours) * 100
        const idealUsagePercent = (totalProductionHours / totalRangeHours) * 100

        return {
          name: reactor.name,
          'Mevcut Kullanım': parseFloat(actualUsagePercent.toFixed(1)),
          'İdeal Kullanım': parseFloat(idealUsagePercent.toFixed(1)),
        }
      })

      setReactorUsageData(reactorUsageArray)
    } catch (error) {
      console.error('Failed to load dashboard data:', error)
    } finally {
      setLoading(false)
    }
  }

  if (loading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '80vh' }}>
        <Typography>Yükleniyor...</Typography>
      </Box>
    )
  }

  return (
    <Container maxWidth="xl">
      <Box sx={{ mt: 4, mb: 4 }}>
        <Typography variant="h4" component="h1" gutterBottom>
          PKT Dashboard
        </Typography>

        {/* Date Range Filter */}
        <Paper sx={{ p: 3, mb: 3 }}>
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
                  onChange={(e) => setSelectedReactorIds(typeof e.target.value === 'string' ? e.target.value.split(',') : e.target.value)}
                  renderValue={(selected) => (
                    <MuiBox sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.5 }}>
                      {selected.length === 0 ? (
                        <em>Tüm Reaktörler</em>
                      ) : (
                        selected.map((id) => {
                          const reactor = reactors.find((r) => r.id === id)
                          return (
                            <Chip
                              key={id}
                              label={reactor?.name || id}
                              size="small"
                            />
                          )
                        })
                      )}
                    </MuiBox>
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
                startIcon={<Refresh />}
                onClick={loadData}
                disabled={loading}
                sx={{ height: '56px' }}
              >
                {loading ? 'Yükleniyor...' : 'Yenile'}
              </Button>
            </Grid>
          </Grid>
        </Paper>

        {/* Stats Cards */}
        <Grid container spacing={3} sx={{ mb: 4 }}>
          <Grid item xs={12} sm={6} md={3}>
            <StatCard
              title="Toplam İşlem"
              value={stats.totalTransactions}
              icon={<AssignmentIcon sx={{ color: 'white' }} />}
              color="#1976d2"
            />
          </Grid>
          <Grid item xs={12} sm={6} md={3}>
            <StatCard
              title="Aktif Reaktörler"
              value={stats.activeReactors}
              icon={<ScienceIcon sx={{ color: 'white' }} />}
              color="#2e7d32"
            />
          </Grid>
          <Grid item xs={12} sm={6} md={3}>
            <StatCard
              title="Toplam Ürün"
              value={stats.totalProducts}
              icon={<InventoryIcon sx={{ color: 'white' }} />}
              color="#ed6c02"
            />
          </Grid>
          <Grid item xs={12} sm={6} md={3}>
            <StatCard
              title="Gecikme Nedenleri"
              value={stats.delayReasons}
              icon={<SettingsIcon sx={{ color: 'white' }} />}
              color="#9c27b0"
            />
          </Grid>
        </Grid>

        {/* Charts */}
        <Grid container spacing={3}>
          {/* Delay Reasons Distribution */}
          <Grid item xs={12} md={6}>
            <Card>
              <CardContent>
                <Typography variant="h6" gutterBottom>
                  Gecikme Nedenleri Dağılımı
                </Typography>
                <Typography variant="body2" color="text.secondary" gutterBottom>
                  {new Date(dateFrom).toLocaleDateString('tr-TR')} - {new Date(dateTo).toLocaleDateString('tr-TR')}
                </Typography>
                <ResponsiveContainer width="100%" height={300}>
                  <PieChart>
                    <Pie
                      data={delayReasonsData}
                      cx="50%"
                      cy="50%"
                      labelLine={false}
                      label={({ name, percentage }: any) => `${name} (${percentage}%)`}
                      outerRadius={80}
                      fill="#8884d8"
                      dataKey="value"
                    >
                      {delayReasonsData.map((_entry, index) => (
                        <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                      ))}
                    </Pie>
                    <Tooltip />
                  </PieChart>
                </ResponsiveContainer>
              </CardContent>
            </Card>
          </Grid>

          {/* Reactor Usage Comparison */}
          <Grid item xs={12} md={6}>
            <Card>
              <CardContent>
                <Typography variant="h6" gutterBottom>
                  Reaktör Kullanım Karşılaştırması (Son 30 Gün)
                </Typography>                <Typography variant="body2" color="text.secondary" gutterBottom>
                  {new Date(dateFrom).toLocaleDateString('tr-TR')} - {new Date(dateTo).toLocaleDateString('tr-TR')}
                </Typography>                <ResponsiveContainer width="100%" height={300}>
                  <BarChart data={reactorUsageData}>
                    <CartesianGrid strokeDasharray="3 3" />
                    <XAxis dataKey="name" />
                    <YAxis label={{ value: '%', angle: -90, position: 'insideLeft' }} />
                    <Tooltip />
                    <Legend />
                    <Bar dataKey="Mevcut Kullanım" fill="#8884d8" />
                    <Bar dataKey="İdeal Kullanım" fill="#82ca9d" />
                  </BarChart>
                </ResponsiveContainer>
              </CardContent>
            </Card>
          </Grid>

          {/* Transaction Status Distribution */}
          <Grid item xs={12} md={6}>
            <Card>
              <CardContent>
                <Typography variant="h6" gutterBottom>
                  İşlem Durumları
                </Typography>                <Typography variant="body2" color="text.secondary" gutterBottom>
                  {new Date(dateFrom).toLocaleDateString('tr-TR')} - {new Date(dateTo).toLocaleDateString('tr-TR')}
                </Typography>                <ResponsiveContainer width="100%" height={300}>
                  <PieChart>
                    <Pie
                      data={statusData}
                      cx="50%"
                      cy="50%"
                      labelLine={false}
                      label={({ name, percent }: any) => `${name} (${(percent * 100).toFixed(0)}%)`}
                      outerRadius={80}
                      fill="#8884d8"
                      dataKey="value"
                    >
                      {statusData.map((_entry, index) => (
                        <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                      ))}
                    </Pie>
                    <Tooltip />
                  </PieChart>
                </ResponsiveContainer>
              </CardContent>
            </Card>
          </Grid>

          {/* Transactions by Reactor */}
          <Grid item xs={12} md={6}>
            <Card>
              <CardContent>
                <Typography variant="h6" gutterBottom>
                  Reaktörlere Göre İşlemler
                </Typography>
                <Typography variant="body2" color="text.secondary" gutterBottom>
                  {new Date(dateFrom).toLocaleDateString('tr-TR')} - {new Date(dateTo).toLocaleDateString('tr-TR')}
                </Typography>
                <ResponsiveContainer width="100%" height={300}>
                  <BarChart data={reactorData}>
                    <CartesianGrid strokeDasharray="3 3" />
                    <XAxis dataKey="name" />
                    <YAxis />
                    <Tooltip />
                    <Legend />
                    <Bar dataKey="count" fill="#8884d8" name="İşlem Sayısı" />
                  </BarChart>
                </ResponsiveContainer>
              </CardContent>
            </Card>
          </Grid>
        </Grid>
      </Box>
    </Container>
  )
}

export default Dashboard
