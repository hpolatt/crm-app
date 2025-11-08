-- =====================================================
-- CRM Database Initialization Script
-- =====================================================
-- This script creates the database, extensions, schemas, and roles
-- Run this first before other migration scripts
-- =====================================================

-- Create database (if running manually, not needed in Docker)
-- In Docker, the database is created automatically via POSTGRES_DB env variable

-- Create extensions (requires superuser privileges)
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pgcrypto";
CREATE EXTENSION IF NOT EXISTS "pg_trgm"; -- For text search optimization

-- Create schemas
CREATE SCHEMA IF NOT EXISTS auth;
CREATE SCHEMA IF NOT EXISTS crm;
CREATE SCHEMA IF NOT EXISTS audit;

-- Set default search path for the database
ALTER DATABASE crmdb SET search_path TO crm, auth, audit, public;

-- Grant schema permissions to crmuser
GRANT USAGE ON SCHEMA auth TO crmuser;
GRANT USAGE ON SCHEMA crm TO crmuser;
GRANT USAGE ON SCHEMA audit TO crmuser;

GRANT CREATE ON SCHEMA auth TO crmuser;
GRANT CREATE ON SCHEMA crm TO crmuser;
GRANT CREATE ON SCHEMA audit TO crmuser;

-- Grant all privileges on all tables in schemas (current and future)
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA auth TO crmuser;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA crm TO crmuser;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA audit TO crmuser;

GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA auth TO crmuser;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA crm TO crmuser;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA audit TO crmuser;

-- Set default privileges for future tables and sequences
ALTER DEFAULT PRIVILEGES IN SCHEMA auth GRANT ALL ON TABLES TO crmuser;
ALTER DEFAULT PRIVILEGES IN SCHEMA crm GRANT ALL ON TABLES TO crmuser;
ALTER DEFAULT PRIVILEGES IN SCHEMA audit GRANT ALL ON TABLES TO crmuser;

ALTER DEFAULT PRIVILEGES IN SCHEMA auth GRANT ALL ON SEQUENCES TO crmuser;
ALTER DEFAULT PRIVILEGES IN SCHEMA crm GRANT ALL ON SEQUENCES TO crmuser;
ALTER DEFAULT PRIVILEGES IN SCHEMA audit GRANT ALL ON SEQUENCES TO crmuser;

-- Create audit log table for tracking all changes
CREATE TABLE IF NOT EXISTS audit.change_log (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    table_name VARCHAR(255) NOT NULL,
    record_id UUID NOT NULL,
    operation VARCHAR(10) NOT NULL, -- INSERT, UPDATE, DELETE
    old_values JSONB,
    new_values JSONB,
    changed_by UUID,
    changed_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ip_address VARCHAR(45),
    user_agent TEXT
);

-- Grant permissions on audit.change_log
GRANT ALL PRIVILEGES ON TABLE audit.change_log TO crmuser;

-- Create indexes for audit log
CREATE INDEX IF NOT EXISTS idx_audit_change_log_table_name ON audit.change_log(table_name);
CREATE INDEX IF NOT EXISTS idx_audit_change_log_record_id ON audit.change_log(record_id);
CREATE INDEX IF NOT EXISTS idx_audit_change_log_changed_at ON audit.change_log(changed_at DESC);
CREATE INDEX IF NOT EXISTS idx_audit_change_log_changed_by ON audit.change_log(changed_by);

-- Success message
DO $$
BEGIN
    RAISE NOTICE '✅ Database initialization completed successfully!';
    RAISE NOTICE '   - Database: crmdb';
    RAISE NOTICE '   - User: crmuser';
    RAISE NOTICE '   - Schemas: auth, crm, audit';
    RAISE NOTICE '   - Extensions: uuid-ossp, pgcrypto, pg_trgm';
END $$;
