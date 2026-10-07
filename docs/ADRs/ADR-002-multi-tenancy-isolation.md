# ADR-002: Multi-Tenancy Isolation

* **Status:** Accepted
* **Date:** 2026-10-07
* **Decision Type:** Security / Data Architecture

## 1. Context

Domain Copilot is an insurance claims adjudication platform that supports multiple insurance tenants.

The assessment requires tenant isolation for users, claims, policies, documents, retrieval results, and adjudication workflows.

The current implementation uses a shared SQL Server database with explicit tenant identifiers. Tenant identity is established from the authenticated user's JWT and is exposed to the application through `ITenantContext`.

The system currently contains two seeded tenants:

* Tenant A
* Tenant B

## 2. Decision

The system uses **application-level tenant isolation enforced in tenant-aware data access code**.

The current tenant is resolved from the authenticated JWT `TenantId` claim through `TenantContext`.

The Application layer accesses the current tenant through:

```csharp
public interface ITenantContext
{
    Guid TenantId { get; }
}
```

The API registers `TenantContext` as the implementation of `ITenantContext`.

Tenant-aware repositories use the current tenant identifier when retrieving tenant-owned data.

## 3. Tenant Identification

`TenantContext` reads the `TenantId` claim from the authenticated user's claims:

```csharp
var tenantClaim =
    user?.FindFirst("TenantId")?.Value;
```

If the claim is missing or cannot be parsed as a GUID, the current implementation throws `UnauthorizedAccessException`.

This makes the authenticated JWT the source of tenant identity for protected API requests.

The UI does not determine the tenant for protected data-access operations.

## 4. Tenant-Aware Data Access

Tenant filtering is implemented in repository queries.

The current implementation includes tenant-aware repositories for the main claims and adjudication workflow.

Examples include:

* `PolicyRepository`
* `ClaimRepository`
* `AdjudicationRepository`
* `ApprovalRepository`
* `ApprovalRequestRepository`
* `DocumentRepository`
* `DocumentChunkRepository`
* `UserRepository`

Repository queries use the current tenant identifier where the underlying entity is tenant-scoped.

For example, policy retrieval receives a `tenantId` and applies it together with the policy number and incident-date conditions.

This means tenant filtering occurs during data access rather than relying on the Angular UI to hide records.

## 5. Claims and Policy Isolation

Claims are associated with a tenant.

Claim retrieval is performed through the tenant-aware claim repository.

Policy retrieval is also tenant-aware.

For policy version matching, the repository applies:

1. Tenant identifier.
2. Policy number.
3. Effective date conditions.
4. Version ordering.

The active policy version is therefore selected within the current tenant's policy data.

## 6. RAG Isolation

The document-processing and retrieval pipeline is tenant-aware.

Documents and document chunks are associated with the tenant that uploaded them.

`DocumentChunkRepository.SearchAsync` applies tenant filtering together with policy and date conditions.

The retrieval flow is therefore:

```text
Authenticated Request
        ↓
Current Tenant
        ↓
Tenant-filtered document chunks
        ↓
Policy/date filtering
        ↓
Embedding + keyword scoring
        ↓
Relevant chunks
```

The tenant filter is applied before the retrieved chunks are returned to the Application layer.

The current retrieval implementation combines:

* Dense embedding similarity.
* Keyword matching.

The final ranking uses the implemented hybrid retrieval weighting.

## 7. Authentication and Authorization

Users are associated with a tenant and a role.

The current roles are:

* `Admin`
* `Adjuster`

JWT authentication establishes the authenticated user's identity and tenant.

Role-based authorization is used for protected operations.

For example, the approval endpoint requires the `Adjuster` role.

Tenant membership and role are separate concepts: having the `Adjuster` role does not by itself grant access to another tenant's records.

## 8. Alternatives Considered

### A. UI-Only Filtering

The UI could retrieve records from multiple tenants and display only the current tenant's records.

**Rejected because:**

* The UI is not a security boundary.
* Direct API callers could bypass the UI.
* Unauthorized data could already have been exposed by the API.

### B. Database-per-Tenant

Each tenant could have a separate database.

**Not selected for the current MVP because:**

* The current assessment scale does not require separate databases.
* It would introduce additional connection, migration, and deployment complexity.

### C. Database Row-Level Security

SQL Server row-level security could enforce tenant filtering directly in the database.

**Not selected for the current MVP because:**

* The current implementation already provides tenant-aware repository filtering.
* Introducing database RLS would add additional configuration and operational complexity.

## 9. Consequences

### Positive

* Tenant boundaries are explicit in the application.
* Tenant filtering is performed during data access.
* The RAG document search is tenant-aware.
* The current implementation supports multiple tenants using the same database.
* Tenant isolation can be tested using separate tenant identities.

### Negative

* New tenant-scoped repository queries must consistently apply tenant filtering.
* A future repository that omits tenant filtering could introduce a security vulnerability.
* The current approach relies on application-level enforcement rather than database-level RLS.

## 10. Verification

The implementation has been manually verified using the two seeded tenants.

### Tenant A → Tenant B Policy

Authenticated as Tenant A, a retrieval attempt for Tenant B's policy returned:

```json
[]
```

### Tenant B → Tenant A Policy

Authenticated as Tenant B, a retrieval attempt for Tenant A's policy returned:

```json
[]
```

Additional tenant isolation checks were performed for claims and retrieval context.

The positive-path checks also confirmed that each tenant can access its own seeded data.

## 11. Security Principle

Tenant identity is treated as a security boundary.

The implementation follows this rule:

> Tenant-scoped data must be filtered using the authenticated tenant before it is returned to the caller.

The Angular UI is therefore not responsible for enforcing tenant isolation.

## 12. Current Scope and Limitations

The current implementation provides application-level tenant isolation.

It does **not** currently claim:

* Database-per-tenant isolation.
* Schema-per-tenant isolation.
* SQL Server Row-Level Security.
* Cryptographic separation of tenant data.
* Independent vector databases per tenant.

Those approaches remain possible future evolutions if stronger infrastructure-level isolation becomes necessary.

## 13. Compliance

This decision supports the T0 Multi-Tenancy requirement through:

* Two seeded tenants.
* Tenant-associated users.
* Tenant-aware claim and policy access.
* Tenant-aware document and chunk retrieval.
* JWT-based tenant identification.
* Tenant-aware repository queries.
* Cross-tenant isolation verification.

## 14. Future Evolution

If stronger isolation is required in a future production deployment, the architecture can evolve toward:

* SQL Server Row-Level Security.
* Schema-per-tenant.
* Database-per-tenant.
* Dedicated vector stores per tenant.

Such changes are intentionally outside the current MVP implementation.
