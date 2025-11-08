-- =====================================================
-- CRM Database Initial Data Script
-- =====================================================
-- This script creates initial roles and admin user
-- =====================================================

SET search_path TO crm, auth, audit, public;

-- =====================================================
-- INSERT DEFAULT ROLES
-- =====================================================

INSERT INTO auth.roles (id, name, description, is_active, created_at)
VALUES 
    (uuid_generate_v4(), 'Admin', 'System administrator with full access', true, CURRENT_TIMESTAMP),
    (uuid_generate_v4(), 'Manager', 'Sales manager with team oversight', true, CURRENT_TIMESTAMP),
    (uuid_generate_v4(), 'Sales', 'Sales representative', true, CURRENT_TIMESTAMP),
    (uuid_generate_v4(), 'Support', 'Customer support representative', true, CURRENT_TIMESTAMP),
    (uuid_generate_v4(), 'Viewer', 'Read-only access', true, CURRENT_TIMESTAMP),
    (uuid_generate_v4(), 'User', 'Standard user with basic access', true, CURRENT_TIMESTAMP)
ON CONFLICT (name) DO NOTHING;

-- =====================================================
-- INSERT DEFAULT ADMIN USER
-- =====================================================
-- Password: Admin@123 (hashed with bcrypt)
-- Make sure to change this password after first login!

DO $$
DECLARE
    admin_user_id UUID;
    admin_role_id UUID;
BEGIN
    -- Insert admin user
    INSERT INTO auth.users (id, email, password_hash, first_name, last_name, phone, is_active, created_at)
    VALUES (
        uuid_generate_v4(),
        'admin@crm.com',
        '$2a$11$JZ5qK5qMZwmYFCX5ZmZn4OqGZQjL5nKxKqmx5eJqLKqGZQjL5nKx5', -- Admin@123
        'System',
        'Administrator',
        '+1234567890',
        true,
        CURRENT_TIMESTAMP
    )
    ON CONFLICT (email) DO NOTHING
    RETURNING id INTO admin_user_id;
    
    -- Get admin role ID
    SELECT id INTO admin_role_id FROM auth.roles WHERE name = 'Admin' LIMIT 1;
    
    -- Assign admin role to admin user
    IF admin_user_id IS NOT NULL AND admin_role_id IS NOT NULL THEN
        INSERT INTO auth.user_roles (id, user_id, role_id, created_at)
        VALUES (uuid_generate_v4(), admin_user_id, admin_role_id, CURRENT_TIMESTAMP)
        ON CONFLICT (user_id, role_id) DO NOTHING;
    END IF;
END $$;

-- =====================================================
-- INSERT DEFAULT DEAL STAGES
-- =====================================================

INSERT INTO crm.deal_stages (id, name, order_position, description, color, is_default, is_active, created_at)
VALUES 
    (uuid_generate_v4(), 'Prospecting', 1, 'Initial contact and qualification', '#3B82F6', true, true, CURRENT_TIMESTAMP),
    (uuid_generate_v4(), 'Qualification', 2, 'Assessing customer needs and fit', '#8B5CF6', false, true, CURRENT_TIMESTAMP),
    (uuid_generate_v4(), 'Proposal', 3, 'Presenting solution and pricing', '#F59E0B', false, true, CURRENT_TIMESTAMP),
    (uuid_generate_v4(), 'Negotiation', 4, 'Working through terms and conditions', '#EF4444', false, true, CURRENT_TIMESTAMP),
    (uuid_generate_v4(), 'Closed Won', 5, 'Deal successfully closed', '#10B981', false, true, CURRENT_TIMESTAMP),
    (uuid_generate_v4(), 'Closed Lost', 6, 'Deal was not successful', '#6B7280', false, true, CURRENT_TIMESTAMP)
ON CONFLICT (name) DO NOTHING;

-- =====================================================
-- INSERT SYSTEM SETTINGS
-- =====================================================

