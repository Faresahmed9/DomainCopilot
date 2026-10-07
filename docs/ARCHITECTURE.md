# Architecture Document

## 1. Overview

**Project:** Domain Copilot — Agentic RAG Platform
**Domain:** D2 — Insurance Claims Adjudication
**Twist:** T0 — Multi-Tenancy

The system follows Clean Architecture principles and separates business rules from infrastructure concerns such as databases, AI providers, document processing, and external services.

The architecture is designed around four main concerns:

1. Business domain and rules.
2. Application workflows and orchestration.
3. Infrastructure integrations.
4. API and user interface delivery.

---

# 2. Architecture Style

The system uses a layered Clean Architecture approach.

```text
                    ┌──────────────────────┐
                    │     Angular UI       │
                    │ DomainCopilot.UI     │
                    └──────────┬───────────┘
                               │
                               ▼
                    ┌──────────────────────┐
                    │     API Layer        │
                    │ DomainCopilot.Api    │
                    └──────────┬───────────┘
                               │
                               ▼
                    ┌──────────────────────┐
                    │ Application Layer    │
                    │ Use Cases / Agents   │
                    │ Tools / Interfaces   │
                    └──────────┬───────────┘
                               │
                               ▼
                    ┌──────────────────────┐
                    │    Domain Layer      │
                    │ Entities / Rules     │
                    └──────────────────────┘
                               ▲
                               │
                    ┌──────────┴───────────┐
                    │ Infrastructure Layer │
                    │ DB / RAG / AI / Files│
                    └──────────────────────┘
```

The dependency direction is inward.

The Domain layer contains the core business concepts and must remain independent from external technologies.

---

# 3. Solution Structure

```text
DomainCopilot/
│
├── DomainCopilot.Domain/
│   ├── Entities/
│   └── Enums/
│
├── DomainCopilot.Application/
│   ├── DTOs/
│   ├── Interfaces/
│   ├── Services/
│   ├── Tenant/
│   ├── Tools/
│   ├── Agents/
│   └── UseCases/
│
├── DomainCopilot.Infrastructure/
│   ├── AI/
│   ├── Embeddings/
│   ├── DocumentProcessing/
│   ├── Persistence/
│   ├── Repositories/
│   └── Storage/
│
├── DomainCopilot.Api/
│   ├── Controllers/
│   ├── Tenant/
│   ├── Authentication/
│   └── Program.cs
│
├── DomainCopilot.Tests/
│   └── GoldenDataset/
│
├── DomainCopilot.UI/
│   └── Angular application
│
└── docs/
    ├── BRD.md
    ├── SYSTEM-DESIGN.md
    └── ARCHITECTURE.md
```

---

# 4. Domain Layer

Project:

`DomainCopilot.Domain`

The Domain layer represents insurance business concepts.

## Main Entities

* Tenant
* User
* Policy
* Claim
* Coverage
* Exclusion
* Anomaly
* AdjudicationDecision
* ApprovalRequest
* Document
* DocumentChunk

## Domain Enums

Examples include:

* UserRole
* ClaimStatus
* DecisionStatus
* ApprovalStatus
* AnomalySeverity

The Domain layer does not reference:

* ASP.NET Core
* Entity Framework Core
* Gemini
* Angular
* SQL Server
* HTTP clients

This keeps the core business model independent from implementation technologies.

---

# 5. Application Layer

Project:

`DomainCopilot.Application`

The Application layer contains application-specific behavior and coordinates business operations.

Responsibilities include:

* Use cases.
* Repository interfaces.
* AI provider abstraction.
* Tenant context abstraction.
* Agent interfaces.
* Tool interfaces.
* Deterministic calculation services.
* Application DTOs.

## Main Use Cases

Examples include:

* `GetClaimContextUseCase`
* `GetDashboardSummaryUseCase`
* `CreateDocumentUseCase`
* `ProcessDocumentUseCase`
* `RetrieveRelevantChunksUseCase`
* `AdjudicateClaimUseCase`

The Application layer depends on abstractions rather than concrete infrastructure implementations.

---

# 6. Infrastructure Layer

Project:

`DomainCopilot.Infrastructure`

Infrastructure implements external technology concerns.

Responsibilities include:

* EF Core database access.
* SQL Server.
* Repository implementations.
* Document storage.
* PDF extraction.
* Text cleaning.
* Chunking.
* Embedding generation.
* Gemini AI integration.
* Local AI provider implementation.

The Infrastructure layer implements interfaces defined by the Application layer.

---

# 7. API Layer

Project:

`DomainCopilot.Api`

The API is the external entry point for the system.

Responsibilities:

* HTTP routing.
* Authentication.
* Authorization.
* Request handling.
* Tenant context.
* Dependency injection.
* CORS.
* Swagger/OpenAPI.

Main controllers include:

* `AuthController`
* `ClaimsController`
* `DashboardController`
* `DocumentsController`
* `ApprovalController`

Controllers delegate business operations to application use cases rather than containing complex business logic.

---

# 8. Frontend Architecture

Project:

`DomainCopilot.UI`

Technology:

Angular.

The frontend is responsible for presentation and user interaction.

Main areas include:

```text
Login
  │
  ▼
Dashboard
  │
  ├── Claims
  │     │
  │     └── Claim Details
  │              │
  │              └── AI Analysis
  │
  └── Approvals
```

The frontend communicates with the backend through HTTP APIs.

The frontend does not:

* Access SQL Server directly.
* Access the vector/retrieval store directly.
* Call the AI provider directly.
* Perform authoritative claim calculations.

---

# 9. Dependency Direction

The intended dependency direction is:

```text
API
 │
 ▼
Application
 │
 ▼
Domain

Infrastructure
 │
 └── implements Application abstractions
```

The most important architectural rule is:

> Business logic must not depend directly on infrastructure technology.

For example, the application depends on:

```text
IClaimRepository
IEmbeddingService
IAiProvider
IDocumentStorage
```

rather than concrete SQL Server, Gemini, or file-system implementations.

---

# 10. Repository Pattern

Repositories provide persistence abstractions to the Application layer.

Examples:

* `IClaimRepository`
* `IPolicyRepository`
* `ICoverageRepository`
* `IExclusionRepository`
* `IAnomalyRepository`
* `IAdjudicationRepository`
* `IApprovalRepository`
* `IApprovalRequestRepository`
* `IDocumentRepository`
* `IDocumentChunkRepository`
* `IUserRepository`

Concrete implementations are located in Infrastructure.

This keeps persistence details outside the core application workflow.

---

# 11. Multi-Tenant Architecture

Multi-tenancy is implemented as a cross-cutting security concern.

The authenticated request contains the tenant identity.

```text
JWT
 │
 ▼
TenantId Claim
 │
 ▼
TenantContext
 │
 ▼
Application Layer
 │
 ▼
Tenant-aware Repository
 │
 ▼
SQL Query with Tenant Filter
```

Tenant filtering is applied at the data-access layer.

This is important because UI-level filtering alone would not provide sufficient isolation.

---

# 12. Tenant Context

The Application layer defines:

```text
ITenantContext
```

The API layer provides the implementation that reads the tenant identity from the authenticated HTTP request.

The Application layer can therefore request the current tenant without depending directly on ASP.NET Core's `HttpContext`.

This preserves the architecture boundary.

---

# 13. Authentication Architecture

Authentication uses JWT bearer tokens.

The authentication workflow is:

```text
User
 │
 ▼
POST /api/Auth/login
 │
 ▼
Validate credentials
 │
 ▼
Create JWT
 │
 ├── User identity
 ├── TenantId
 └── Role
 │
 ▼
Angular stores token
 │
 ▼
Authorization header
 │
 ▼
Protected API
```

The Angular HTTP interceptor adds the bearer token to authenticated requests.

---

# 14. Authorization Architecture

Authorization is enforced by the API.

Supported roles:

* Admin
* Adjuster

Approval operations require the Adjuster role.

Authorization is performed on the server and is not trusted from the frontend.

---

# 15. RAG Architecture

The RAG subsystem consists of two main flows.

## Ingestion

