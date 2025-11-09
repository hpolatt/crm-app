# CRM Application

I built this CRM as a practical, containerized starter for small-to-medium projects. It combines a .NET 8 API backend with a React + TypeScript frontend and includes PostgreSQL, Redis, Elasticsearch and Kibana in the development stack so you can run everything locally with Docker.

License: This repository is published under the Business Source License 1.1 (BSL-1.1). Commercial use and offering this project as a hosted service are restricted until 2026-11-09, after which the code will be available under the MIT license. See `LICENSE` for details.

## What this repo includes

- Core modules: Companies, Contacts, Leads, Opportunities, Notes
- Role-based access control (SuperAdmin, Admin, User)
- JWT authentication with refresh tokens
- Request/response logging to Elasticsearch (viewable in Kibana)
- Redis caching for common queries
- Docker Compose setup for local development and testing

## Tech stack

- Backend: .NET 8, Entity Framework Core, MediatR, Serilog
- Frontend: React 18, TypeScript, Vite, Material-UI
- Database: PostgreSQL
- Cache: Redis
- Logging: Elasticsearch + Kibana
- Containerization: Docker & Docker Compose

---

## Quick start (development)

These are the steps I use locally. It should work on macOS, Linux, and Windows with Docker Desktop installed.

Prerequisites:

- Docker Desktop (v20.10+)
- Docker Compose (v2+)

1. Copy the example environment file and edit it for your machine:

```bash
cp .env.example .env
# edit .env and set local passwords/ports. Do NOT commit .env
```

2. Start everything with Docker Compose (this will also use `docker-compose.override.yml` if present for development overrides):

```bash
docker-compose up -d
```

3. Open the services:

- Frontend: http://localhost (or the port defined in `docker-compose.yml`)
- Backend API: http://localhost:5001 (Swagger: `/swagger`)
- Kibana: http://localhost:5601

Tips:

- To tail logs for a specific service: `docker-compose logs -f <service>`
- `docker-compose.override.yml` is ignored by git and designed for local dev mounts (hot reload)

---

## Run locally (without Docker)

If you prefer running services directly during development:

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

---

## Environment & secrets

I keep placeholders in `appsettings.json` and provide `.env.example` as a template. For local development use `appsettings.Development.json` or environment variables. In production, I expect secrets to come from the environment or a secrets manager. If you use Docker Compose you can provide values via an `.env` file or CI/CD secrets.

Note: ASP.NET Core supports hierarchical env var names, e.g. `ConnectionStrings__DefaultConnection`.

---

## Tests

If there are tests in the repo, run them like this:

Backend tests

```bash
cd backend
dotnet test
```

Frontend tests

```bash
cd frontend
npm run test
```

---

## Contributing

If you want to contribute, please open an issue first to discuss larger changes. Keep PRs focused and include a clear description of the change and any setup steps.

See `CONTRIBUTING.md` for more details.

---

## License

This project is MIT licensed — see the `LICENSE` file.

---

If you run into problems, open an issue in this repository and I will take a look.

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
