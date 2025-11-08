-- =====================================================
-- CRM Database Functions and Triggers Script
-- =====================================================
-- This script creates functions and triggers for automation
-- =====================================================

SET search_path TO crm, auth, audit, public;

-- =====================================================
-- UTILITY FUNCTIONS
-- =====================================================

-- Function to update the updated_at timestamp
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Function to log changes to audit.change_log
CREATE OR REPLACE FUNCTION log_audit_change()
RETURNS TRIGGER AS $$
DECLARE
    changed_by_id UUID;
BEGIN
    -- Get the user ID from the session if available
    changed_by_id := NULLIF(current_setting('app.current_user_id', TRUE), '')::UUID;
    
    IF (TG_OP = 'DELETE') THEN
        INSERT INTO audit.change_log (
            table_name,
            record_id,
            operation,
            old_values,
            new_values,
            changed_by
        ) VALUES (
            TG_TABLE_SCHEMA || '.' || TG_TABLE_NAME,
            OLD.id,
            'DELETE',
            row_to_json(OLD),
            NULL,
            changed_by_id
        );
        RETURN OLD;
    ELSIF (TG_OP = 'UPDATE') THEN
        INSERT INTO audit.change_log (
            table_name,
            record_id,
            operation,
            old_values,
            new_values,
            changed_by
        ) VALUES (
            TG_TABLE_SCHEMA || '.' || TG_TABLE_NAME,
            NEW.id,
            'UPDATE',
            row_to_json(OLD),
            row_to_json(NEW),
            changed_by_id
        );
        RETURN NEW;
    ELSIF (TG_OP = 'INSERT') THEN
        INSERT INTO audit.change_log (
            table_name,
            record_id,
            operation,
            old_values,
            new_values,
            changed_by
        ) VALUES (
            TG_TABLE_SCHEMA || '.' || TG_TABLE_NAME,
            NEW.id,
            'INSERT',
            NULL,
            row_to_json(NEW),
            changed_by_id
        );
        RETURN NEW;
    END IF;
    RETURN NULL;
END;
$$ LANGUAGE plpgsql;

-- Function to ensure only one primary contact per company
CREATE OR REPLACE FUNCTION ensure_one_primary_contact()
RETURNS TRIGGER AS $$
BEGIN
    IF NEW.is_primary = TRUE AND NEW.company_id IS NOT NULL THEN
        UPDATE crm.contacts
        SET is_primary = FALSE
        WHERE company_id = NEW.company_id
          AND id != NEW.id
          AND is_primary = TRUE
          AND is_deleted = FALSE;
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Function to ensure only one default deal stage
CREATE OR REPLACE FUNCTION ensure_one_default_deal_stage()
RETURNS TRIGGER AS $$
BEGIN
    IF NEW.is_default = TRUE THEN
        UPDATE crm.deal_stages
        SET is_default = FALSE
        WHERE id != NEW.id
          AND is_default = TRUE
          AND is_deleted = FALSE;
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Function to soft delete related entities
CREATE OR REPLACE FUNCTION soft_delete_company_relations()
RETURNS TRIGGER AS $$
BEGIN
    IF NEW.is_deleted = TRUE AND OLD.is_deleted = FALSE THEN
        -- Soft delete related contacts
        UPDATE crm.contacts
        SET is_deleted = TRUE, updated_at = CURRENT_TIMESTAMP
        WHERE company_id = NEW.id AND is_deleted = FALSE;
        
        -- Soft delete related leads
        UPDATE crm.leads
        SET is_deleted = TRUE, updated_at = CURRENT_TIMESTAMP
        WHERE company_id = NEW.id AND is_deleted = FALSE;
        
        -- Soft delete related opportunities
        UPDATE crm.opportunities
        SET is_deleted = TRUE, updated_at = CURRENT_TIMESTAMP
        WHERE company_id = NEW.id AND is_deleted = FALSE;
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Function to validate email format
CREATE OR REPLACE FUNCTION validate_email(email VARCHAR)
RETURNS BOOLEAN AS $$
BEGIN
    IF email IS NULL OR email = '' THEN
        RETURN TRUE;
    END IF;
    RETURN email ~* '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$';
END;
$$ LANGUAGE plpgsql;

-- Function to calculate opportunity win rate for a user
CREATE OR REPLACE FUNCTION calculate_user_win_rate(user_id UUID)
RETURNS NUMERIC AS $$
DECLARE
    total_count INTEGER;
    won_count INTEGER;
BEGIN
    SELECT COUNT(*) INTO total_count
    FROM crm.opportunities
    WHERE assigned_to = user_id
      AND is_deleted = FALSE
      AND actual_close_date IS NOT NULL;
    
    IF total_count = 0 THEN
        RETURN 0;
    END IF;
    
    SELECT COUNT(*) INTO won_count
    FROM crm.opportunities
    WHERE assigned_to = user_id
      AND is_deleted = FALSE
      AND actual_close_date IS NOT NULL
      AND stage IN ('won', 'closed_won', 'closed');
    
    RETURN ROUND((won_count::NUMERIC / total_count::NUMERIC) * 100, 2);
