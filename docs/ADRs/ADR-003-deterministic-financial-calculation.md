# ADR-003: Deterministic Financial Calculation

* **Status:** Accepted
* **Date:** 2026-10-07
* **Decision Type:** Business Logic / AI Safety

## 1. Context

The claims adjudication workflow calculates financially significant values such as:

* Approved claim amount.
* Coverage limit.
* Deductible.

These calculations must be predictable and reproducible.

Using an LLM to perform financial arithmetic could produce inconsistent or incorrect results because language models are not deterministic calculation engines.

The assessment explicitly identifies arithmetic hallucination as a core risk and requires limits, deductible, and payout calculations to be deterministic code rather than LLM-generated values.

## 2. Decision

All claim amount calculations are implemented as deterministic application code.

The system uses the `ClaimAmountCalculator` service to calculate the approved amount.

The calculation is exposed to the agentic workflow through `ClaimAmountCalculatorTool`, but the tool delegates the actual arithmetic to the deterministic application service.

The LLM does not perform or decide the numerical calculation.

## 3. Calculation Rules

The implemented calculation follows these rules:

1. A non-positive claimed amount results in an approved amount of zero.
2. A negative coverage limit is invalid.
3. A negative deductible is invalid.
4. The deductible is subtracted from the claimed amount.
5. If the amount remaining after the deductible is zero or negative, the approved amount is zero.
6. Otherwise, the approved amount is the smaller of:

   * Amount after deductible.
   * Coverage limit.

Conceptually:

```text
Amount After Deductible
        =
Claimed Amount - Deductible

Approved Amount
        =
minimum(
    Amount After Deductible,
    Coverage Limit
)
```

## 4. Example

For the seeded claim `CLM-1001`:

* Claimed amount: 10,000
* Deductible: 1,000
* Coverage limit: 7,000

The deterministic calculation produces:

```text
10,000 - 1,000 = 9,000

minimum(9,000, 7,000) = 7,000
```

Therefore:

* Claimed amount = 10,000
* Recommended/approved amount = 7,000

The resulting adjudication status is `PartiallyApproved`.

## 5. Separation from the LLM

The AI agents are responsible for tasks such as:

* Interpreting policy language.
* Identifying relevant coverage information.
* Identifying potential exclusions.
* Drafting an adjudication explanation.

The AI agents do not perform the final financial calculation.

The deterministic calculation service is called by the application workflow after the required coverage and exclusion information has been evaluated.

This creates a clear boundary:

```text
Policy / Claim Evidence
        ↓
AI-assisted analysis
        ↓
Coverage + Exclusion evaluation
        ↓
Deterministic calculation
        ↓
Adjudication decision
```

## 6. Tool Boundary

`ClaimAmountCalculatorTool` acts as an application tool used by the adjudication workflow.

The tool does not delegate arithmetic to the AI provider.

Instead:

```text
Orchestrator
    ↓
ClaimAmountCalculatorTool
    ↓
ClaimAmountCalculator
    ↓
Deterministic result
```

This keeps the numerical business rule independent from the selected AI provider.

## 7. Validation

The calculator validates invalid financial inputs.

The implementation rejects:

* Negative coverage limits.
* Negative deductibles.

It also handles zero or negative claim amounts and claims where the deductible consumes the entire claimed amount.

These rules are implemented directly in application code.

## 8. Alternatives Considered

### A. LLM-Based Calculation

The LLM could receive the claim amount, deductible, and limit and generate the approved amount.

**Rejected because:**

* Arithmetic errors are possible.
* Results may vary between model calls.
* Financial decisions should be reproducible.
* The assessment explicitly identifies arithmetic hallucination as a risk.

### B. Calculation in the UI

The Angular UI could calculate the approved amount.

**Rejected because:**

* The UI is not the authoritative business layer.
* API and other clients could produce different results.
* Business rules should be enforced server-side.

### C. Calculation in SQL

The calculation could be impl
