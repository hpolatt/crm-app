# CRM Application# CRM Application



Modern, enterprise-grade CRM (Customer Relationship Management) system built with **.NET 8**, **React**, **TypeScript**, and **PostgreSQL**.Kapsamlı bir CRM (Customer Relationship Management) uygulaması. Backend .NET 8, Frontend React + TypeScript ile geliştirilmiştir.



## 🚀 Features## Proje Yapısı



### Core Modules```

- **👥 Companies** - Complete company management with industry, website, contact infoCrmApp/

- **📇 Contacts** - Contact management with company association├── backend/              # .NET 8 Backend API

- **🎯 Leads** - Lead tracking with status, source, and score management│   ├── CrmApp.API/      # Web API Layer

- **💼 Opportunities** - Sales pipeline with stages, values, and probability tracking│   ├── CrmApp.Application/  # Application Layer (CQRS, MediatR)

- **📝 Notes** - Contextual notes for companies, contacts, leads, and opportunities│   ├── CrmApp.Core/     # Core Layer (Interfaces, Repository Pattern)

- **⚙️ System Settings** - Configurable application settings│   ├── CrmApp.Domain/   # Domain Layer (Entities)

- **👤 User Management** - Role-based access control (SuperAdmin, Admin, User)│   └── CrmApp.Infrastructure/  # Infrastructure Layer (EF Core, Redis)

├── frontend/            # React + TypeScript Frontend

### Technical Features│   ├── src/

- **🔐 JWT Authentication** - Secure token-based authentication│   │   ├── components/  # Reusable UI components

- **📊 Activity Logging** - Elasticsearch-powered request/response logging│   │   ├── pages/       # Page components

- **🔍 Kibana Dashboard** - Real-time log visualization and monitoring│   │   ├── store/       # Redux Toolkit store

- **⚡ Redis Caching** - High-performance data caching│   │   ├── services/    # API services

- **🐳 Docker** - Fully containerized deployment│   │   └── types/       # TypeScript types

- **📱 Responsive UI** - Material-UI based modern interface├── database/            # PostgreSQL initialization scripts

- **🔄 Real-time Updates** - Instant data synchronization│   ├── 01_init_database.sql

│   ├── 02_create_tables.sql

## 📋 Prerequisites│   ├── 03_create_indexes.sql

│   ├── 04_create_triggers.sql

- **Docker Desktop** (v20.10+)│   ├── 05_seed_data.sql

- **Docker Compose** (v2.0+)│   └── Dockerfile

└── docker-compose.yml   # Docker Compose configuration

That's it! No need to install .NET, Node.js, PostgreSQL, or any other dependency.```



## ⚡ Quick Start## Teknolojiler



### 1. Clone the repository### Backend (.NET 8)

```bash- **Architecture**: Layered Architecture (Katmanlı Mimari)

git clone <your-repo-url>- **Patterns**: 

cd CrmApp  - Generic Repository Pattern

```  - Unit of Work Pattern

  - CQRS (MediatR)

### 2. Configure environment variables- **Database**: PostgreSQL + Entity Framework Core

```bash- **Cache**: Redis (StackExchange.Redis)

# Copy example environment file- **Authentication**: JWT Bearer Authentication

cp .env.example .env- **Validation**: FluentValidation

- **Logging**: Serilog

# Edit .env file and update passwords/secrets- **Mapping**: AutoMapper

nano .env  # or use your preferred editor- **API Documentation**: Swagger/OpenAPI

```

### Frontend (React + TypeScript)

**Important:** Change these values in `.env`:- **Framework**: React 18 with TypeScript

- `POSTGRES_PASSWORD`- **Build Tool**: Vite

- `REDIS_PASSWORD`- **UI Library**: Material-UI (MUI)

- `ELASTIC_PASSWORD`- **State Management**: Redux Toolkit

- `JWT_SECRET` (must be at least 32 characters)- **Data Fetching**: React Query (TanStack Query)

- **Form Management**: React Hook Form + Yup

### 3. Start all services- **Routing**: React Router v6

```bash- **HTTP Client**: Axios with interceptors

docker-compose up -d

```### Database

- **PostgreSQL 16**: Relational database

This will start:- **Schemas**: 

- PostgreSQL (Port: 5433)  - `auth` - Authentication & Authorization

- Redis (Port: 6379)  - `crm` - CRM entities (Companies, Contacts, Leads, etc.)

- Elasticsearch (Port: 9200)  - `audit` - Audit logging

- Kibana (Port: 5601)- **Features**: 

- Backend API (Port: 5001)  - Triggers for auto-updates

- Frontend (Port: 80)  - Indexes for performance

  - Full-text search support

### 4. Access the application

### DevOps

- **Frontend**: http://localhost- **Containerization**: Docker & Docker Compose

- **Backend API**: http://localhost:5001- **Web Server**: Nginx (for frontend)

- **Swagger Documentation**: http://localhost:5001/swagger

