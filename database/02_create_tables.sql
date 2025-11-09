-- =====================================================
-- CRM Database Tables Creation Script
-- =====================================================
-- This script creates all tables for the CRM application
-- Follows the entity models defined in CrmApp.Domain
-- =====================================================

SET search_path TO crm, auth, audit, public;

-- =====================================================
-- AUTH SCHEMA TABLES
-- =====================================================

-- Users table
CREATE TABLE IF NOT EXISTS auth.users (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    email VARCHAR(255) NOT NULL,
    password_hash VARCHAR(500) NOT NULL,
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    phone VARCHAR(50),
    is_active BOOLEAN NOT NULL DEFAULT true,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP,
    created_by UUID,
    updated_by UUID,
    is_deleted BOOLEAN NOT NULL DEFAULT false,
    
    -- Constraints
    CONSTRAINT uk_users_email UNIQUE (email),
    CONSTRAINT chk_users_email_format CHECK (email ~* '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$')
);

-- Roles table
CREATE TABLE IF NOT EXISTS auth.roles (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name VARCHAR(255) NOT NULL,
    description TEXT,
    is_active BOOLEAN NOT NULL DEFAULT true,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP,
    created_by UUID,
    updated_by UUID,
    is_deleted BOOLEAN NOT NULL DEFAULT false,
    
    -- Constraints
    CONSTRAINT uk_roles_name UNIQUE (name)
);

-- User Roles junction table
CREATE TABLE IF NOT EXISTS auth.user_roles (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID NOT NULL,
    role_id UUID NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    -- Constraints
    CONSTRAINT fk_user_roles_user FOREIGN KEY (user_id) REFERENCES auth.users(id) ON DELETE CASCADE,
    CONSTRAINT fk_user_roles_role FOREIGN KEY (role_id) REFERENCES auth.roles(id) ON DELETE CASCADE,
    CONSTRAINT uk_user_roles UNIQUE (user_id, role_id)
);

-- Refresh Tokens table
CREATE TABLE IF NOT EXISTS auth.refresh_tokens (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID NOT NULL,
    token VARCHAR(500) NOT NULL,
    expires_at TIMESTAMP NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    is_revoked BOOLEAN NOT NULL DEFAULT false,
    
    -- Constraints
    CONSTRAINT fk_refresh_tokens_user FOREIGN KEY (user_id) REFERENCES auth.users(id) ON DELETE CASCADE,
    CONSTRAINT uk_refresh_tokens_token UNIQUE (token)
);

-- =====================================================
-- CRM SCHEMA TABLES
-- =====================================================

-- Companies table
CREATE TABLE IF NOT EXISTS crm.companies (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name VARCHAR(255) NOT NULL,
    industry VARCHAR(100),
    website VARCHAR(255),
    phone VARCHAR(50),
    email VARCHAR(255),
    address TEXT,
    city VARCHAR(100),
    country VARCHAR(100),
    postal_code VARCHAR(20),
    source VARCHAR(100),
    employee_count INTEGER,
    annual_revenue DECIMAL(18, 2),
    notes TEXT,
    is_active BOOLEAN NOT NULL DEFAULT true,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP,
    created_by UUID,
    updated_by UUID,
    is_deleted BOOLEAN NOT NULL DEFAULT false,
    
    -- Constraints
    CONSTRAINT fk_companies_created_by FOREIGN KEY (created_by) REFERENCES auth.users(id) ON DELETE SET NULL,
    CONSTRAINT fk_companies_updated_by FOREIGN KEY (updated_by) REFERENCES auth.users(id) ON DELETE SET NULL,
    CONSTRAINT chk_companies_employee_count CHECK (employee_count >= 0),
    CONSTRAINT chk_companies_annual_revenue CHECK (annual_revenue >= 0),
    CONSTRAINT chk_companies_email_format CHECK (email IS NULL OR email ~* '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$')
);

