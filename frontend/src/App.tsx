import { Routes, Route, Navigate } from 'react-router-dom'
import Dashboard from './pages/Dashboard'
import DelayReasons from './pages/DelayReasons'
import Reactors from './pages/Reactors'
import Products from './pages/Products'
import PktTransactions from './pages/PktTransactions'
import Login from './pages/Login'
import Layout from './components/Layout'

// Simple auth check
const PrivateRoute = ({ children }: { children: React.ReactNode }) => {
  const token = localStorage.getItem('token')
  return token ? <>{children}</> : <Navigate to="/login" replace />
}

function App() {
  return (
    <Routes>
      <Route path="/login" element={<Login />} />
      <Route path="/" element={<PrivateRoute><Layout /></PrivateRoute>}>
        <Route index element={<Dashboard />} />
        <Route path="delayreasons" element={<DelayReasons />} />
        <Route path="reactors" element={<Reactors />} />
        <Route path="products" element={<Products />} />
        <Route path="transactions" element={<PktTransactions />} />
      </Route>
    </Routes>
  )
}

export default App

