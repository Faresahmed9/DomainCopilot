# DomainCopilot — 90-Minute Teaching Session

## Slide 1 — Title

**DomainCopilot**

Agentic RAG for Insurance Claims Adjudication

D2 — Insurance Claims Adjudication
T0 — Multi-Tenancy

---

## Slide 2 — Learning Objectives

By the end of the session, trainees will be able to:

* Explain the basic RAG pipeline.
* Distinguish retrieval from generation.
* Explain an agentic workflow.
* Identify where deterministic code should replace LLM reasoning.
* Explain tenant isolation.
* Trace an insurance claim through the system.

---

## Slide 3 — The Business Problem

Insurance claims require:

* Policy lookup.
* Coverage verification.
* Exclusion checking.
* Limit and deductible calculation.
* Anomaly detection.
* Human review.

The goal is to assist adjusters without allowing AI to make uncontrolled financial decisions.

---

## Slide 4 — What Is RAG?

**RAG = Retrieval-Augmented Generation**

Instead of asking an LLM to answer from memory:

```text
Question
   ↓
Retrieve relevant documents
   ↓
Provide evidence to LLM
   ↓
Generate grounded answer
```

RAG reduces unsupported answers by grounding generation in retrieved evidence.

---

## Slide 5 — RAG Pipeline

```text
Documents
   ↓
Extract
   ↓
Clean
   ↓
Chunk
   ↓
Embed
   ↓
Index
   ↓
Retrieve
   ↓
Generate
```

Each stage has a specific responsibility.

---

## Slide 6 — Embeddings

An embedding represents text as a numerical vector.

Similar meanings tend to produce vectors that are closer together.

Example:

```text
"vehicle collision coverage"
        ↓
     Embedding
        ↓
[0.21, 0.74, ...]
```

Embeddings support semantic retrieval.

---

## Slide 7 — Hybrid Retrieval

DomainCopilot combines two signals:

* Dense semantic similarity.
* Keyword matching.

Current weighting:

```text
70% Dense Similarity
+
30% Keyword Score
```

This combines semantic understanding with exact terminology.

---

## Slide 8 — Why Policy Versions Matter

A policy can have multiple versions.

Example:

```text
POL-1001
 ├── V1 → effective 2025
 └── V2 → effective 2026
```

A claim from 2026 must use the applicable 2026 policy version.

The system uses the incident date during retrieval.

---

## Slide 9 — What Is an AI Agent?

An AI agent is a component that can use context, tools, and reasoning to perform a focused task.

DomainCopilot uses specialist agents instead of one large unrestricted prompt.

---

## Slide 10 — Specialist Agents

### Coverage Matcher

Finds applicable coverage.

### Exclusion Analyst

Identifies applicable exclusions.

### Adjudication Drafter

Produces a human-readable recommendation.

Each agent has a focused responsibility.

---

## Slide 11 — The Orchestrator

The orchestrator coordinates the workflow:

```text
Claim
 ↓
Context
 ↓
Retrieval
 ↓
Coverage
 ↓
Exclusions
 ↓
Anomalies
 ↓
Calculation
 ↓
Draft
 ↓
Approval
```

It controls the sequence and combines structured results.

---

## Slide 12 — Deterministic Business Rules

Financial calculations should not depend on LLM output.

Example:

```text
Claimed = 10,000
Deductible = 1,000
Limit = 7,000

10,000 - 1,000 = 9,000
min(9,000, 7,000) = 7,000
```

The calculator is deterministic application code.

---

## Slide 13 — Human-in-the-Loop

AI produces a recommendation.

It does not automatically finalize the claim.

```text
AI Recommendation
       ↓
Pending Approval
       ↓
Adjuster
   ↙       ↘
Approve   Reject
```

This creates a controlled decision boundary.

---

## Slide 14 — Multi-Tenancy

The platform supports multiple insurance tenants.

Each tenant has isolated:

* Users.
* Claims.
* Policies.
* Documents.
* Document chunks.
* Adjudications.

Tenant identity comes from the authenticated JWT.

---

## Slide 15 — Tenant Isolation

The system must prevent:

```text
Tenant A → Tenant B data
Tenant B → Tenant A data
```

The filtering is enforced in the backend data/repository layer.

The UI is not trusted to provide isolation.

---

## Slide 16 — Security Boundaries

Important security principles:

* Authenticate users.
* Authorize actions by role.
* Derive tenant identity from authentication.
* Keep secrets outside source control.
* Keep financial calculations deterministic.
* Require human approval.
* Validate retrieved evidence.

---

## Slide 17 — End-to-End Example

Claim:

```text
CLM-1001
POL-1001
Incident: 2026-06-15
Claimed: 10,000
```

System finds:

```text
Policy V2
Vehicle Damage Coverage
Limit = 7,000
Deductible = 1,000
No applicable exclusion
```

---

## Slide 18 — Final Recommendation

Deterministic calculation:

```text
10,000 - 1,000 = 9,000
min(9,000, 7,000) = 7,000
```

Result:

```text
Partially Approved
Recommended Amount = 7,000
```

An approval request is then sent to the adjuster.

---

## Slide 19 — Hands-On Lab

Trainees will:

1. Run the API.
2. Open Swagger.
3. Authenticate.
4. Retrieve a claim.
5. Inspect policy evidence.
6. Trace the adjudication workflow.
7. Verify the calculated amount.
8. Approve or reject the recommendation.

---

## Slide 20 — Discussion & Takeaways

Key takeaways:

* RAG retrieves evidence before generation.
* Agents should have focused responsibilities.
* Deterministic rules belong in code.
* Sensitive decisions should include human oversight.
* Tenant isolation must be enforced server-side.
* Good AI systems combine AI reasoning with traditional software engineering.
