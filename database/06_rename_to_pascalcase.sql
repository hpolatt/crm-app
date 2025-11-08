-- =============================================
-- Convert all column names from snake_case to PascalCase
-- =============================================

-- AUTH.USERS
ALTER TABLE auth.users RENAME COLUMN first_name TO FirstName;
ALTER TABLE auth.users RENAME COLUMN last_name TO LastName;
ALTER TABLE auth.users RENAME COLUMN email TO Email;
ALTER TABLE auth.users RENAME COLUMN password_hash TO PasswordHash;
ALTER TABLE auth.users RENAME COLUMN phone TO Phone;
ALTER TABLE auth.users RENAME COLUMN is_active TO IsActive;
ALTER TABLE auth.users RENAME COLUMN last_login TO LastLogin;
ALTER TABLE auth.users RENAME COLUMN created_at TO CreatedAt;
ALTER TABLE auth.users RENAME COLUMN updated_at TO UpdatedAt;
ALTER TABLE auth.users RENAME COLUMN created_by TO CreatedBy;
ALTER TABLE auth.users RENAME COLUMN updated_by TO UpdatedBy;
ALTER TABLE auth.users RENAME COLUMN is_deleted TO IsDeleted;

-- AUTH.ROLES
ALTER TABLE auth.roles RENAME COLUMN name TO Name;
ALTER TABLE auth.roles RENAME COLUMN description TO Description;
ALTER TABLE auth.roles RENAME COLUMN is_active TO IsActive;
ALTER TABLE auth.roles RENAME COLUMN created_at TO CreatedAt;
ALTER TABLE auth.roles RENAME COLUMN updated_at TO UpdatedAt;
ALTER TABLE auth.roles RENAME COLUMN is_deleted TO IsDeleted;

-- AUTH.USER_ROLES
ALTER TABLE auth.user_roles RENAME COLUMN user_id TO UserId;
ALTER TABLE auth.user_roles RENAME COLUMN role_id TO RoleId;
ALTER TABLE auth.user_roles RENAME COLUMN assigned_at TO AssignedAt;
ALTER TABLE auth.user_roles RENAME COLUMN assigned_by TO AssignedBy;

-- AUTH.REFRESH_TOKENS
ALTER TABLE auth.refresh_tokens RENAME COLUMN user_id TO UserId;
ALTER TABLE auth.refresh_tokens RENAME COLUMN token TO Token;
ALTER TABLE auth.refresh_tokens RENAME COLUMN expires_at TO ExpiresAt;
ALTER TABLE auth.refresh_tokens RENAME COLUMN created_at TO CreatedAt;
ALTER TABLE auth.refresh_tokens RENAME COLUMN revoked_at TO RevokedAt;
ALTER TABLE auth.refresh_tokens RENAME COLUMN replaced_by_token TO ReplacedByToken;

-- CRM.COMPANIES
ALTER TABLE crm.companies RENAME COLUMN name TO Name;
ALTER TABLE crm.companies RENAME COLUMN industry TO Industry;
ALTER TABLE crm.companies RENAME COLUMN website TO Website;
ALTER TABLE crm.companies RENAME COLUMN phone TO Phone;
ALTER TABLE crm.companies RENAME COLUMN email TO Email;
ALTER TABLE crm.companies RENAME COLUMN address TO Address;
ALTER TABLE crm.companies RENAME COLUMN city TO City;
ALTER TABLE crm.companies RENAME COLUMN state TO State;
ALTER TABLE crm.companies RENAME COLUMN country TO Country;
ALTER TABLE crm.companies RENAME COLUMN postal_code TO PostalCode;
ALTER TABLE crm.companies RENAME COLUMN notes TO Notes;
ALTER TABLE crm.companies RENAME COLUMN is_active TO IsActive;
ALTER TABLE crm.companies RENAME COLUMN created_at TO CreatedAt;
ALTER TABLE crm.companies RENAME COLUMN updated_at TO UpdatedAt;
ALTER TABLE crm.companies RENAME COLUMN created_by TO CreatedBy;
ALTER TABLE crm.companies RENAME COLUMN updated_by TO UpdatedBy;
ALTER TABLE crm.companies RENAME COLUMN is_deleted TO IsDeleted;

