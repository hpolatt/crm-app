import { useEffect, useState } from 'react'
import {
  Box,
  Container,
  Grid,
  Typography,
  Card,
  CardContent,
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
import axios from 'axios'

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

function Dashboard() {
  const [loading, setLoading] = useState(true)
  const [stats, setStats] = useState({
    totalTransactions: 0,
    activeReactors: 0,
    totalProducts: 0,
    delayReasons: 0,
  })
  const [statusData, setStatusData] = useState<any[]>([])
  const [reactorData, setReactorData] = useState<any[]>([])

  useEffect(() => {
    loadData()
  }, [])

  const loadData = async () => {
    try {
      setLoading(true)
      const [summary, reactorAnalytics, statusDist] = await Promise.all([
        axios.get('http://localhost:5000/api/dashboard/summary'),
        axios.get('http://localhost:5000/api/dashboard/reactor-analytics'),
        axios.get('http://localhost:5000/api/dashboard/status-distribution'),
      ])

      setStats({
        totalTransactions: summary.data.data?.totalProductionCount || 0,
        activeReactors: summary.data.data?.activeProductionCount || 0,
        totalProducts: summary.data.data?.completedProductionCount || 0,
        delayReasons: Math.round(summary.data.data?.totalDelayDurationHours || 0),
      })

      // Status distribution data
      const statusData = statusDist.data.data || []
      setStatusData(
        statusData.map((item: any) => ({
          name: item.status,
          value: item.count,
        }))
      )

      // Reactor analytics data
      const reactorData = reactorAnalytics.data.data || []
      setReactorData(
        reactorData.map((item: any) => ({
          name: item.reactorName,
          count: item.productionCount,
        }))
      )
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
          {/* Transaction Status Distribution */}
          <Grid item xs={12} md={6}>
            <Card>
              <CardContent>
                <Typography variant="h6" gutterBottom>
                  İşlem Durumları
                </Typography>
                <ResponsiveContainer width="100%" height={300}>
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