- **Kibana**: http://localhost:5601## Özellikler



### 5. Login### Backend Features

- ✅ JWT Authentication & Authorization

Default credentials:- ✅ Role-based Access Control (RBAC)

- **Email**: `admin@crm.com`- ✅ Generic Repository Pattern

- **Password**: `Admin@123`- ✅ Unit of Work Pattern

- ✅ CQRS with MediatR

## 📚 Documentation- ✅ Global Exception Handling

- ✅ Redis Caching

- [Setup Guide](SETUP.md) - Detailed installation and configuration- ✅ Async/Await operations

- [Architecture](docs/ARCHITECTURE.md) - System architecture and design- ✅ Fluent Validation

- [API Documentation](docs/API.md) - Backend API reference- ✅ AutoMapper

- [Database Schema](docs/DATABASE.md) - Database structure and relationships- ✅ Serilog Logging

- ✅ Swagger Documentation

## 🛠️ Tech Stack- ✅ Soft Delete support

- ✅ Audit Trail

### Backend

- **.NET 8** - Modern C# framework### Frontend Features

- **Entity Framework Core** - ORM for PostgreSQL- ✅ JWT-based Authentication

- **MediatR** - CQRS pattern implementation- ✅ Private Routes

- **AutoMapper** - Object-to-object mapping- ✅ Responsive Design

- **Serilog** - Structured logging- ✅ Dark/Light Theme support

- **JWT** - Authentication- ✅ Form Validation

- ✅ API Error Handling

### Frontend- ✅ Loading States

- **React 18** - UI library- ✅ Toast Notifications

- **TypeScript** - Type-safe JavaScript

- **Material-UI (MUI)** - Component library### CRM Modules

- **Vite** - Fast build tool- 📊 Dashboard

- **Axios** - HTTP client- 🏢 Companies Management

- **React Router** - Client-side routing- 👥 Contacts Management

- 📈 Leads Management

### Infrastructure- 💼 Opportunities Management

- **PostgreSQL 16** - Relational database- 📅 Activities & Tasks

- **Redis 7** - Caching layer- 💰 Deals Management

- **Elasticsearch 8** - Log storage and search- 📦 Products Management

- **Kibana 8** - Log visualization- 📄 Quotes Management

- **Docker** - Containerization

- **Nginx** - Frontend web server## Kurulum



## 🏗️ Project Structure### Önkoşullar

- Docker & Docker Compose

```- .NET 8 SDK (geliştirme için)

CrmApp/- Node.js 18+ (geliştirme için)

├── backend/                    # .NET 8 Backend

│   ├── CrmApp.API/            # REST API Controllers### Docker ile Çalıştırma

│   ├── CrmApp.Application/    # Business Logic (MediatR)

│   ├── CrmApp.Core/           # DTOs, Interfaces1. **Repository'yi klonlayın:**

│   ├── CrmApp.Domain/         # Entities, Models```bash

│   └── CrmApp.Infrastructure/ # Data Access, Servicesgit clone <repository-url>

│cd CrmApp

├── frontend/                   # React Frontend```

│   ├── src/

│   │   ├── components/        # Reusable components2. **Docker Compose ile tüm servisleri başlatın:**

│   │   ├── pages/             # Page components```bash

│   │   ├── services/          # API servicesdocker-compose up -d

│   │   ├── types/             # TypeScript types```

│   │   └── utils/             # Helper functions

│   └── nginx.conf             # Nginx configurationBu komut:

│- PostgreSQL veritabanını başlatır ve SQL scriptlerini çalıştırır

├── database/                   # PostgreSQL- Redis cache servisini başlatır

│   ├── 01_init_database.sql  # Schema creation- Backend API'yi derler ve çalıştırır (Port: 5000)

│   ├── 02_create_tables.sql  # Table definitions- Frontend'i derler ve Nginx ile servis eder (Port: 80)

│   ├── 03_create_indexes.sql # Performance indexes

│   ├── 04_create_functions_triggers.sql # DB logic3. **Uygulamaya erişin:**

│   └── 05_seed_data.sql      # Initial data- Frontend: http://localhost

│- Backend API: http://localhost:5000

├── docs/                       # Documentation- Swagger UI: http://localhost:5000/swagger

├── docker-compose.yml          # Container orchestration

├── .env.example               # Environment template### Manuel Kurulum

└── README.md                  # This file

```#### Backend



## 🔧 Development```bash

cd backend

### Backend Developmentdotnet restore

```bashdotnet build

cd backenddotnet run --project CrmApp.API

dotnet restore```

dotnet run --project CrmApp.API

```#### Frontend



### Frontend Development```bash

```bashcd frontend

cd frontendnpm install

npm installnpm run dev

npm run dev```

```

## Veritabanı

### Database Migrations

```bashVeritabanı otomatik olarak oluşturulur ve seed data ile doldurulur.

cd backend/CrmApp.Infrastructure

dotnet ef migrations add MigrationName### Default Admin Kullanıcı

