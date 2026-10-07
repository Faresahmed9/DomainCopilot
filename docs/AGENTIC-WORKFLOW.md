# Agentic Workflow

## 1. Purpose

DomainCopilot uses an agentic workflow to support insurance claims adjudication while keeping policy retrieval, tenant isolation, financial calculations, and human approval deterministic and controlled.

The workflow receives an insurance claim, retrieves the applicable policy evidence, evaluates coverage and exclusions, detects anomalies, calculates the recommended amount deterministically, drafts an adjudication recommendation, and sends the result to a human adjuster for approval.

## 2. High-Level Workflow

```text
Claim
  ↓
Orchestrator
  ↓
Claim Context Tool
  ↓
Policy Retrieval Tool
  ↓
Coverage Matcher Agent
  ↓
Exclusion Analyst Agent
  ↓
Anomaly Detection Tool
  ↓
Deterministic Claim Amount Calculator
  ↓
Adjudication Drafter Agent
  ↓
Adjudication Decision
  ↓
Human Adjuster Approval
```

## 3. Orchestrator

The `ClaimAdjudicationOrchestrator` coordinates the complete workflow.

Responsibilities:

* Load claim and policy context.
* Retrieve relevant policy evidence.
* Coordinate specialist agents.
* Run deterministic coverage and exclusion evaluation.
* Detect anomalies.
* Calculate the recommended amount.
* Create the adjudication decision.
* Create a pending approval request.
* Report progress.
* Support cancellation.

The orchestrator does not perform financial calculations using an LLM.

## 4. Specialist Agents

### 4.1 Coverage Matcher Agent

Purpose:

Determine whether the claim description matches an available policy coverage.

Inputs:

* Claim description.
* Applicable policy.
* Retrieved policy evidence.

Output:

* Coverage finding.
* Matching evidence.

### 4.2 Exclusion Analyst Agent

Purpose:

Identify whether an exclusion applies to the claim.

Inputs:

* Claim description.
* Applicable policy.
* Retrieved policy evidence.

Output:

* Exclusion finding.
* Supporting evidence.

### 4.3 Adjudication Drafter Agent

Purpose:

Produce a human-readable adjudication recommendation using the structured results produced by the workflow.

The agent drafts:

* Decision explanation.
* Coverage findings.
* Exclusion findings.
* Anomaly explanation.
* Recommended action.

The agent does not determine the final financial amount.

## 5. Deterministic Tools

The following operations remain deterministic and are not delegated to an LLM:

### Claim Amount Calculator

```text
amountAfterDeductible = claimedAmount - deductible
approvedAmount = min(amountAfterDeductible, coverageLimit)
```

The calculator validates inputs and returns the approved amount.

### Anomaly Detection

Detects conditions such as:

* Invalid claim amount.
* Invalid incident date.
* Claim amount exceeding coverage limit.

### Policy Retrieval

Retrieval is tenant-aware and policy-version-aware.

Retrieved chunks are filtered by:

* Tenant.
* Policy number.
* Incident date.
* Applicable policy version.

## 6. RAG Workflow

The RAG workflow uses:

1. Document extraction.
2. Text cleaning.
3. Chunking.
4. Embedding generation.
5. Dense similarity search.
6. Keyword matching.
7. Hybrid scoring.
8. Top relevant evidence selection.

Current hybrid ranking:

```text
Final Score = 70% Dense Similarity + 30% Keyword Score
```

The retrieval layer enforces tenant isolation before returning evidence.

## 7. Policy Version Awareness

The incident date determines which policy version is applicable.

For example:

```text
Claim incident date: 2026-06-15
Policy: POL-1001
Applicable version: V2
```

The system does not retrieve all versions and ask an LLM to select the correct one.

Instead, effective dates and policy metadata are applied during retrieval.

## 8. Multi-Tenant Isolation

The platform supports multiple tenants.

Tenant identity is obtained from the authenticated user's JWT `TenantId` claim.

Tenant filtering is applied at the repository/data-access layer.

This prevents a request belonging to Tenant A from retrieving:

* Tenant B claims.
* Tenant B users.
* Tenant B policies.
* Tenant B documents.
* Tenant B document chunks.
* Tenant B adjudication records.

Cross-tenant retrieval tests return no results.

## 9. Human Approval Boundary

AI-generated recommendations do not automatically become final business decisions.

After adjudication:

```text
AI Recommendation
       ↓
Pending Approval
       ↓
Adjuster Review
       ↓
Approve / Reject
```

The approval operation requires an authenticated user with the `Adjuster` role.

## 10. Progress and Cancellation

The orchestrator reports progress between major workflow steps.

Example progress stages:

```text
Loading claim context
Retrieving policy evidence
Running coverage analysis
Running exclusion analysis
Detecting anomalies
Calculating recommended amount
Drafting adjudication
Creating approval request
```

The workflow accepts a `CancellationToken` and checks cancellation between major steps.

## 11. Provider Abstraction

AI generation is accessed through the `IAiProvider` abstraction.

Current implementations include:

* `GeminiAiProvider`
* `LocalAiProvider`

Embeddings are intentionally separated through:

* `IEmbeddingService`
* `GeminiEmbeddingService`

This prevents application logic from depending directly on a specific AI provider SDK.

## 12. Agentic Design Principles

The implementation follows these principles:

1. Use agents for language-oriented reasoning and drafting.
2. Use deterministic code for financial calculations.
3. Keep tenant isolation outside the LLM.
4. Keep policy version selection outside the LLM.
5. Require human approval for consequential decisions.
6. Keep AI providers behind application interfaces.
7. Provide evidence for AI-generated recommendations.
8. Make workflow progress observable.
9. Support cancellation.
10. Document limitations and evaluation results honestly.

## 13. Current Limitations

The current implementation is an MVP assessment solution.

Known limitations include:

* Local AI provider is a lightweight alternative rather than full hosted-provider parity.
* Embedding implementation currently uses Gemini.
* Tenant isolation is application-level rather than database-per-tenant or SQL Row-Level Security.
* The current RAG evaluation score is documented in the evaluation report and is not presented as production-grade accuracy.
* Advanced production security controls such as enterprise identity integration, WAF, and dedicated vector infrastructure are outside the current scope.

## 14. Verification

The workflow has been verified through:

* Unit tests.
* Deterministic calculation tests.
* Golden dataset validation.
* RAG evaluation.
* Tenant isolation tests.
* Manual end-to-end claim adjudication.
* Manual approval flow verification.

## 15. Example End-to-End Execution

For claim `CLM-1001`:

```text
Claim received
      ↓
POL-1001 identified
      ↓
Incident date = 2026-06-15
      ↓
Policy V2 selected
      ↓
Relevant policy evidence retrieved
      ↓
Vehicle damage coverage matched
      ↓
No applicable exclusion detected
      ↓
Claim amount anomaly detected
      ↓
Deterministic calculation:
10,000 - 1,000 = 9,000
min(9,000, 7,000) = 7,000
      ↓
Decision = Partially Approved
      ↓
Approval Request created
      ↓
Adjuster reviews recommendation
```

## 16. Assessment Alignment

The workflow directly addresses the D2 Insurance claims adjudication requirements:

* Coverage Matcher Agent.
* Exclusion Analyst Agent.
* Adjudication Drafter Agent.
* Orchestrator.
* Version-aware policy retrieval.
* Deterministic financial calculation.
* Anomaly detection.
* Human approval gate.
* Multi-tenant isolation.
* Evidence-backed recommendations.
* Progress and cancellation.
* AI provider abstraction.
