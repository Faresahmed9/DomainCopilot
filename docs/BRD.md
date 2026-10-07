# Business Requirements Document (BRD)

## 1. Project Overview

**Project:** Domain Copilot — Agentic RAG Platform
**Domain Variant:** D2 — Insurance Claims Adjudication
**Mandatory Twist:** T0 — Multi-tenancy
**Technology:** .NET 8, C#, SQL Server, Angular
**Primary Goal:** Assist insurance adjusters in reviewing claims using grounded retrieval, specialist AI agents, deterministic calculations, and explicit human approval.

The system helps an insurance organization process claims by retrieving the policy version applicable to the incident date, evaluating coverage and exclusions, detecting anomalies, calculating the deterministic approved amount, and preparing an adjudication recommendation for human review.

The system is designed around three principles:

1. AI recommendations must be grounded in the available policy corpus.
2. Consequential decisions require explicit human approval.
3. Tenant data must remain isolated at the data-access layer.

---

## 2. Business Context

Insurance claim adjudication requires reviewers to locate the correct policy version, understand coverage and exclusions, calculate applicable limits and deductibles, and document the resulting decision.

Manual review can be time-consuming and can introduce risks such as:

* Using the wrong policy version.
* Missing applicable exclusions.
* Incorrectly calculating claim amounts.
* Failing to identify suspicious or anomalous claims.
* Making recommendations without traceable supporting evidence.

Domain Copilot provides an AI-assisted workflow while keeping deterministic business calculations and final approval under controlled application logic.

---

## 3. Personas

### 3.1 Adjuster

The primary business user.

Responsibilities:

* Review claims.
* Inspect policy evidence.
* Review AI analysis.
* Review anomalies.
* Review the adjudication draft.
* Approve or reject the recommendation.

### 3.2 Administrator

Responsible for system and tenant-level administration.

Responsibilities include:

* Managing users and tenant configuration.
* Accessing administrative functionality.

### 3.3 System / AI Agents

Specialized software components supporting the adjuster.

Agents:

* Coverage Matcher
* Exclusion Analyst
* Adjudication Drafter
* Claim Adjudication Orchestrator

---

## 4. Business Objectives

### BO-01 — Reduce manual policy lookup

Provide relevant policy evidence for a claim without requiring the adjuster to manually search the complete corpus.

### BO-02 — Select the correct policy version

Ensure retrieval considers the incident date and policy version.

### BO-03 — Produce explainable recommendations

Every recommendation should be supported by retrieved policy evidence.

### BO-04 — Prevent arithmetic hallucination

Coverage limits, deductibles, and approved claim amounts must be calculated by deterministic application code rather than by the LLM.

### BO-05 — Preserve human control

The AI recommendation must not become the final consequential decision without explicit adjuster approval.

### BO-06 — Enforce tenant isolation

Users must only access claims, policies, documents, retrieval results, and approval requests belonging to their tenant.

---

# 5. Functional Requirements

## FR-01 — Authentication

The system shall authenticate users and issue JWT-based access tokens.

**Acceptance Criteria:**

* Valid credentials produce an authenticated session.
* Invalid credentials are rejected.
* The token contains the user's tenant and role information.

**Status:** Implemented.

---

## FR-02 — Role-Based Authorization

The system shall support at least two roles:

* Admin
* Adjuster

**Acceptance Criteria:**

* Authorized endpoints require authentication.
* Approval actions require the Adjuster role.
* Unauthorized roles cannot perform approval actions.

**Status:** Implemented.

---

## FR-03 — Multi-Tenant Isolation

The system shall support multiple tenants with isolated data.

**Acceptance Criteria:**

* At least two tenants exist.
* Tenant ID is associated with protected business data.
* Repository queries filter by tenant.
* A user from Tenant A cannot retrieve Tenant B data.
* A user from Tenant B cannot retrieve Tenant A data.

**Status:** Implemented.

---

## FR-04 — Policy Version Matching

The system shall select the policy version applicable to the claim incident date.

**Acceptance Criteria:**

* Policy number is considered.
* Incident date is considered.
* Effective-from and effective-to dates are considered.
* The selected policy version is returned to the adjudication workflow.

**Status:** Implemented.

---

## FR-05 — Document Ingestion

The system shall support policy document ingestion.

The ingestion pipeline shall perform:

1. Document upload
2. Text extraction
3. Text cleaning
4. Chunking
5. Embedding
6. Storage with metadata

**Status:** Implemented for the MVP corpus.

---

## FR-06 — Tenant-Aware Retrieval

The system shall retrieve relevant policy chunks using tenant-aware filtering.

Retrieval considers:

* Tenant
* Policy number
* Policy version/effective date
* Semantic similarity
* Keyword relevance

**Acceptance Criteria:**

* Retrieval results belong to the current tenant.
* Retrieval considers the applicable policy period.
* Results contain source metadata such as page number.
* Relevant chunks are returned with similarity scores.

**Status:** Implemented.

---

## FR-07 — Hybrid Retrieval

The retrieval process shall combine semantic and keyword relevance.

The implemented MVP combines:

* Dense similarity: 70%
* Keyword relevance: 30%

**Status:** Implemented.

---

## FR-08 — Coverage Analysis

The Coverage Matcher shall identify the coverage relevant to the claim.

**Acceptance Criteria:**

* Claim description is compared with available coverage information.
* The resulting analysis references the applicable policy.
* Retrieved policy evidence supports the analysis.

**Status:** Implemented.

---

## FR-09 — Exclusion Analysis

The Exclusion Analyst shall identify applicable exclusions.

**Acceptance Criteria:**

* Policy exclusions are considered.
* Matching exclusions are reported.
* Claims with applicable exclusions can be rejected by deterministic workflow logic.

**Status:** Implemented.

---

## FR-10 — Anomaly Detection

The system shall identify claim anomalies.

Examples include:

* Claim amount exceeding the coverage limit.
* Invalid claim amount.
* Invalid incident date.
* Other configured claim consistency issues.

**Status:** Implemented.

---

## FR-11 — Deterministic Claim Calculation

The system shall calculate the approved amount using deterministic application code.

The calculation considers:

* Claimed amount
* Deductible
* Coverage limit

The LLM shall not perform the final arithmetic calculation.

**Status:** Implemented.

---

## FR-12 — Adjudication Recommendation

The system shall produce an adjudication recommendation.

Possible outcomes include:

* Approved
* Partially Approved
* Rejected

The recommendation includes reasoning based on coverage, exclusions, anomalies, and deterministic calculation results.

**Status:** Implemented.

---

## FR-13 — Specialist Agent Orchestration

The system shall orchestrate specialist agents.

Required specialists:

1. Coverage Matcher
2. Exclusion Analyst
3. Adjudication Drafter

The orchestrator coordinates the workflow and combines their results.

**Status:** Implemented.

---

## FR-14 — Human Approval Gate

The system shall create an approval request after an adjudication recommendation.

An authorized adjuster must explicitly approve or reject the recommendation.

**Acceptance Criteria:**

* An approval request is created.
* The request remains pending until reviewed.
* Adjuster can approve or reject.
* Approval actions are authorized by role and tenant.

**Status:** Implemented.

---

## FR-15 — Streaming / Progress

The system shall expose workflow progress to the client.

The orchestration workflow supports progress reporting and cancellation through the application layer.

The Angular MVP presents orchestration progress to the user.

**Status:** Implemented.

---

## FR-16 — Claims UI

The Angular UI shall allow users to:

* Log in.
* View dashboard statistics.
* View claims.
* Open claim details.
* Run AI orchestration.
* Review evidence.
* Review recommendations.
* Navigate to approval.
* Approve or reject a recommendation.

**Status:** Implemented.

---

# 6. Business Rules

### BR-01 — Policy Version

The policy version applicable to a claim is determined using the policy number and incident date.

### BR-02 — Coverage Limit

The final approved amount must not exceed the applicable coverage limit.

### BR-03 — Deductible

The deductible is subtracted from the claimed amount before applying the coverage limit.

### BR-04 — Exclusion

If an applicable exclusion is detected, the claim recommendation must be rejected.

### BR-05 — Human Approval

AI-generated recommendations are not final until approved by an authorized adjuster.

### BR-06 — Tenant Ownership

A tenant can only access its own claims, policies, documents, retrieval results, adjudication records, and approval requests.

### BR-07 — Deterministic Arithmetic

Claim amount calculations are performed by application code and are not delegated to the LLM.

---

# 7. Non-Functional Requirements

## NFR-01 — Security

The system shall enforce authentication, authorization, tenant isolation, validated inputs, and protected secrets.

**Status:** Partially implemented and documented.

## NFR-02 — Explainability

AI recommendations should expose supporting policy evidence.

**Status:** Implemented.

## NFR-03 — Maintainability

Business logic shall remain separated from infrastructure and external AI/vector/database implementations.

**Status:** Implemented through the layered architecture.

## NFR-04 — Testability

Domain and application logic shall be testable without depending on live LLM calls where possible.

**Status:** Implemented for the current evaluation and deterministic logic tests.

## NFR-05 — Observability

