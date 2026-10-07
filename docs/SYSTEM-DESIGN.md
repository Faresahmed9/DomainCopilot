# System Design Document

## 1. Document Overview

**Project:** Domain Copilot — Agentic RAG Platform
**Domain:** D2 — Insurance Claims Adjudication
**Twist:** T0 — Multi-Tenancy
**Version:** MVP
**Technology:** .NET 8, C#, ASP.NET Core, Entity Framework Core, SQL Server, Angular

---

# 2. System Purpose

Domain Copilot is an AI-assisted insurance claims adjudication platform.

The system receives an insurance claim, identifies the applicable policy version, retrieves relevant policy evidence, evaluates coverage and exclusions, detects anomalies, calculates the deterministic approved amount, and produces an adjudication recommendation.

The final consequential decision remains under human control through an explicit adjuster approval gate.

---

# 3. High-Level Architecture

The system follows a layered architecture with clear separation between business logic, application orchestration, infrastructure integrations, and the presentation layer.

```text
┌──────────────────────────────────────────────────────────────┐
│                     Angular Web UI                           │
│ Dashboard │ Claims │ Claim Details │ Approvals │ Login       │
└───────────────────────────────┬──────────────────────────────┘
                                │ HTTPS / JSON
                                ▼
┌──────────────────────────────────────────────────────────────┐
│                    ASP.NET Core API                           │
│ Controllers │ Authentication │ Authorization │ Swagger       │
└───────────────────────────────┬──────────────────────────────┘
                                │
                                ▼
┌──────────────────────────────────────────────────────────────┐
│                     Application Layer                         │
│ Use Cases │ Orchestrator │ Agents │ Tools │ Interfaces       │
└───────────────────────────────┬──────────────────────────────┘
                                │
                                ▼
┌──────────────────────────────────────────────────────────────┐
│                       Domain Layer                            │
│ Claims │ Policies │ Coverage │ Exclusions │ Decisions        │
│ Approval Requests │ Tenant Rules │ Domain Services            │
└───────────────────────────────┬──────────────────────────────┘
                                │
                                ▼
┌──────────────────────────────────────────────────────────────┐
│                    Infrastructure Layer                       │
│ EF Core │ SQL Server │ Repositories │ RAG │ Gemini │ Storage  │
└──────────────────────────────────────────────────────────────┘
```

---

# 4. Architectural Layers

## 4.1 Domain Layer

Project:

`DomainCopilot.Domain`

Responsibilities:

* Core business entities.
* Domain enums.
* Business concepts.
* Domain-level rules.

Important entities include:

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

The Domain layer does not depend on:

* ASP.NET Core
* Entity Framework Core
* Gemini SDK
* Angular
* Vector database SDKs

---

## 4.2 Application Layer

Project:

`DomainCopilot.Application`

Responsibilities:

* Use cases.
* Application workflows.
* Agent orchestration.
* Tool abstractions.
* Repository interfaces.
* AI provider abstractions.
* Tenant context abstraction.
* Deterministic business services.

Examples:

* Get Claim Context
* Retrieve Relevant Chunks
* Create Document
* Process Document
* Adjudicate Claim
* Get Dashboard Summary

---

## 4.3 Infrastructure Layer

Project:

`DomainCopilot.Infrastructure`

Responsibilities:

* Entity Framework Core.
* SQL Server persistence.
* Repository implementations.
* Document storage.
* PDF text extraction.
* Text cleaning.
* Chunking.
* Embeddings.
* Gemini integration.
* Local AI provider implementation.

Infrastructure implements interfaces defined by the Application layer.

---

## 4.4 API Layer

Project:

`DomainCopilot.Api`

Responsibilities:

* HTTP endpoints.
* Authentication.
* Authorization.
* Request validation.
* Swagger/OpenAPI.
* Dependency injection.
* Tenant context integration.
* CORS configuration.

Main API areas include:

* Authentication
* Claims
* Dashboard
* Documents
* Approval

