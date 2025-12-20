-- =====================================================
-- PKT Database Initialization Script
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

-- Set default search path for the database
ALTER DATABASE pktdb SET search_path TO public;

-- Grant schema permissions to pktuser
GRANT ALL PRIVILEGES ON SCHEMA public TO pktuser;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO pktuser;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO pktuser;

-- Set default privileges for future tables and sequences
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON TABLES TO pktuser;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON SEQUENCES TO pktuser;

-- Success message
DO $$
BEGIN
    RAISE NOTICE '✅ Database initialization completed successfully!';
    RAISE NOTICE '   - Database: pktdb';
    RAISE NOTICE '   - User: pktuser';
    RAISE NOTICE '   - Schema: public';
    RAISE NOTICE '   - Extensions: uuid-ossp, pgcrypto, pg_trgm';
END $$;

