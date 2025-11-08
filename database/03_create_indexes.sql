-- =====================================================
-- CRM Database Indexes Creation Script
-- =====================================================
-- This script creates optimized indexes for performance
-- =====================================================

SET search_path TO crm, auth, audit, public;

-- =====================================================
-- AUTH SCHEMA INDEXES
-- =====================================================

-- Users indexes
CREATE INDEX IF NOT EXISTS idx_users_email ON auth.users(email) WHERE is_deleted = false;
CREATE INDEX IF NOT EXISTS idx_users_is_active ON auth.users(is_active) WHERE is_deleted = false;
CREATE INDEX IF NOT EXISTS idx_users_created_at ON auth.users(created_at DESC);
CREATE INDEX IF NOT EXISTS idx_users_full_name ON auth.users(first_name, last_name);

-- Roles indexes
CREATE INDEX IF NOT EXISTS idx_roles_name ON auth.roles(name) WHERE is_deleted = false;
CREATE INDEX IF NOT EXISTS idx_roles_is_active ON auth.roles(is_active) WHERE is_deleted = false;

-- User Roles indexes
CREATE INDEX IF NOT EXISTS idx_user_roles_user_id ON auth.user_roles(user_id);
CREATE INDEX IF NOT EXISTS idx_user_roles_role_id ON auth.user_roles(role_id);

-- Refresh Tokens indexes
CREATE INDEX IF NOT EXISTS idx_refresh_tokens_user_id ON auth.refresh_tokens(user_id);
CREATE INDEX IF NOT EXISTS idx_refresh_tokens_token ON auth.refresh_tokens(token) WHERE is_revoked = false;
CREATE INDEX IF NOT EXISTS idx_refresh_tokens_expires_at ON auth.refresh_tokens(expires_at) WHERE is_revoked = false;

-- =====================================================
-- CRM SCHEMA INDEXES
-- =====================================================

-- Companies indexes
CREATE INDEX IF NOT EXISTS idx_companies_name ON crm.companies(name) WHERE is_deleted = false;
CREATE INDEX IF NOT EXISTS idx_companies_name_trgm ON crm.companies USING gin(name gin_trgm_ops);
CREATE INDEX IF NOT EXISTS idx_companies_industry ON crm.companies(industry) WHERE is_deleted = false;
CREATE INDEX IF NOT EXISTS idx_companies_city ON crm.companies(city) WHERE is_deleted = false;
CREATE INDEX IF NOT EXISTS idx_companies_country ON crm.companies(country) WHERE is_deleted = false;
CREATE INDEX IF NOT EXISTS idx_companies_source ON crm.companies(source) WHERE is_deleted = false;
CREATE INDEX IF NOT EXISTS idx_companies_is_active ON crm.companies(is_active) WHERE is_deleted = false;
CREATE INDEX IF NOT EXISTS idx_companies_created_at ON crm.companies(created_at DESC);
CREATE INDEX IF NOT EXISTS idx_companies_created_by ON crm.companies(created_by);
CREATE INDEX IF NOT EXISTS idx_companies_annual_revenue ON crm.companies(annual_revenue DESC) WHERE is_deleted = false;

-- Contacts indexes
CREATE INDEX IF NOT EXISTS idx_contacts_company_id ON crm.contacts(company_id) WHERE is_deleted = false;
CREATE INDEX IF NOT EXISTS idx_contacts_email ON crm.contacts(email) WHERE is_deleted = false;
CREATE INDEX IF NOT EXISTS idx_contacts_full_name ON crm.contacts(first_name, last_name) WHERE is_deleted = false;
CREATE INDEX IF NOT EXISTS idx_contacts_name_trgm ON crm.contacts USING gin((first_name || ' ' || last_name) gin_trgm_ops);
CREATE INDEX IF NOT EXISTS idx_contacts_is_primary ON crm.contacts(is_primary) WHERE is_deleted = false AND is_primary = true;
CREATE INDEX IF NOT EXISTS idx_contacts_is_active ON crm.contacts(is_active) WHERE is_deleted = false;
CREATE INDEX IF NOT EXISTS idx_contacts_created_at ON crm.contacts(created_at DESC);
CREATE INDEX IF NOT EXISTS idx_contacts_created_by ON crm.contacts(created_by);