-- CRM.CONTACTS
ALTER TABLE crm.contacts RENAME COLUMN company_id TO CompanyId;
ALTER TABLE crm.contacts RENAME COLUMN first_name TO FirstName;
ALTER TABLE crm.contacts RENAME COLUMN last_name TO LastName;
ALTER TABLE crm.contacts RENAME COLUMN email TO Email;
ALTER TABLE crm.contacts RENAME COLUMN phone TO Phone;
ALTER TABLE crm.contacts RENAME COLUMN mobile TO Mobile;
ALTER TABLE crm.contacts RENAME COLUMN title TO Title;
ALTER TABLE crm.contacts RENAME COLUMN department TO Department;
ALTER TABLE crm.contacts RENAME COLUMN notes TO Notes;
ALTER TABLE crm.contacts RENAME COLUMN is_active TO IsActive;
ALTER TABLE crm.contacts RENAME COLUMN created_at TO CreatedAt;
ALTER TABLE crm.contacts RENAME COLUMN updated_at TO UpdatedAt;
ALTER TABLE crm.contacts RENAME COLUMN created_by TO CreatedBy;
ALTER TABLE crm.contacts RENAME COLUMN updated_by TO UpdatedBy;
ALTER TABLE crm.contacts RENAME COLUMN is_deleted TO IsDeleted;

-- CRM.LEADS
ALTER TABLE crm.leads RENAME COLUMN company_id TO CompanyId;
ALTER TABLE crm.leads RENAME COLUMN contact_id TO ContactId;
ALTER TABLE crm.leads RENAME COLUMN title TO Title;
ALTER TABLE crm.leads RENAME COLUMN description TO Description;
ALTER TABLE crm.leads RENAME COLUMN source TO Source;
ALTER TABLE crm.leads RENAME COLUMN status TO Status;
ALTER TABLE crm.leads RENAME COLUMN value TO Value;
ALTER TABLE crm.leads RENAME COLUMN probability TO Probability;
ALTER TABLE crm.leads RENAME COLUMN expected_close_date TO ExpectedCloseDate;
ALTER TABLE crm.leads RENAME COLUMN assigned_to TO AssignedUserId;
ALTER TABLE crm.leads RENAME COLUMN notes TO Notes;
ALTER TABLE crm.leads RENAME COLUMN is_active TO IsActive;
ALTER TABLE crm.leads RENAME COLUMN created_at TO CreatedAt;
ALTER TABLE crm.leads RENAME COLUMN updated_at TO UpdatedAt;
ALTER TABLE crm.leads RENAME COLUMN created_by TO CreatedBy;
ALTER TABLE crm.leads RENAME COLUMN updated_by TO UpdatedBy;
ALTER TABLE crm.leads RENAME COLUMN is_deleted TO IsDeleted;

-- CRM.OPPORTUNITIES
ALTER TABLE crm.opportunities RENAME COLUMN lead_id TO LeadId;
ALTER TABLE crm.opportunities RENAME COLUMN company_id TO CompanyId;
ALTER TABLE crm.opportunities RENAME COLUMN contact_id TO ContactId;
ALTER TABLE crm.opportunities RENAME COLUMN title TO Title;
ALTER TABLE crm.opportunities RENAME COLUMN description TO Description;
ALTER TABLE crm.opportunities RENAME COLUMN stage TO Stage;
ALTER TABLE crm.opportunities RENAME COLUMN value TO Value;
ALTER TABLE crm.opportunities RENAME COLUMN probability TO Probability;
ALTER TABLE crm.opportunities RENAME COLUMN expected_close_date TO ExpectedCloseDate;
ALTER TABLE crm.opportunities RENAME COLUMN actual_close_date TO ActualCloseDate;
ALTER TABLE crm.opportunities RENAME COLUMN assigned_to TO AssignedUserId;
ALTER TABLE crm.opportunities RENAME COLUMN notes TO Notes;
ALTER TABLE crm.opportunities RENAME COLUMN is_active TO IsActive;
ALTER TABLE crm.opportunities RENAME COLUMN created_at TO CreatedAt;
ALTER TABLE crm.opportunities RENAME COLUMN updated_at TO UpdatedAt;
ALTER TABLE crm.opportunities RENAME COLUMN created_by TO CreatedBy;
ALTER TABLE crm.opportunities RENAME COLUMN updated_by TO UpdatedBy;
ALTER TABLE crm.opportunities RENAME COLUMN is_deleted TO IsDeleted;

