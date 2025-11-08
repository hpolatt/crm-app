import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Box,
  Paper,
  Typography,
  TextField,
  Button,
  Grid,
  CircularProgress,
  Alert,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
} from '@mui/material';
import { Save as SaveIcon, Cancel as CancelIcon } from '@mui/icons-material';
import { CreateOpportunityDto, UpdateOpportunityDto } from '../../types/opportunity';
import { opportunityService } from '../../services/opportunityService';
import { companyService } from '../../services/companyService';
import { contactService } from '../../services/contactService';
import { dealStageService } from '../../services/dealStageService';
import userService from '../../services/userService';
import { Company } from '../../types/company';
import { Contact } from '../../types/contact';
import { DealStage } from '../../types/dealStage';

interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
}

const OpportunityForm: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const isEditMode = Boolean(id);

  const [formData, setFormData] = useState<CreateOpportunityDto>({
    title: '',
    description: '',
    value: 0,
    probability: 50,
    expectedCloseDate: '',
    dealStageId: '',
    contactId: '',
    companyId: '',
    assignedUserId: '',
    notes: '',
  });

  const [loading, setLoading] = useState(false);
  const [fetchLoading, setFetchLoading] = useState(isEditMode);
  const [error, setError] = useState<string | null>(null);
  const [errors, setErrors] = useState<Record<string, string>>({});

  const [companies, setCompanies] = useState<Company[]>([]);
  const [contacts, setContacts] = useState<Contact[]>([]);
  const [dealStages, setDealStages] = useState<DealStage[]>([]);
  const [users, setUsers] = useState<User[]>([]);

  useEffect(() => {
    loadDropdownData();
    if (id) {
      fetchOpportunity(id);
    }
  }, [id]);

  const loadDropdownData = async () => {
    try {
      const [companiesData, contactsData, dealStagesData, usersData] = await Promise.all([
        companyService.getCompanies(1, 1000),
        contactService.getContacts(1, 1000),
        dealStageService.getAll(),
        userService.getBasicList(), // Basit user listesi - tüm authenticated kullanıcılar erişebilir
      ]);

      setCompanies(companiesData.items);
      setContacts(contactsData.items);
      setDealStages(dealStagesData);
      setUsers(usersData);
    } catch (err) {
      console.error('Error loading dropdown data:', err);
    }
  };

  const fetchOpportunity = async (opportunityId: string) => {
    try {
      setFetchLoading(true);
      const data = await opportunityService.getById(opportunityId);
      setFormData({
        title: data.title,
        description: data.description || '',
        value: data.value,
        probability: data.probability ?? 50,
        expectedCloseDate: data.expectedCloseDate
          ? new Date(data.expectedCloseDate).toISOString().split('T')[0]
          : '',
        dealStageId: data.dealStageId ?? '',
        contactId: data.contactId || '',
        companyId: data.companyId || '',
        assignedUserId: data.assignedUserId || '',
        notes: data.notes || '',
      });
    } catch (err: any) {
      setError(err.response?.data?.message || 'Fırsat yüklenirken bir hata oluştu');
    } finally {
      setFetchLoading(false);
    }
  };

  const validateForm = (): boolean => {
    const newErrors: Record<string, string> = {};

    if (!formData.title.trim()) {
      newErrors.title = 'Başlık gereklidir';
    }

    if (formData.value < 0) {
      newErrors.value = 'Değer 0\'dan küçük olamaz';
    }

    if ((formData.probability ?? 0) < 0 || (formData.probability ?? 0) > 100) {
      newErrors.probability = 'Olasılık 0-100 arasında olmalıdır';
    }

    if (!formData.dealStageId || formData.dealStageId.trim() === '') {
      newErrors.dealStageId = 'Aşama seçilmelidir';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!validateForm()) {
      return;
    }

    try {
      setLoading(true);
      setError(null);

      const submitData: CreateOpportunityDto | UpdateOpportunityDto = {
        title: formData.title,
        description: formData.description || undefined,
        value: formData.value,
        probability: formData.probability,
        expectedCloseDate: formData.expectedCloseDate || undefined,
        dealStageId: formData.dealStageId,
        contactId: formData.contactId || undefined,
        companyId: formData.companyId || undefined,
        assignedUserId: formData.assignedUserId || undefined,
        notes: formData.notes || undefined,
      };

      if (isEditMode && id) {
        await opportunityService.update(id, submitData as UpdateOpportunityDto);
      } else {
        await opportunityService.create(submitData as CreateOpportunityDto);
      }

      navigate('/opportunities');
    } catch (err: any) {
      setError(err.response?.data?.message || 'Kaydetme sırasında bir hata oluştu');
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (field: keyof CreateOpportunityDto) => (
    e: React.ChangeEvent<HTMLInputElement>
  ) => {
    const value = e.target.value;
    setFormData((prev) => ({
      ...prev,
      [field]:
        field === 'value' || field === 'probability'
          ? value ? parseFloat(value) : 0
          : value,
    }));

    // Clear error for this field
    if (errors[field]) {
      setErrors((prev) => {
        const newErrors = { ...prev };
        delete newErrors[field];
        return newErrors;
      });
    }
  };

  if (fetchLoading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="400px">
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box>
      <Typography variant="h5" gutterBottom>
        {isEditMode ? 'Fırsat Düzenle' : 'Yeni Fırsat'}
      </Typography>

      {error && (
        <Alert severity="error" onClose={() => setError(null)} sx={{ mb: 2 }}>
          {error}
        </Alert>
      )}

      <Paper sx={{ p: 3 }}>
        <form onSubmit={handleSubmit}>
          <Grid container spacing={3}>
            <Grid item xs={12}>
              <TextField
                label="Başlık"
                fullWidth
                required
                value={formData.title}
                onChange={handleChange('title')}
                error={Boolean(errors.title)}
                helperText={errors.title}
              />
            </Grid>

            <Grid item xs={12}>
              <TextField
                label="Açıklama"
                fullWidth
                multiline
                rows={4}
                value={formData.description}
                onChange={handleChange('description')}
              />
            </Grid>

            <Grid item xs={12}>
              <TextField
                label="Notlar"
                fullWidth
                multiline
                rows={3}
                value={formData.notes}
                onChange={handleChange('notes')}
                helperText="İç notlar (müşteri görmez)"
              />
            </Grid>

            <Grid item xs={12} sm={6}>
              <TextField
                label="Değer (TRY)"
                type="number"
                fullWidth
                required
                value={formData.value}
                onChange={handleChange('value')}
                error={Boolean(errors.value)}
                helperText={errors.value}
                InputProps={{
                  inputProps: { min: 0, step: 0.01 },
                }}
              />
            </Grid>

            <Grid item xs={12} sm={6}>
              <TextField
                label="Başarı Olasılığı (%)"
                type="number"
                fullWidth
                required
                value={formData.probability}
                onChange={handleChange('probability')}
                error={Boolean(errors.probability)}
                helperText={errors.probability}
                InputProps={{
                  inputProps: { min: 0, max: 100, step: 1 },
                }}
              />
            </Grid>

            <Grid item xs={12} sm={6}>
              <TextField
                label="Tahmini Kapanış Tarihi"
                type="date"
                fullWidth
                value={formData.expectedCloseDate}
                onChange={handleChange('expectedCloseDate')}
                InputLabelProps={{
                  shrink: true,
                }}
              />
            </Grid>

            <Grid item xs={12} sm={6}>
              <FormControl fullWidth required error={Boolean(errors.dealStageId)}>
                <InputLabel>Aşama</InputLabel>
                <Select
                  value={formData.dealStageId}
                  label="Aşama"
                  onChange={(e) => setFormData({ ...formData, dealStageId: e.target.value })}
                >
                  <MenuItem value="">
                    <em>Seçiniz</em>
                  </MenuItem>
                  {dealStages
                    .filter(stage => stage.isActive)
                    .sort((a, b) => a.order - b.order)
                    .map((stage) => (
                      <MenuItem key={stage.id} value={stage.id}>
                        {stage.name}
                      </MenuItem>
                    ))}
                </Select>
                {errors.dealStageId && (
                  <Typography variant="caption" color="error" sx={{ mt: 0.5, ml: 1.5 }}>
                    {errors.dealStageId}
                  </Typography>
                )}
              </FormControl>
            </Grid>

            <Grid item xs={12} sm={6}>
              <FormControl fullWidth>
                <InputLabel>Şirket</InputLabel>
                <Select
                  value={formData.companyId}
                  label="Şirket"
                  onChange={(e) => setFormData({ ...formData, companyId: e.target.value })}
                >
                  <MenuItem value="">
                    <em>Seçiniz</em>
                  </MenuItem>
                  {companies
                    .filter(company => company.isActive)
                    .map((company) => (
                      <MenuItem key={company.id} value={company.id}>
                        {company.name}
                      </MenuItem>
                    ))}
                </Select>
              </FormControl>
            </Grid>

            <Grid item xs={12} sm={6}>
              <FormControl fullWidth>
                <InputLabel>İletişim</InputLabel>
                <Select
                  value={formData.contactId}
                  label="İletişim"
                  onChange={(e) => setFormData({ ...formData, contactId: e.target.value })}
                >
                  <MenuItem value="">
                    <em>Seçiniz</em>
                  </MenuItem>
                  {contacts
                    .filter(contact => contact.isActive)
                    .map((contact) => (
                      <MenuItem key={contact.id} value={contact.id}>
                        {contact.firstName} {contact.lastName} {contact.companyName ? `(${contact.companyName})` : ''}
                      </MenuItem>
                    ))}
                </Select>
              </FormControl>
            </Grid>

            <Grid item xs={12} sm={6}>
              <FormControl fullWidth>
                <InputLabel>Atanan Kullanıcı</InputLabel>
                <Select
                  value={formData.assignedUserId}
                  label="Atanan Kullanıcı"
                  onChange={(e) => setFormData({ ...formData, assignedUserId: e.target.value })}
                  disabled={users.length === 0}
                >
                  <MenuItem value="">
                    <em>Seçiniz</em>
                  </MenuItem>
                  {users.map((user) => (
                    <MenuItem key={user.id} value={user.id}>
                      {user.firstName} {user.lastName} ({user.email})
                    </MenuItem>
                  ))}
                </Select>
                {users.length === 0 && (
                  <Typography variant="caption" color="text.secondary" sx={{ mt: 0.5, ml: 1.5 }}>
                    Kullanıcı listesi yüklenemedi
                  </Typography>
                )}
              </FormControl>
            </Grid>

            <Grid item xs={12}>
              <Box display="flex" gap={2} justifyContent="flex-end">
                <Button
                  variant="outlined"
                  startIcon={<CancelIcon />}
                  onClick={() => navigate('/opportunities')}
                  disabled={loading}
                >
                  İptal
                </Button>
                <Button
                  type="submit"
                  variant="contained"
                  startIcon={<SaveIcon />}
                  disabled={loading}
                >
                  {loading ? <CircularProgress size={24} /> : 'Kaydet'}
                </Button>
              </Box>
            </Grid>
          </Grid>
        </form>
      </Paper>
    </Box>
  );
};

export default OpportunityForm;
