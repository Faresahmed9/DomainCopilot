# DomainCopilot

## Agentic RAG Platform for Insurance Claims Adjudication

DomainCopilot is an AI-assisted insurance claims adjudication platform built for the ITI Technical Instructor assessment.

The project implements **D2 — Insurance Claims Adjudication** with **T0 — Multi-Tenancy**.

The system combines agentic AI, hybrid RAG, deterministic financial calculations, tenant isolation, and human approval.

---

## 1. Key Capabilities

* Insurance claim intake and analysis.
* Policy-version-aware retrieval.
* Hybrid dense + keyword RAG.
* Coverage analysis.
* Exclusion analysis.
* Anomaly detection.
* Deterministic claim amount calculation.
* AI-generated adjudication draft.
* Human adjuster approval.
* Multi-tenant data isolation.
* JWT authentication.
* Role-based authorization.
* Progress reporting and cancellation.
* Angular web UI.
* Swagger/OpenAPI API.
* Hosted/local AI provider abstraction.

---

## 2. Architecture

```text
Angular UI
    |
    v
ASP.NET Core API
    |
    +-----------------------------+
    |                             |
    v                             v
Application Layer          Infrastructure Layer
    |                             |
    |                             +--> SQL Server / EF Core
    |                             +--> Document Processing
    |                             +--> RAG / Embeddings
    |                             +--> AI Providers
    |
    +--> Claim Adjudication Orchestrator
              |
              +--> Coverage Matcher Agent
              +--> Exclusion Analyst Agent
              +--> Adjudication Drafter Agent
              +--> Deterministic Tools
              +--> Human Approval
```

The project follows Clean Architecture principles.

---

## 3. Technology Stack

### Backend

* C#
* .NET 8
* ASP.NET Core Web API
* Entity Framework Core
* SQL Server
* JWT Authentication
* Swagger / OpenAPI

### Frontend

* Angular
* TypeScript
* Bootstrap
* SCSS

### AI / RAG

* Gemini AI provider
* Local AI provider abstraction
* Embeddings
* Hybrid dense + keyword retrieval

---

## 4. Project Structure

```text
DomainCopilot/
├── DomainCopilot.Domain/
├── DomainCopilot.Application/
├── DomainCopilot.Infrastructure/
├── DomainCopilot.Api/
├── DomainCopilot.Tests/
├── DomainCopilot.UI/
├── docs/
└── README.md
```

---

## 5. Prerequisites

Install:

* .NET 8 SDK
* Visual Studio 2022 with ASP.NET Core development tools
* SQL Server / SQL Server Express
* Node.js and npm
* Angular CLI

Optional:

* Gemini API key for hosted AI functionality.

---

## 6. Configuration

AI and JWT secrets must not be committed to source control.

The application uses configuration values such as:

```text
Gemini:ApiKey
Jwt:Key
AI:Provider
ConnectionStrings:DefaultConnection
```

For local development, use ASP.NET User Secrets or environment variables.

A sample environment configuration is documented separately.

---

## 7. Database Setup

The application uses SQL Server.

The default development connection targets:

```text
Server=DESKTOP-42I26G0;
Database=DomainCopilotDb;
Trusted_Connection=True;
TrustServerCertificate=True;
```

For another machine, configure `ConnectionStrings:DefaultConnection`.

Apply migrations with:

```powershell
dotnet ef database update
```

---

## 8. Run the Backend

From the repository root:

```powershell
dotnet restore
dotnet build
dotnet run --project DomainCopilot.Api
```

Swagger is available from the running API.

---

## 9. Run the Angular UI

Open another terminal:

```powershell
cd DomainCopilot.UI
npm install
ng serve
```

Then open:

```text
http://localhost:4200
```

---

## 10. Demo Accounts

The seeded development environment contains two tenants.

### Tenant A

```text
Admin
Username: admin.a
Password: Admin123!

Adjuster
Username: adjuster.a
Password: Adjuster123!
```

### Tenant B

```text
Admin
Username: admin.b
Password: Admin123!

Adjuster
Username: adjuster.b
Password: Adjuster123!
```

These are development/demo credentials only.

Do not use them in production.

---

## 11. Five-Minute Demo Path

The fastest demonstration path is:

### Step 1 — Login

Login as:

```text
adjuster.a
```

### Step 2 — Dashboard

Show:

* Total claims.
* Pending approvals.

### Step 3 — Claims

Open:

```text
CLM-1001
```

Show:

* Policy POL-1001.
* Incident date.
* Correct policy version V2.
* Coverage analysis.
* Exclusion analysis.
* Retrieved policy evidence.
* Recommended amount.

### Step 4 — Deterministic Calculation

Show that:

```text
Claimed amount = 10,000
Deductible = 1,000
Coverage limit = 7,000
Recommended amount = 7,000
```

The financial amount is calculated by deterministic application code, not by the LLM.

