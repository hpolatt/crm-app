# PKT Application - Setup Instructions

## Quick Start Guide

### 1. Prerequisites
Make sure you have the following installed:
- Docker Desktop (https://www.docker.com/products/docker-desktop)
- Docker Compose (included with Docker Desktop)

### 2. Clone and Navigate
```bash
cd /Users/huseyin/Dev/CrmApp
```

License: This repository is published under the Business Source License 1.1 (BSL-1.1). Commercial use and offering the software as a hosted service are restricted until 2026-11-09. See `LICENSE` for details.

### 3. Build and Run

Run ALL services with a single command:
```bash
docker-compose up --build
```

This will:
- ✅ Build and start PostgreSQL database (Port: 5432)
- ✅ Run all SQL scripts automatically (tables, indexes, triggers, seed data)
- ✅ Build and start Redis cache (Port: 6379)
- ✅ Build and start .NET Backend API (Port: 5000)
- ✅ Build and start React Frontend (Port: 80)

### 4. Access the Application

Once all containers are running:
- **Frontend**: http://localhost
- **Backend API**: http://localhost:5000
- **Swagger Documentation**: http://localhost:5000/swagger

### 5. Login

Default admin credentials:
- **Email**: admin@crm.com
- **Password**: Admin@123

docker exec -it crm-database psql -U crmuser -d crmdb -c "SELECT 1;"
```


# Setup — how I run this locally

This file explains how I set up and run the project on my machine. I keep it intentionally simple so you can get started quickly.

## Prerequisites

- Docker Desktop (or Docker + Docker Compose)
- Git

## Quick start (recommended)

1. From the repo root, copy the environment template and edit values for your machine:

```bash
cp .env.example .env
# edit .env (do not commit)
```

2. Build and start everything:

```bash
docker-compose up --build -d
```

What this does:

- Creates PostgreSQL, runs our SQL scripts, and seeds initial data
- Starts Redis, Elasticsearch and Kibana
- Builds and runs the backend API and the frontend

Services I typically check after startup:

- Frontend: http://localhost
- Backend API: http://localhost:5001 (or 5000 depending on your compose file) — Swagger available at `/swagger`
- Kibana: http://localhost:5601

## Manual database setup (optional)

If you want to apply the SQL scripts manually:

```bash
# Start only the DB container
docker-compose up -d database

# Run scripts from the host
cd database
docker exec -i crm-database psql -U crmuser -d crmdb < 01_init_database.sql
docker exec -i crm-database psql -U crmuser -d crmdb < 02_create_tables.sql
docker exec -i crm-database psql -U crmuser -d crmdb < 03_create_indexes.sql
docker exec -i crm-database psql -U crmuser -d crmdb < 04_create_triggers.sql
docker exec -i crm-database psql -U crmuser -d crmdb < 05_seed_data.sql
```

Then start the remaining services:

```bash
docker-compose up -d
```

## Running locally without Docker

Backend

```bash
cd backend
dotnet restore
dotnet build
dotnet run --project CrmApp.API
```

Frontend

```bash
cd frontend
npm install
npm run dev
```

## Useful Docker commands I use

- Tail logs for all services: `docker-compose logs -f`
- Tail logs for a service: `docker-compose logs -f backend`
- Stop everything: `docker-compose down`
- Stop and remove volumes (clean restart): `docker-compose down -v`
- Rebuild a specific service: `docker-compose up -d --build backend`
- Check status: `docker-compose ps`

## Troubleshooting notes (common issues)

- Database connection errors: ensure the DB container is healthy (`docker-compose ps database`) and inspect `docker-compose logs database`.
- Backend startup problems: check `docker-compose logs backend` and verify environment variables in `docker-compose.yml`.
- Frontend not loading: inspect `docker-compose logs frontend` and `frontend/nginx.conf`.

## Project layout (quick overview)

```
CrmApp/
├── database/           # SQL scripts and DB Dockerfile
├── backend/            # .NET backend (CrmApp.API, application layers)
├── frontend/           # React app and nginx config
└── docker-compose.yml  # Orchestration for local development
```

## After the app is running

1. Open Swagger for the API and explore endpoints
2. Login to the frontend using the seeded admin user (see seed data)
3. Start developing and open PRs for review

## Support

If you hit an issue, open an issue with logs and steps to reproduce. I’ll respond and help debug.