-- CRM.ACTIVITIES
ALTER TABLE crm.activities RENAME COLUMN type TO Type;
ALTER TABLE crm.activities RENAME COLUMN subject TO Subject;
ALTER TABLE crm.activities RENAME COLUMN description TO Description;
ALTER TABLE crm.activities RENAME COLUMN status TO Status;
ALTER TABLE crm.activities RENAME COLUMN priority TO Priority;
ALTER TABLE crm.activities RENAME COLUMN due_date TO DueDate;
ALTER TABLE crm.activities RENAME COLUMN completed_date TO CompletedDate;
ALTER TABLE crm.activities RENAME COLUMN company_id TO CompanyId;
ALTER TABLE crm.activities RENAME COLUMN contact_id TO ContactId;
ALTER TABLE crm.activities RENAME COLUMN lead_id TO LeadId;
ALTER TABLE crm.activities RENAME COLUMN opportunity_id TO OpportunityId;
ALTER TABLE crm.activities RENAME COLUMN assigned_to TO AssignedUserId;
ALTER TABLE crm.activities RENAME COLUMN is_active TO IsActive;
ALTER TABLE crm.activities RENAME COLUMN created_at TO CreatedAt;
ALTER TABLE crm.activities RENAME COLUMN updated_at TO UpdatedAt;
ALTER TABLE crm.activities RENAME COLUMN created_by TO CreatedBy;
ALTER TABLE crm.activities RENAME COLUMN updated_by TO UpdatedBy;
ALTER TABLE crm.activities RENAME COLUMN is_deleted TO IsDeleted;

-- CRM.DEAL_STAGES
ALTER TABLE crm.deal_stages RENAME COLUMN name TO Name;
ALTER TABLE crm.deal_stages RENAME COLUMN display_order TO DisplayOrder;
ALTER TABLE crm.deal_stages RENAME COLUMN probability TO Probability;
ALTER TABLE crm.deal_stages RENAME COLUMN is_active TO IsActive;
ALTER TABLE crm.deal_stages RENAME COLUMN created_at TO CreatedAt;
ALTER TABLE crm.deal_stages RENAME COLUMN updated_at TO UpdatedAt;
ALTER TABLE crm.deal_stages RENAME COLUMN is_deleted TO IsDeleted;

-- CRM.NOTES
ALTER TABLE crm.notes RENAME COLUMN company_id TO CompanyId;
ALTER TABLE crm.notes RENAME COLUMN contact_id TO ContactId;
ALTER TABLE crm.notes RENAME COLUMN lead_id TO LeadId;
ALTER TABLE crm.notes RENAME COLUMN opportunity_id TO OpportunityId;
ALTER TABLE crm.notes RENAME COLUMN content TO Content;
ALTER TABLE crm.notes RENAME COLUMN is_active TO IsActive;
ALTER TABLE crm.notes RENAME COLUMN created_at TO CreatedAt;
ALTER TABLE crm.notes RENAME COLUMN updated_at TO UpdatedAt;
ALTER TABLE crm.notes RENAME COLUMN created_by TO CreatedBy;
ALTER TABLE crm.notes RENAME COLUMN updated_by TO UpdatedBy;
ALTER TABLE crm.notes RENAME COLUMN is_deleted TO IsDeleted;

-- CRM.SYSTEM_SETTINGS
ALTER TABLE crm.system_settings RENAME COLUMN key TO Key;
ALTER TABLE crm.system_settings RENAME COLUMN value TO Value;
ALTER TABLE crm.system_settings RENAME COLUMN description TO Description;
ALTER TABLE crm.system_settings RENAME COLUMN is_active TO IsActive;
ALTER TABLE crm.system_settings RENAME COLUMN created_at TO CreatedAt;
ALTER TABLE crm.system_settings RENAME COLUMN updated_at TO UpdatedAt;
ALTER TABLE crm.system_settings RENAME COLUMN is_deleted TO IsDeleted;

-- AUDIT.CHANGE_LOG
ALTER TABLE audit.change_log RENAME COLUMN table_name TO TableName;
ALTER TABLE audit.change_log RENAME COLUMN record_id TO RecordId;
ALTER TABLE audit.change_log RENAME COLUMN operation TO Operation;
ALTER TABLE audit.change_log RENAME COLUMN old_values TO OldValues;
ALTER TABLE audit.change_log RENAME COLUMN new_values TO NewValues;
ALTER TABLE audit.change_log RENAME COLUMN changed_by TO ChangedBy;
ALTER TABLE audit.change_log RENAME COLUMN changed_at TO ChangedAt;

-- Keep 'id' lowercase as it's a standard convention
-- ALTER TABLE statements don't include 'id' column
ALTER TABLE crm.opportunities RENAME COLUMN assigned_to TO assigned_user_id;
ALTER TABLE crm.activities RENAME COLUMN assigned_to TO assigned_user_id;
ALTER TABLE crm.leads RENAME COLUMN assigned_to TO assigned_user_id;
ALTER TABLE crm.opportunities ADD COLUMN deal_stage_id UUID REFERENCES crm.deal_stages(id) ON DELETE SET NULL;
ALTER TABLE crm.deal_stages RENAME COLUMN order_position TO "order";

SELECT 'All columns renamed to PascalCase successfully!' as result;