-- Contacts table
CREATE TABLE IF NOT EXISTS crm.contacts (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    company_id UUID,
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    email VARCHAR(255),
    phone VARCHAR(50),
    mobile VARCHAR(50),
    position VARCHAR(100),
    department VARCHAR(100),
    address TEXT,
    city VARCHAR(100),
    country VARCHAR(100),
    postal_code VARCHAR(20),
    birth_date DATE,
    notes TEXT,
    is_primary BOOLEAN NOT NULL DEFAULT false,
    is_active BOOLEAN NOT NULL DEFAULT true,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP,
    created_by UUID,
    updated_by UUID,
    is_deleted BOOLEAN NOT NULL DEFAULT false,
    
    -- Constraints
    CONSTRAINT fk_contacts_company FOREIGN KEY (company_id) REFERENCES crm.companies(id) ON DELETE SET NULL,
    CONSTRAINT fk_contacts_created_by FOREIGN KEY (created_by) REFERENCES auth.users(id) ON DELETE SET NULL,
    CONSTRAINT fk_contacts_updated_by FOREIGN KEY (updated_by) REFERENCES auth.users(id) ON DELETE SET NULL,
    CONSTRAINT chk_contacts_email_format CHECK (email IS NULL OR email ~* '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$'),
    CONSTRAINT chk_contacts_birth_date CHECK (birth_date IS NULL OR birth_date <= CURRENT_DATE)
);

-- Deal Stages table
CREATE TABLE IF NOT EXISTS crm.deal_stages (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name VARCHAR(100) NOT NULL,
    order INTEGER NOT NULL,
    description TEXT,
    color VARCHAR(50),
    is_default BOOLEAN NOT NULL DEFAULT false,
    is_active BOOLEAN NOT NULL DEFAULT true,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP,
    created_by UUID,
    updated_by UUID,
    is_deleted BOOLEAN NOT NULL DEFAULT false,
    
    -- Constraints
    CONSTRAINT uk_deal_stages_name UNIQUE (name),
    CONSTRAINT fk_deal_stages_created_by FOREIGN KEY (created_by) REFERENCES auth.users(id) ON DELETE SET NULL,
    CONSTRAINT fk_deal_stages_updated_by FOREIGN KEY (updated_by) REFERENCES auth.users(id) ON DELETE SET NULL,
    CONSTRAINT chk_deal_stages_order CHECK ("order" >= 0)
);

-- Leads table
CREATE TABLE IF NOT EXISTS crm.leads (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    company_id UUID,
    contact_id UUID,
    title VARCHAR(255) NOT NULL,
    description TEXT,
    source VARCHAR(100),
    status VARCHAR(50) NOT NULL DEFAULT 'new',
    value DECIMAL(18, 2),
    probability INTEGER,
    expected_close_date DATE,
    assigned_user_id UUID,
    notes TEXT,
    is_active BOOLEAN NOT NULL DEFAULT true,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP,
    created_by UUID,
    updated_by UUID,
    is_deleted BOOLEAN NOT NULL DEFAULT false,
    
    -- Constraints
    CONSTRAINT fk_leads_company FOREIGN KEY (company_id) REFERENCES crm.companies(id) ON DELETE SET NULL,
    CONSTRAINT fk_leads_contact FOREIGN KEY (contact_id) REFERENCES crm.contacts(id) ON DELETE SET NULL,
    CONSTRAINT fk_leads_assigned_user FOREIGN KEY (assigned_user_id) REFERENCES auth.users(id) ON DELETE SET NULL,
    CONSTRAINT fk_leads_created_by FOREIGN KEY (created_by) REFERENCES auth.users(id) ON DELETE SET NULL,
    CONSTRAINT fk_leads_updated_by FOREIGN KEY (updated_by) REFERENCES auth.users(id) ON DELETE SET NULL,
    CONSTRAINT chk_leads_value CHECK (value IS NULL OR value >= 0),
    CONSTRAINT chk_leads_probability CHECK (probability IS NULL OR (probability >= 0 AND probability <= 100))
);