---

## 4.5 UI Layer

Project:

`DomainCopilot.UI`

Technology:

Angular.

Responsibilities:

* Login.
* Dashboard.
* Claims list.
* Claim details.
* AI analysis visualization.
* Retrieved policy evidence.
* Approval workflow.

The UI communicates with the ASP.NET Core API and does not directly access the database or AI provider.

---

# 5. Data Architecture

SQL Server is used as the primary relational database.

The database stores:

* Tenants
* Users
* Policies
* Claims
* Coverages
* Exclusions
* Anomalies
* Adjudication decisions
* Approval requests
* Documents
* Document chunks

Each tenant-owned business record contains a `TenantId`.

Tenant filtering is enforced in repository queries.

---

# 6. Multi-Tenancy Architecture

Multi-tenancy is a mandatory requirement of this implementation.

The system uses application-level tenant isolation with tenant-aware repositories.

The authenticated user's JWT contains a `TenantId` claim.

The API resolves the current tenant through:

`ITenantContext`

The implementation obtains the tenant from the authenticated request and passes the tenant context into application operations and repositories.

Example flow:

```text
JWT
 │
 ├── UserId
 ├── TenantId
 └── Role
      │
      ▼
TenantContext
      │
      ▼
Application Use Case
      │
      ▼
Tenant-aware Repository
      │
      ▼
SQL Server
```

Repository queries include tenant filtering.

This prevents a user from retrieving records belonging to another tenant through normal application operations.

---

# 7. Authentication and Authorization

The system uses JWT bearer authentication.

A successful login returns a JWT containing identity and authorization information.

Important claims include:

* User identity
* Tenant ID
* Role

Supported roles:

* Admin
* Adjuster

Authorization is enforced at API endpoints.

Approval actions require the Adjuster role.

---

# 8. Claims Adjudication Workflow

The main workflow is:

```text
Receive Claim
     │
     ▼
Match Policy Version
     │
     ▼
Retrieve Relevant Policy Evidence
     │
     ▼
Coverage Matcher
     │
     ▼
Exclusion Analyst
     │
     ▼
Anomaly Detection
     │
     ▼
Deterministic Amount Calculation
     │
     ▼
Adjudication Drafter
     │
     ▼
Recommendation
     │
     ▼
Create Approval Request
     │
     ▼
Human Adjuster Review
     │
     ├───────────────┐
     ▼               ▼
  Approve          Reject
```

---

# 9. Agent Architecture

The system uses specialist agents coordinated by an orchestrator.

## 9.1 Coverage Matcher

Purpose:

Identify which policy coverage applies to the claim.

Inputs:

* Claim information.
* Retrieved policy evidence.

Outputs:

* Coverage finding.
* Supporting evidence.

---

## 9.2 Exclusion Analyst

Purpose:

Identify whether any policy exclusion applies.

Inputs:

* Claim information.
* Policy exclusions.
* Retrieved evidence.

Outputs:

* Exclusion finding.
* Supporting evidence.

---

## 9.3 Adjudication Drafter

Purpose:

Prepare a human-readable adjudication recommendation.

Inputs:

* Coverage result.
* Exclusion result.
* Anomaly result.
* Deterministic calculation result.
* Retrieved evidence.

Outputs:

* Recommendation.
* Reasoning.
* Evidence references.

The drafter does not independently determine the final numeric payout.

---

# 10. Orchestrator

The `ClaimAdjudicationOrchestrator` coordinates the complete adjudication process.

Responsibilities:

1. Load claim context.
2. Retrieve applicable policy evidence.
3. Run coverage analysis.
4. Run exclusion analysis.
5. Detect anomalies.
6. Calculate approved amount deterministically.
7. Produce adjudication draft.
8. Persist the adjudication decision.
9. Create an approval request.
10. Report workflow progress.
11. Respect cancellation requests.

The orchestrator acts as the coordinator rather than implementing every business rule itself.

---

