FROM postgres:16-alpine

# Set environment variables
ENV POSTGRES_DB=crmdb
ENV POSTGRES_USER=crmuser
ENV POSTGRES_PASSWORD=CrmPass@2024

# Copy SQL scripts in order
COPY 01_init_database.sql /docker-entrypoint-initdb.d/
COPY 02_create_tables.sql /docker-entrypoint-initdb.d/
COPY 03_create_indexes.sql /docker-entrypoint-initdb.d/
COPY 04_create_functions_triggers.sql /docker-entrypoint-initdb.d/
COPY 05_seed_data.sql /docker-entrypoint-initdb.d/

# Expose PostgreSQL port
EXPOSE 5432

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD pg_isready -U crmuser -d crmdb || exit 1