END;
$$ LANGUAGE plpgsql;

-- Function to get total pipeline value for a user
CREATE OR REPLACE FUNCTION get_user_pipeline_value(user_id UUID)
RETURNS NUMERIC AS $$
DECLARE
    total_value NUMERIC;
BEGIN
    SELECT COALESCE(SUM(value), 0) INTO total_value
    FROM crm.opportunities
    WHERE assigned_to = user_id
      AND is_deleted = FALSE
      AND is_active = TRUE
      AND actual_close_date IS NULL;
    
    RETURN total_value;
END;
$$ LANGUAGE plpgsql;

-- Function to clean up expired refresh tokens
CREATE OR REPLACE FUNCTION cleanup_expired_refresh_tokens()
RETURNS INTEGER AS $$
DECLARE
    deleted_count INTEGER;
BEGIN
    DELETE FROM auth.refresh_tokens
    WHERE expires_at < CURRENT_TIMESTAMP
      OR is_revoked = TRUE;
    
    GET DIAGNOSTICS deleted_count = ROW_COUNT;
    RETURN deleted_count;
END;
$$ LANGUAGE plpgsql;

-- Note: archive_old_activity_logs function removed - Activity logs are in Elasticsearch

-- =====================================================
-- TRIGGERS FOR updated_at
-- =====================================================

-- Auth schema triggers
CREATE TRIGGER trigger_users_updated_at
    BEFORE UPDATE ON auth.users
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER trigger_roles_updated_at
    BEFORE UPDATE ON auth.roles
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();

-- CRM schema triggers
CREATE TRIGGER trigger_companies_updated_at
    BEFORE UPDATE ON crm.companies
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER trigger_contacts_updated_at
    BEFORE UPDATE ON crm.contacts
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER trigger_leads_updated_at
    BEFORE UPDATE ON crm.leads
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER trigger_deal_stages_updated_at
    BEFORE UPDATE ON crm.deal_stages
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER trigger_opportunities_updated_at
    BEFORE UPDATE ON crm.opportunities
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER trigger_activities_updated_at
    BEFORE UPDATE ON crm.activities
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER trigger_notes_updated_at
    BEFORE UPDATE ON crm.notes
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();

-- Note: trigger_activity_logs_updated_at removed - Activity logs are in Elasticsearch

CREATE TRIGGER trigger_system_settings_updated_at
    BEFORE UPDATE ON crm.system_settings
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();

-- =====================================================
-- TRIGGERS FOR AUDIT LOGGING
-- =====================================================

-- Users audit
CREATE TRIGGER trigger_users_audit
    AFTER INSERT OR UPDATE OR DELETE ON auth.users
    FOR EACH ROW
    EXECUTE FUNCTION log_audit_change();

-- Companies audit
CREATE TRIGGER trigger_companies_audit
    AFTER INSERT OR UPDATE OR DELETE ON crm.companies
    FOR EACH ROW
    EXECUTE FUNCTION log_audit_change();

-- Contacts audit
CREATE TRIGGER trigger_contacts_audit
    AFTER INSERT OR UPDATE OR DELETE ON crm.contacts
    FOR EACH ROW
    EXECUTE FUNCTION log_audit_change();

-- Leads audit
CREATE TRIGGER trigger_leads_audit
    AFTER INSERT OR UPDATE OR DELETE ON crm.leads
    FOR EACH ROW
    EXECUTE FUNCTION log_audit_change();

-- Opportunities audit
CREATE TRIGGER trigger_opportunities_audit
    AFTER INSERT OR UPDATE OR DELETE ON crm.opportunities
    FOR EACH ROW
    EXECUTE FUNCTION log_audit_change();

-- =====================================================
-- BUSINESS LOGIC TRIGGERS
-- =====================================================

-- Ensure one primary contact per company
CREATE TRIGGER trigger_ensure_one_primary_contact
    AFTER INSERT OR UPDATE ON crm.contacts
    FOR EACH ROW
    WHEN (NEW.is_primary = TRUE)
    EXECUTE FUNCTION ensure_one_primary_contact();

-- Ensure one default deal stage
CREATE TRIGGER trigger_ensure_one_default_deal_stage
    AFTER INSERT OR UPDATE ON crm.deal_stages
    FOR EACH ROW
    WHEN (NEW.is_default = TRUE)
    EXECUTE FUNCTION ensure_one_default_deal_stage();

-- Soft delete company relations
CREATE TRIGGER trigger_soft_delete_company_relations
    AFTER UPDATE ON crm.companies
    FOR EACH ROW
    WHEN (NEW.is_deleted = TRUE AND OLD.is_deleted = FALSE)
    EXECUTE FUNCTION soft_delete_company_relations();
