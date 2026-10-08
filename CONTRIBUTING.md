# Contributing to Domain Copilot

Thank you for contributing to Domain Copilot.

## Development Workflow

1. Create a feature branch from the current development branch.
2. Make focused changes related to one task.
3. Build and test the solution locally.
4. Use clear Conventional Commit messages.
5. Push the feature branch to GitHub.
6. Open a Pull Request for review.

## Commit Convention

Commits should follow the Conventional Commits format:

* `feat:` for new features
* `fix:` for bug fixes
* `docs:` for documentation
* `test:` for tests
* `refactor:` for code restructuring
* `ci:` for CI/CD changes
* `chore:` for maintenance

Example:

`feat: add tenant-aware policy retrieval`

## Security

Do not commit:

* API keys
* Passwords
* JWT signing keys
* Connection strings containing secrets
* Personal or production data

Use environment variables or local development secret storage for sensitive configuration.

## Testing

Before submitting a Pull Request:

```text
dotnet build --configuration Release
dotnet test --configuration Release --filter "Category!=Integration"
```

Integration tests may require local infrastructure and external service configuration.

## Pull Requests

Pull Requests should:

* Explain the purpose of the change.
* Keep changes focused.
* Include relevant tests where applicable.
* Avoid committing secrets or generated files.
