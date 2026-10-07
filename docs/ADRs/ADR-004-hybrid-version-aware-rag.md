# ADR-004: Hybrid and Version-Aware RAG Retrieval

* **Status:** Accepted
* **Date:** 2026-10-07
* **Decision Type:** Retrieval / AI Architecture

## 1. Context

The Domain Copilot platform uses Retrieval-Augmented Generation (RAG) to provide policy evidence to the claims adjudication workflow.

Insurance policies may contain multiple versions for the same policy number. Selecting the wrong version can lead to an incorrect claims decision.

The retrieval system therefore needs to consider:

* Tenant.
* Policy number.
* Incident date.
* Semantic relevance.
* Keyword relevance.

A purely semantic search could retrieve a relevant-looking passage from the wrong policy version. A purely keyword-based search could miss semantically relevant policy language.

## 2. Decision

The system uses **hybrid retrieval with policy-version and tenant-aware filtering**.

The current retrieval implementation combines:

* Dense embedding similarity.
* Keyword matching.

The scores are combined using the implemented weighting:

```text
Final Score = 70% Dense Similarity + 30% Keyword Score
```

The retrieval process returns the highest-ranked relevant document chunks.

Retrieval is intentionally separated from adjudication.

RAG is responsible for finding and ranking policy evidence. The adjudication workflow is responsible for interpreting the available evidence, applying business rules, performing deterministic financial calculations, and producing the adjudication decision.

## 3. Retrieval Responsibilities

The retrieval layer is responsible for:

* Generating the query embedding.
* Applying tenant-aware filtering.
* Applying policy and date filtering.
* Calculating dense similarity.
* Calculating keyword relevance.
* Combining retrieval scores.
* Returning the most relevant document chunks.

The retrieval layer does **not** own the final claim decision.

In particular, retrieval does not independently determine:

* Approved claim amount.
* Final decision status.
* Approval or rejection.
* Human approval state.

## 4. Adjudication Responsibilities

The adjudication workflow consumes the retrieved policy evidence and combines it with claim information.

The adjudication workflow is responsible for:

* Evaluating coverage.
* Evaluating exclusions.
* Detecting anomalies.
* Applying deterministic financial calculations.
* Creating the adjudication decision.
* Creating the approval request for human review.

The deterministic claim amount calculation remains outside the LLM and is performed by the application business logic.

The responsibility boundary is therefore:

```text
Retrieval
    ↓
Policy Evidence
    ↓
Adjudication Workflow
    ↓
Coverage / Exclusion Evaluation
    ↓
Deterministic Calculation
    ↓
Adjudication Decision
    ↓
Human Approval
```

## 5. Retrieval Flow

The current retrieval flow is:

```text
User Query
    ↓
Query Embedding
    ↓
Tenant + Policy + Date Filtering
    ↓
Dense Similarity
    +
Keyword Matching
    ↓
Combined Score
    ↓
Top Relevant Chunks
```

Tenant and policy/date filtering are applied during document chunk retrieval.

## 6. Tenant Filtering

Document chunks are tenant-scoped.

The retrieval query applies the current tenant identifier before returning candidate chunks.

This ensures that chunks belonging to another tenant are not included in the candidate retrieval set.

The RAG layer therefore inherits the application's tenant isolation boundary.

## 7. Policy Version Awareness

Policy retrieval is date-aware.

For a policy number, the applicable policy version is determined using the claim's incident date and the policy effective dates.

This prevents the retrieval process from treating all historical policy versions as equally applicable to the same claim.

For example:

```text
Policy: POL-1001

Incident Date: 2025-06-15
        ↓
Applicable Version: V1

Incident Date: 2026-06-15
        ↓
Applicable Version: V2
```

The 2026 claim therefore retrieves evidence from the 2026 policy version rather than the older version.

## 8. Hybrid Retrieval

Dense retrieval is useful for semantic similarity.

For example, a claim description may use wording that differs from the exact wording used in the policy.

Keyword matching provides an additional signal for important policy terms.

The implementation combines both signals:

```text
Dense Similarity × 0.70
        +
Keyword Score × 0.30
        =
Final Retrieval Score
```

The resulting score is used to rank candidate chunks.

## 9. No-Result Behavior

If tenant, policy, and date filtering produces no eligible document chunks, the retrieval operation returns an empty result set:

```json
[]
```

The retrieval layer does not fabricate policy evidence.

An empty retrieval result is therefore an explicit absence of retrieved evidence.

The retrieval layer does not convert an empty result into an approval or rejection decision.

That decision belongs to the adjudication workflow.

## 10. Decision-Boundary Failure Behavior

The adjudication workflow owns the decision boundary.

When retrieval provides no applicable evidence, retrieval itself does not decide the claim outcome.

Instead, the downstream adjudication logic evaluates whether the available claim and policy information is sufficient to establish coverage and exclusions.

Under the current implementation, when no applicable coverage or exclusion result is available to support the decision, the adjudication use case produces a rejected decision rather than assuming coverage.

Conceptually:

```text
No applicable coverage/exclusion result
                 ↓
        Adjudication Workflow
                 ↓
              Rejected
```

This separation is intentional:

* Retrieval reports what evidence was found.
* Adjudication determines what that evidence means for the claim.
* Deterministic business logic calculates the financial result.
* Human approval remains the final approval boundary.

