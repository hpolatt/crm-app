# CRM Application - Setup Instructions

## Quick Start Guide

### 1. Prerequisites
Make sure you have the following installed:
- Docker Desktop (https://www.docker.com/products/docker-desktop)
- Docker Compose (included with Docker Desktop)

### 2. Clone and Navigate
```bash
cd /Users/huseyin/Dev/CrmApp
```

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

## Manual Database Setup (Optional)

If you prefer to run SQL scripts manually:

### Step 1: Start only the database
```bash
docker-compose up -d database
```

### Step 2: Connect to PostgreSQL
```bash
docker exec -it crm-database psql -U crmuser -d crmdb
```

### Step 3: Run scripts in order
```bash
# From your host machine
cd database
docker exec -i crm-database psql -U crmuser -d crmdb < 01_init_database.sql
docker exec -i crm-database psql -U crmuser -d crmdb < 02_create_tables.sql
docker exec -i crm-database psql -U crmuser -d crmdb < 03_create_indexes.sql
docker exec -i crm-database psql -U crmuser -d crmdb < 04_create_triggers.sql
docker exec -i crm-database psql -U crmuser -d crmdb < 05_seed_data.sql
```

### Step 4: Start remaining services
```bash
docker-compose up -d
```

## Development Mode (Without Docker)

### Backend Development
```bash
cd backend
dotnet restore
dotnet run --project CrmApp.API
```

Backend will be available at: http://localhost:5000

### Frontend Development
```bash
cd frontend
npm install
npm run dev
```

Frontend will be available at: http://localhost:3000

## Useful Docker Commands

### View logs
```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f backend
docker-compose logs -f frontend
docker-compose logs -f database
```

### Stop all services
```bash
docker-compose down
```

### Stop and remove volumes (clean restart)
```bash
docker-compose down -v
```

### Rebuild specific service
```bash
docker-compose up -d --build backend
docker-compose up -d --build frontend
```

### Check service status
```bash
docker-compose ps
```

## Troubleshooting

### Database connection issues
1. Check if database is healthy:
```bash
docker-compose ps database
```

2. Check database logs:
```bash
docker-compose logs database
```

3. Verify database is accessible:
```bash
docker exec -it crm-database psql -U crmuser -d crmdb -c "SELECT 1;"
```

### Backend not starting
1. Check backend logs:
```bash
docker-compose logs backend
```

2. Verify environment variables in docker-compose.yml

3. Ensure database is running and healthy before backend starts

### Frontend not loading
1. Check frontend logs:
```bash
docker-compose logs frontend
```

2. Verify nginx configuration in frontend/nginx.conf

3. Check if backend is accessible from browser: http://localhost:5000/swagger

## Project Structure Overview

```
CrmApp/
├── database/           # PostgreSQL setup
│   ├── Dockerfile     # Database container config
│   └── *.sql          # Database scripts (RUN THESE IN ORDER)
│
├── backend/           # .NET 8 API
│   ├── Dockerfile     # Backend container config
│   ├── CrmApp.API/    # API endpoints
│   ├── CrmApp.Application/  # Business logic
│   ├── CrmApp.Core/   # Interfaces
│   ├── CrmApp.Domain/ # Entities
│   └── CrmApp.Infrastructure/  # Data access
│
├── frontend/          # React + TypeScript
│   ├── Dockerfile     # Frontend container config
│   ├── nginx.conf     # Web server config
│   └── src/           # React application
│
└── docker-compose.yml # Orchestration
```

## Next Steps

After the application is running:
1. ✅ Explore the API documentation at http://localhost:5000/swagger
2. ✅ Login to the frontend at http://localhost with admin credentials
3. ✅ Check database tables and data in PostgreSQL
4. ✅ Start developing!

## Support

If you encounter any issues:
1. Check the logs: `docker-compose logs -f`
2. Ensure all ports (80, 5001, 5432, 5433, 6379, 9200, 5601) are available
3. Try a clean restart: `docker-compose down -v && docker-compose up --build`
4. Check `.env` file for correct configuration
5. Verify Docker Desktop is running and has enough resources (4GB+ RAM recommended)
