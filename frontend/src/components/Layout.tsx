import { useState, useEffect } from 'react'
import { Outlet, useNavigate, useLocation } from 'react-router-dom'
import {
  AppBar,
  Box,
  Drawer,
  IconButton,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Toolbar,
  Typography,
  Button,
} from '@mui/material'
import MenuIcon from '@mui/icons-material/Menu'
import DashboardIcon from '@mui/icons-material/Dashboard'
import ScienceIcon from '@mui/icons-material/Science'
import InventoryIcon from '@mui/icons-material/Inventory'
import WarningIcon from '@mui/icons-material/Warning'
import AssignmentIcon from '@mui/icons-material/Assignment'
import PeopleIcon from '@mui/icons-material/People'
import LogoutIcon from '@mui/icons-material/Logout'

const drawerWidth = 240

const menuItems = [
  { text: 'Dashboard', icon: <DashboardIcon />, path: '/' },
  { text: 'Reaktörler', icon: <ScienceIcon />, path: '/reactors' },
  { text: 'Ürünler', icon: <InventoryIcon />, path: '/products' },
  { text: 'Gecikme Nedenleri', icon: <WarningIcon />, path: '/delayreasons' },
  { text: 'PKT İşlemleri', icon: <AssignmentIcon />, path: '/transactions' },
  { text: 'Kullanıcılar', icon: <PeopleIcon />, path: '/users', adminOnly: true },
]

function Layout() {
  const [mobileOpen, setMobileOpen] = useState(false)
  const navigate = useNavigate()
  const location = useLocation()

  // Foreman kullanıcısını her zaman transactions sayfasına yönlendir
  useEffect(() => {
    if (isForeman() && location.pathname !== '/transactions') {
      navigate('/transactions', { replace: true })
    }
  }, [location.pathname])

  const handleDrawerToggle = () => {
    setMobileOpen(!mobileOpen)
  }

  const handleLogout = () => {
    localStorage.removeItem('token')
    localStorage.removeItem('user')
    navigate('/login')
  }

  const getUserName = () => {
    const userStr = localStorage.getItem('user')
    if (userStr) {
      try {
        const user = JSON.parse(userStr)
        return `${user.firstName} ${user.lastName}`
      } catch {
        return 'Kullanıcı'
      }
    }
    return 'Kullanıcı'
  }

  const getUserRole = () => {
    const userStr = localStorage.getItem('user')
    if (userStr) {
      try {
        const user = JSON.parse(userStr)
        return user.role
      } catch {
        return null
      }
    }
    return null
  }

  const isAdmin = () => {
    return getUserRole() === 'Admin'
  }

  const isForeman = () => {
    return getUserRole() === 'Foreman'
  }

  const drawer = (
    <div>
      <Toolbar>
        <Typography variant="h6" noWrap component="div">
          PKT App
        </Typography>
      </Toolbar>
      <List>
        {menuItems
          .filter((item) => {
            // Admin sadece admin menülerini görebilir
            if ((item as any).adminOnly && !isAdmin()) return false
            // Foreman sadece Transactions görebilir
            if (isForeman() && item.path !== '/transactions') return false
            return true
          })
          .map((item) => (
            <ListItem key={item.text} disablePadding>
              <ListItemButton onClick={() => navigate(item.path)}>
                <ListItemIcon>{item.icon}</ListItemIcon>
                <ListItemText primary={item.text} />
              </ListItemButton>
            </ListItem>
          ))}
      </List>
    </div>
  )

  return (
    <Box sx={{ display: 'flex' }}>
      <AppBar
        position="fixed"
        sx={{
          width: isForeman() ? '100%' : { sm: `calc(100% - ${drawerWidth}px)` },
          ml: isForeman() ? 0 : { sm: `${drawerWidth}px` },
        }}
      >
        <Toolbar>
          {!isForeman() && (
            <IconButton
              color="inherit"
              edge="start"
              onClick={handleDrawerToggle}
              sx={{ mr: 2, display: { sm: 'none' } }}
            >
              <MenuIcon />
            </IconButton>
          )}
          <Typography variant="h6" noWrap component="div" sx={{ flexGrow: 1 }}>
            PKT Uygulaması
          </Typography>
          <Typography variant="body2" sx={{ mr: 2 }}>
            {getUserName()}
          </Typography>
          <Button 
            color="inherit" 
            startIcon={<LogoutIcon />}
            onClick={handleLogout}
          >
            Çıkış
          </Button>
        </Toolbar>
      </AppBar>
      {!isForeman() && (
        <Box
          component="nav"
          sx={{ width: { sm: drawerWidth }, flexShrink: { sm: 0 } }}
        >
          <Drawer
            variant="temporary"
            open={mobileOpen}
            onClose={handleDrawerToggle}
            ModalProps={{
              keepMounted: true,
            }}
            sx={{
              display: { xs: 'block', sm: 'none' },
              '& .MuiDrawer-paper': { boxSizing: 'border-box', width: drawerWidth },
            }}
          >
            {drawer}
          </Drawer>
          <Drawer
            variant="permanent"
            sx={{
              display: { xs: 'none', sm: 'block' },
              '& .MuiDrawer-paper': { boxSizing: 'border-box', width: drawerWidth },
            }}
            open
          >
            {drawer}
          </Drawer>
        </Box>
      )}
      <Box
        component="main"
        sx={{
          flexGrow: 1,
          p: 3,
          width: isForeman() ? '100%' : { sm: `calc(100% - ${drawerWidth}px)` },
        }}
      >
        <Toolbar />
        <Outlet />
      </Box>
    </Box>
  )
}

export default Layout
