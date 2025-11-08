import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import {
  Container,
  Box,
  Typography,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TextField,
  InputAdornment,
  IconButton,
  Tooltip,
} from '@mui/material'
import SearchIcon from '@mui/icons-material/Search'
import VisibilityIcon from '@mui/icons-material/Visibility'
import { ActivityLog } from '../types/activityLog'
import { activityLogService } from '../services/activityLogService'
import LoadingSpinner from '../components/LoadingSpinner'
import Pagination from '../components/Pagination'
import { useToast } from '../components/Toast'

export default function ActivityLogs() {
  const navigate = useNavigate()
  const [logs, setLogs] = useState<ActivityLog[]>([])
  const [loading, setLoading] = useState(false)
  const [searchTerm, setSearchTerm] = useState('')
  const [totalCount, setTotalCount] = useState(0)
  const [pageNumber, setPageNumber] = useState(1)
  const [pageSize, setPageSize] = useState(25)
  const [totalPages, setTotalPages] = useState(0)
  const { showToast } = useToast()

  useEffect(() => {
    loadLogs()
  }, [pageNumber, pageSize])

  const loadLogs = async () => {
    try {
      setLoading(true)
      const response = await activityLogService.getAll({ pageNumber, pageSize })
      setLogs(response.items || [])
      setTotalCount(response.totalCount || 0)
      setTotalPages(response.totalPages || 0)
    } catch (error) {
      showToast('Failed to load activity logs', 'error')
      console.error('Failed to load activity logs:', error)
      setLogs([])
      setTotalCount(0)
      setTotalPages(0)
    } finally {
      setLoading(false)
    }
  }

  const filteredLogs = logs.filter(
    (log) =>
      log.method.toLowerCase().includes(searchTerm.toLowerCase()) ||
      log.path.toLowerCase().includes(searchTerm.toLowerCase()) ||
      log.userEmail?.toLowerCase().includes(searchTerm.toLowerCase()) ||
      log.requestId.toLowerCase().includes(searchTerm.toLowerCase())
  )

  return (
    <Container maxWidth="xl">
      <LoadingSpinner open={loading} />
      
      <Box sx={{ mt: 4, mb: 4 }}>
        <Typography variant="h4" component="h1" gutterBottom>
          Activity Logs
        </Typography>

        <Box sx={{ mb: 3 }}>
          <TextField
            fullWidth
            placeholder="Search logs..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            InputProps={{
              startAdornment: (
                <InputAdornment position="start">
                  <SearchIcon />
                </InputAdornment>
              ),
            }}
          />
        </Box>

        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Timestamp</TableCell>
                <TableCell>Method</TableCell>
                <TableCell>Path</TableCell>
                <TableCell>Status</TableCell>
                <TableCell>Duration (ms)</TableCell>
                <TableCell>User Email</TableCell>
                <TableCell>IP Address</TableCell>
                <TableCell align="right">İşlemler</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {filteredLogs.map((log) => (
                <TableRow key={log.requestId}>
                  <TableCell>
                    {new Date(log.timestamp).toLocaleString()}
                  </TableCell>
                  <TableCell>
                    <Typography variant="body2" fontWeight="medium">
                      {log.method}
                    </Typography>
                  </TableCell>
                  <TableCell>
                    <Typography variant="body2" sx={{ fontFamily: 'monospace' }}>
                      {log.path}
                    </Typography>
                  </TableCell>
                  <TableCell>
                    <Typography
                      variant="body2"
                      sx={{
                        color: log.statusCode >= 200 && log.statusCode < 300 ? 'success.main' : 
                               log.statusCode >= 400 ? 'error.main' : 'warning.main',
                        fontWeight: 'medium'
                      }}
                    >
                      {log.statusCode}
                    </Typography>
                  </TableCell>
                  <TableCell>{log.durationMs}</TableCell>
                  <TableCell>{log.userEmail || 'N/A'}</TableCell>
                  <TableCell>
                    <Typography variant="caption" sx={{ fontFamily: 'monospace' }}>
                      {log.ipAddress || 'N/A'}
                    </Typography>
                  </TableCell>
                  <TableCell align="right">
                    <Tooltip title="Detayları Göster">
                      <IconButton
                        size="small"
                        onClick={() => navigate(`/activity-logs/${log.requestId}`)}
                      >
                        <VisibilityIcon fontSize="small" />
                      </IconButton>
                    </Tooltip>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
          <Pagination
            totalCount={totalCount}
            pageNumber={pageNumber}
            pageSize={pageSize}
            totalPages={totalPages}
            onPageChange={setPageNumber}
            onPageSizeChange={setPageSize}
          />
        </TableContainer>
      </Box>
    </Container>
  )
}
