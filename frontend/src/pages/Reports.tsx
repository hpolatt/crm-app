import { useState, useEffect } from 'react'
import {
  Container,
  Box,
  Typography,
  Paper,
  Grid,
  Card,
  CardContent,
  Tabs,
  Tab,
} from '@mui/material'
import {
  BarChart,
  Bar,
  LineChart,
  Line,
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
import { reportsService } from '../services/reportsService'
import { SalesReportDto, CustomerReportDto } from '../types/reports'
import LoadingSpinner from '../components/LoadingSpinner'
import { useToast } from '../components/Toast'

const COLORS = ['#0088FE', '#00C49F', '#FFBB28', '#FF8042', '#8884D8', '#FF6B9D']

interface TabPanelProps {
  children?: React.ReactNode
  index: number
  value: number
}

function TabPanel({ children, value, index }: TabPanelProps) {
  return (
    <div hidden={value !== index}>
      {value === index && <Box sx={{ pt: 3 }}>{children}</Box>}
    </div>
  )
}

export default function Reports() {
  const [loading, setLoading] = useState(false)
  const [salesReport, setSalesReport] = useState<SalesReportDto | null>(null)
  const [customerReport, setCustomerReport] = useState<CustomerReportDto | null>(null)
  const [tabValue, setTabValue] = useState(0)
  const { showToast } = useToast()

  useEffect(() => {
    loadReports()
  }, [])

  const loadReports = async () => {
    try {
      setLoading(true)
      const [sales, customers] = await Promise.all([
        reportsService.getSalesReport(),
        reportsService.getCustomerReport(),
      ])
      setSalesReport(sales)
      setCustomerReport(customers)
    } catch (error) {
      showToast('Failed to load reports', 'error')
      console.error('Failed to load reports:', error)
    } finally {
      setLoading(false)
    }
  }

  return (
    <Container maxWidth="xl">
      <LoadingSpinner open={loading} />
      
      <Box sx={{ mt: 4, mb: 4 }}>
        <Typography variant="h4" component="h1" gutterBottom>
          Reports & Analytics
        </Typography>

        <Paper sx={{ mt: 3 }}>
          <Tabs value={tabValue} onChange={(_, value) => setTabValue(value)}>
            <Tab label="Sales Report" />
            <Tab label="Customer Report" />
          </Tabs>

          {/* Sales Report Tab */}
          <TabPanel value={tabValue} index={0}>
            {salesReport && (
              <>
                {/* Sales Summary Cards */}
                <Grid container spacing={3} sx={{ mb: 4 }}>
                  <Grid item xs={12} md={4}>
                    <Card>
                      <CardContent>
                        <Typography color="textSecondary" gutterBottom>
                          Total Revenue
                        </Typography>
                        <Typography variant="h4">
                          ${salesReport.totalRevenue.toLocaleString()}
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
                          ${salesReport.averageDealSize.toLocaleString()}
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
                          {(salesReport.winRate * 100).toFixed(1)}%
                        </Typography>
                      </CardContent>
                    </Card>
                  </Grid>
                </Grid>

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
                      <Line type="monotone" dataKey="count" stroke="#ffc658" name="Count" />
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
                        </BarChart>
                      </ResponsiveContainer>
                    </Paper>
                  </Grid>

                  {/* Sales by User */}
                  <Grid item xs={12} md={6}>
                    <Paper sx={{ p: 3 }}>
                      <Typography variant="h6" gutterBottom>
                        Sales Performance by User
                      </Typography>
                      <ResponsiveContainer width="100%" height={300}>
                        <BarChart data={salesReport.salesByUser}>
                          <CartesianGrid strokeDasharray="3 3" />
                          <XAxis dataKey="userName" />
                          <YAxis />
                          <Tooltip />
                          <Legend />
                          <Bar dataKey="totalValue" fill="#82ca9d" name="Total Value" />
                          <Bar dataKey="wonCount" fill="#8884d8" name="Won Deals" />
                        </BarChart>
                      </ResponsiveContainer>
                    </Paper>
                  </Grid>
                </Grid>
              </>
            )}
          </TabPanel>

          {/* Customer Report Tab */}
          <TabPanel value={tabValue} index={1}>
            {customerReport && (
              <>
                {/* Customer Summary Cards */}
                <Grid container spacing={3} sx={{ mb: 4 }}>
                  <Grid item xs={12} md={3}>
                    <Card>
                      <CardContent>
                        <Typography color="textSecondary" gutterBottom>
                          Total Customers
                        </Typography>
                        <Typography variant="h4">
                          {customerReport.totalCustomers}
                        </Typography>
                      </CardContent>
                    </Card>
                  </Grid>
                  <Grid item xs={12} md={3}>
                    <Card>
                      <CardContent>
                        <Typography color="textSecondary" gutterBottom>
                          Active Customers
                        </Typography>
                        <Typography variant="h4">
                          {customerReport.activeCustomers}
                        </Typography>
                      </CardContent>
                    </Card>
                  </Grid>
                  <Grid item xs={12} md={3}>
                    <Card>
                      <CardContent>
                        <Typography color="textSecondary" gutterBottom>
                          New This Month
                        </Typography>
                        <Typography variant="h4">
                          {customerReport.newCustomersThisMonth}
                        </Typography>
                      </CardContent>
                    </Card>
                  </Grid>
                  <Grid item xs={12} md={3}>
                    <Card>
                      <CardContent>
                        <Typography color="textSecondary" gutterBottom>
                          Top Customers
                        </Typography>
                        <Typography variant="h4">
                          {customerReport.topCustomersByRevenue?.length || 0}
                        </Typography>
                      </CardContent>
                    </Card>
                  </Grid>
                </Grid>

                {/* Customer Growth */}
                <Paper sx={{ p: 3, mb: 4 }}>
                  <Typography variant="h6" gutterBottom>
                    Customer Growth
                  </Typography>
                  <ResponsiveContainer width="100%" height={300}>
                    <LineChart data={customerReport.customerGrowth}>
                      <CartesianGrid strokeDasharray="3 3" />
                      <XAxis dataKey="monthName" />
                      <YAxis />
                      <Tooltip />
                      <Legend />
                      <Line type="monotone" dataKey="newCustomers" stroke="#8884d8" name="New Customers" />
                      <Line type="monotone" dataKey="totalCustomers" stroke="#82ca9d" name="Total Customers" />
                    </LineChart>
                  </ResponsiveContainer>
                </Paper>

                <Grid container spacing={3}>
                  {/* Customers by Source */}
                  <Grid item xs={12} md={6}>
                    <Paper sx={{ p: 3 }}>
                      <Typography variant="h6" gutterBottom>
                        Customers by Source
                      </Typography>
                      <ResponsiveContainer width="100%" height={300}>
                        <PieChart>
                          <Pie
                            data={customerReport.customersBySource as any}
                            dataKey="count"
                            nameKey="source"
                            cx="50%"
                            cy="50%"
                            outerRadius={100}
                            label
                          >
                            {customerReport.customersBySource.map((_, index) => (
                              <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                            ))}
                          </Pie>
                          <Tooltip />
                          <Legend />
                        </PieChart>
                      </ResponsiveContainer>
                    </Paper>
                  </Grid>

                  {/* Customers by Industry */}
                  <Grid item xs={12} md={6}>
                    <Paper sx={{ p: 3 }}>
                      <Typography variant="h6" gutterBottom>
                        Customers by Industry
                      </Typography>
                      <ResponsiveContainer width="100%" height={300}>
                        <PieChart>
                          <Pie
                            data={customerReport.customersByIndustry as any}
                            dataKey="count"
                            nameKey="industry"
                            cx="50%"
                            cy="50%"
                            outerRadius={100}
                            label
                          >
                            {customerReport.customersByIndustry.map((_, index) => (
                              <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                            ))}
                          </Pie>
                          <Tooltip />
                          <Legend />
                        </PieChart>
                      </ResponsiveContainer>
                    </Paper>
                  </Grid>

                  {/* Top Customers by Revenue */}
                  <Grid item xs={12} md={12}>
                    <Paper sx={{ p: 3 }}>
                      <Typography variant="h6" gutterBottom>
                        Top Customers by Revenue
                      </Typography>
                      <ResponsiveContainer width="100%" height={300}>
                        <BarChart data={customerReport.topCustomersByRevenue}>
                          <CartesianGrid strokeDasharray="3 3" />
                          <XAxis dataKey="companyName" angle={-45} textAnchor="end" height={100} />
                          <YAxis />
                          <Tooltip />
                          <Legend />
                          <Bar dataKey="totalRevenue" fill="#8884d8" name="Revenue" />
                        </BarChart>
                      </ResponsiveContainer>
                    </Paper>
                  </Grid>
                </Grid>
              </>
            )}
          </TabPanel>
        </Paper>
      </Box>
    </Container>
  )
}
