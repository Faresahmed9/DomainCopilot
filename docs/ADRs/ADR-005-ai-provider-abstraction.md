# ADR-005: AI Provider and Embedding Abstraction

* **Status:** Accepted
* **Date:** 2026-10-07
* **Decision Type:** AI Architecture

## 1. Context

Domain Copilot uses two different AI capabilities:

1. **Text generation** for AI-assisted agent responses and reasoning support.
2. **Text embeddings** for semantic document retrieval.

These capabilities have different responsibilities, lifecycles, and infrastructure requirements.

Generation produces natural-language responses, while embeddings transform text into numerical vectors used by the RAG retrieval pipeline.

Coupling both capabilities to one provider would make it harder to change the embedding model independently from the generation provider.

## 2. Decision

The solution separates **generation** from **embeddings**.

Generation is exposed through:

```csharp
IAiProvider
```

Embeddings are exposed through:

```csharp
IEmbeddingService
```

The Application layer depends on these abstractions rather than directly depending on provider-specific SDKs.

The Infrastructure layer contains the concrete implementations.

Current implementations include:

* `GeminiAiProvider` — hosted generation provider.
* `LocalAiProvider` — alternative/local generation implementation.
* `GeminiEmbeddingService` — embedding implementation used by the RAG pipeline.

## 3. Responsibility Separation

The responsibility boundary is:

```text
AI Generation
     ↓
IAiProvider
     ↓
Agent / Application Workflow


Document / Query Text
     ↓
IEmbeddingService
     ↓
Embedding Vector
     ↓
RAG Retrieval
```

Generation and embeddings are therefore separate application capabilities.

## 4. Generation Abstraction

`IAiProvider` represents AI generation capabilities used by the application.

The Application layer requests generation through this abstraction.

The Application layer does not directly depend on:

* Gemini SDKs.
* Provider-specific HTTP APIs.
* Provider credentials.

Current generation implementations include:

* `GeminiAiProvider`
* `LocalAiProvider`

## 5. Embedding Abstraction

`IEmbeddingService` represents embedding generation for semantic retrieval.

The embedding service is responsible for converting text into numerical vectors.

It is used during:

* Document processing.
* Query embedding.
* Semantic similarity retrieval.

The current implementation is:

```text
IEmbeddingService
        ↓
GeminiEmbeddingService
        ↓
Embedding API
```

The embedding service is intentionally separate from `IAiProvider`.

## 6. RAG Responsibility

The RAG pipeline uses embeddings for retrieval rather than using generated text as the retrieval representation.

The flow is:

```text
Policy Document
      ↓
Text Extraction
      ↓
Chunking
      ↓
IEmbeddingService
      ↓
Embedding Vector
      ↓
Stored Document Chunk


User / Claim Query
      ↓
IEmbeddingService
      ↓
Query Vector
      ↓
Similarity Search
      ↓
Relevant Policy Evidence
```

The retrieved evidence is then provided to the adjudication workflow and AI-assisted agents where appropriate.

## 7. Provider Independence

Generation and embeddings can evolve independently.

For example:

```text
Generation:
GeminiAiProvider
        +
Embeddings:
GeminiEmbeddingService
```

could later become:

```text
Generation:
LocalAiProvider
        +
Embeddings:
AnotherEmbeddingProvider
```

without requiring the Application layer to change its business responsibilities.

This separation also allows the embedding model to be changed for retrieval-quality reasons without requiring the generation provider to change.

## 8. Configuration

Generation provider selection is controlled through:

```text
AI:Provider
```

When configured as `Local`, the local generation implementation is selected.

Embedding configuration is maintained independently from the generation provider abstraction.

Provider credentials are supplied through application configuration rather than being hard-coded into source code.

## 9. Security

Provider credentials must not be committed to source control.

Secrets are supplied through configuration mechanisms such as:

* .NET User Secrets during local development.
* Environment variables or secret management in deployment environments.

The repository should contain configuration examples without real credentials.

## 10. Architecture Boundaries

The dependency direction is:

```text
Domain
   ↑
Application
   ├── IAiProvider
   └── IEmbeddingService
          ↑
          │
Infrastructure
   ├── GeminiAiProvider
   ├── LocalAiProvider
   └── GeminiEmbeddingService
```

The Domain layer does not depend on either generation or embedding infrastructure.

The Application layer depends only on abstractions.

Infrastructure owns provider-specific implementation details.

## 11. Alternatives Considered

### A. One AI Interface for Generation and Embeddings

Rejected because generation and embeddings have different responsibilities and may need to use different models or providers.

### B. Direct Gemini Calls From Application

Rejected because this would couple application use cases to a specific provider and make provider replacement harder.

### C. Use Generated Text Instead of Embeddings for Retrieval

Rejected because semantic retrieval requires vector representations and similarity scoring.

### D. Put AI Integrations Inside the Domain Layer

Rejected because external AI services are infrastructure concerns and should not be part of the core business model.

## 12. Consequences

### Positive

* Generation and embeddings can evolve independently.
* RAG retrieval remains separated from text generation.
* Application logic is independent of provider-specific SDKs.
* Hosted and local generation implementations are supported.
* Embedding implementation can be replaced independently.
* Provider credentials remain outside business logic.
* The architecture is easier to test and maintain.

### Negative

* More interfaces and infrastructure components are required.
* Different providers may expose different capabilities.
* Embedding and generation configurations must be managed separately.
* Provider-specific failures still need Infrastructure-level handling.

## 13. Verification

The implementation has been verified with:

* `IAiProvider` resolving the configured generation implementation.
* `GeminiAiProvider` used for hosted generation.
* `LocalAiProvider` available as an alternative generation implementation.
* `IEmbeddingService` used by the document processing and retrieval pipeline.
* `GeminiEmbeddingService` providing embeddings independently of the generation abstraction.

The RAG workflow therefore does not require `IAiProvider` to perform embeddings.

## 14. Current Limitations

The current MVP does not claim complete feature parity between all providers.

The local generation provider is an alternative implementation boundary and is not claimed to provide identical capabilities to Gemini.

The current embedding implementation uses Gemini and does not yet provide multiple selectable embedding providers.

## 15. Future Evolution

Future versions could add:

* Additional generation providers.
* Additional embedding providers.
* Fully local embedding models.
* Provider health checks.
* Provider fallback policies.
* Independent embedding model configuration.
* Embedding model version management.
* AI cost and latency monitoring.
* Retrieval-quality monitoring by embedding model version.

## 16. Compliance

This decision supports the assessment requirements for:

* AI provider abstraction.
* Hosted AI provider.
* Alternative/local generation implementation.
* Separate embedding capability.
* RAG architecture.
* Clean Architecture dependency direction.
* Separation of application logic from external AI SDKs.
* Secure provider credential management.