INSERT INTO crm.system_settings (id, key, value, description, data_type, category, is_public, created_at)
VALUES 
    (uuid_generate_v4(), 'company_name', 'CRM Application', 'Company name displayed in the application', 'string', 'general', true, CURRENT_TIMESTAMP),
    (uuid_generate_v4(), 'currency', 'USD', 'Default currency', 'string', 'general', true, CURRENT_TIMESTAMP),
    (uuid_generate_v4(), 'date_format', 'YYYY-MM-DD', 'Default date format', 'string', 'general', true, CURRENT_TIMESTAMP),
    (uuid_generate_v4(), 'timezone', 'UTC', 'Default timezone', 'string', 'general', true, CURRENT_TIMESTAMP),
    (uuid_generate_v4(), 'items_per_page', '20', 'Default number of items per page', 'int', 'general', true, CURRENT_TIMESTAMP),
    (uuid_generate_v4(), 'session_timeout', '60', 'Session timeout in minutes', 'int', 'security', false, CURRENT_TIMESTAMP),
    (uuid_generate_v4(), 'password_min_length', '8', 'Minimum password length', 'int', 'security', false, CURRENT_TIMESTAMP),
    (uuid_generate_v4(), 'password_require_special_char', 'true', 'Require special character in password', 'bool', 'security', false, CURRENT_TIMESTAMP),
    (uuid_generate_v4(), 'enable_two_factor_auth', 'false', 'Enable two-factor authentication', 'bool', 'security', false, CURRENT_TIMESTAMP),
    (uuid_generate_v4(), 'email_notifications_enabled', 'true', 'Enable email notifications', 'bool', 'notifications', false, CURRENT_TIMESTAMP),
    (uuid_generate_v4(), 'activity_reminder_hours', '24', 'Hours before activity to send reminder', 'int', 'notifications', false, CURRENT_TIMESTAMP)
ON CONFLICT (key) DO NOTHING;

-- =====================================================
-- INSERT SAMPLE DATA (FOR DEVELOPMENT/TESTING)
-- =====================================================
-- Comment out this section in production

DO $$
DECLARE
    sample_company_id UUID;
    sample_contact_id UUID;
    sample_user_id UUID;
BEGIN
    -- Get admin user ID
    SELECT id INTO sample_user_id FROM auth.users WHERE email = 'admin@crm.com' LIMIT 1;
    
    IF sample_user_id IS NOT NULL THEN
        -- Insert sample company
        INSERT INTO crm.companies (id, name, industry, website, phone, email, city, country, source, employee_count, annual_revenue, created_by, created_at)
        VALUES (
            uuid_generate_v4(),
            'Acme Corporation',
            'Technology',
            'https://www.acme.com',
            '+1-555-0100',
            'contact@acme.com',
            'San Francisco',
            'USA',
            'Website',
            150,
            5000000.00,
            sample_user_id,
            CURRENT_TIMESTAMP
        )
        RETURNING id INTO sample_company_id;
        
        -- Insert sample contact
        IF sample_company_id IS NOT NULL THEN
            INSERT INTO crm.contacts (id, company_id, first_name, last_name, email, phone, position, is_primary, created_by, created_at)
            VALUES (
                uuid_generate_v4(),
                sample_company_id,
                'John',
                'Doe',
                'john.doe@acme.com',
                '+1-555-0101',
                'CEO',
                true,
                sample_user_id,
                CURRENT_TIMESTAMP
            )
            RETURNING id INTO sample_contact_id;
            
            -- Insert sample lead
            INSERT INTO crm.leads (id, company_id, contact_id, title, description, source, status, value, probability, assigned_to, created_by, created_at)
            VALUES (
                uuid_generate_v4(),
                sample_company_id,
                sample_contact_id,
                'Enterprise Software Deal',
                'Potential enterprise license agreement',
                'Referral',
                'qualified',
                250000.00,
                60,
                sample_user_id,
                sample_user_id,
                CURRENT_TIMESTAMP
            );
        END IF;
    END IF;
END $$;

-- =====================================================
-- CREATE SCHEDULED JOB PLACEHOLDERS
-- =====================================================
-- These would typically be handled by an external job scheduler
-- or application-level scheduling

COMMENT ON FUNCTION cleanup_expired_refresh_tokens() IS 
'Run daily: SELECT cleanup_expired_refresh_tokens();';

-- Note: archive_old_activity_logs comment removed - Activity logs are in Elasticsearch