```text
PDF
 │
 ▼
Text Extraction
 │
 ▼
Text Cleaning
 │
 ▼
Chunking
 │
 ▼
Embedding
 │
 ▼
Chunk + Metadata
 │
 ▼
Persistence
```

## Retrieval

```text
User Claim
 │
 ▼
Query Construction
 │
 ▼
Query Embedding
 │
 ▼
Dense Similarity
 │
 ├──────────────┐
 ▼              ▼
Keyword Score   Semantic Score
 │              │
 └──────┬───────┘
        ▼
 Combined Score
        │
        ▼
Top Relevant Chunks
```

The MVP combines dense and keyword relevance.

---

# 16. Document Processing Components

The document-processing subsystem uses separate abstractions for each stage.

Examples:

* `IDocumentTextExtractor`
* `ITextCleaner`
* `IChunkingService`
* `IEmbeddingService`
* `IDocumentStorage`

This allows individual processing stages to be replaced without changing the main application workflow.

---

# 17. AI Provider Abstraction

The application defines:

```text
IAiProvider
```

The infrastructure layer provides implementations.

Current provider options include:

```text
IAiProvider
   │
   ├── GeminiAiProvider
   │
   └── LocalAiProvider
```

This prevents the application workflow from becoming tightly coupled to a single AI vendor.

---

# 18. Agent Architecture

The agent system contains three specialist agents and one orchestrator.

```text
                ClaimAdjudicationOrchestrator
                         │
          ┌──────────────┼──────────────┐
          ▼              ▼              ▼
 Coverage Matcher  Exclusion Analyst  Adjudication Drafter
```

Each specialist has a focused responsibility.

The orchestrator controls sequencing and combines their outputs.

---

# 19. Tool Architecture

Tools expose controlled operations to the agentic workflow.

Examples include:

* Claim Context Tool
* Policy Retrieval Tool
* Claim Amount Calculator Tool
* Anomaly Detection Tool

Tools separate capabilities from the reasoning process.

The deterministic calculator is deliberately implemented as application code rather than allowing the LLM to calculate financial values.

---

# 20. Deterministic Business Logic

Financial calculations are treated as authoritative application logic.

The calculation service:

```text
Claimed Amount
       │
       ▼
Subtract Deductible
       │
       ▼
Apply Coverage Limit
       │
       ▼
Approved Amount
```

The LLM may explain the result, but it does not determine the authoritative numeric value.

This protects the system against arithmetic hallucinations.

---

# 21. Claim Adjudication Architecture

The complete workflow is:

```text
Claim
 │
 ▼
Claim Context
 │
 ▼
Policy Version Matching
 │
 ▼
Policy Retrieval
 │
 ▼
Coverage Analysis
 │
 ▼
Exclusion Analysis
 │
 ▼
Anomaly Detection
 │
 ▼
Deterministic Calculation
 │
 ▼
Adjudication Draft
 │
 ▼
Adjudication Decision
 │
 ▼
Approval Request
 │
 ▼
Adjuster
 │
 ├── Approve
 └── Reject
```

---

# 22. Human-in-the-Loop Boundary

The human approval boundary is intentionally placed after the AI-assisted analysis.

```text
AI / Application Workflow
          │
          ▼
Recommendation
          │
          ▼
   ┌──────────────┐
   │ Human Review │
   └──────┬───────┘
          │
     ┌────┴────┐
     ▼         ▼
  Approve    Reject
```

The AI cannot independently finalize the consequential decision.

---

# 23. Data Isolation

Tenant isolation applies to:

* Claims
* Policies
* Documents
* Document chunks
* Adjudication decisions
* Approval requests
* Users

The tenant identifier is carried through the application workflow and used by repository queries.

This prevents cross-tenant retrieval through normal application APIs.

---

# 24. Policy Version Isolation

Policy retrieval considers the incident date.

Example:

```text
POL-1001
│
├── V1
│   Effective: 2025
│
└── V2
    Effective: 2026
```

A claim with a 2026 incident date must use the applicable 2026 policy version.

This requirement is enforced during retrieval rather than being left entirely to the LLM.

---

# 25. API and UI Communication

The communication path is:

```text
Angular
   │
   │ HTTPS / JSON
   ▼
ASP.NET Core API
   │
   ▼
Application Use Case
   │
   ▼
Repository / Service / Agent
   │
   ▼
Infrastructure
```

This ensures the frontend remains independent from persistence and AI infrastructure.

---

# 26. Error Boundaries

Errors are handled at appropriate architectural boundaries.

Examples:

* Authentication failures are handled by authentication middleware.
* Authorization failures are handled by authorization policies.
* Validation errors are handled by API/application validation.
* Business rule failures are handled by application/domain logic.
* Persistence errors are handled by Infrastructure.
* AI provider errors are isolated within the AI provider implementation.

The API exposes appropriate HTTP responses without exposing sensitive implementation details.

---

# 27. Observability

Application logging is used to track important workflow events.

Examples:

* Authentication attempts.
* Document processing.
* Retrieval operations.
* Agent execution.
* Adjudication decisions.
* Approval actions.
* Exceptions.

The architecture leaves room for future distributed tracing and centralized logging.

---

# 28. Testing Architecture

Testing is organized around behavior and architectural boundaries.

Important test areas include:

### Deterministic Calculation

Verify deductible and coverage-limit calculations.

### Tenant Isolation

Verify that a tenant cannot retrieve another tenant's records.

### Policy Version Retrieval

Verify that the incident date selects the correct policy version.

### Golden Dataset

Verify expected retrieval and adversarial scenarios.

### Application Workflow

Verify the major claim-adjudication workflow.

The test suite avoids requiring live AI calls for deterministic business logic tests whenever possible.

---

# 29. Security Boundaries

Important security boundaries are:

```text
Browser
   │
   │ JWT
   ▼
API Authentication
   │
   ▼
Authorization
   │
   ▼
Tenant Context
   │
   ▼
Application
   │
   ▼
Tenant-aware Data Access
```

Security is enforced server-side.

Frontend visibility is not considered a security boundary.

---

# 30. Architecture Trade-offs

## 30.1 SQL Server for MVP

SQL Server provides strong relational support for claims, policies, users, approvals, and metadata.

A dedicated vector database could be introduced later if corpus size and retrieval requirements increase.

## 30.2 Application-Level Multi-Tenancy

The MVP uses tenant-aware application repositories.

A future enterprise deployment could additionally use database-level controls such as Row-Level Security.

## 30.3 Hosted AI with Provider Abstraction

Gemini is used for the hosted implementation while the application depends on an abstraction.

This balances implementation speed with future provider flexibility.

## 30.4 Local MVP Deployment

The system is currently optimized for local demonstration and assessment delivery.

Production deployment concerns are documented separately rather than represented as completed functionality.

---

# 31. Architecture Quality Attributes

| Attribute       | Approach                              |
| --------------- | ------------------------------------- |
| Security        | JWT, RBAC, tenant filtering           |
| Maintainability | Clean Architecture                    |
| Testability     | Interfaces and deterministic services |
| Explainability  | Retrieved policy evidence             |
| Reliability     | Deterministic calculations            |
| Extensibility   | Provider abstractions                 |
| Isolation       | Tenant-aware repositories             |
| Usability       | Angular workflow UI                   |
| Observability   | Application logging                   |

---

# 32. Architecture Constraints

The architecture must preserve the following constraints:

1. LLMs must not be responsible for authoritative financial arithmetic.
2. Tenant filtering must not depend solely on frontend behavior.
3. AI recommendations must be distinguishable from human-approved decisions.
4. Policy version selection must consider incident dates.
5. Infrastructure dependencies must not leak into the Domain layer.
6. Secrets must not be committed to source control.
7. Real personal insurance data must not be used in the assessment corpus.

---

# 33. Future Evolution

Potential future architecture improvements include:

* Dedicated vector database.
* Retrieval reranking service.
* Event-driven document ingestion.
* Distributed tracing.
* Centralized observability.
* Cloud deployment.
* Enterprise secrets management.
* Database Row-Level Security.
* Advanced fraud detection.
* Model gateway supporting multiple hosted and local providers.

These are architectural evolution paths and are not claimed as completed MVP functionality.
