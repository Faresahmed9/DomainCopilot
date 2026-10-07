# DomainCopilot Lab — Answer Key

## Task 1 — Start the API

Expected:

* Solution builds successfully.
* API starts successfully.
* Swagger is accessible.

---

## Task 2 — Authenticate

Expected account:

```text
Tenant: A
Role: Adjuster
Username: adjuster.a
```

Successful authentication returns a JWT.

---

## Task 3 — Retrieve the Claim

Expected:

```text
Claim: CLM-1001
Policy: POL-1001
Incident Date: 2026-06-15
Claimed Amount: 10,000
Tenant: Tenant A
```

---

## Task 4 — Policy Version

**Answer: V2**

Reason:

The incident occurred in 2026, and V2 is the applicable policy version based on effective-date metadata.

---

## Task 5 — RAG Evidence

Expected evidence includes:

```text
Vehicle Damage
Coverage Limit: 7,000
Deductible: 1,000
Policy Version: V2
```

The retrieval process combines dense similarity and keyword matching.

---

## Task 6 — Specialist Agents

Answers:

1. Coverage Matcher → coverage analysis.
2. Exclusion Analyst → exclusion analysis.
3. Adjudication Drafter → recommendation drafting.

The Orchestrator coordinates the workflow.

---

## Task 7 — Financial Calculation

Given:

```text
Claimed = 10,000
Deductible = 1,000
Limit = 7,000
```

Calculation:

```text
10,000 - 1,000 = 9,000
min(9,000, 7,000) = 7,000
```

**Answer: 7,000**

This is calculated deterministically by application code.

---

## Task 8 — Anomaly

Expected anomaly:

**Claimed amount exceeds coverage limit.**

The anomaly does not cause the LLM to override the deterministic financial rule.

---

## Task 9 — Recommendation

Expected:

text
Decision: Partially Approved
Recommended Amount: 7,000


Reason:

The claim has applicable coverage and no applicable exclusion, but the calculated amount is limited by the policy coverage limit.

---

## Task 10 — Human Approval

Expected:

* Approval request is pending.
* Adjuster reviews the recommendation.
* Adjuster can approve or reject.
* Approval endpoint requires the Adjuster role.

---

## Task 11 — Tenant Isolation

Expected:

text
Tenant A → Tenant B = []
Tenant B → Tenant A = []
```

Reason:

Tenant filtering is enforced in backend repositories/data access.

The Angular UI is not the security boundary.

---

# Discussion Answers

## 1. Why not let the LLM calculate the amount?

LLMs can make arithmetic mistakes. Financial calculations are therefore implemented as deterministic business logic.

## 2. Why does incident date matter?

Insurance policies may have multiple versions. The incident date determines which version was active when the event occurred.

## 3. Why not trust the Angular UI for tenant isolation?

Client-side filtering can be bypassed. Security must be enforced server-side at the authenticated and data-access layers.

## 4. What does the orchestrator do?

It coordinates retrieval, specialist agents, deterministic tools, adjudication, and approval creation.

## 5. What happens if the wrong policy version is retrieved?

The recommendation may be based on incorrect coverage, limits, or exclusions. This is why version-aware retrieval is a critical control.

---

# Stretch Challenge Answers

## Stretch Challenge 1

The expected result should be calculated using:

```text
approvedAmount =
min(claimedAmount - deductible, coverageLimit)
```

Subject to the calculator's input validation rules.

## Stretch Challenge 2

A new policy version should include:

* Policy number.
* Version identifier.
* Effective-from date.
* Effective-to date where applicable.
* Tenant identity.
* Policy content.

## Stretch Challenge 3

A valid adversarial question should attempt to trigger unsupported behavior while the expected answer remains grounded in authorized policy evidence and business rules.

## Stretch Challenge 4

Possible improvements include:

* Better chunking.
* Metadata-aware reranking.
* Improved keyword normalization.
* Additional evaluation cases.
* Reranking models.

Tenant and policy-version filters must remain enforced.

---

# Final Expected Result

text
Policy Version: V2
Coverage: Vehicle Damage
Limit: 7,000
Deductible: 1,000
Recommended Amount: 7,000
Decision: Partially Approved
Human Approval: Required
Tenant Isolation: Enforced

