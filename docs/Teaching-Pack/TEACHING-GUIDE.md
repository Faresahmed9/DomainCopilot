# Teaching Guide — Learning Outcomes, Assessment Map & Common Mistakes

## 1. Session Overview

**Topic:** Domain Copilot — Agentic RAG for Insurance Claims Adjudication
**Duration:** 90 minutes
**Domain:** Insurance Claims Adjudication
**Focus:** RAG, Agentic Workflow, Deterministic Business Rules, Human-in-the-Loop, and Multi-Tenancy

---

## 2. Learning Outcomes

By the end of the session, trainees should be able to:

### LO1 — Explain RAG

Explain the purpose of Retrieval-Augmented Generation and describe how retrieval provides domain-specific evidence to an AI system.

### LO2 — Describe the RAG Pipeline

Identify the main stages:

1. Document ingestion
2. Text extraction
3. Cleaning
4. Chunking
5. Embedding
6. Indexing
7. Retrieval
8. Context generation

### LO3 — Understand Hybrid Retrieval

Explain why combining dense semantic retrieval with keyword retrieval can improve domain search.

The example system combines:

* 70% semantic similarity
* 30% keyword relevance

### LO4 — Explain Policy Version Awareness

Explain why insurance claims must use the policy version applicable to the incident date rather than simply retrieving the latest policy.

### LO5 — Understand Agentic Architecture

Identify the responsibilities of:

* Orchestrator
* Coverage Matcher
* Exclusion Analyst
* Adjudication Drafter

and explain how they cooperate to solve a claim.

### LO6 — Distinguish AI Reasoning from Deterministic Logic

Explain why financial calculations such as deductible and coverage-limit calculations should be implemented using deterministic application code instead of relying on an LLM.

### LO7 — Explain Human-in-the-Loop

Explain why the AI system produces a recommendation while an Adjuster remains responsible for approving or rejecting the final decision.

### LO8 — Explain Multi-Tenant Isolation

Explain how Tenant A and Tenant B data must remain isolated at the data-access layer.

A request from Tenant A must never retrieve Tenant B documents, policies, claims, or users.

### LO9 — Trace an End-to-End Claim

Given a claim, identify:

* Applicable policy version
* Relevant retrieved evidence
* Coverage result
* Exclusion result
* Anomalies
* Deterministic approved amount
* Final recommendation
* Human approval step

### LO10 — Identify AI System Risks

Recognize important risks including:

* Wrong policy version
* Hallucinated financial calculations
* Cross-tenant data leakage
* Unsupported AI recommendations
* Missing or weak evidence

---

## 3. Assessment Map

| Learning Outcome | Learning Activity          | Evidence of Learning                                         |
| ---------------- | -------------------------- | ------------------------------------------------------------ |
| LO1              | RAG explanation            | Trainee explains why retrieval is needed                     |
| LO2              | RAG pipeline walkthrough   | Trainee identifies pipeline stages                           |
| LO3              | Hybrid retrieval example   | Trainee explains dense + keyword retrieval                   |
| LO4              | Policy version exercise    | Trainee selects V1/V2 using incident date                    |
| LO5              | Agent workflow walkthrough | Trainee identifies specialist-agent responsibilities         |
| LO6              | Claim calculation exercise | Trainee calculates approved amount using deterministic rules |
| LO7              | Approval workflow          | Trainee identifies where human approval is required          |
| LO8              | Tenant isolation challenge | Trainee verifies cross-tenant retrieval returns no data      |
| LO9              | End-to-end lab             | Trainee traces CLM-1001 from claim to recommendation         |
| LO10             | Discussion questions       | Trainee identifies major AI/security risks                   |

---

## 4. Suggested 90-Minute Session Plan

### 0–10 min — Business Problem

Introduce insurance claims adjudication.

Discuss:

* Claims
* Policies
* Coverage
* Exclusions
* Deductibles
* Coverage limits
* Adjuster approval

---

### 10–25 min — RAG Fundamentals

Explain:

* Why an LLM alone is insufficient
* Document ingestion
* Chunking
* Embeddings
* Retrieval
* Context
* Generation

Use the insurance policy corpus as the example.

---

### 25–40 min — Hybrid and Version-Aware Retrieval

Demonstrate:

* Dense retrieval
* Keyword retrieval
* Combined ranking
* Policy version filtering
* Effective dates

Use CLM-1001 to show why the 2026 policy version is selected.

---

### 40–55 min — Agentic Workflow

Explain:

```text
Claim
  ↓
Orchestrator
  ├── Coverage Matcher
  ├── Exclusion Analyst
  └── Adjudication Drafter
  ↓
Deterministic Tools
  ├── Claim Amount Calculator
  └── Anomaly Detector
  ↓
Recommendation
  ↓
Human Adjuster Approval
```

