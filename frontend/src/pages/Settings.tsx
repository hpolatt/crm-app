import { useState, useEffect } from 'react'
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
  IconButton,
  Chip,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Button,
  Switch,
} from '@mui/material'
import EditIcon from '@mui/icons-material/Edit'
import RefreshIcon from '@mui/icons-material/Refresh'
import { SystemSetting, UpdateSystemSettingDto } from '../types/settings'
import { settingsService } from '../services/settingsService'
import LoadingSpinner from '../components/LoadingSpinner'
import { useToast } from '../components/Toast'

export default function Settings() {
  const [settings, setSettings] = useState<SystemSetting[]>([])
  const [loading, setLoading] = useState(false)
  const [openDialog, setOpenDialog] = useState(false)
  const [selectedSetting, setSelectedSetting] = useState<SystemSetting | null>(null)
  const [formData, setFormData] = useState<UpdateSystemSettingDto>({
    value: '',
    description: '',
    isActive: true,
  })
  const { showToast } = useToast()

  useEffect(() => {
    loadSettings()
  }, [])

  const loadSettings = async () => {
    try {
      setLoading(true)
      const data = await settingsService.getAll()
      setSettings(data)
    } catch (error) {
      showToast('Failed to load settings', 'error')
      console.error('Failed to load settings:', error)
    } finally {
      setLoading(false)
    }
  }

  const handleOpenDialog = (setting: SystemSetting) => {
    setSelectedSetting(setting)
    setFormData({
      value: setting.value,
      description: setting.description,
      isActive: setting.isActive,
    })
    setOpenDialog(true)
  }

  const handleCloseDialog = () => {
    setOpenDialog(false)
    setSelectedSetting(null)
  }

  const handleSubmit = async () => {
    if (selectedSetting) {
      try {
        await settingsService.update(selectedSetting.key, formData)
        showToast('Setting updated successfully', 'success')
        handleCloseDialog()
        loadSettings()
      } catch (error) {
        showToast('Failed to update setting', 'error')
        console.error('Failed to update setting:', error)
      }
    }
  }

  return (
    <Container maxWidth="xl">
      <LoadingSpinner open={loading} />
      
      <Box sx={{ mt: 4, mb: 4 }}>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
          <Typography variant="h4" component="h1">
            System Settings
          </Typography>
          <Button
            variant="outlined"
            startIcon={<RefreshIcon />}
            onClick={loadSettings}
            disabled={loading}
          >
            Refresh
          </Button>
        </Box>

        <TableContainer component={Paper} sx={{ mt: 3 }}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Key</TableCell>
                <TableCell>Value</TableCell>
                <TableCell>Description</TableCell>
                <TableCell>Status</TableCell>
                <TableCell>Last Updated</TableCell>
                <TableCell align="right">Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {settings.map((setting) => (
                <TableRow key={setting.id}>
                  <TableCell>
                    <Typography variant="body2" fontWeight="medium">
                      {setting.key}
                    </Typography>
                  </TableCell>
                  <TableCell>{setting.value}</TableCell>
                  <TableCell>{setting.description || 'N/A'}</TableCell>
                  <TableCell>
                    <Chip
                      label={setting.isActive ? 'Active' : 'Inactive'}
                      color={setting.isActive ? 'success' : 'default'}
                      size="small"
                    />
                  </TableCell>
                  <TableCell>
                    {new Date(setting.updatedAt).toLocaleDateString()}
                  </TableCell>
                  <TableCell align="right">
                    <IconButton
                      size="small"
                      onClick={() => handleOpenDialog(setting)}
                      color="primary"
                    >
                      <EditIcon />
                    </IconButton>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      </Box>

      {/* Setting Edit Dialog */}
      <Dialog open={openDialog} onClose={handleCloseDialog} maxWidth="sm" fullWidth>
        <DialogTitle>
          Edit Setting: {selectedSetting?.key}
        </DialogTitle>
        <DialogContent>
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, pt: 2 }}>
            <TextField
              label="Value"
              fullWidth
              value={formData.value}
              onChange={(e) => setFormData({ ...formData, value: e.target.value })}
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
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
              <Typography>Active:</Typography>
              <Switch
                checked={formData.isActive}
                onChange={(e) => setFormData({ ...formData, isActive: e.target.checked })}
              />
            </Box>
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseDialog}>Cancel</Button>
          <Button onClick={handleSubmit} variant="contained">
            Update
          </Button>
        </DialogActions>
      </Dialog>
    </Container>
  )
}