### Step 5 — Approval

Open the approval request and demonstrate:

```text
Pending Approval
        ↓
Adjuster Review
        ↓
Approve / Reject
```

This demonstrates the human approval boundary.

---

## 12. RAG

The document pipeline performs:

```text
PDF
 ↓
Text Extraction
 ↓
Cleaning
 ↓
Chunking
 ↓
Embedding
 ↓
Indexing
```

Retrieval combines:

```text
70% Dense Similarity
+
30% Keyword Matching
```

Retrieval is filtered by:

* Tenant.
* Policy number.
* Incident date.
* Applicable policy version.

The current corpus contains:

* 30 PDF documents.
* 150 total pages.
* Multiple policy versions.
* Two isolated tenants.

---

## 13. Multi-Tenancy

The platform implements T0 multi-tenancy.

Tenant identity is derived from the authenticated JWT `TenantId` claim.

Tenant filtering is enforced at the data/repository layer.

Cross-tenant retrieval tests demonstrate:

```text
Tenant A → Tenant B = []
Tenant B → Tenant A = []
```

The current implementation uses application-level isolation.

It does not claim database-per-tenant, schema-per-tenant, or SQL Row-Level Security.

---

## 14. AI Agents

The system contains three specialist agents:

### Coverage Matcher

Identifies applicable coverage based on the claim and policy evidence.

### Exclusion Analyst

Identifies applicable policy exclusions.

### Adjudication Drafter

Produces a human-readable recommendation using structured findings.

An orchestrator coordinates the complete workflow.

---

## 15. Deterministic Financial Safety

Financial calculations are intentionally kept outside the LLM.

The deterministic calculator applies:

```text
amountAfterDeductible = claimedAmount - deductible

approvedAmount =
    min(amountAfterDeductible, coverageLimit)
```

This prevents arithmetic hallucination from changing the financial recommendation.

---

## 16. Authentication and Authorization

The API uses JWT authentication.

Roles include:

* Admin
* Adjuster

Approval actions require the `Adjuster` role.

Tenant identity is taken from the authenticated token rather than trusted from the Angular UI.

---

## 17. Testing

Run all automated tests:

```powershell
dotnet test
```

Build the solution:

```powershell
dotnet build
```

The test suite includes:

* Golden dataset validation.
* Adversarial evaluation cases.
* Deterministic financial calculation tests.
* Tenant isolation tests.
* RAG-related evaluation checks.

---

## 18. Evaluation

The golden dataset contains:

* 25 questions.
* 5+ adversarial questions.

The current RAG evaluation baseline is:

**76.47% retrieval accuracy**

This is documented as an MVP baseline rather than production-grade accuracy.

See:

`docs/EVALUATION.md`

---

## 19. Documentation

Important project documentation:

```text
docs/
├── BRD.md
├── SYSTEM-DESIGN.md
├── ARCHITECTURE.md
├── SECURITY.md
├── EVALUATION.md
├── AGENTIC-WORKFLOW.md
├── AI-USAGE-LOG.md
└── ADRs/
    ├── ADR-001-clean-architecture.md
    ├── ADR-002-multi-tenancy-isolation.md
    ├── ADR-003-deterministic-financial-calculation.md
    ├── ADR-004-hybrid-version-aware-rag.md
    └── ADR-005-ai-provider-abstraction.md
```

---

## 20. Security

Security controls include:

* JWT authentication.
* Role-based authorization.
* Tenant-aware repositories.
* Tenant-aware document retrieval.
* Password hashing.
* External secret configuration.
* Deterministic financial calculations.
* Human approval for consequential decisions.

Known MVP limitations are documented in:

`docs/SECURITY.md`

---

## 21. AI Provider Abstraction

Application code depends on abstractions rather than directly on an AI SDK.

Generation:

```text
IAiProvider
 ├── GeminiAiProvider
 └── LocalAiProvider
```

Embeddings:

```text
IEmbeddingService
 └── GeminiEmbeddingService
```

This allows the AI implementation to evolve without coupling the application layer to a specific provider.

---

## 22. Assessment Variant

```text
Domain: D2 — Insurance Claims Adjudication
Twist:  T0 — Multi-Tenancy
```

The implementation focuses on:

* Correct policy version retrieval.
* Coverage and exclusion analysis.
* Anomaly detection.
* Deterministic financial calculations.
* Evidence-backed recommendations.
* Human approval.
* Strong tenant boundaries.

---

## 23. Known Limitations

This project is an assessment MVP.

Known limitations include:

* Application-level tenant isolation rather than database-level isolation.
* Current embedding implementation uses Gemini.
* Local AI provider does not provide complete hosted-provider parity.
* RAG evaluation is based on a relatively small golden dataset.
* Advanced production infrastructure and security controls are outside the current scope.

These limitations are intentionally documented rather than hidden.

---

## 24. License

This project is provided for educational and assessment purposes.