-- Opportunities table
CREATE TABLE IF NOT EXISTS crm.opportunities (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    lead_id UUID,
    company_id UUID,
    contact_id UUID,
    title VARCHAR(255) NOT NULL,
    description TEXT,
    stage VARCHAR(50) NOT NULL DEFAULT 'prospecting',
    value DECIMAL(18, 2) NOT NULL DEFAULT 0,
    probability INTEGER,
    expected_close_date DATE,
    actual_close_date DATE,
    assigned_user_id UUID,
    notes TEXT,
    is_active BOOLEAN NOT NULL DEFAULT true,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP,
    created_by UUID,
    updated_by UUID,
    deal_stage_id UUID,
    is_deleted BOOLEAN NOT NULL DEFAULT false,
    
    -- Constraints
    CONSTRAINT fk_opportunities_lead FOREIGN KEY (lead_id) REFERENCES crm.leads(id) ON DELETE SET NULL,
    CONSTRAINT fk_opportunities_company FOREIGN KEY (company_id) REFERENCES crm.companies(id) ON DELETE SET NULL,
    CONSTRAINT fk_opportunities_contact FOREIGN KEY (contact_id) REFERENCES crm.contacts(id) ON DELETE SET NULL,
    CONSTRAINT fk_opportunities_assigned_user FOREIGN KEY (assigned_user_id) REFERENCES auth.users(id) ON DELETE SET NULL,
    CONSTRAINT fk_opportunities_created_by FOREIGN KEY (created_by) REFERENCES auth.users(id) ON DELETE SET NULL,
    CONSTRAINT fk_opportunities_updated_by FOREIGN KEY (updated_by) REFERENCES auth.users(id) ON DELETE SET NULL,
    CONSTRAINT fk_opportunities_deal_stage FOREIGN KEY (deal_stage_id) REFERENCES crm.deal_stages(id) ON DELETE SET NULL,
    CONSTRAINT chk_opportunities_value CHECK (value >= 0),
    CONSTRAINT chk_opportunities_probability CHECK (probability IS NULL OR (probability >= 0 AND probability <= 100)),
    CONSTRAINT chk_opportunities_close_dates CHECK (actual_close_date IS NULL OR actual_close_date >= created_at::DATE)
);

-- Activities table
CREATE TABLE IF NOT EXISTS crm.activities (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    type VARCHAR(50) NOT NULL,
    subject VARCHAR(255) NOT NULL,
    description TEXT,
    status VARCHAR(50) NOT NULL DEFAULT 'planned',
    priority VARCHAR(50) NOT NULL DEFAULT 'medium',
    due_date TIMESTAMP,
    completed_date TIMESTAMP,
    company_id UUID,
    contact_id UUID,
    lead_id UUID,
    opportunity_id UUID,
    assigned_user_id UUID,
    is_active BOOLEAN NOT NULL DEFAULT true,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP,
    created_by UUID,
    updated_by UUID,
    is_deleted BOOLEAN NOT NULL DEFAULT false,
    
    -- Constraints
    CONSTRAINT fk_activities_company FOREIGN KEY (company_id) REFERENCES crm.companies(id) ON DELETE SET NULL,
    CONSTRAINT fk_activities_contact FOREIGN KEY (contact_id) REFERENCES crm.contacts(id) ON DELETE SET NULL,
    CONSTRAINT fk_activities_lead FOREIGN KEY (lead_id) REFERENCES crm.leads(id) ON DELETE SET NULL,
    CONSTRAINT fk_activities_opportunity FOREIGN KEY (opportunity_id) REFERENCES crm.opportunities(id) ON DELETE SET NULL,
    CONSTRAINT fk_activities_assigned_user FOREIGN KEY (assigned_user_id) REFERENCES auth.users(id) ON DELETE SET NULL,
    CONSTRAINT fk_activities_created_by FOREIGN KEY (created_by) REFERENCES auth.users(id) ON DELETE SET NULL,
    CONSTRAINT fk_activities_updated_by FOREIGN KEY (updated_by) REFERENCES auth.users(id) ON DELETE SET NULL,
    CONSTRAINT chk_activities_type CHECK (type IN ('call', 'meeting', 'email', 'task', 'deadline', 'other')),
    CONSTRAINT chk_activities_status CHECK (status IN ('planned', 'in_progress', 'completed', 'cancelled')),
    CONSTRAINT chk_activities_priority CHECK (priority IN ('low', 'medium', 'high', 'urgent')),
    CONSTRAINT chk_activities_dates CHECK (completed_date IS NULL OR completed_date >= created_at)
);

