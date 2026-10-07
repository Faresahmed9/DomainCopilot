# ADR-001: Clean Architecture

* **Status:** Accepted
* **Date:** 2026-10-07
* **Decision Type:** Architecture

## 1. Context

The Domain Copilot platform contains multiple responsibilities including claims adjudication, policy retrieval, RAG processing, AI agents, deterministic business rules, authentication, persistence, and the Angular user interface.

The system also needs to remain maintainable and testable while allowing AI providers, databases, retrieval technologies, and external SDKs to evolve independently.

A tightly coupled architecture would make it difficult to test business rules independently and would make future changes to AI or infrastructure components more expensive.

## 2. Decision

The system will use a **Clean Architecture / layered architecture** approach with the following projects:

* `DomainCopilot.Domain`
* `DomainCopilot.Application`
* `DomainCopilot.Infrastructure`
* `DomainCopilot.Api`
* `DomainCopilot.UI`
* `DomainCopilot.Tests`

The dependency direction is:

```text
UI
 ↓
API
 ↓
Application
 ↓
Domain

Infrastructure → Application / Domain
```

The Domain and Application layers must not depend directly on infrastructure technologies such as Entity Framework Core, Gemini SDKs, vector databases, or HTTP clients.

Infrastructure-specific implementations are exposed to the Application layer through interfaces.

## 3. Layer Responsibilities

### Domain Layer

Contains core business entities, value-related concepts, and domain enums.

Examples:

* Claim
* Policy
* Coverage
* Exclusion
* AdjudicationDecision
* ApprovalRequest
* Tenant
* User

The Domain layer must remain independent of external frameworks whenever practical.

### Application Layer

Contains application use cases, business workflows, interfaces, deterministic services, and orchestration logic.

Examples:

* Claim adjudication
* Policy version matching
* RAG retrieval use cases
* Document processing workflow
* Tenant context abstraction
* Approval workflow
* Deterministic claim amount calculation

### Infrastructure Layer

Contains implementations that interact with external technologies.

Examples:

* Entity Framework Core
* SQL Server
* Gemini AI provider
* Embedding provider
* Document extraction
* Local document storage
* Repository implementations

### API Layer

Provides HTTP endpoints, authentication, authorization, dependency injection, middleware, Swagger/OpenAPI, and streaming endpoints.

### UI Layer

The Angular application provides the user-facing dashboard, claims screens, claim details, and human approval interface.

## 4. Dependency Rules

The following rules apply:

1. Domain must not depend on Application, Infrastructure, API, or UI.
2. Application must not directly depend on Infrastructure implementations.
3. Infrastructure implements interfaces defined by Application.
4. API composes the application and infrastructure dependencies through dependency injection.
5. UI communicates with the API through HTTP APIs.
6. Business-critical calculations must remain independent from the LLM.

## 5. Rationale

This architecture was selected because it provides:

* Separation of concerns.
* Better unit and integration testing.
* Easier replacement of infrastructure technologies.
* Clear boundaries around AI providers.
* Reduced coupling between business rules and external SDKs.
* Easier enforcement of tenant isolation.
* Better long-term maintainability.

For example, the claim amount calculation is implemented as deterministic application logic rather than delegated to an AI model. This allows the financial result to be tested independently of any AI provider.

Similarly, the application depends on AI abstractions such as `IAiProvider` rather than directly depending on a specific provider implementation.

## 6. Alternatives Considered

### A. Monolithic Layered Application

A single application containing controllers, business logic, database access, and AI calls was considered.

**Rejected because:**

* Creates strong coupling.
* Makes testing business logic harder.
* Makes provider replacement harder.
* Makes architectural boundaries less clear.

### B. Direct Infrastructure Access from Controllers

Controllers could directly use Entity Framework Core and AI SDKs.

**Rejected because:**

* Couples API endpoints to infrastructure.
* Makes business workflows difficult to reuse.
* Makes testing more difficult.
* Violates the desired dependency direction.

### C. Microservices

The system could be divided into multiple independent services.

**Rejected for the current scope because:**

* The assessment is an MVP-sized platform.
* Microservices would introduce unnecessary deployment and operational complexity.
* The current requirements can be satisfied with a modular monolith.

## 7. Consequences

### Positive

* Clear project responsibilities.
* Easier testing.
* Better maintainability.
* AI provider abstraction is possible.
* Infrastructure components can be replaced with less impact on business logic.
* Business-critical calculations remain deterministic.

### Negative

* More projects and interfaces introduce some initial complexity.
* Dependency injection configuration becomes more important.
* Developers must respect the dependency boundaries.

## 8. Compliance

The implementation follows this decision through:

* Separate Domain, Application, Infrastructure, API, UI, and Tests projects.
* Repository interfaces in the Application layer.
* Repository implementations in Infrastructure.
* AI provider abstraction through `IAiProvider`.
* Deterministic claim calculation in Application services.
* Dependency injection for infrastructure implementations.
* Angular UI communicating with the API rather than directly accessing the database.

## 9. Related Requirements

This decision supports the following assessment requirements:

* Clean Architecture.
* Provider abstraction.
* Testability.
* Deterministic financial calculations.
* Multi-tenancy isolation.
* Maintainability and extensibility.
