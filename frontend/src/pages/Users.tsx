import { useState, useEffect } from 'react';
import {
  Box,
  Paper,
  Typography,
  Button,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  IconButton,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Grid,
  MenuItem,
  Chip,
} from '@mui/material';
import { Add, Edit, Delete, LockReset } from '@mui/icons-material';
import { api } from '../services/api';

interface User {
  id: string;
  username: string;
  firstName: string;
  lastName: string;
  email: string;
  role: string;
  isActive: boolean;
  createdAt: string;
}

const roles = [
  { value: 'Admin', label: 'Admin' },
  { value: 'PowerUser', label: 'Power User' },
  { value: 'Foreman', label: 'Foreman' },
];

function Users() {
  const [users, setUsers] = useState<User[]>([]);
  const [open, setOpen] = useState(false);
  const [passwordDialogOpen, setPasswordDialogOpen] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [selectedUserId, setSelectedUserId] = useState<string | null>(null);
  const [formData, setFormData] = useState({
    username: '',
    password: '',
    firstName: '',
    lastName: '',
    email: '',
    role: 'Foreman',
    isActive: true,
  });
  const [passwordData, setPasswordData] = useState({
    currentPassword: '',
    newPassword: '',
    confirmPassword: '',
  });

  useEffect(() => {
    fetchUsers();
  }, []);

  const fetchUsers = async () => {
    try {
      const response = await api.get('/users');
      setUsers(response.data.data);
    } catch (error) {
      console.error('Error fetching users:', error);
    }
  };

  const handleOpen = (user?: User) => {
    if (user) {
      setEditingId(user.id);
      setFormData({
        username: user.username,
        password: '',
        firstName: user.firstName,
        lastName: user.lastName,
        email: user.email,
        role: user.role,
        isActive: user.isActive,
      });
    } else {
      setEditingId(null);
      setFormData({
        username: '',
        password: '',
        firstName: '',
        lastName: '',
        email: '',
        role: 'Foreman',
        isActive: true,
      });
    }
    setOpen(true);
  };

  const handleClose = () => {
    setOpen(false);
    setEditingId(null);
  };

  const handleSubmit = async () => {
    try {
      if (editingId) {
        // Update
        await api.put(`/users/${editingId}`, {
          firstName: formData.firstName,
          lastName: formData.lastName,
          email: formData.email,
          role: formData.role,
          isActive: formData.isActive,
        });
      } else {
        // Create
        await api.post('/users', formData);
      }
      handleClose();
      fetchUsers();
    } catch (error) {
      console.error('Error saving user:', error);
    }
  };

  const handleDelete = async (id: string) => {
    if (window.confirm('Bu kullanıcıyı silmek istediğinizden emin misiniz?')) {
      try {
        await api.delete(`/users/${id}`);
        fetchUsers();
      } catch (error) {
        console.error('Error deleting user:', error);
        alert('Son admin kullanıcısı silinemez!');
      }
    }
  };

  const handleOpenPasswordDialog = (userId: string) => {
    setSelectedUserId(userId);
    setPasswordData({
      currentPassword: '',
      newPassword: '',
      confirmPassword: '',
    });
    setPasswordDialogOpen(true);
  };

  const handleClosePasswordDialog = () => {
    setPasswordDialogOpen(false);
    setSelectedUserId(null);
  };

  const handleChangePassword = async () => {
    if (passwordData.newPassword !== passwordData.confirmPassword) {
      alert('Yeni şifreler eşleşmiyor!');
      return;
    }

    if (passwordData.newPassword.length < 6) {
      alert('Şifre en az 6 karakter olmalıdır!');
      return;
    }

    try {
      await api.post(`/users/${selectedUserId}/change-password`, {
        newPassword: passwordData.newPassword,
      });
      alert('Şifre başarıyla değiştirildi!');
      handleClosePasswordDialog();
    } catch (error: any) {
      console.error('Error changing password:', error);
      const message = error.response?.data?.message || 'Şifre değiştirme başarısız!';
      alert(message);
    }
  };

  const getRoleColor = (role: string) => {
    switch (role) {
      case 'Admin':
        return 'error';
      case 'PowerUser':
        return 'warning';
      default:
        return 'default';
    }
  };

  const getRoleLabel = (role: string) => {
    const roleObj = roles.find(r => r.value === role);
    return roleObj ? roleObj.label : role;
  };

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 3 }}>
        <Typography variant="h4">Kullanıcılar</Typography>
        <Button variant="contained" startIcon={<Add />} onClick={() => handleOpen()}>
          Yeni Kullanıcı
        </Button>
      </Box>

      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Kullanıcı Adı</TableCell>
              <TableCell>Ad Soyad</TableCell>
              <TableCell>Email</TableCell>
              <TableCell>Rol</TableCell>
              <TableCell>Durum</TableCell>
              <TableCell>Oluşturma</TableCell>
              <TableCell align="right">İşlemler</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {users.map((user) => (
              <TableRow key={user.id}>
                <TableCell>{user.username}</TableCell>
                <TableCell>{`${user.firstName} ${user.lastName}`}</TableCell>
                <TableCell>{user.email}</TableCell>
                <TableCell>
                  <Chip label={getRoleLabel(user.role)} color={getRoleColor(user.role)} size="small" />
                </TableCell>
                <TableCell>
                  <Chip 
                    label={user.isActive ? 'Aktif' : 'Pasif'} 
                    color={user.isActive ? 'success' : 'default'} 
                    size="small" 
                  />
                </TableCell>
                <TableCell>{new Date(user.createdAt).toLocaleDateString('tr-TR')}</TableCell>
                <TableCell align="right">
                  <IconButton onClick={() => handleOpen(user)} color="primary" title="Düzenle">
                    <Edit />
                  </IconButton>
                  <IconButton onClick={() => handleOpenPasswordDialog(user.id)} color="warning" title="Şifre Değiştir">
                    <LockReset />
                  </IconButton>
                  <IconButton onClick={() => handleDelete(user.id)} color="error" title="Sil">
                    <Delete />
                  </IconButton>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>

      {/* Create/Edit Dialog */}
      <Dialog open={open} onClose={handleClose} maxWidth="md" fullWidth>
        <DialogTitle>{editingId ? 'Kullanıcıyı Düzenle' : 'Yeni Kullanıcı'}</DialogTitle>
        <DialogContent>
          <Grid container spacing={2} sx={{ mt: 1 }}>
            {!editingId && (
              <>
                <Grid item xs={12} sm={6}>
                  <TextField
                    label="Kullanıcı Adı"
                    fullWidth
                    required
                    value={formData.username}
                    onChange={(e) => setFormData({ ...formData, username: e.target.value })}
                  />
                </Grid>
                <Grid item xs={12} sm={6}>
                  <TextField
                    label="Şifre"
                    type="password"
                    fullWidth
                    required
                    value={formData.password}
                    onChange={(e) => setFormData({ ...formData, password: e.target.value })}
                  />
                </Grid>
              </>
            )}
            
            <Grid item xs={12} sm={6}>
              <TextField
                label="Ad"
                fullWidth
                required
                value={formData.firstName}
                onChange={(e) => setFormData({ ...formData, firstName: e.target.value })}
              />
            </Grid>
            <Grid item xs={12} sm={6}>
              <TextField
                label="Soyad"
                fullWidth
                required
                value={formData.lastName}
                onChange={(e) => setFormData({ ...formData, lastName: e.target.value })}
              />
            </Grid>
            <Grid item xs={12} sm={6}>
              <TextField
                label="Email"
                type="email"
                fullWidth
                required
                value={formData.email}
                onChange={(e) => setFormData({ ...formData, email: e.target.value })}
              />
            </Grid>
            <Grid item xs={12} sm={6}>
              <TextField
                select
                label="Rol"
                fullWidth
                required
                value={formData.role}
                onChange={(e) => setFormData({ ...formData, role: e.target.value })}
              >
                {roles.map((role) => (
                  <MenuItem key={role.value} value={role.value}>
                    {role.label}
                  </MenuItem>
                ))}
              </TextField>
            </Grid>
            {editingId && (
              <Grid item xs={12} sm={6}>
                <TextField
                  select
                  label="Durum"
                  fullWidth
                  value={formData.isActive.toString()}
                  onChange={(e) => setFormData({ ...formData, isActive: e.target.value === 'true' })}
                >
                  <MenuItem value="true">Aktif</MenuItem>
                  <MenuItem value="false">Pasif</MenuItem>
                </TextField>
              </Grid>
            )}
          </Grid>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose}>İptal</Button>
          <Button onClick={handleSubmit} variant="contained">
            {editingId ? 'Güncelle' : 'Kaydet'}
          </Button>
        </DialogActions>
      </Dialog>

      {/* Change Password Dialog */}
      <Dialog open={passwordDialogOpen} onClose={handleClosePasswordDialog} maxWidth="sm" fullWidth>
        <DialogTitle>Şifre Değiştir</DialogTitle>
        <DialogContent>
          <Grid container spacing={2} sx={{ mt: 1 }}>
            <Grid item xs={12}>
              <TextField
                label="Yeni Şifre"
                type="password"
                fullWidth
                required
                value={passwordData.newPassword}
                onChange={(e) => setPasswordData({ ...passwordData, newPassword: e.target.value })}
                helperText="En az 6 karakter olmalıdır"
              />
            </Grid>
            <Grid item xs={12}>
              <TextField
                label="Yeni Şifre (Tekrar)"
                type="password"
                fullWidth
                required
                value={passwordData.confirmPassword}
                onChange={(e) => setPasswordData({ ...passwordData, confirmPassword: e.target.value })}
                error={passwordData.confirmPassword !== '' && passwordData.newPassword !== passwordData.confirmPassword}
                helperText={passwordData.confirmPassword !== '' && passwordData.newPassword !== passwordData.confirmPassword ? 'Şifreler eşleşmiyor' : ''}
              />
            </Grid>
          </Grid>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClosePasswordDialog}>İptal</Button>
          <Button 
            onClick={handleChangePassword} 
            variant="contained"
            disabled={!passwordData.newPassword || passwordData.newPassword !== passwordData.confirmPassword}
          >
            Şifreyi Değiştir
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}

export default Users;