# 11. RAG Architecture

The RAG pipeline consists of:

```text
Policy Document
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
Embedding Generation
      │
      ▼
Chunk Storage + Metadata
      │
      ▼
Hybrid Retrieval
      │
      ▼
Relevant Policy Evidence
```

---

# 12. Document Metadata

Each document and chunk carries metadata required for safe retrieval.

Important metadata includes:

* Tenant ID
* Policy number
* Policy version
* Effective-from date
* Effective-to date
* Document ID
* Chunk ID
* Page number

This metadata is important for both tenant isolation and policy-version correctness.

---

# 13. Hybrid Retrieval

The retrieval implementation combines two signals:

### Dense similarity

The query and document chunks are represented as embeddings.

Cosine similarity is used to identify semantically related chunks.

### Keyword relevance

The system also evaluates keyword overlap between the query and available document text.

### Combined score

The MVP combines:

* Dense similarity: 70%
* Keyword relevance: 30%

The highest-ranked chunks are returned as evidence.

---

# 14. Version-Aware Retrieval

Insurance policies may contain multiple versions.

The retrieval process therefore considers:

* Tenant
* Policy number
* Incident date
* Effective-from date
* Effective-to date

Example:

```text
Claim Incident Date
        │
        ▼
2026-06-15
        │
        ▼
POL-1001
        │
        ▼
Applicable Version
        │
        ▼
V2
```

This prevents a 2026 claim from accidentally using an older policy version.

---

# 15. Deterministic Calculation

Financial calculations are implemented in application code.

The approved amount is calculated using:

* Claimed amount.
* Deductible.
* Coverage limit.

Conceptually:

```text
Amount After Deductible
        =
Claimed Amount - Deductible

Approved Amount
        =
minimum(
    Amount After Deductible,
    Coverage Limit
)
```

The LLM is not responsible for performing this calculation.

This design reduces the risk of arithmetic hallucination.

---

# 16. Approval Architecture

The AI workflow creates an `ApprovalRequest`.

The request remains pending until an authorized adjuster reviews it.

The adjuster can:

* Approve
* Reject

The approval API verifies the authenticated role and tenant before changing the request.

This creates a human-in-the-loop safety boundary between AI recommendation and consequential action.

---

# 17. API Architecture

The ASP.NET Core API exposes REST endpoints.

Representative endpoint groups:

```text
/api/Auth
/api/Dashboard
/api/Claims
/api/Documents
/api/Approval
```

Swagger/OpenAPI provides interactive API documentation.

The Angular application consumes these endpoints over HTTPS.

---

# 18. Streaming and Progress

The orchestration workflow supports progress reporting.

Example progress stages:

```text
Loading claim
      ↓
Retrieving policy
      ↓
Analyzing coverage
      ↓
Checking exclusions
      ↓
Detecting anomalies
      ↓
Calculating amount
      ↓
Preparing recommendation
      ↓
Creating approval request
```

Cancellation is supported through `CancellationToken`.

---

# 19. AI Provider Abstraction

The application does not directly depend on a specific hosted AI provider.

The Application layer defines an AI provider abstraction.

The Infrastructure layer supplies implementations.

Current implementations include:

* Gemini AI provider.
* Local AI provider abstraction.

This allows the application workflow to remain independent from a specific AI SDK.

---

# 20. Security Architecture

Security controls include:

* JWT authentication.
* Role-based authorization.
* Tenant-aware data access.
* Protected API keys through configuration/user secrets.
* HTTPS during local development.
* Input validation.
* Human approval for consequential actions.
* No real personal data in the demo corpus.

Tenant filtering is implemented at repository/data-access boundaries rather than relying only on UI filtering.

---

# 21. Error Handling

The API should return appropriate HTTP responses for common failures.

Examples:

| Scenario               | Expected Response         |
| ---------------------- | ------------------------- |
| Missing authentication | 401 Unauthorized          |
| Invalid role           | 403 Forbidden             |
| Missing resource       | 404 Not Found             |
| Invalid request        | 400 Bad Request           |
| Internal failure       | 500 Internal Server Error |

