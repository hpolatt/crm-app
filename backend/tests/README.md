# Backend Tests

This directory contains backend test projects for the CRM application.

## Test Projects

### PktApp.UnitTests
Unit tests for business logic, services, and validation rules. Uses InMemory database for data isolation.

**Test Coverage:**
- ✅ AuthService (Login, Register, validation)
- ✅ Note entity constraint validation
- ✅ Test data factory helpers

**Running unit tests:**
```bash
cd backend/tests/PktApp.UnitTests
dotnet test
```

**Results:** 14/14 tests passing ✅

---

### PktApp.IntegrationTests
Integration tests for controllers and endpoints using WebApplicationFactory.

**Test Coverage:**
- ⚠️ Auth controller (Register, Login)
- ⚠️ Users controller (/users/basic, authorization)
- ✅ Unauthenticated access tests
- ✅ Invalid credentials tests

**Running integration tests:**
```bash
cd backend/tests/PktApp.IntegrationTests
dotnet test
```

**Results:** 5/9 tests passing (4 fail due to seed data issues - can be fixed with proper role/user seeding in test factory)

**Known Issues:**
- Some tests fail because Role seeding happens per-test but database context is separate per factory instance
- Solution: Use a shared context or better seed approach in CustomWebApplicationFactory

---

## Quick Commands

Run all backend tests:
```bash
cd backend
dotnet test
```

Run with detailed output:
```bash
dotnet test --verbosity normal
```

Run only unit tests:
```bash
dotnet test tests/PktApp.UnitTests
```

Run only integration tests:
```bash
dotnet test tests/PktApp.IntegrationTests
```

---

## Test Structure

```
tests/
├── PktApp.UnitTests/
│   ├── Helpers/
│   │   └── TestDataFactory.cs       # Factory for creating test entities
│   ├── Services/
│   │   └── AuthServiceTests.cs      # AuthService unit tests
│   └── Validation/
│       └── NoteValidationTests.cs   # Note constraint validation tests
│
└── PktApp.IntegrationTests/
    ├── CustomWebApplicationFactory.cs   # Custom factory with InMemory DB
    └── Controllers/
        ├── AuthControllerTests.cs       # Auth endpoint integration tests
        └── UsersControllerTests.cs      # Users endpoint integration tests
```

---

## Next Steps

1. **Fix integration test seeding** — Refactor role/user seeding in CustomWebApplicationFactory to ensure consistent database state across tests
2. **Add more unit tests** — Cover CompanyService, ContactService, NoteService business logic
3. **Add Company CRUD integration tests** — Test full CRUD operations with authorization
4. **Add Note entity constraint tests** — Integration test ensuring DB constraint is enforced
5. **CI Integration** — Add GitHub Actions workflow to run tests on PR (see TESTS.md for example)

---

## Dependencies

- xUnit — Test framework
- Moq — Mocking library (unit tests)
- FluentAssertions — Readable assertions
- Microsoft.AspNetCore.Mvc.Testing — Integration testing with TestServer
- Microsoft.EntityFrameworkCore.InMemory — In-memory database for tests

---

For detailed testing strategy and CI setup, see [TESTS.md](../../TESTS.md).
