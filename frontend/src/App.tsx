import { Routes, Route } from 'react-router-dom'
import Dashboard from './pages/Dashboard'
import DelayReasons from './pages/DelayReasons'
import Reactors from './pages/Reactors'
import Products from './pages/Products'
import PktTransactions from './pages/PktTransactions'
import Layout from './components/Layout'

function App() {
  return (
    <Routes>
      <Route path="/" element={<Layout />}>
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
