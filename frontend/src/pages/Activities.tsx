import { useState, useEffect } from 'react'
import {
  Container,
  Box,
  Typography,
  Button,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  IconButton,
  Chip,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  MenuItem,
  FormControl,
  InputLabel,
  Select,
} from '@mui/material'
import AddIcon from '@mui/icons-material/Add'
import EditIcon from '@mui/icons-material/Edit'
import DeleteIcon from '@mui/icons-material/Delete'
import RefreshIcon from '@mui/icons-material/Refresh'
import { Activity, CreateActivityDto, UpdateActivityDto } from '../types/activity'
import { activityService } from '../services/activityService'
import LoadingSpinner from '../components/LoadingSpinner'
import ConfirmDialog from '../components/ConfirmDialog'
import Pagination from '../components/Pagination'
import { useToast } from '../components/Toast'

const statusColors: Record<string, 'default' | 'primary' | 'success' | 'error'> = {
  'planned': 'default',
  'in-progress': 'primary',
  'completed': 'success',
  'cancelled': 'error',
}

const priorityColors: Record<string, 'default' | 'info' | 'warning' | 'error'> = {
  'low': 'default',
  'medium': 'info',
  'high': 'warning',
  'urgent': 'error',
}

export default function Activities() {
  const [activities, setActivities] = useState<Activity[]>([])
  const [loading, setLoading] = useState(false)
  const [openDialog, setOpenDialog] = useState(false)
  const [openConfirm, setOpenConfirm] = useState(false)
  const [selectedActivity, setSelectedActivity] = useState<Activity | null>(null)
  const [formData, setFormData] = useState<CreateActivityDto>({
    subject: '',
    description: '',
    type: 'Task',
    status: 'planned',
    priority: 'medium',
    dueDate: '',
  })
  const [totalCount, setTotalCount] = useState(0)
  const [pageNumber, setPageNumber] = useState(1)
  const [pageSize, setPageSize] = useState(10)
  const { showToast } = useToast()

  useEffect(() => {
    loadActivities()
  }, [pageNumber, pageSize])

  const loadActivities = async () => {
    try {
      setLoading(true)
      const response = await activityService.getAll({ pageNumber, pageSize })
      setActivities(response.items ?? [])
      setTotalCount(response.totalCount)
    } catch (error) {
      showToast('Failed to load activities', 'error')
      console.error('Failed to load activities:', error)
    } finally {
      setLoading(false)
    }
  }

  const handleOpenDialog = (activity?: Activity) => {
    if (activity) {
      setSelectedActivity(activity)
      setFormData({
        subject: activity.subject,
        description: activity.description,
        type: activity.type,
        status: activity.status,
        priority: activity.priority,
        dueDate: activity.dueDate || '',
      })
    } else {
      setSelectedActivity(null)
      setFormData({
        subject: '',
        description: '',
        type: 'task',
        status: 'planned',
        priority: 'medium',
        dueDate: '',
      })
    }
    setOpenDialog(true)
  }

  const handleCloseDialog = () => {
    setOpenDialog(false)
    setSelectedActivity(null)
  }

  const handleSubmit = async () => {
    try {
      if (selectedActivity) {
        await activityService.update(selectedActivity.id, formData as UpdateActivityDto)
        showToast('Activity updated successfully', 'success')
      } else {
        await activityService.create(formData)
        showToast('Activity created successfully', 'success')
      }
      handleCloseDialog()
      loadActivities()
    } catch (error) {
      showToast('Failed to save activity', 'error')
      console.error('Failed to save activity:', error)
    }
  }

  const handleDelete = async () => {
    if (selectedActivity) {
      try {
        await activityService.delete(selectedActivity.id)
        showToast('Activity deleted successfully', 'success')
        setOpenConfirm(false)
        setSelectedActivity(null)
        loadActivities()
      } catch (error) {
        showToast('Failed to delete activity', 'error')
        console.error('Failed to delete activity:', error)
      }
    }
  }

  const handleOpenConfirm = (activity: Activity) => {
    setSelectedActivity(activity)
    setOpenConfirm(true)
  }

  return (
    <Container maxWidth="xl">
      <LoadingSpinner open={loading} />
      
      <Box sx={{ mt: 4, mb: 4 }}>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
          <Typography variant="h4" component="h1">
            Activities & Tasks
          </Typography>
          <Box sx={{ display: 'flex', gap: 1 }}>
            <Button
              variant="outlined"
              startIcon={<RefreshIcon />}
              onClick={loadActivities}
              disabled={loading}
            >
              Refresh
            </Button>
            <Button
              variant="contained"
              startIcon={<AddIcon />}
              onClick={() => handleOpenDialog()}
            >
              Add Activity
            </Button>
          </Box>
        </Box>

        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Subject</TableCell>
                <TableCell>Type</TableCell>
                <TableCell>Status</TableCell>
                <TableCell>Priority</TableCell>
                <TableCell>Due Date</TableCell>
                <TableCell>Created</TableCell>
                <TableCell align="right">Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {activities.map((activity) => (
                <TableRow key={activity.id}>
                  <TableCell>{activity.subject}</TableCell>
                  <TableCell>
                    <Chip 
                      label={activity.type.charAt(0).toUpperCase() + activity.type.slice(1)} 
                      size="small" 
                    />
                  </TableCell>
                  <TableCell>
                    <Chip
                      label={activity.status.replace('_', ' ').split(' ').map(w => w.charAt(0).toUpperCase() + w.slice(1)).join(' ')}
                      color={statusColors[activity.status] || 'default'}
                      size="small"
                    />
                  </TableCell>
                  <TableCell>
                    <Chip
                      label={activity.priority.charAt(0).toUpperCase() + activity.priority.slice(1)}
                      color={priorityColors[activity.priority] || 'default'}
                      size="small"
                    />
                  </TableCell>
                  <TableCell>
                    {activity.dueDate
                      ? new Date(activity.dueDate).toLocaleDateString()
                      : 'N/A'}
                  </TableCell>
                  <TableCell>
                    {new Date(activity.createdAt).toLocaleDateString()}
                  </TableCell>
                  <TableCell align="right">
                    <IconButton
                      size="small"
                      onClick={() => handleOpenDialog(activity)}
                      color="primary"
                    >
                      <EditIcon />
                    </IconButton>
                    <IconButton
                      size="small"
                      onClick={() => handleOpenConfirm(activity)}
                      color="error"
                    >
                      <DeleteIcon />
                    </IconButton>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
          <Pagination
            totalCount={totalCount}
            pageNumber={pageNumber}
            pageSize={pageSize}
            onPageChange={setPageNumber}
            onPageSizeChange={setPageSize}
          />
        </TableContainer>
      </Box>

      {/* Activity Form Dialog */}
      <Dialog open={openDialog} onClose={handleCloseDialog} maxWidth="sm" fullWidth>
        <DialogTitle>
          {selectedActivity ? 'Edit Activity' : 'Add Activity'}
        </DialogTitle>
        <DialogContent>
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, pt: 2 }}>
            <TextField
              label="Title"
              fullWidth
              value={formData.subject}
              onChange={(e) => setFormData({ ...formData, subject: e.target.value })}
              required
            />
            <TextField
              label="Description"
              fullWidth
              multiline
              rows={3}
              value={formData.description}
              onChange={(e) => setFormData({ ...formData, description: e.target.value })}
            />
            <FormControl fullWidth>
              <InputLabel>Type</InputLabel>
              <Select
                value={formData.type}
                label="Type"
                onChange={(e) => setFormData({ ...formData, type: e.target.value })}
              >
                <MenuItem value="task">Task</MenuItem>
                <MenuItem value="call">Call</MenuItem>
                <MenuItem value="email">Email</MenuItem>
                <MenuItem value="meeting">Meeting</MenuItem>
                <MenuItem value="deadline">Deadline</MenuItem>
                <MenuItem value="other">Other</MenuItem>
              </Select>
            </FormControl>
            <FormControl fullWidth>
              <InputLabel>Status</InputLabel>
              <Select
                value={formData.status}
                label="Status"
                onChange={(e) => setFormData({ ...formData, status: e.target.value })}
              >
                <MenuItem value="planned">Planned</MenuItem>
                <MenuItem value="in_progress">In Progress</MenuItem>
                <MenuItem value="completed">Completed</MenuItem>
                <MenuItem value="cancelled">Cancelled</MenuItem>
              </Select>
            </FormControl>
            <FormControl fullWidth>
              <InputLabel>Priority</InputLabel>
              <Select
                value={formData.priority}
                label="Priority"
                onChange={(e) => setFormData({ ...formData, priority: e.target.value })}
              >
                <MenuItem value="low">Low</MenuItem>
                <MenuItem value="medium">Medium</MenuItem>
                <MenuItem value="high">High</MenuItem>
                <MenuItem value="urgent">Urgent</MenuItem>
              </Select>
            </FormControl>
            <TextField
              label="Due Date"
              type="datetime-local"
              fullWidth
              InputLabelProps={{ shrink: true }}
              value={formData.dueDate}
              onChange={(e) => setFormData({ ...formData, dueDate: e.target.value })}
            />
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseDialog}>Cancel</Button>
          <Button onClick={handleSubmit} variant="contained">
            {selectedActivity ? 'Update' : 'Create'}
          </Button>
        </DialogActions>
      </Dialog>

      {/* Delete Confirmation */}
      <ConfirmDialog
        open={openConfirm}
        title="Delete Activity"
        message="Are you sure you want to delete this activity?"
        onConfirm={handleDelete}
        onCancel={() => setOpenConfirm(false)}
        confirmText="Delete"
        confirmColor="error"
      />
    </Container>
  )
}