The system should provide logs and inspectable workflow results.

**Status:** Partially implemented.

---

# 8. Out of Scope

The following are outside the current MVP scope:

* Production-grade cloud deployment.
* Fully managed vector database.
* Horizontal autoscaling.
* Production message broker.
* Enterprise secrets manager.
* Production-grade distributed rate limiting.
* Advanced claims fraud scoring.
* Real insurance company integrations.
* Real customer personal data.
* Automated final approval without human review.

These items are documented as future improvements rather than being represented as implemented functionality.

---

# 9. Assumptions

1. The corpus contains synthetic or public insurance policy information.
2. Users belong to exactly one tenant in the current MVP.
3. Policy versions are represented using effective dates and version numbers.
4. Gemini is the hosted AI provider used by the MVP.
5. A local AI provider abstraction exists for environments where a hosted provider is unavailable.
6. SQL Server is used as the relational persistence store.
7. The Angular application and .NET API run locally during the demonstration.

---

# 10. Risks

| Risk                      | Impact   | Mitigation                                    |
| ------------------------- | -------- | --------------------------------------------- |
| Wrong policy version      | High     | Date-aware policy retrieval                   |
| Arithmetic hallucination  | High     | Deterministic calculation service             |
| Cross-tenant data leakage | Critical | Tenant-aware repository filtering             |
| Prompt injection          | High     | Input/content separation and evaluation cases |
| Unsupported AI claims     | High     | Retrieved policy evidence                     |
| Excessive AI agency       | High     | Human approval gate                           |
| API key exposure          | Critical | User Secrets / environment configuration      |
| Retrieval errors          | Medium   | Golden evaluation dataset                     |
| AI provider outage        | Medium   | Provider abstraction and local provider       |

---

# 11. Traceability Matrix

| Requirement                       | Status      | Evidence                                  |
| --------------------------------- | ----------- | ----------------------------------------- |
| FR-01 Authentication              | Implemented | AuthController + JWT                      |
| FR-02 RBAC                        | Implemented | Authorize roles                           |
| FR-03 Multi-tenancy               | Implemented | TenantContext + tenant-aware repositories |
| FR-04 Policy version matching     | Implemented | Policy retrieval by incident date         |
| FR-05 Document ingestion          | Implemented | Extraction/chunking/embedding pipeline    |
| FR-06 Tenant-aware retrieval      | Implemented | DocumentChunkRepository                   |
| FR-07 Hybrid retrieval            | Implemented | Dense + keyword scoring                   |
| FR-08 Coverage analysis           | Implemented | Coverage Matcher                          |
| FR-09 Exclusion analysis          | Implemented | Exclusion Analyst                         |
| FR-10 Anomaly detection           | Implemented | Anomaly Detection tool                    |
| FR-11 Deterministic calculation   | Implemented | ClaimAmountCalculator                     |
| FR-12 Adjudication recommendation | Implemented | Adjudication workflow                     |
| FR-13 Agent orchestration         | Implemented | ClaimAdjudicationOrchestrator             |
| FR-14 Human approval              | Implemented | Approval API + Angular UI                 |
| FR-15 Progress/streaming          | Implemented | Streaming endpoint + progress reporting   |
| FR-16 Claims UI                   | Implemented | Angular application                       |
| Production deployment             | Deferred    | Local MVP                                 |
| Managed rate limiting             | Deferred    | Documented architectural gap              |
| Distributed observability stack   | Deferred    | Application logging in MVP                |
| Cloud infrastructure              | Deferred    | Local deployment                          |

---

# 12. MVP Success Criteria

The MVP is considered successful when:

1. An authenticated adjuster can access only their tenant's data.
2. A claim retrieves the correct policy version based on incident date.
3. Relevant policy chunks are returned with source metadata.
4. Coverage and exclusions are analyzed.
5. Claim amounts are calculated deterministically.
6. Anomalies are reported.
7. Specialist agents produce an adjudication recommendation.
8. A human adjuster must approve or reject the recommendation.
9. The Angular UI exposes the complete workflow.
10. Evaluation tests demonstrate retrieval and tenant-isolation behavior.

---

# 13. Future Improvements

With additional implementation time, the following improvements would be prioritized:

1. Production-grade distributed rate limiting.
2. Managed vector database.
3. Distributed tracing and centralized observability.
4. Cloud deployment and CI/CD environments.
5. Automated fraud/anomaly scoring using richer signals.
6. More advanced retrieval reranking.
7. Production-grade secrets management.
8. Full streaming UI using the backend SSE endpoint.
9. Expanded evaluation dataset and automated regression tracking.
10. Additional tenant administration capabilities.