-- Deal Stages indexes
CREATE INDEX IF NOT EXISTS idx_deal_stages_order ON crm.deal_stages(order_position) WHERE is_deleted = false;
CREATE INDEX IF NOT EXISTS idx_deal_stages_is_default ON crm.deal_stages(is_default) WHERE is_deleted = false AND is_default = true;
CREATE INDEX IF NOT EXISTS idx_deal_stages_is_active ON crm.deal_stages(is_active) WHERE is_deleted = false;

-- Leads indexes
CREATE INDEX IF NOT EXISTS idx_leads_company_id ON crm.leads(company_id) WHERE is_deleted = false;
CREATE INDEX IF NOT EXISTS idx_leads_contact_id ON crm.leads(contact_id) WHERE is_deleted = false;
CREATE INDEX IF NOT EXISTS idx_leads_status ON crm.leads(status) WHERE is_deleted = false;
CREATE INDEX IF NOT EXISTS idx_leads_assigned_to ON crm.leads(assigned_to) WHERE is_deleted = false;
CREATE INDEX IF NOT EXISTS idx_leads_expected_close_date ON crm.leads(expected_close_date) WHERE is_deleted = false;
CREATE INDEX IF NOT EXISTS idx_leads_value ON crm.leads(value DESC) WHERE is_deleted = false;
CREATE INDEX IF NOT EXISTS idx_leads_created_at ON crm.leads(created_at DESC);
CREATE INDEX IF NOT EXISTS idx_leads_created_by ON crm.leads(created_by);
CREATE INDEX IF NOT EXISTS idx_leads_title_trgm ON crm.leads USING gin(title gin_trgm_ops);

-- Opportunities indexes
CREATE INDEX IF NOT EXISTS idx_opportunities_lead_id ON crm.opportunities(lead_id) WHERE is_deleted = false;
CREATE INDEX IF NOT EXISTS idx_opportunities_company_id ON crm.opportunities(company_id) WHERE is_deleted = false;
CREATE INDEX IF NOT EXISTS idx_opportunities_contact_id ON crm.opportunities(contact_id) WHERE is_deleted = false;
CREATE INDEX IF NOT EXISTS idx_opportunities_stage ON crm.opportunities(stage) WHERE is_deleted = false;
CREATE INDEX IF NOT EXISTS idx_opportunities_assigned_to ON crm.opportunities(assigned_to) WHERE is_deleted = false;
CREATE INDEX IF NOT EXISTS idx_opportunities_expected_close_date ON crm.opportunities(expected_close_date) WHERE is_deleted = false;
CREATE INDEX IF NOT EXISTS idx_opportunities_actual_close_date ON crm.opportunities(actual_close_date) WHERE is_deleted = false;
CREATE INDEX IF NOT EXISTS idx_opportunities_value ON crm.opportunities(value DESC) WHERE is_deleted = false;
CREATE INDEX IF NOT EXISTS idx_opportunities_created_at ON crm.opportunities(created_at DESC);
CREATE INDEX IF NOT EXISTS idx_opportunities_created_by ON crm.opportunities(created_by);
CREATE INDEX IF NOT EXISTS idx_opportunities_title_trgm ON crm.opportunities USING gin(title gin_trgm_ops);

