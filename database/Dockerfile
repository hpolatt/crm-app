FROM postgres:16-alpine

# Set environment variables (these will be overridden by docker-compose.yml)
ENV POSTGRES_DB=pktdb
ENV POSTGRES_USER=pktuser
ENV POSTGRES_PASSWORD=PktDb@2024

# Copy SQL scripts in order
COPY 01_init_database.sql /docker-entrypoint-initdb.d/
COPY 02_create_tables.sql /docker-entrypoint-initdb.d/
COPY 03_create_indexes.sql /docker-entrypoint-initdb.d/
COPY 05_seed_data.sql /docker-entrypoint-initdb.d/

# Expose PostgreSQL port
EXPOSE 5432

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD pg_isready -U pktuser -d pktdb || exit 1


