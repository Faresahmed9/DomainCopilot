# AI Usage Log

## 1. Purpose

This document records how AI-assisted development tools were used during the implementation of DomainCopilot.

AI assistance was used as a development aid. Final implementation decisions, code integration, testing, and verification were performed by the developer.

## 2. Areas Where AI Assistance Was Used

### Architecture and Planning

AI assistance was used to:

* Break the assessment requirements into implementation tasks.
* Discuss Clean Architecture boundaries.
* Suggest project structure and responsibilities.
* Review architectural trade-offs.

### Domain and Application Design

AI assistance helped with:

* Drafting initial entity and DTO structures.
* Designing use-case boundaries.
* Discussing tenant-aware repository patterns.
* Designing the claims adjudication workflow.

### RAG Design

AI assistance was used to understand and implement concepts related to:

* Document ingestion.
* Text extraction and cleaning.
* Chunking.
* Embeddings.
* Dense retrieval.
* Keyword retrieval.
* Hybrid retrieval.
* Policy-version-aware retrieval.

The final retrieval behavior was tested against the project corpus.

### Agentic Workflow

AI assistance helped design:

* Coverage Matcher Agent.
* Exclusion Analyst Agent.
* Adjudication Drafter Agent.
* Claim adjudication orchestrator.
* Tool boundaries.
* Progress reporting.
* Cancellation handling.

The final implementation was integrated and verified in the application.

### Security and Multi-Tenancy

AI assistance was used to reason about:

* JWT authentication.
* Role-based authorization.
* Tenant context.
* Repository-level tenant filtering.
* Cross-tenant access risks.
* Secret management.
* Human approval boundaries.

The implemented controls were manually and/or automatically verified.

### Testing and Evaluation

AI assistance helped create and review:

* Golden dataset structure.
* Adversarial evaluation scenarios.
* Deterministic financial calculation tests.
* Tenant isolation verification.
* RAG evaluation documentation.

Evaluation results were recorded based on actual project runs.

### Documentation

AI assistance was used to draft and improve:

* BRD.
* System Design.
* Architecture documentation.
* ADRs.
* Security documentation.
* Agentic Workflow documentation.
* Evaluation documentation.
* This AI Usage Log.

## 3. Human Verification

AI-generated suggestions were not treated as automatically correct.

The developer reviewed and verified:

* Compilation.
* Runtime behavior.
* API behavior through Swagger.
* Angular UI behavior.
* Database migrations.
* RAG retrieval behavior.
* Tenant isolation.
* Deterministic calculations.
* Approval workflow.
* Automated test results.

## 4. Security and Privacy

No real customer insurance information or real personal data was intentionally included in the project corpus.

Secrets such as API keys and JWT signing keys were kept outside source-controlled application code.

AI assistance was not used as a replacement for authentication, authorization, tenant isolation, or deterministic financial calculations.

## 5. AI Limitations

AI assistance can produce incorrect code, architectural suggestions, or technical explanations.

Therefore, generated suggestions were treated as proposals and were validated against:

* Project requirements.
* Existing code.
* Compiler/build results.
* Automated tests.
* Manual application testing.

## 6. Development Principle

The project follows this principle:

> AI assists development; the developer remains responsible for the final implementation, verification, and submitted result.

## 7. Assessment Transparency

This log is included to provide an honest record of AI-assisted development during the assessment project.

AI assistance was used extensively for planning, explanation, implementation support, debugging, testing guidance, and documentation. Final decisions and verification remained the developer's responsibility.
