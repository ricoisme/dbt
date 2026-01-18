---
agent: agent
tools: ['search/changes', 'search/codebase', 'edit/editFiles', 'read/problems']
description: 'Create ASP.NET Code API endpoints with proper OpenAPI documentation'
---
# ASP.NET Core Web API (.NET 8, Layered Architecture)

You are a senior .NET backend engineer.

Your task is to create an ASP.NET Core Web API project using **.NET 8**, following a **layered architecture**, based on the **default ASP.NET Core Web API templates**.

## Project Requirements

### 1. Framework & Templates
- Target framework: **.NET 8**
- Use the **default ASP.NET Core Web API template** (not Minimal API)
- Do not use third-party frameworks unless explicitly requested

### 2. Solution Structure

Create the solution using the following folder structure:

src/
└── backend/
  ├── Api/ # ASP.NET Core Web API project
  ├── Application/ # Application layer (business logic, DTOs, interfaces)
  ├── Domain/ # Domain layer (entities, value objects, enums)
  └── Infrastructure/ # Infrastructure layer (EF Core, repositories, external services)

tests/
└── Api.Tests/ # MSTest test project


### 3. Layered Architecture Rules

Follow standard layered architecture principles:

- **Api**
  - Controllers only
  - Request/Response models
  - No business logic
- **Application**
  - Application services
  - DTOs
  - Interfaces (e.g. services)
- **Domain**
  - Domain entities
  - Domain rules and core logic
  - No infrastructure dependencies
- **Infrastructure**
  - Database context
  - Repository implementations
  - External service integrations(e.g. email, api clients)

### 4. Dependency Rules

- Api → Application
- Application → Domain
- Infrastructure → Application, Domain
- Domain must not depend on any other layer

### 5. Test Project

- Testing framework: **MSTest**
- Use the **default MSTest test project template**
- Test project location: `tests/`
- Naming convention: `*.Tests`
- Focus on:
  - Application layer unit tests
  - API controller tests where appropriate

### 6. Coding Conventions

- Use async/await for all I/O operations
- Follow .NET naming conventions
- Enable nullable reference types
- Use dependency injection
- Prefer constructor injection
- Keep code clean, readable, and production-ready

### 7. Output Expectations

When generating code:
- Explain the purpose of each project briefly
- Provide clear steps for creating the solution and projects
- Include example commands using `dotnet new` and `dotnet sln`
- Ensure all projects compile successfully together
- Do not include unnecessary boilerplate or sample business logic unless required

---

Generate the solution structure, project setup steps, and example code accordingly.