-- Notes table
CREATE TABLE IF NOT EXISTS crm.notes (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    company_id UUID,
    contact_id UUID,
    lead_id UUID,
    opportunity_id UUID,
    content TEXT NOT NULL,
    is_pinned BOOLEAN NOT NULL DEFAULT false,
    is_active BOOLEAN NOT NULL DEFAULT true,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP,
    created_by UUID,
    updated_by UUID,
    is_deleted BOOLEAN NOT NULL DEFAULT false,
    
    -- Constraints
    CONSTRAINT fk_notes_company FOREIGN KEY (company_id) REFERENCES crm.companies(id) ON DELETE CASCADE,
    CONSTRAINT fk_notes_contact FOREIGN KEY (contact_id) REFERENCES crm.contacts(id) ON DELETE CASCADE,
    CONSTRAINT fk_notes_lead FOREIGN KEY (lead_id) REFERENCES crm.leads(id) ON DELETE CASCADE,
    CONSTRAINT fk_notes_opportunity FOREIGN KEY (opportunity_id) REFERENCES crm.opportunities(id) ON DELETE CASCADE,
    CONSTRAINT fk_notes_created_by FOREIGN KEY (created_by) REFERENCES auth.users(id) ON DELETE SET NULL,
    CONSTRAINT fk_notes_updated_by FOREIGN KEY (updated_by) REFERENCES auth.users(id) ON DELETE SET NULL,
    CONSTRAINT chk_notes_entity CHECK (
        (company_id IS NOT NULL)::INTEGER +
        (contact_id IS NOT NULL)::INTEGER +
        (lead_id IS NOT NULL)::INTEGER +
        (opportunity_id IS NOT NULL)::INTEGER = 1
    )
);

-- Note: activity_logs has been removed - Activity logging is handled by Elasticsearch
-- See RequestLoggingMiddleware for HTTP request/response logging
-- audit.change_log table is used for database change tracking via triggers

-- System Settings table
CREATE TABLE IF NOT EXISTS crm.system_settings (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    key VARCHAR(100) NOT NULL,
    value TEXT NOT NULL,
    description TEXT,
    data_type VARCHAR(50) NOT NULL DEFAULT 'string',
    category VARCHAR(100),
    is_public BOOLEAN NOT NULL DEFAULT false,
    is_active BOOLEAN NOT NULL DEFAULT true,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP,
    created_by UUID,
    updated_by UUID,
    is_deleted BOOLEAN NOT NULL DEFAULT false,
    
    -- Constraints
    CONSTRAINT uk_system_settings_key UNIQUE (key),
    CONSTRAINT fk_system_settings_created_by FOREIGN KEY (created_by) REFERENCES auth.users(id) ON DELETE SET NULL,
    CONSTRAINT fk_system_settings_updated_by FOREIGN KEY (updated_by) REFERENCES auth.users(id) ON DELETE SET NULL,
    CONSTRAINT chk_system_settings_data_type CHECK (data_type IN ('string', 'int', 'bool', 'json', 'decimal'))
);
