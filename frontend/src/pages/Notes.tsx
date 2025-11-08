import { useState, useEffect } from 'react'
import {
  Container,
  Box,
  Typography,
  Button,
  Grid,
  Card,
  CardContent,
  CardActions,
  IconButton,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  MenuItem,
  FormControl,
  InputLabel,
  Select,
  Autocomplete,
} from '@mui/material'
import AddIcon from '@mui/icons-material/Add'
import EditIcon from '@mui/icons-material/Edit'
import DeleteIcon from '@mui/icons-material/Delete'
import RefreshIcon from '@mui/icons-material/Refresh'
import { Note, CreateNoteDto, UpdateNoteDto } from '../types/note'
import { noteService } from '../services/noteService'
import { companyService } from '../services/companyService'
import { contactService } from '../services/contactService'
import { leadService } from '../services/leadService'
import { opportunityService } from '../services/opportunityService'
import LoadingSpinner from '../components/LoadingSpinner'
import ConfirmDialog from '../components/ConfirmDialog'
import Pagination from '../components/Pagination'
import { useToast } from '../components/Toast'

export default function Notes() {
  const [notes, setNotes] = useState<Note[]>([])
  const [loading, setLoading] = useState(false)
  const [openDialog, setOpenDialog] = useState(false)
  const [openConfirm, setOpenConfirm] = useState(false)
  const [selectedNote, setSelectedNote] = useState<Note | null>(null)
  const [formData, setFormData] = useState<CreateNoteDto>({ 
    content: '',
    isPinned: false,
    isActive: true,
  })
  const [entityType, setEntityType] = useState<'company' | 'contact' | 'lead' | 'opportunity'>('company')
  
  // Entity options
  const [companies, setCompanies] = useState<Array<{id: string, name: string}>>([])
  const [contacts, setContacts] = useState<Array<{id: string, name: string}>>([])
  const [leads, setLeads] = useState<Array<{id: string, title: string}>>([])
  const [opportunities, setOpportunities] = useState<Array<{id: string, title: string}>>([])
  
  const [totalCount, setTotalCount] = useState(0)
  const [pageNumber, setPageNumber] = useState(1)
  const [pageSize, setPageSize] = useState(12)
  const [totalPages, setTotalPages] = useState(0)
  const [hasPreviousPage, setHasPreviousPage] = useState(false)
  const [hasNextPage, setHasNextPage] = useState(false)
  const { showToast } = useToast()

  useEffect(() => {
    loadNotes()
    loadEntities() // Load all entity options
  }, [pageNumber, pageSize])

  const loadEntities = async () => {
    try {
      // Load companies
      const companiesData = await companyService.getCompanies(1, 1000, true)
      setCompanies(companiesData.items.map(c => ({ id: c.id, name: c.name })))

      // Load contacts
      const contactsData = await contactService.getContacts(1, 1000, undefined, true)
      setContacts(contactsData.items.map(c => ({ 
        id: c.id, 
        name: `${c.firstName} ${c.lastName}` 
      })))

      // Load leads
      const leadsData = await leadService.getLeads(1, 1000, undefined, undefined, true)
      setLeads(leadsData.items.map(l => ({ id: l.id, title: l.title })))

      // Load opportunities
      const opportunitiesData = await opportunityService.getAll({ pageNumber: 1, pageSize: 1000 })
      setOpportunities(opportunitiesData.items.map(o => ({ id: o.id, title: o.title })))
    } catch (error) {
      console.error('Failed to load entities:', error)
    }
  }

  const loadNotes = async () => {
    try {
      setLoading(true)
      const response = await noteService.getAll({ pageNumber, pageSize })
      // response.data: { items, totalCount, pageNumber, pageSize, totalPages, hasPreviousPage, hasNextPage }
      const notesData = response.data?.items || []
      setNotes(notesData)
      setTotalCount(response.data?.totalCount || 0)
      setTotalPages(response.data?.totalPages || 0)
      setHasPreviousPage(response.data?.hasPreviousPage || false)
      setHasNextPage(response.data?.hasNextPage || false)
    } catch (error) {
      showToast('Failed to load notes', 'error')
      console.error('Failed to load notes:', error)
      setNotes([])
      setTotalCount(0)
      setTotalPages(0)
      setHasPreviousPage(false)
      setHasNextPage(false)
    } finally {
      setLoading(false)
    }
  }

  const handleOpenDialog = (note?: Note) => {
    if (note) {
      setSelectedNote(note)
      setFormData({
        content: note.content,
        companyId: note.companyId,
        contactId: note.contactId,
        leadId: note.leadId,
        opportunityId: note.opportunityId,
        isActive: note.isActive,
        isPinned: note.isPinned,
      })
      // Determine entity type from note
      if (note.companyId) setEntityType('company')
      else if (note.contactId) setEntityType('contact')
      else if (note.leadId) setEntityType('lead')
      else if (note.opportunityId) setEntityType('opportunity')
    } else {
      setSelectedNote(null)
      setFormData({
        content: '',
        isActive: true,
        isPinned: false,
      })
      setEntityType('company') // Default to company
    }
    setOpenDialog(true)
  }

  const handleCloseDialog = () => {
    setOpenDialog(false)
    setSelectedNote(null)
  }

  const handleSubmit = async () => {
    try {
      if (selectedNote) {
        await noteService.update(selectedNote.id, formData as UpdateNoteDto)
        showToast('Note updated successfully', 'success')
      } else {
        await noteService.create(formData)
        showToast('Note created successfully', 'success')
      }
      handleCloseDialog()
      loadNotes()
    } catch (error) {
      showToast('Failed to save note', 'error')
      console.error('Failed to save note:', error)
    }
  }

  const handleDelete = async () => {
    if (selectedNote) {
      try {
        await noteService.delete(selectedNote.id)
        showToast('Note deleted successfully', 'success')
        setOpenConfirm(false)
        setSelectedNote(null)
        loadNotes()
      } catch (error) {
        showToast('Failed to delete note', 'error')
        console.error('Failed to delete note:', error)
      }
    }
  }

  const handleOpenConfirm = (note: Note) => {
    setSelectedNote(note)
    setOpenConfirm(true)
  }

  return (
    <Container maxWidth="xl">
      <LoadingSpinner open={loading} />
      
      <Box sx={{ mt: 4, mb: 4 }}>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
          <Typography variant="h4" component="h1">
            Notes
          </Typography>
          <Box sx={{ display: 'flex', gap: 1 }}>
            <Button
              variant="outlined"
              startIcon={<RefreshIcon />}
              onClick={loadNotes}
              disabled={loading}
            >
              Refresh
            </Button>
            <Button
              variant="contained"
              startIcon={<AddIcon />}
              onClick={() => handleOpenDialog()}
            >
              Add Note
            </Button>
          </Box>
        </Box>

        <Grid container spacing={3}>
          {notes.map((note) => (
            <Grid item xs={12} sm={6} md={4} key={note.id}>
              <Card sx={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
                <CardContent sx={{ flexGrow: 1 }}>
                  <Typography
                    variant="body2"
                    color="text.secondary"
                    sx={{
                      overflow: 'hidden',
                      textOverflow: 'ellipsis',
                      display: '-webkit-box',
                      WebkitLineClamp: 5,
                      WebkitBoxOrient: 'vertical',
                      mb: 2,
                    }}
                  >
                    {note.content}
                  </Typography>
                  {(note.companyName || note.contactName || note.leadName || note.opportunityTitle) && (
                    <Typography variant="caption" color="text.secondary" sx={{ display: 'block', mb: 1 }}>
                      {[note.companyName, note.contactName, note.leadName, note.opportunityTitle].filter(Boolean).join(' • ')}
                    </Typography>
                  )}
                  <Typography variant="caption" color="text.secondary" sx={{ display: 'block' }}>
                    {new Date(note.createdAt).toLocaleDateString()}
                  </Typography>
                </CardContent>
                <CardActions>
                  <IconButton
                    size="small"
                    onClick={() => handleOpenDialog(note)}
                    color="primary"
                  >
                    <EditIcon />
                  </IconButton>
                  <IconButton
                    size="small"
                    onClick={() => handleOpenConfirm(note)}
                    color="error"
                  >
                    <DeleteIcon />
                  </IconButton>
                </CardActions>
              </Card>
            </Grid>
          ))}
        </Grid>

        {totalCount > 0 && (
          <Box sx={{ mt: 3 }}>
            <Pagination
              totalCount={totalCount}
              pageNumber={pageNumber}
              pageSize={pageSize}
              totalPages={totalPages}
              hasPreviousPage={hasPreviousPage}
              hasNextPage={hasNextPage}
              onPageChange={setPageNumber}
              onPageSizeChange={setPageSize}
            />
          </Box>
        )}
      </Box>

      {/* Note Form Dialog */}
      <Dialog open={openDialog} onClose={handleCloseDialog} maxWidth="md" fullWidth>
        <DialogTitle>
          {selectedNote ? 'Edit Note' : 'Add Note'}
        </DialogTitle>
        <DialogContent>
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, pt: 2 }}>
            <FormControl fullWidth required>
              <InputLabel>Entity Type</InputLabel>
              <Select
                value={entityType}
                label="Entity Type"
                onChange={(e) => {
                  const newEntityType = e.target.value as typeof entityType
                  setEntityType(newEntityType)
                  // Clear all entity IDs when switching type
                  setFormData({
                    ...formData,
                    companyId: undefined,
                    contactId: undefined,
                    leadId: undefined,
                    opportunityId: undefined,
                  })
                }}
              >
                <MenuItem value="company">Company</MenuItem>
                <MenuItem value="contact">Contact</MenuItem>
                <MenuItem value="lead">Lead</MenuItem>
                <MenuItem value="opportunity">Opportunity</MenuItem>
              </Select>
            </FormControl>

            {entityType === 'company' && (
              <Autocomplete
                options={companies}
                getOptionLabel={(option) => option.name}
                value={companies.find(c => c.id === formData.companyId) || null}
                onChange={(_, newValue) => {
                  setFormData({
                    ...formData,
                    companyId: newValue?.id,
                    contactId: undefined,
                    leadId: undefined,
                    opportunityId: undefined,
                  })
                }}
                renderInput={(params) => (
                  <TextField {...params} label="Company" required />
                )}
              />
            )}

            {entityType === 'contact' && (
              <Autocomplete
                options={contacts}
                getOptionLabel={(option) => option.name}
                value={contacts.find(c => c.id === formData.contactId) || null}
                onChange={(_, newValue) => {
                  setFormData({
                    ...formData,
                    companyId: undefined,
                    contactId: newValue?.id,
                    leadId: undefined,
                    opportunityId: undefined,
                  })
                }}
                renderInput={(params) => (
                  <TextField {...params} label="Contact" required />
                )}
              />
            )}

            {entityType === 'lead' && (
              <Autocomplete
                options={leads}
                getOptionLabel={(option) => option.title}
                value={leads.find(l => l.id === formData.leadId) || null}
                onChange={(_, newValue) => {
                  setFormData({
                    ...formData,
                    companyId: undefined,
                    contactId: undefined,
                    leadId: newValue?.id,
                    opportunityId: undefined,
                  })
                }}
                renderInput={(params) => (
                  <TextField {...params} label="Lead" required />
                )}
              />
            )}

            {entityType === 'opportunity' && (
              <Autocomplete
                options={opportunities}
                getOptionLabel={(option) => option.title}
                value={opportunities.find(o => o.id === formData.opportunityId) || null}
                onChange={(_, newValue) => {
                  setFormData({
                    ...formData,
                    companyId: undefined,
                    contactId: undefined,
                    leadId: undefined,
                    opportunityId: newValue?.id,
                  })
                }}
                renderInput={(params) => (
                  <TextField {...params} label="Opportunity" required />
                )}
              />
            )}

            <TextField
              label="Content"
              fullWidth
              multiline
              rows={8}
              value={formData.content}
              onChange={(e) => setFormData({ ...formData, content: e.target.value })}
              required
            />
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseDialog}>Cancel</Button>
          <Button onClick={handleSubmit} variant="contained">
            {selectedNote ? 'Update' : 'Create'}
          </Button>
        </DialogActions>
      </Dialog>

      {/* Delete Confirmation */}
      <ConfirmDialog
        open={openConfirm}
        title="Delete Note"
        message="Are you sure you want to delete this note?"
        onConfirm={handleDelete}
        onCancel={() => setOpenConfirm(false)}
        confirmText="Delete"
        confirmColor="error"
      />
    </Container>
  )
}
