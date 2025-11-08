# Contributing to CRM Application

Thank you for your interest in contributing to this CRM Application! This document provides guidelines and instructions for contributing.

## 🤝 How to Contribute

### Reporting Bugs

1. Check if the bug has already been reported in [Issues](../../issues)
2. If not, create a new issue with:
   - Clear, descriptive title
   - Steps to reproduce
   - Expected vs actual behavior
   - Screenshots (if applicable)
   - Environment details (OS, Docker version, etc.)

### Suggesting Features

1. Check if the feature has already been suggested
2. Create a new issue with:
   - Clear description of the feature
   - Use cases and benefits
   - Possible implementation approach

### Pull Requests

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Make your changes
4. Write/update tests if applicable
5. Update documentation
6. Commit your changes (`git commit -m 'Add amazing feature'`)
7. Push to the branch (`git push origin feature/amazing-feature`)
8. Open a Pull Request

## 💻 Development Setup

### Prerequisites
- Docker Desktop
- Git
- Code editor (VS Code recommended)

### Setup Steps

1. **Clone your fork**
   ```bash
   git clone https://github.com/YOUR_USERNAME/CrmApp.git
   cd CrmApp
   ```

2. **Configure environment**
   ```bash
   cp .env.example .env
   # Edit .env with your settings
   ```

3. **Start development environment**
   ```bash
   docker-compose up -d
   ```

4. **Backend development** (optional, without Docker)
   ```bash
   cd backend
   dotnet restore
   dotnet run --project CrmApp.API
   ```

5. **Frontend development** (optional, without Docker)
   ```bash
   cd frontend
   npm install
   npm run dev
   ```

## 📝 Coding Standards

### Backend (.NET)
- Follow C# naming conventions
- Use async/await for asynchronous operations
- Add XML documentation for public APIs
- Follow SOLID principles
- Use dependency injection
- Write unit tests for business logic

### Frontend (React/TypeScript)
- Use TypeScript for type safety
- Follow React best practices
- Use functional components with hooks
- Keep components small and focused
- Use Material-UI components consistently
- Write meaningful component and variable names

### Database
- Use migrations for schema changes
- Never modify `*_seed_data.sql` directly in production
- Add indexes for frequently queried columns
- Document complex queries

### Git Commit Messages
Follow conventional commits:
- `feat:` New feature
- `fix:` Bug fix
- `docs:` Documentation changes
- `style:` Code style changes (formatting)
- `refactor:` Code refactoring
- `test:` Adding/updating tests
- `chore:` Maintenance tasks

Example:
```
feat: add email notification for new leads
fix: resolve null reference in company service
docs: update API documentation for contacts endpoint
```

## 🧪 Testing

### Backend Tests
```bash
cd backend
dotnet test
```

### Frontend Tests
```bash
cd frontend
npm test
```

### Integration Tests
```bash
# Start all services first
docker-compose up -d

# Run integration tests
# (Implementation pending)
```

## 📚 Documentation

When adding new features:
1. Update relevant documentation in `/docs`
2. Add API documentation in Swagger comments
3. Update README.md if needed
4. Add inline code comments for complex logic

## 🔍 Code Review Process

1. All PRs require at least one approval
2. CI/CD checks must pass
3. Code should follow project standards
4. Tests should be included for new features
5. Documentation should be updated

## 🐛 Debugging

### View Logs
```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f backend
docker-compose logs -f frontend
docker-compose logs -f database
```

### Access Database
```bash
docker exec -it crm-database psql -U crmuser -d crmdb
```

### Access Redis
```bash
docker exec -it crm-redis redis-cli -a YOUR_REDIS_PASSWORD
```

### View Elasticsearch Logs
- Open Kibana: http://localhost:5601
- Navigate to Discover
- Create index pattern: `crm-logs-*`

## 🚀 Release Process

1. Update version in relevant files
2. Update CHANGELOG.md
3. Create a release branch
4. Tag the release
5. Deploy to production
6. Announce the release

## 📞 Getting Help

- **Documentation**: Check [SETUP.md](SETUP.md) and [README.md](README.md)
- **Issues**: Search existing issues or create a new one
- **Discussions**: Use GitHub Discussions for questions

## 📜 License

By contributing, you agree that your contributions will be licensed under the MIT License.

---

Thank you for contributing! 🎉
