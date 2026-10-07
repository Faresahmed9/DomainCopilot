# Security Documentation

## 1. Purpose

This document describes the security controls implemented in the Domain Copilot platform.

The platform handles insurance claims, policy documents, AI-assisted retrieval, adjudication recommendations, and human approval.

The main security priorities are:

* Authentication.
* Authorization.
* Tenant isolation.
* Protection of secrets.
* Secure document retrieval.
* Deterministic financial calculations.
* Human approval for side-effecting decisions.
* Protection against AI-specific risks.

This document describes the security controls that are implemented in the current MVP.

---

## 2. Security Boundaries

The main security boundaries are:

```text
User
  ↓
Angular UI
  ↓
JWT Authentication
  ↓
ASP.NET Core API
  ↓
Tenant Context + Authorization
  ↓
Application Use Cases
  ↓
Tenant-Aware Repositories
  ↓
SQL Server / Document Storage
```

AI services and retrieval are accessed through application and infrastructure abstractions.

---

## 3. Authentication

The API uses JWT bearer authentication.

A user first authenticates through the login endpoint.

After successful authentication, the API returns a JWT containing the authenticated user's identity and tenant information.

Protected API endpoints require a valid JWT.

The application does not treat the tenant identifier supplied by the client as sufficient authentication by itself.

The authenticated tenant is obtained from the JWT `TenantId` claim through the application `ITenantContext` abstraction.

---

## 4. Authorization

The platform uses role-based authorization.

Current roles are:

* `Admin`
* `Adjuster`

Authorization is enforced at the API boundary for protected operations.

For example, claim approval and rejection operations require the `Adjuster` role.

This prevents a normal authenticated user without the required role from performing the approval action.

The authorization model is intentionally separated from authentication:

```text
Authentication
    ↓
Who is the user?
    ↓
Authorization
    ↓
What is the user allowed to do?
```

---

## 5. Multi-Tenant Isolation

Multi-tenancy is a core security requirement of the D2/T0 assessment variant.

The application currently uses application-level tenant isolation with a shared SQL Server database.

Each authenticated user belongs to a tenant.

The authenticated tenant is represented by the JWT `TenantId` claim.

The `TenantContext` exposes this value to the application layer.

Tenant-aware repositories then use the authenticated tenant identifier when querying tenant-owned data.

The current implementation applies tenant filtering to areas including:

* Users.
* Claims.
* Policies.
* Adjudication decisions.
* Approval requests.
* Documents.
* Document chunks.

The retrieval layer also applies tenant filtering before returning candidate document chunks.

---

## 6. Tenant Isolation Principle

The system follows the principle:

```text
Authenticated Tenant
        ↓
TenantContext
        ↓
Tenant-Aware Use Case
        ↓
Tenant-Aware Repository
        ↓
Tenant-Scoped Data
```

The application does not rely only on the Angular UI to hide another tenant's data.

Tenant filtering is applied in the backend data-access layer.

This is important because client-side filtering alone would not provide sufficient isolation.

---

## 7. Tenant Isolation Verification

Cross-tenant retrieval was manually verified using the two seeded tenants.

The following checks were performed:

```text
Tenant A → Tenant B policy retrieval
Result: []

Tenant B → Tenant A policy retrieval
Result: []
```

The same tenant-aware behavior is applied to claim and policy access.

These checks verify that the retrieval layer does not return another tenant's policy chunks.

---

## 8. RAG Security

The RAG pipeline applies security-relevant metadata filtering before ranking retrieved chunks.

The retrieval process considers:

* Tenant.
* Policy number.
* Incident date.
* Policy version.
* Semantic relevance.
* Keyword relevance.

Tenant filtering occurs before relevant chunks are returned to downstream processing.

This reduces the risk of cross-tenant information leakage and incorrect policy-version retrieval.

---

## 9. Policy Version Security

Insurance policies can have multiple versions.

Using the wrong version can result in an incorrect claim decision.

The retrieval process therefore uses the claim incident date and policy effective dates when selecting the applicable policy version.

Example:

```text
POL-1001
    │
    ├── Incident 2025-06-15 → V1
    │
    └── Incident 2026-06-15 → V2
```

