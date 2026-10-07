# Evaluation

## 1. Purpose

This document describes how DomainCopilot is evaluated for retrieval quality, adversarial robustness, deterministic financial calculations, and multi-tenant isolation.

The evaluation is designed to verify that the system produces evidence-backed recommendations while keeping high-risk calculations and tenant boundaries outside LLM control.

## 2. Golden Dataset

The project includes a golden dataset containing:

* 25 evaluation questions.
* At least 5 adversarial questions.
* Questions covering policy coverage, exclusions, limits, deductibles, policy versions, and claim adjudication.
* Tenant-aware retrieval scenarios.
* Version-aware retrieval scenarios.

Dataset location:

`DomainCopilot.Tests/GoldenDataset/GoldenQuestions.json`

The automated tests verify the expected dataset structure and minimum question counts.

## 3. Retrieval Evaluation

The retrieval evaluation measures whether the correct policy evidence is retrieved for a question.

The current retrieval pipeline combines:

* Dense embedding similarity.
* Keyword matching.
* Tenant filtering.
* Policy number filtering.
* Incident-date/version filtering.

Current ranking formula:

```text
Final Score = 70% Dense Similarity + 30% Keyword Score
```

The evaluation uses the top retrieved evidence to determine whether the expected policy information was found.

## 4. Current Retrieval Result

The current evaluation run achieved:

**76.47% retrieval accuracy**

This result represents the current MVP evaluation state and is not presented as production-grade retrieval accuracy.

The result is documented honestly so that future improvements can be measured against the same baseline.

## 5. Adversarial Evaluation

The golden dataset includes adversarial questions designed to test whether the system incorrectly follows unsupported instructions or retrieves inappropriate evidence.

Examples include attempts to:

* Ignore policy restrictions.
* Override deductible or coverage limits.
* Request unsupported policy information.
* Mix information from different policy versions.
* Access information belonging to another tenant.

The expected behavior is to rely on authorized policy evidence and deterministic business rules rather than unsupported instructions.

## 6. Policy Version Evaluation

Policy version correctness is a critical D2 requirement.

The evaluation verifies that the incident date determines the applicable policy version.

Example:

```text
Policy: POL-1001
Incident date: 2026-06-15
Expected version: V2
```

The retrieval layer applies effective-date filtering before returning evidence.

This prevents the LLM from selecting a policy version from an unfiltered set of documents.

## 7. Tenant Isolation Evaluation

Tenant isolation is a critical T0 requirement.

The system was manually verified using two tenants.

Expected behavior:

```text
Tenant A → Tenant B policy retrieval = []
Tenant B → Tenant A policy retrieval = []
```

Tenant filtering is enforced at the data/repository layer rather than relying on the Angular UI.

The same principle is applied to claims, users, documents, document chunks, adjudications, and approval requests.

## 8. Deterministic Financial Calculation Evaluation

Financial calculations are evaluated separately from LLM generation.

The calculation rules are deterministic:

```text
amountAfterDeductible = claimedAmount - deductible
approvedAmount = min(amountAfterDeductible, coverageLimit)
```

Example:

```text
Claimed amount = 10,000
Deductible = 1,000
Coverage limit = 7,000

10,000 - 1,000 = 9,000
min(9,000, 7,000) = 7,000
```

Expected result:

```text
Approved amount = 7,000
Decision = Partially Approved
```

The LLM is not responsible for calculating the final amount.

## 9. Automated Tests

The test suite covers:

* Golden dataset validation.
* Minimum adversarial question count.
* Deterministic claim amount calculations.
* Tenant isolation.
* RAG-related evaluation behavior.

Run the test suite with:

```powershell
dotnet test
```

## 10. Evaluation Reproducibility

Evaluation should be reproducible using the same:

* Golden dataset.
* Corpus.
* Retrieval configuration.
* Tenant configuration.
* Policy metadata.
* Evaluation procedure.

Changes to retrieval weights, chunking, embeddings, corpus documents, or prompts should be reflected in future evaluation runs.

## 11. Known Limitations

The current evaluation has several limitations:

* The corpus is an assessment corpus rather than a production insurance corpus.
* Retrieval accuracy depends on the embedding provider and document quality.
* The current evaluation dataset is relatively small.
* Some evaluation cases require semantic judgment.
* The current score should be treated as an MVP baseline.
* The evaluation does not represent a formal insurance compliance certification.

## 12. Future Improvements

Future iterations could improve evaluation through:

* Larger golden datasets.
* More adversarial cases.
* Retrieval recall@K and precision@K.
* Answer faithfulness evaluation.
* Citation correctness evaluation.
* Automated regression evaluation in CI.
* Multiple embedding providers.
* Human review of difficult evaluation cases.
* Production-scale anonymized insurance corpora.

## 13. Assessment Alignment

The evaluation addresses the assessment requirements through:

* 25-question golden dataset.
* 5+ adversarial questions.
* Version-aware retrieval evaluation.
* Tenant isolation verification.
* Deterministic financial calculation tests.
* Documented evaluation baseline.
* Reproducible evaluation process.
* Honest documentation of limitations.
