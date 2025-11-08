import { Routes, Route, Navigate } from 'react-router-dom'
import { useSelector } from 'react-redux'
import { RootState } from './store'
import Login from './pages/Login'
import Dashboard from './pages/Dashboard'
import Companies from './pages/Companies'
import CompanyDetails from './pages/Companies/CompanyDetails'
import CompanyForm from './pages/Companies/CompanyForm'
import ContactList from './pages/Contacts/ContactList'
import ContactDetails from './pages/Contacts/ContactDetails'
import ContactForm from './pages/Contacts/ContactForm'
import Leads from './pages/Leads'
import LeadDetails from './pages/Leads/LeadDetails'
import LeadForm from './pages/Leads/LeadForm'
import Opportunities from './pages/Opportunities'
import OpportunityDetails from './pages/Opportunities/OpportunityDetails'
import OpportunityForm from './pages/Opportunities/OpportunityForm'
import Activities from './pages/Activities'
import Notes from './pages/Notes'
import Reports from './pages/Reports'
import Settings from './pages/Settings'
import ActivityLogs from './pages/ActivityLogs'
import ActivityLogDetail from './pages/ActivityLogDetail'
import DealStageList from './pages/DealStages/DealStageList'
import DealStageForm from './pages/DealStages/DealStageForm'
import Layout from './components/Layout'

function PrivateRoute({ children }: { children: React.ReactNode }) {
  const { token, user } = useSelector((state: RootState) => state.auth)
  console.log('PrivateRoute check:', { 
    hasToken: !!token, 
    hasUser: !!user,
    token: token?.substring(0, 20) + '...',
    user 
  })
  
  if (!token) {
    console.log('PrivateRoute: No token, redirecting to /login')
    return <Navigate to="/login" />
  }
  
  console.log('PrivateRoute: Authorized, rendering children')
  return <>{children}</>
}

function App() {
  return (
    <Routes>
      <Route path="/login" element={<Login />} />
      <Route
        path="/"
        element={
          <PrivateRoute>
            <Layout />
          </PrivateRoute>
        }
      >
        <Route index element={<Dashboard />} />
        <Route path="companies" element={<Companies />} />
        <Route path="companies/new" element={<CompanyForm />} />
        <Route path="companies/:id" element={<CompanyDetails />} />
        <Route path="companies/:id/edit" element={<CompanyForm />} />
        <Route path="contacts" element={<ContactList />} />
        <Route path="contacts/new" element={<ContactForm />} />
        <Route path="contacts/:id" element={<ContactDetails />} />
        <Route path="contacts/:id/edit" element={<ContactForm />} />
        <Route path="leads" element={<Leads />} />
        <Route path="leads/new" element={<LeadForm />} />
        <Route path="leads/:id" element={<LeadDetails />} />
        <Route path="leads/:id/edit" element={<LeadForm />} />
        <Route path="opportunities" element={<Opportunities />} />
        <Route path="opportunities/new" element={<OpportunityForm />} />
        <Route path="opportunities/:id" element={<OpportunityDetails />} />
        <Route path="opportunities/:id/edit" element={<OpportunityForm />} />
        <Route path="activities" element={<Activities />} />
        <Route path="notes" element={<Notes />} />
        <Route path="reports" element={<Reports />} />
        <Route path="settings" element={<Settings />} />
        <Route path="activity-logs" element={<ActivityLogs />} />
        <Route path="activity-logs/:requestId" element={<ActivityLogDetail />} />
        <Route path="deal-stages" element={<DealStageList />} />
        <Route path="deal-stages/new" element={<DealStageForm />} />
        <Route path="deal-stages/:id" element={<DealStageForm />} />
      </Route>
    </Routes>
  )
}

export default App
