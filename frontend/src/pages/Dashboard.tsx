import { useEffect, useState } from 'react'
import {
  Box,
  Container,
  Grid,
  Paper,
  Typography,
  Card,
  CardContent,
} from '@mui/material'
import {
  LineChart,
  Line,
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  ResponsiveContainer,
} from 'recharts'
import BusinessIcon from '@mui/icons-material/Business'
import ContactsIcon from '@mui/icons-material/Contacts'
import TrendingUpIcon from '@mui/icons-material/TrendingUp'
import AssignmentIcon from '@mui/icons-material/Assignment'
import { dashboardService } from '../services/dashboardService'
import { reportsService } from '../services/reportsService'
import { DashboardSummary } from '../types/dashboard'
import { SalesReportDto } from '../types/reports'
import LoadingSpinner from '../components/LoadingSpinner'
import { useToast } from '../components/Toast'

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

function Dashboard() {
  const [loading, setLoading] = useState(true)
  const [summary, setSummary] = useState<DashboardSummary | null>(null)
  const [salesReport, setSalesReport] = useState<SalesReportDto | null>(null)
  const { showToast } = useToast()

  useEffect(() => {
    loadData()
  }, [])

  const loadData = async () => {
    try {
      setLoading(true)
      const [summaryData, salesData] = await Promise.all([
        dashboardService.getSummary(),
        reportsService.getSalesReport(),
      ])
      setSummary(summaryData)
      setSalesReport(salesData)
    } catch (error) {
      showToast('Failed to load dashboard data', 'error')
      console.error('Failed to load dashboard data:', error)
    } finally {
      setLoading(false)
    }
  }

  if (loading) {
    return <LoadingSpinner open={loading} />
  }

  return (
    <Container maxWidth="xl">
      <Box sx={{ mt: 4, mb: 4 }}>
        <Typography variant="h4" component="h1" gutterBottom>
          Dashboard
        </Typography>

        {/* Stats Cards */}
        <Grid container spacing={3} sx={{ mb: 4 }}>
          <Grid item xs={12} sm={6} md={3}>
            <StatCard
              title="Companies"
              value={summary?.totalCompanies || 0}
              icon={<BusinessIcon sx={{ color: 'white' }} />}
              color="#1976d2"
            />
          </Grid>
          <Grid item xs={12} sm={6} md={3}>
            <StatCard
              title="Contacts"
              value={summary?.totalContacts || 0}
              icon={<ContactsIcon sx={{ color: 'white' }} />}
              color="#2e7d32"
            />
          </Grid>
          <Grid item xs={12} sm={6} md={3}>
            <StatCard
              title="Opportunities"
              value={summary?.totalOpportunities || 0}
              icon={<TrendingUpIcon sx={{ color: 'white' }} />}
              color="#ed6c02"
            />
          </Grid>
          <Grid item xs={12} sm={6} md={3}>
            <StatCard
              title="Activities"
              value={summary?.totalActivities || 0}
              icon={<AssignmentIcon sx={{ color: 'white' }} />}
              color="#9c27b0"
            />
          </Grid>
        </Grid>

        {/* Revenue Cards */}
        <Grid container spacing={3} sx={{ mb: 4 }}>
          <Grid item xs={12} md={4}>
            <Card>
              <CardContent>
                <Typography color="textSecondary" gutterBottom>
                  Total Revenue
                </Typography>
                <Typography variant="h4">
                  ${(summary?.totalRevenue || 0).toLocaleString()}
                </Typography>
              </CardContent>
            </Card>
          </Grid>
          <Grid item xs={12} md={4}>
            <Card>
              <CardContent>
                <Typography color="textSecondary" gutterBottom>
                  Average Deal Size
                </Typography>
                <Typography variant="h4">
                  ${(summary?.averageDealSize || 0).toLocaleString()}
                </Typography>
              </CardContent>
            </Card>
          </Grid>
          <Grid item xs={12} md={4}>
            <Card>
              <CardContent>
                <Typography color="textSecondary" gutterBottom>
                  Win Rate
                </Typography>
                <Typography variant="h4">
                  {((summary?.winRate || 0) * 100).toFixed(1)}%
                </Typography>
              </CardContent>
            </Card>
          </Grid>
        </Grid>

        {/* Charts */}
        {salesReport && (
          <>
            {/* Monthly Sales Trend */}
            <Paper sx={{ p: 3, mb: 4 }}>
              <Typography variant="h6" gutterBottom>
                Monthly Sales Trend
              </Typography>
              <ResponsiveContainer width="100%" height={300}>
                <LineChart data={salesReport.monthlySales}>
                  <CartesianGrid strokeDasharray="3 3" />
                  <XAxis dataKey="month" />
                  <YAxis />
                  <Tooltip />
                  <Legend />
                  <Line type="monotone" dataKey="totalValue" stroke="#8884d8" name="Total Value" />
                  <Line type="monotone" dataKey="wonValue" stroke="#82ca9d" name="Won Value" />
                </LineChart>
              </ResponsiveContainer>
            </Paper>

            <Grid container spacing={3}>
              {/* Sales by Stage */}
              <Grid item xs={12} md={6}>
                <Paper sx={{ p: 3 }}>
                  <Typography variant="h6" gutterBottom>
                    Sales by Stage
                  </Typography>
                  <ResponsiveContainer width="100%" height={300}>
                    <BarChart data={salesReport.salesByStage}>
                      <CartesianGrid strokeDasharray="3 3" />
                      <XAxis dataKey="stage" />
                      <YAxis />
                      <Tooltip />
                      <Legend />
                      <Bar dataKey="totalValue" fill="#8884d8" name="Total Value" />
                      <Bar dataKey="count" fill="#82ca9d" name="Count" />
                    </BarChart>
                  </ResponsiveContainer>
                </Paper>
              </Grid>

              {/* Sales by User */}
              <Grid item xs={12} md={6}>
                <Paper sx={{ p: 3 }}>
                  <Typography variant="h6" gutterBottom>
                    Sales by User
                  </Typography>
                  <ResponsiveContainer width="100%" height={300}>
                    <BarChart data={salesReport.salesByUser}>
                      <CartesianGrid strokeDasharray="3 3" />
                      <XAxis dataKey="userName" />
                      <YAxis />
                      <Tooltip />
                      <Legend />
                      <Bar dataKey="totalValue" fill="#8884d8" name="Total Value" />
                      <Bar dataKey="wonCount" fill="#82ca9d" name="Won Count" />
                    </BarChart>
                  </ResponsiveContainer>
                </Paper>
              </Grid>
            </Grid>
          </>
        )}
      </Box>
    </Container>
  )
}

export default Dashboard
