import { createSlice, createAsyncThunk, PayloadAction } from '@reduxjs/toolkit'
import contactService from '../../services/contactService'
import { Contact, CreateContactDto, UpdateContactDto, ContactsResponse } from '../../types/contact'

interface ContactState {
  contacts: Contact[]
  selectedContact: Contact | null
  totalCount: number
  pageNumber: number
  pageSize: number
  totalPages: number
  loading: boolean
  error: string | null
}

const initialState: ContactState = {
  contacts: [],
  selectedContact: null,
  totalCount: 0,
  pageNumber: 1,
  pageSize: 10,
  totalPages: 0,
  loading: false,
  error: null,
}

// Async thunks
export const fetchContacts = createAsyncThunk(
  'contacts/fetchContacts',
  async (params: { pageNumber?: number; pageSize?: number; companyId?: string; isActive?: boolean }) => {
    const response = await contactService.getContacts(
      params.pageNumber,
      params.pageSize,
      params.companyId,
      params.isActive
    )
    return response
  }
)

export const fetchContactById = createAsyncThunk(
  'contacts/fetchContactById',
  async (id: string) => {
    const response = await contactService.getContactById(id)
    return response
  }
)

export const fetchContactsByCompany = createAsyncThunk(
  'contacts/fetchContactsByCompany',
  async (companyId: string) => {
    const response = await contactService.getContactsByCompany(companyId)
    return response
  }
)

export const createContact = createAsyncThunk(
  'contacts/createContact',
  async (contact: CreateContactDto) => {
    const response = await contactService.createContact(contact)
    return response
  }
)

export const updateContact = createAsyncThunk(
  'contacts/updateContact',
  async ({ id, contact }: { id: string; contact: UpdateContactDto }) => {
    const response = await contactService.updateContact(id, contact)
    return response
  }
)

export const deleteContact = createAsyncThunk(
  'contacts/deleteContact',
  async (id: string) => {
    await contactService.deleteContact(id)
    return id
  }
)

const contactSlice = createSlice({
  name: 'contacts',
  initialState,
  reducers: {
    clearSelectedContact: (state) => {
      state.selectedContact = null
    },
    clearError: (state) => {
      state.error = null
    },
  },
  extraReducers: (builder) => {
    // Fetch contacts
    builder
      .addCase(fetchContacts.pending, (state) => {
        state.loading = true
        state.error = null
      })
      .addCase(fetchContacts.fulfilled, (state, action: PayloadAction<ContactsResponse>) => {
        state.loading = false
        state.contacts = action.payload?.items || []
        state.totalCount = action.payload?.totalCount || 0
        state.pageNumber = action.payload?.pageNumber || 1
        state.pageSize = action.payload?.pageSize || 10
        state.totalPages = action.payload?.totalPages || 0
      })
      .addCase(fetchContacts.rejected, (state, action) => {
        state.loading = false
        state.contacts = []
        state.error = action.error.message || 'Failed to fetch contacts'
      })

    // Fetch contact by ID
    builder
      .addCase(fetchContactById.pending, (state) => {
        state.loading = true
        state.error = null
      })
      .addCase(fetchContactById.fulfilled, (state, action: PayloadAction<Contact>) => {
        state.loading = false
        state.selectedContact = action.payload
      })
      .addCase(fetchContactById.rejected, (state, action) => {
        state.loading = false
        state.error = action.error.message || 'Failed to fetch contact'
      })

    // Fetch contacts by company
    builder
      .addCase(fetchContactsByCompany.pending, (state) => {
        state.loading = true
        state.error = null
      })
      .addCase(fetchContactsByCompany.fulfilled, (state, action: PayloadAction<Contact[]>) => {
        state.loading = false
        state.contacts = action.payload || []
        state.totalCount = action.payload?.length || 0
      })
      .addCase(fetchContactsByCompany.rejected, (state, action) => {
        state.loading = false
        state.contacts = []
        state.error = action.error.message || 'Failed to fetch contacts by company'
      })

    // Create contact
    builder
      .addCase(createContact.pending, (state) => {
        state.loading = true
        state.error = null
      })
      .addCase(createContact.fulfilled, (state, action: PayloadAction<Contact>) => {
        state.loading = false
        state.contacts.unshift(action.payload)
        state.totalCount += 1
      })
      .addCase(createContact.rejected, (state, action) => {
        state.loading = false
        state.error = action.error.message || 'Failed to create contact'
      })

    // Update contact
    builder
      .addCase(updateContact.pending, (state) => {
        state.loading = true
        state.error = null
      })
      .addCase(updateContact.fulfilled, (state, action: PayloadAction<Contact>) => {
        state.loading = false
        const index = state.contacts.findIndex((c) => c.id === action.payload.id)
        if (index !== -1) {
          state.contacts[index] = action.payload
        }
        if (state.selectedContact?.id === action.payload.id) {
          state.selectedContact = action.payload
        }
      })
      .addCase(updateContact.rejected, (state, action) => {
        state.loading = false
        state.error = action.error.message || 'Failed to update contact'
      })

    // Delete contact
    builder
      .addCase(deleteContact.pending, (state) => {
        state.loading = true
        state.error = null
      })
      .addCase(deleteContact.fulfilled, (state, action: PayloadAction<string>) => {
        state.loading = false
        state.contacts = state.contacts.filter((c) => c.id !== action.payload)
        state.totalCount -= 1
        if (state.selectedContact?.id === action.payload) {
          state.selectedContact = null
        }
      })
      .addCase(deleteContact.rejected, (state, action) => {
        state.loading = false
        state.error = action.error.message || 'Failed to delete contact'
      })
  },
})

export const { clearSelectedContact, clearError } = contactSlice.actions
export default contactSlice.reducer