dotnet ef database update- Email: `admin@crm.com`

```- Password: `Admin@123`



## 📊 Monitoring### Manuel SQL Çalıştırma

Eğer SQL scriptlerini manuel çalıştırmak isterseniz:

### View Logs

```bash```bash

# All servicescd database

docker-compose logs -fpsql -U crmuser -d crmdb -f 01_init_database.sql

psql -U crmuser -d crmdb -f 02_create_tables.sql

# Specific servicepsql -U crmuser -d crmdb -f 03_create_indexes.sql

docker-compose logs -f backendpsql -U crmuser -d crmdb -f 04_create_triggers.sql

docker-compose logs -f frontendpsql -U crmuser -d crmdb -f 05_seed_data.sql

``````



### Kibana Dashboard## Environment Variables

1. Open http://localhost:5601

2. Go to **Analytics → Discover**### Backend (.env veya appsettings.json)

3. Create data view: `crm-logs-*````json

4. View real-time logs with filters{

  "ConnectionStrings": {

## 🧪 Testing    "DefaultConnection": "Host=localhost;Port=5432;Database=crmdb;Username=crmuser;Password=CrmPass@2024",

    "RedisConnection": "localhost:6379"

### Run Backend Tests  },

```bash  "JwtSettings": {

cd backend    "Secret": "YourSuperSecretKeyForJWTTokenGeneration123!@#",

dotnet test    "Issuer": "CrmApp",

```    "Audience": "CrmAppUsers",

    "ExpiryMinutes": 60

### Run Frontend Tests  }

```bash}

cd frontend```

npm test

```### Frontend (.env)

```

## 🚢 DeploymentVITE_API_URL=http://localhost:5000/api

```

### Production Build

```bash## API Endpoints

# Build all services

docker-compose build### Authentication

- `POST /api/auth/login` - Login

# Start in production mode- `POST /api/auth/register` - Register

docker-compose up -d- `POST /api/auth/refresh-token` - Refresh JWT token

```- `POST /api/auth/logout` - Logout



### Environment Variables### Companies

Ensure all sensitive variables in `.env` are properly configured:- `GET /api/companies` - List all companies

- Database credentials- `GET /api/companies/{id}` - Get company by ID

- Redis password- `POST /api/companies` - Create company

- JWT secret key- `PUT /api/companies/{id}` - Update company

- Elasticsearch credentials- `DELETE /api/companies/{id}` - Delete company (soft delete)



## 🤝 Contributing### Contacts

- `GET /api/contacts` - List all contacts

See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.- `GET /api/contacts/{id}` - Get contact by ID

- `POST /api/contacts` - Create contact

## 📄 License- `PUT /api/contacts/{id}` - Update contact

- `DELETE /api/contacts/{id}` - Delete contact

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

### Leads

## 🆘 Support- `GET /api/leads` - List all leads

- `GET /api/leads/{id}` - Get lead by ID

For issues and questions:- `POST /api/leads` - Create lead

- Open an issue on GitHub- `PUT /api/leads/{id}` - Update lead

- Check the [Setup Guide](SETUP.md)- `DELETE /api/leads/{id}` - Delete lead

- Review [API Documentation](docs/API.md)

### Opportunities

## 🎯 Roadmap- `GET /api/opportunities` - List all opportunities

- `GET /api/opportunities/{id}` - Get opportunity by ID

- [ ] Email notifications- `POST /api/opportunities` - Create opportunity

- [ ] Task management- `PUT /api/opportunities/{id}` - Update opportunity

- [ ] Calendar integration- `DELETE /api/opportunities/{id}` - Delete opportunity

- [ ] Advanced reporting

- [ ] Mobile app## Development

- [ ] Multi-language support

- [ ] Dark mode### Backend Geliştirme

- [ ] Export/Import functionality

```bash

## ⭐ Show Your Supportcd backend

dotnet watch run --project CrmApp.API

If you find this project helpful, please give it a star!```



---### Frontend Geliştirme



**Built with ❤️ using .NET 8 and React**```bash

cd frontend
npm run dev
```

### Migration Oluşturma

```bash
cd backend/CrmApp.Infrastructure
dotnet ef migrations add MigrationName -s ../CrmApp.API
dotnet ef database update -s ../CrmApp.API
```

## Testing

```bash
# Backend tests
cd backend
dotnet test

# Frontend tests
cd frontend
npm run test
```

## Production Deployment

1. Environment variables'ı production değerleriyle güncelleyin
2. SSL sertifikalarını yapılandırın
3. Docker Compose'u production modda çalıştırın:

```bash
docker-compose -f docker-compose.prod.yml up -d
```

## Lisans

MIT

## Katkıda Bulunma

Pull request'ler memnuniyetle karşılanır. Büyük değişiklikler için lütfen önce bir issue açarak neyi değiştirmek istediğinizi tartışın.

## İletişim

Sorularınız için lütfen issue açın.