Detailed internal exception information should not be exposed to end users in production environments.

---

# 22. Observability

The system uses application logging to provide visibility into workflow execution.

Important events include:

* Authentication failures.
* Document processing.
* Retrieval operations.
* Agent workflow stages.
* Adjudication creation.
* Approval operations.
* Errors and exceptions.

A future production deployment can extend this with distributed tracing and centralized log aggregation.

---

# 23. Evaluation Architecture

The project includes a golden evaluation dataset.

The dataset contains:

* Normal questions.
* Policy retrieval questions.
* Version-aware questions.
* Tenant-isolation scenarios.
* Adversarial questions.

Evaluation covers areas such as:

* Retrieval correctness.
* Policy-version correctness.
* Tenant isolation.
* Deterministic calculation.
* Adversarial behavior.

The evaluation dataset is version-controlled with the project.

---

# 24. Example End-to-End Scenario

### Input

Claim:

`CLM-1001`

Policy:

`POL-1001`

Incident date:

`2026-06-15`

Claimed amount:

`10,000`

### System Processing

1. Authenticate adjuster.
2. Resolve Tenant A.
3. Load claim.
4. Match `POL-1001` version using the incident date.
5. Select policy V2.
6. Retrieve relevant policy chunks.
7. Identify vehicle damage coverage.
8. Identify applicable deductible and limit.
9. Check exclusions.
10. Detect that the claimed amount exceeds the coverage limit.
11. Calculate the approved amount deterministically.
12. Produce a partial approval recommendation.
13. Create an approval request.
14. Present the recommendation and evidence to the adjuster.

### Expected Result

The system recommends a partial approval based on the applicable policy rules and calculated amount.

The adjuster remains responsible for the final approval decision.

---

# 25. Deployment Model

The MVP is designed to run locally.

```text
Developer Machine
│
├── Angular UI
│      └── localhost:4200
│
├── ASP.NET Core API
│      └── HTTPS localhost
│
├── SQL Server
│      └── DomainCopilotDb
│
└── AI Provider
       └── Gemini API / Local Provider
```

A future production deployment may separate these components into independently deployed services.

---

# 26. Architectural Decisions Summary

| Decision                  | Reason                                     |
| ------------------------- | ------------------------------------------ |
| .NET 8                    | Stable supported backend platform          |
| Angular                   | Structured enterprise-style frontend       |
| SQL Server                | Relational persistence for business data   |
| EF Core                   | Strong integration with .NET               |
| JWT                       | Stateless API authentication               |
| Repository abstraction    | Tenant filtering and persistence isolation |
| Hybrid RAG                | Combines semantic and lexical retrieval    |
| Effective-date retrieval  | Prevents wrong policy version              |
| Deterministic calculation | Prevents arithmetic hallucination          |
| Specialist agents         | Separates domain responsibilities          |
| Orchestrator              | Coordinates agentic workflow               |
| Human approval            | Maintains human control                    |
| AI provider abstraction   | Avoids vendor lock-in                      |

---

# 27. Known MVP Gaps

The current implementation intentionally prioritizes core assessment requirements.

Known areas for future improvement include:

* Production-grade distributed observability.
* Cloud deployment.
* Managed vector infrastructure.
* Advanced reranking.
* More sophisticated fraud detection.
* Full production streaming UX.
* Enterprise secrets management.
* Advanced tenant administration.

These are not represented as completed production features.

---

# 28. Design Principles

The system follows these principles:

1. **Tenant isolation by design**
2. **Deterministic business calculations**
3. **Evidence-grounded AI**
4. **Human-in-the-loop approval**
5. **Separation of concerns**
6. **Provider abstraction**
7. **Testable application logic**
8. **Explicit workflow orchestration**
9. **Secure handling of secrets**
10. **Traceable AI recommendations**