-- Activities indexes
CREATE INDEX IF NOT EXISTS idx_activities_type ON crm.activities(type) WHERE is_deleted = false;
CREATE INDEX IF NOT EXISTS idx_activities_status ON crm.activities(status) WHERE is_deleted = false;
CREATE INDEX IF NOT EXISTS idx_activities_priority ON crm.activities(priority) WHERE is_deleted = false;
CREATE INDEX IF NOT EXISTS idx_activities_company_id ON crm.activities(company_id) WHERE is_deleted = false;
CREATE INDEX IF NOT EXISTS idx_activities_contact_id ON crm.activities(contact_id) WHERE is_deleted = false;
CREATE INDEX IF NOT EXISTS idx_activities_lead_id ON crm.activities(lead_id) WHERE is_deleted = false;
CREATE INDEX IF NOT EXISTS idx_activities_opportunity_id ON crm.activities(opportunity_id) WHERE is_deleted = false;
CREATE INDEX IF NOT EXISTS idx_activities_assigned_to ON crm.activities(assigned_to) WHERE is_deleted = false;
CREATE INDEX IF NOT EXISTS idx_activities_due_date ON crm.activities(due_date) WHERE is_deleted = false;
CREATE INDEX IF NOT EXISTS idx_activities_completed_date ON crm.activities(completed_date) WHERE is_deleted = false;
CREATE INDEX IF NOT EXISTS idx_activities_created_at ON crm.activities(created_at DESC);
CREATE INDEX IF NOT EXISTS idx_activities_created_by ON crm.activities(created_by);

-- Notes indexes
CREATE INDEX IF NOT EXISTS idx_notes_company_id ON crm.notes(company_id) WHERE is_deleted = false;
CREATE INDEX IF NOT EXISTS idx_notes_contact_id ON crm.notes(contact_id) WHERE is_deleted = false;
CREATE INDEX IF NOT EXISTS idx_notes_lead_id ON crm.notes(lead_id) WHERE is_deleted = false;
CREATE INDEX IF NOT EXISTS idx_notes_opportunity_id ON crm.notes(opportunity_id) WHERE is_deleted = false;
CREATE INDEX IF NOT EXISTS idx_notes_is_pinned ON crm.notes(is_pinned) WHERE is_deleted = false AND is_pinned = true;
CREATE INDEX IF NOT EXISTS idx_notes_created_at ON crm.notes(created_at DESC);
CREATE INDEX IF NOT EXISTS idx_notes_created_by ON crm.notes(created_by);
CREATE INDEX IF NOT EXISTS idx_notes_content_trgm ON crm.notes USING gin(content gin_trgm_ops);

-- Note: activity_logs indexes removed - Activity logging handled by Elasticsearch

-- System Settings indexes
CREATE INDEX IF NOT EXISTS idx_system_settings_key ON crm.system_settings(key) WHERE is_deleted = false;
CREATE INDEX IF NOT EXISTS idx_system_settings_category ON crm.system_settings(category) WHERE is_deleted = false;
CREATE INDEX IF NOT EXISTS idx_system_settings_is_public ON crm.system_settings(is_public) WHERE is_deleted = false;

-- =====================================================
-- COMPOSITE INDEXES FOR COMMON QUERIES
-- =====================================================

-- User roles composite
CREATE INDEX IF NOT EXISTS idx_user_roles_composite ON auth.user_roles(user_id, role_id);

-- Contacts by company and primary
CREATE INDEX IF NOT EXISTS idx_contacts_company_primary ON crm.contacts(company_id, is_primary) WHERE is_deleted = false;

-- Leads by assigned user and status
CREATE INDEX IF NOT EXISTS idx_leads_assigned_status ON crm.leads(assigned_to, status) WHERE is_deleted = false;

-- Opportunities by assigned user and stage
CREATE INDEX IF NOT EXISTS idx_opportunities_assigned_stage ON crm.opportunities(assigned_to, stage) WHERE is_deleted = false;

-- Activities by assigned user and status
CREATE INDEX IF NOT EXISTS idx_activities_assigned_status ON crm.activities(assigned_to, status) WHERE is_deleted = false;

-- Activities by due date and status
CREATE INDEX IF NOT EXISTS idx_activities_due_status ON crm.activities(due_date, status) WHERE is_deleted = false;
