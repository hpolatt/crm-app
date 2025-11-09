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

# Contributing

Thanks for wanting to help improve this project — I appreciate it. Below I explain how I prefer contributions to be made so reviews go smoothly and the repository stays easy to maintain.

## Reporting bugs

If you find a bug, please search existing issues first. If none exist, open a new issue with:

- A short, descriptive title
- Steps to reproduce (minimal, repeatable)
- Expected vs actual behavior
- Environment details (OS, Docker version, any relevant logs)
- Screenshots or curl examples when helpful

That helps me reproduce and prioritize the fix.

License note: This project is distributed under the Business Source License 1.1 (BSL-1.1). Commercial use and offering the software as a hosted service are restricted until 2026-11-09. See `LICENSE` for details.

## Suggesting features

Before implementing a big feature, open an issue describing the use case and benefits. Include a short proposal for the implementation and any UI changes. I’ll give feedback and we can iterate on the design before you start coding.

## Pull requests

My preferred workflow:

1. Fork the repo and create a topic branch: `git checkout -b feat/my-feature`
2. Keep changes focused and small
3. Add or update tests where appropriate
4. Update documentation if behavior changes
5. Commit with clear messages (see below)
6. Push the branch and open a PR describing the change and any setup steps

I try to review PRs promptly; expect a request for small changes sometimes.

### Commit message style

I follow conventional commit prefixes. Example messages:

```
feat: add email notification for new leads
fix: handle nulls in company service
docs: update API docs for contacts endpoint
```

## Development setup

Quick steps to get the project running locally (I recommend Docker for a reproducible environment):

1. Clone your fork and add the upstream remote

```bash
git clone https://github.com/YOUR_USERNAME/crm-app.git
cd crm-app
git remote add upstream https://github.com/hpolatt/crm-app.git
```

2. Copy environment template and edit local values:

```bash
cp .env.example .env
# edit .env (do not commit .env)
```

3. Start services:

```bash
docker-compose up -d
```

Alternatively run backend/frontend locally without Docker (see `SETUP.md`).

## Coding standards

Backend (.NET): follow C# conventions, prefer `async/await`, use dependency injection and write unit tests for business logic.

Frontend (React/TypeScript): use TypeScript, functional components and hooks, favor small focused components and clear naming.

Database: use EF Core migrations for schema changes and document complex queries.

## Tests

Run unit tests with:

```bash
cd backend
dotnet test

cd frontend
npm run test
```

Integration tests are welcome; if you add them, document how to run them in this file.

## Code review

- All PRs should have at least one reviewer
- CI must pass before merging
- Include tests for new behavior
- Keep changes focused and easy to review

## Debugging and logs

To view logs:

```bash
docker-compose logs -f
docker-compose logs -f backend
```

To access the database or Redis (if running in Docker):

```bash
docker exec -it crm-database psql -U crmuser -d crmdb
docker exec -it crm-redis redis-cli -a <REDIS_PASSWORD>
```

For Elasticsearch logs, open Kibana at `http://localhost:5601` and create the `crm-logs-*` index pattern.

## Release process

When preparing a release I:

1. Update version/CHANGELOG
2. Create a release branch and tag the release
3. Merge and deploy

If you want to help with releases, open an issue and I’ll explain the process.

## Getting help

If you get stuck, open an issue and include the steps you followed and any errors or logs. I’ll respond as soon as I can.

## License

By contributing you agree your changes will be licensed under the MIT License (see `LICENSE`).

Thanks again — I appreciate your help!