This prevents historical policy text from being treated as equally applicable to a claim when a newer or older version should apply.

---

## 10. Secret Management

Sensitive credentials are not intended to be stored in source control.

The application uses configuration mechanisms such as .NET User Secrets for local development.

Sensitive configuration includes:

* Gemini API key.
* JWT signing key.

The repository contains configuration examples rather than real production credentials.

The `.gitignore` configuration also prevents local utility folders such as the password-hash generation utility from being committed accidentally.

---

## 11. Password Security

User passwords are not stored as plain text.

The application uses ASP.NET Core password hashing functionality.

Authentication verifies the submitted password against the stored password hash.

The application therefore does not need to store or compare plain-text passwords.

---

## 12. Financial Calculation Safety

Financial calculations are treated as a business-critical security and correctness boundary.

The LLM is not responsible for calculating the final approved claim amount.

The application uses the deterministic:

```text
ClaimAmountCalculator
```

The calculation applies explicit business rules for:

* Claimed amount.
* Deductible.
* Coverage limit.
* Approved amount.

For example:

```text
Claimed Amount = 10,000
Deductible     = 1,000
Coverage Limit = 7,000

Amount after deductible = 9,000

Approved Amount = min(9,000, 7,000)
                = 7,000
```

This prevents an LLM from inventing or incorrectly calculating a financial result.

---

## 13. Human Approval Boundary

AI-generated recommendations are not treated as final authorization.

The adjudication workflow creates an approval request after producing the recommendation.

A human Adjuster must approve or reject the request.

The boundary is:

```text
Claim
  ↓
AI-assisted analysis
  ↓
Deterministic calculation
  ↓
Adjudication recommendation
  ↓
Approval Request
  ↓
Human Adjuster
  ↓
Approve / Reject
```

This provides a human control point before the final side-effecting approval action.

---

## 14. AI Provider Security

The Application layer does not directly depend on provider-specific AI SDKs.

AI generation is accessed through:

```text
IAiProvider
```

Embedding generation is accessed separately through:

```text
IEmbeddingService
```

Provider-specific credentials and API communication are handled by Infrastructure.

This separation reduces coupling and makes provider-specific security controls easier to manage.

---

## 15. Embedding and Generation Separation

Embeddings and text generation have different responsibilities.

Embeddings are used for:

* Document indexing.
* Query embedding.
* Semantic similarity retrieval.

Generation is used for:

* AI-assisted responses.
* Agent processing.
* Natural-language output.

Separating these capabilities prevents the generation provider from becoming an implicit security or retrieval dependency.

---

## 16. AI-Specific Risks

The platform considers several AI-specific risks.

### Cross-Tenant Retrieval

**Risk:** AI receives policy evidence belonging to another tenant.

**Mitigation:** Tenant filtering is applied in the document chunk retrieval layer.

### Wrong Policy Version

**Risk:** AI receives a historically incorrect policy version.

**Mitigation:** Retrieval uses policy number and incident/effective dates.

### Arithmetic Hallucination

**Risk:** AI generates an incorrect financial calculation.

**Mitigation:** Financial calculations are performed by deterministic application code.

### Missing Evidence

**Risk:** AI assumes policy coverage when retrieval produces no evidence.

**Mitigation:** Retrieval returns an empty result when no eligible evidence exists, and the adjudication boundary does not treat missing evidence as an approval basis.

### Prompt Injection

**Risk:** Malicious or misleading text attempts to influence the agent's behavior.

**Mitigation:** The evaluation dataset includes adversarial questions and injection cases, while business-critical decisions remain constrained by deterministic application logic and human approval.

---

## 17. Document Upload Security

Uploaded policy documents are associated with the authenticated tenant.

Document processing is performed within the tenant context.

Document chunks inherit tenant ownership and are filtered by tenant during retrieval.

This prevents uploaded policy content from one tenant from becoming available to another tenant through the RAG pipeline.

---

## 18. API Security

Protected API operations require authentication.

Role-sensitive operations additionally require the appropriate role.

The API uses HTTPS during local development.

CORS is restricted to the configured Angular development origin rather than allowing arbitrary browser origins.

