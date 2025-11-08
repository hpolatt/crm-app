#!/bin/bash
# =====================================================
# CRM Database Setup Script
# =====================================================
# This script sets up the complete CRM database with
# all tables, indexes, functions, triggers, and seed data
# =====================================================

set -e

HOST="localhost"
PORT="5433"
USER="crmuser"
PASSWORD="CrmPass@2024"
DATABASE="crmdb"

echo "🚀 Starting CRM Database Setup..."
echo "=================================="

# Function to execute SQL file
execute_sql_file() {
    local file=$1
    local description=$2
    echo "📦 $description..."
    PGPASSWORD=$PASSWORD psql -h $HOST -p $PORT -U $USER -d $DATABASE -f $file
    if [ $? -eq 0 ]; then
        echo "✅ $description completed successfully"
    else
        echo "❌ Error executing $description"
        exit 1
    fi
}

# 1. Check if database is running
echo "📦 Step 1/7: Checking database connection..."
PGPASSWORD=$PASSWORD psql -h $HOST -p $PORT -U $USER -d postgres -c "SELECT version();" > /dev/null 2>&1
if [ $? -eq 0 ]; then
    echo "✅ Database connection successful"
else
    echo "❌ Cannot connect to database. Please ensure PostgreSQL is running."
    exit 1
fi

# 2. Drop and recreate database
echo "📦 Step 2/7: Recreating database..."
PGPASSWORD=$PASSWORD psql -h $HOST -p $PORT -U $USER -d postgres <<EOF
-- Terminate existing connections
SELECT pg_terminate_backend(pid)
FROM pg_stat_activity
WHERE datname = '$DATABASE' AND pid <> pg_backend_pid();

-- Drop and recreate database
DROP DATABASE IF EXISTS $DATABASE;

CREATE DATABASE $DATABASE
    WITH 
    OWNER = $USER
    ENCODING = 'UTF8'
    LC_COLLATE = 'en_US.utf8'
    LC_CTYPE = 'en_US.utf8'
    TEMPLATE = template0;
EOF

if [ $? -eq 0 ]; then
    echo "✅ Database recreated successfully"
else
    echo "❌ Error recreating database"
    exit 1
fi

# 3. Initialize database (extensions, schemas, audit)
execute_sql_file "01_init_database.sql" "Step 3/7: Initializing database structure"

# 4. Create tables
execute_sql_file "02_create_tables.sql" "Step 4/7: Creating tables"

# 5. Create indexes
execute_sql_file "03_create_indexes.sql" "Step 5/7: Creating indexes"

# 6. Create functions and triggers
execute_sql_file "04_create_functions_triggers.sql" "Step 6/7: Creating functions and triggers"

# 7. Seed initial data
execute_sql_file "05_seed_data.sql" "Step 7/7: Seeding initial data"

echo ""
echo "=================================="
echo "🎉 Database setup completed successfully!"
echo ""
echo "Database Details:"
echo "  Host: $HOST"
echo "  Port: $PORT"
echo "  Database: $DATABASE"
echo "  User: $USER"
echo ""
echo "Default Admin Credentials:"
echo "  Email: admin@crm.com"
echo "  Password: Admin@123"
echo ""
echo "⚠️  IMPORTANT: Change the default admin password after first login!"
echo "=================================="

# 4. Create indexes
echo "📦 Step 4/5: Creating indexes..."
PGPASSWORD=$PASSWORD psql -h $HOST -p $PORT -U $USER -d crm_db -f 03_create_indexes.sql

echo "✅ Indexes created successfully"

# 5. Create functions
echo "📦 Step 5/5: Creating functions and triggers..."
PGPASSWORD=$PASSWORD psql -h $HOST -p $PORT -U $USER -d crm_db -f 04_create_functions.sql

echo "✅ Functions created successfully"

# 6. Seed data
echo "📦 Step 6/6: Seeding initial data..."
PGPASSWORD=$PASSWORD psql -h $HOST -p $PORT -U $USER -d crm_db -f 05_seed_data.sql

echo ""
echo "🎉 Database setup completed successfully!"
echo "=================================="
echo "Database: crm_db"
echo "Admin User: admin@crm.local"
echo "Password: Admin123!"
echo "=================================="