---

### 55–70 min — Security and Multi-Tenancy

Discuss:

* Authentication
* Authorization
* Tenant ID
* Data-layer filtering
* RAG tenant isolation
* Cross-tenant leakage

Demonstrate that Tenant A cannot retrieve Tenant B documents.

---

### 70–85 min — Hands-On Lab

Trainees:

1. Authenticate
2. Retrieve a claim
3. Identify the policy version
4. Inspect retrieved evidence
5. Trace specialist agents
6. Verify deterministic calculation
7. Review anomaly detection
8. Review recommendation
9. Approve/reject as Adjuster
10. Verify tenant isolation

---

### 85–90 min — Assessment & Discussion

Ask:

1. Why can't we let the LLM calculate the payout?
2. Why does the incident date matter?
3. Where should tenant isolation be enforced?
4. Why is human approval still required?
5. What happens if retrieval returns the wrong policy version?

---

## 5. Assessment Success Criteria

A trainee successfully completes the session when they can:

* Explain RAG in their own words.
* Identify the main RAG pipeline stages.
* Explain the purpose of hybrid retrieval.
* Select the correct policy version using the incident date.
* Explain the role of each specialist agent.
* Calculate the claim amount correctly.
* Identify the deterministic calculation boundary.
* Explain the human approval boundary.
* Explain tenant isolation.
* Complete the end-to-end claim lab successfully.

---

## 6. Common Trainee Mistakes

### Mistake 1 — Letting the LLM Calculate Money

**Incorrect approach:**

Ask the LLM to calculate the final payout.

**Why it is dangerous:**

LLMs can make arithmetic mistakes or produce inconsistent results.

**Correct approach:**

Use deterministic application code for:

* Deductible
* Coverage limit
* Approved amount

The LLM can explain the result but should not be the source of the calculation.

---

### Mistake 2 — Retrieving the Latest Policy Instead of the Applicable Version

**Incorrect approach:**

Always retrieve the newest policy version.

**Why it is dangerous:**

A claim must be evaluated against the policy applicable when the incident occurred.

**Correct approach:**

Filter retrieval using:

* Tenant
* Policy number
* Incident date
* Effective dates

---

### Mistake 3 — Implementing Tenant Isolation Only in the UI

**Incorrect approach:**

Hide Tenant B data from Tenant A using Angular UI logic.

**Why it is dangerous:**

The API can still expose another tenant's data.

**Correct approach:**

Enforce tenant filtering at the data-access/repository layer and use the authenticated tenant context.

---

### Mistake 4 — Trusting Retrieved Text Without Checking Evidence

**Incorrect approach:**

Accept an AI recommendation without inspecting the retrieved policy evidence.

**Why it is dangerous:**

Wrong or irrelevant context can produce an incorrect recommendation.

**Correct approach:**

Expose retrieved evidence and verify:

* Policy version
* Effective date
* Coverage
* Limits
* Exclusions

---

### Mistake 5 — Treating Every Agent as an Unrestricted Chatbot

**Incorrect approach:**

Allow every agent to perform every operation.

**Why it is dangerous:**

It makes the workflow difficult to control, test, and secure.

**Correct approach:**

Give each specialist agent a focused responsibility and let the orchestrator coordinate them.

---

### Mistake 6 — Removing the Human Approval Step

**Incorrect approach:**

Automatically finalize every AI recommendation.

**Why it is dangerous:**

The system is intended to support the Adjuster, not replace the approval boundary.

**Correct approach:**

AI produces a recommendation → Adjuster reviews → Adjuster approves or rejects.

---

## 7. Instructor Evaluation Checklist

Before finishing the session, verify that the trainee can answer:

* [ ] What problem does RAG solve?
* [ ] What is an embedding?
* [ ] Why use hybrid retrieval?
* [ ] Why does policy version matter?
* [ ] What does the Coverage Matcher do?
* [ ] What does the Exclusion Analyst do?
* [ ] What does the Adjudication Drafter do?
* [ ] Why is the calculator deterministic?
* [ ] Where is human approval performed?
* [ ] How is tenant isolation enforced?
* [ ] What evidence supports the final recommendation?

---

## 8. Expected End State

For the sample claim **CLM-1001**:

* Policy: POL-1001
* Applicable version: V2
* Claimed amount: 10,000
* Deductible: 1,000
* Coverage limit: 7,000
* Approved amount: 7,000
* Decision: Partially Approved
* Anomaly: Claimed amount exceeds coverage limit
* Human action: Adjuster approval required

The trainee should be able to explain not only the final result, but also **why the system reached it and which parts were deterministic versus AI-assisted**.