The current development configuration allows:

```text
http://localhost:4200
```

---

## 19. Security Logging and Observability

Security-relevant operations should remain observable through application logging.

Examples include:

* Authentication failures.
* Authorization failures.
* Claim processing.
* Adjudication workflow progress.
* Approval operations.
* Retrieval operations.

Logs must not contain secrets such as API keys, passwords, or JWT signing keys.

---

## 20. Security Limitations

The current MVP uses application-level tenant isolation with a shared SQL Server database.

It does **not** currently claim to provide:

* Database-per-tenant isolation.
* Schema-per-tenant isolation.
* SQL Server Row-Level Security.
* Dedicated vector database per tenant.
* Cryptographic separation between tenant datasets.
* Enterprise identity provider integration.
* Production-grade secrets management infrastructure.
* Advanced WAF or network-level protection.

These are deployment and hardening considerations beyond the current MVP scope.

---

## 21. Security Verification Summary

The current implementation has verified:

* JWT authentication.
* Role-based authorization.
* Admin and Adjuster roles.
* Tenant-aware repositories.
* Tenant-aware RAG retrieval.
* Cross-tenant retrieval returning no results.
* Policy-version-aware retrieval.
* Deterministic financial calculations.
* Human approval before approval/rejection actions.
* Password hashing.
* Secrets stored outside source code.

---

## 22. Security Principles

The platform follows these principles:

1. **Authenticate before accessing protected operations.**
2. **Authorize sensitive operations by role.**
3. **Derive tenant context from authenticated identity.**
4. **Enforce tenant isolation in backend data access.**
5. **Never rely on the UI alone for authorization or isolation.**
6. **Keep secrets outside source control.**
7. **Keep financial calculations deterministic.**
8. **Separate retrieval from adjudication.**
9. **Keep human approval as the final control boundary.**
10. **Document limitations instead of claiming controls that are not implemented.**


## 20. Security Test Commands

The following commands can be used to verify the security-related implementation locally.

### 20.1 Run the Automated Test Suite

From the repository root:

```powershell
dotnet test
```

This verifies the current automated test suite, including tenant isolation and deterministic business-rule tests.

### 20.2 Build the Solution

```powershell
dotnet build
```

A successful build confirms that the current source compiles across the solution.

### 20.3 Verify Tenant Isolation Tests

Run the test project directly:

```powershell
dotnet test .\DomainCopilot.Tests\DomainCopilot.Tests.csproj
```

The test suite includes tenant-isolation verification for protected data and retrieval behavior.

### 20.4 Verify No Real Secrets Are Tracked

To inspect tracked files for common secret patterns:

```powershell
git grep -n -I -E "AIza[0-9A-Za-z_-]{20,}|Password=|Jwt:Key|Gemini:ApiKey"
```

Configuration values containing real secrets must not be committed.

Local development secrets should remain in User Secrets or environment configuration.

### 20.5 Check the Local Password Utility Is Ignored

```powershell
git check-ignore -v PasswordHashGenerator/
```

Expected behavior is that Git reports the `.gitignore` rule responsible for ignoring the local password-hash utility.

### 20.6 Review the Working Tree Before Commit

```powershell
git status
```

Before committing security-sensitive changes, verify that:

* No `.env` files containing secrets are staged.
* No local secret files are staged.
* `PasswordHashGenerator/` is not staged.
* Only intended source and documentation files are included.

### 20.7 Review Staged Files

```powershell
git diff --cached --name-only
```

This command should be used before commits to verify exactly which files will enter the repository.

### 20.8 Tenant Isolation Manual Verification

The API can also be verified manually using the seeded Tenant A and Tenant B accounts.

Expected behavior:

```text
Tenant A → Tenant B policy retrieval = []
Tenant B → Tenant A policy retrieval = []
```

These checks confirm that the authenticated tenant boundary is respected by the retrieval layer.

### 20.9 Security Test Boundary

The commands above verify the security controls currently implemented by the MVP.

They do not represent a full production penetration test or formal security audit.

Additional production hardening would require dedicated security testing, dependency scanning, infrastructure review, and penetration testing.