The retrieval layer therefore never becomes the source of truth for the final adjudication status.

## 11. Tie-Breaking Behavior

The current retrieval implementation ranks results by the calculated hybrid relevance score and selects the configured top results.

There is **no separate business-level tie-breaking rule** based on document ID, page number, or chunk ID.

When two candidates have exactly the same calculated score, their relative ordering is determined by the underlying result ordering rather than by a separately defined domain rule.

The system does not claim deterministic secondary ordering for equal scores.

If deterministic ordering of equal-score results becomes a requirement, a future implementation can add an explicit secondary key such as chunk ID or document/page order.

## 12. Retrieved Evidence

The retrieval use case returns relevant chunks containing information such as:

* Policy version.
* Effective dates.
* Coverage conditions.
* Coverage limits.
* Deductibles.
* Exclusions.
* Adjudication conditions.

The claim details UI displays the retrieved evidence and its retrieval score to make the AI-assisted decision more transparent.

The UI presentation of evidence does not change the retrieval/adjudication responsibility boundary.

## 13. Example

For claim `CLM-1001`:

* Policy number: `POL-1001`
* Incident date: `2026-06-15`

The applicable policy is version V2.

Retrieved evidence includes policy passages describing:

* Collision coverage.
* Coverage limit of 7,000.
* Deductible of 1,000.
* Policy effective period.
* Relevant exclusions.
* Incident-date-based policy version selection.

The retrieval layer returns this evidence.

The adjudication workflow then evaluates the evidence and applies the deterministic claim amount calculation.

For this claim:

```text
Claimed Amount: 10,000
Deductible:      1,000
Coverage Limit:  7,000
Approved Amount: 7,000
Decision:        PartiallyApproved
```

## 14. Alternatives Considered

### A. Dense-Only Retrieval

Use only embedding similarity.

**Rejected because:**

* Important exact terms may receive insufficient weight.
* Exact policy terminology can be better captured by keyword matching.

### B. Keyword-Only Retrieval

Use only keyword matching.

**Rejected because:**

* Different wording can express the same concept.
* Semantic similarity is important when claim descriptions differ from policy wording.

### C. Retrieve All Policy Versions and Let the LLM Choose

The system could retrieve historical versions and ask the LLM to determine which one applies.

**Rejected because:**

* Version selection is a critical business rule.
* The LLM should not be responsible for deciding which policy version is authoritative.
* Date-aware filtering is more predictable and testable.

### D. Let Retrieval Produce the Final Adjudication

The retrieval component could directly classify a claim as approved or rejected based on retrieved text.

**Rejected because:**

* Retrieval and adjudication have different responsibilities.
* Financial calculations must remain deterministic.
* Coverage, exclusions, anomalies, and approval workflow belong to the adjudication layer.
* Separating these responsibilities makes the decision boundary easier to test and audit.

## 15. Consequences

### Positive

* Retrieval remains focused on evidence discovery.
* Adjudication remains the owner of business decisions.
* Financial calculations remain deterministic.
* Missing retrieval evidence cannot silently become an AI-generated policy conclusion.
* Tenant and policy-version filtering remain explicit retrieval responsibilities.
* The architecture is easier to test and audit.

### Negative

* The workflow contains more explicit boundaries between retrieval and adjudication.
* Missing or low-quality retrieval evidence must be handled by downstream adjudication logic.
* Retrieval quality directly affects the evidence available to adjudication.

## 16. Verification

The implementation has been verified with multiple policy versions.

For `POL-1001`:

```text
Incident: 2025-06-15
Expected: V1
Result:    V1 chunks returned
```

```text
Incident: 2026-06-15
Expected: V2
Result:    V2 chunks returned
```

The implementation was also manually checked for cross-tenant retrieval:

```text
Tenant A → Tenant B policy: []
Tenant B → Tenant A policy: []
```

The seeded adjudication scenario also demonstrates the responsibility boundary:

```text
RAG Evidence
    ↓
Coverage / Exclusion Evaluation
    ↓
Deterministic Calculation
    ↓
PartiallyApproved + 7,000
```

## 17. Current Limitations

The current retrieval implementation does not claim to provide:

* A dedicated external vector database.
* Neural reranking using a separate reranker model.
* Learned retrieval weights.
* An explicit secondary tie-breaking rule for equal scores.
* Production-scale distributed retrieval.
* Automatic retrieval-quality optimization.

The current MVP uses the implemented embedding, keyword, filtering, and scoring pipeline.

## 18. Compliance

This decision supports the assessment requirements for:

* RAG retrieval.
* Hybrid dense + keyword retrieval.
* Metadata-aware retrieval.
* Policy version/date awareness.
* Tenant isolation.
* Explainable retrieved evidence.
* Safe behavior when retrieval evidence is unavailable.
* Deterministic adjudication boundaries.

## 19. Future Evolution

Future versions could introduce:

* Dedicated vector database infrastructure.
* Learned or configurable retrieval weights.
* Explicit deterministic secondary ordering for equal scores.
* Cross-encoder reranking.
* Query expansion.
* Retrieval caching.
* Retrieval quality monitoring.
* Automatic evaluation against an expanded golden dataset.
