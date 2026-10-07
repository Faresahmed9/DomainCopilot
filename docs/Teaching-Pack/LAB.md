# DomainCopilot Hands-On Lab

## 1. Lab Overview

**Duration:** 30–35 minutes

**Topic:** Agentic RAG for Insurance Claims Adjudication

The trainee will trace an insurance claim through DomainCopilot and verify how RAG, specialist agents, deterministic calculations, and human approval work together.

---

## 2. Learning Outcomes

By the end of the lab, trainees should be able to:

* Run the DomainCopilot API.
* Authenticate using a demo account.
* Retrieve an insurance claim.
* Identify the applicable policy version.
* Inspect retrieved policy evidence.
* Explain the role of specialist agents.
* Verify deterministic financial calculations.
* Complete the human approval step.
* Explain why tenant isolation is enforced by the backend.

---

## 3. Prerequisites

Trainees should have:

* .NET 8 SDK.
* SQL Server.
* Visual Studio 2022.
* Node.js and npm.
* Basic C# knowledge.
* Basic understanding of REST APIs.

---

## 4. Lab Scenario

You are an insurance adjuster reviewing:

```text
Claim Number: CLM-1001
Policy Number: POL-1001
Incident Date: 2026-06-15
Claimed Amount: 10,000
```

Your task is to determine how DomainCopilot reaches its recommendation.

---

## 5. Task 1 — Start the API

From the solution directory:

```powershell
dotnet restore
dotnet build
dotnet run --project DomainCopilot.Api
```

Open Swagger from the running API.

### Expected Output

The ASP.NET Core API starts successfully and Swagger is available.

---

## 6. Task 2 — Authenticate

Use the login endpoint with the Tenant A adjuster account:

```text
Username: adjuster.a
Password: Adjuster123!
```

### Expected Output

A valid JWT token is returned.

The authenticated identity contains the Tenant A identity and Adjuster role.

---

## 7. Task 3 — Retrieve the Claim

Retrieve claim:

```text
CLM-1001
```

Inspect:

* Claim number.
* Policy number.
* Incident date.
* Claimed amount.
* Claim description.

### Expected Output

The claim belongs to Tenant A and has a claimed amount of 10,000.

---

## 8. Task 4 — Identify the Policy Version

Determine which version of `POL-1001` applies to the incident date.

### Question

Which policy version should be used for an incident on:

```text
2026-06-15
```

### Expected Output

**Policy V2**

The system uses policy effective dates rather than asking an LLM to select between versions.

---

## 9. Task 5 — Inspect RAG Evidence

Inspect the retrieved policy evidence.

Look for:

* Vehicle damage coverage.
* Coverage limit.
* Deductible.
* Exclusions.
* Policy version information.

### Expected Output

Relevant evidence includes:

```text
Coverage: Vehicle Damage
Limit: 7,000
Deductible: 1,000
Policy Version: V2
```

---

## 10. Task 6 — Trace the Agents

Identify the three specialist agents.

Answer:

1. Which agent analyzes coverage?
2. Which agent analyzes exclusions?
3. Which agent drafts the adjudication recommendation?

### Expected Output

```text
Coverage Matcher
Exclusion Analyst
Adjudication Drafter
```

The orchestrator coordinates them.

---

## 11. Task 7 — Verify the Financial Calculation

Given:

```text
Claimed Amount = 10,000
Deductible = 1,000
Coverage Limit = 7,000
```

Calculate the recommended amount.

### Expected Output

```text
10,000 - 1,000 = 9,000

min(9,000, 7,000) = 7,000
```

The recommended amount is:

**7,000**

The calculation is performed by deterministic application code.

---

## 12. Task 8 — Inspect the Anomaly

Check whether an anomaly exists.

### Expected Output

The claim amount exceeds the coverage limit.

This is recorded as an anomaly while the deterministic calculation still limits the approved amount.

---

## 13. Task 9 — Review the Recommendation

The expected adjudication result is:

```text
Decision: Partially Approved
Recommended Amount: 7,000
```

Review the explanation and retrieved evidence before continuing.

---

## 14. Task 10 — Human Approval

Open the approval request.

As an Adjuster:

* Review the recommendation.
* Add a reviewer comment.
* Approve or reject the request.

### Expected Output

The approval action succeeds only for an authenticated user with the Adjuster role.

---

## 15. Task 11 — Tenant Isolation Challenge

Authenticate as Tenant A.

Attempt to retrieve a policy belonging to Tenant B.

### Expected Output

```text
[]
```

Repeat the opposite direction:

```text
Tenant B → Tenant A
```

### Expected Output

```text
[]
```

This demonstrates that tenant isolation is enforced by the backend.

---

## 16. Discussion Questions

1. Why should the LLM not calculate the final claim amount?
2. Why is incident date important when retrieving policies?
3. Why should tenant filtering not depend on the Angular UI?
4. What is the role of the orchestrator?
5. What would happen if the retrieval layer returned the wrong policy version?

---

## 17. Stretch Challenges

### Stretch Challenge 1

Change the claim amount and predict the deterministic result.

### Stretch Challenge 2

Create a new policy version and explain what metadata is required for version-aware retrieval.

### Stretch Challenge 3

Add an additional adversarial question to the golden evaluation dataset.

### Stretch Challenge 4

Propose an improvement to hybrid retrieval without removing tenant filtering.

---

## 18. Expected Final Result

For `CLM-1001`:

```text
Applicable Policy: V2
Coverage: Vehicle Damage
Coverage Limit: 7,000
Deductible: 1,000
Exclusion: None applicable
Anomaly: Claim exceeds coverage limit
Recommended Amount: 7,000
Decision: Partially Approved
Approval: Human Adjuster
```

---

## 19. Common Troubleshooting

### API does not start

Verify .NET 8 and SQL Server are available.

### Authentication fails

Verify the demo credentials and Tenant A configuration.

### Swagger cannot call the API

Verify the API is running and the correct HTTPS URL is being used.

### Angular cannot connect

Verify the API URL and CORS configuration.

### RAG returns unexpected results

Check policy number, incident date, tenant identity, and document processing status.
