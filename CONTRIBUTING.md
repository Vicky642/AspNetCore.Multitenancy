# Contributing to AspNetCore.Multitenancy

Thank you for considering contributing! This guide will help you get started.

## Getting Started

1. **Fork** the repository
2. **Clone** your fork locally
3. **Create a branch** for your feature or fix: `git checkout -b feature/my-feature`
4. **Install** the .NET SDK (8.0+)
5. **Build** the solution: `dotnet build`
6. **Run tests**: `dotnet test`

## Development Workflow

1. Make your changes in a feature branch
2. Add or update tests for your changes
3. Ensure all tests pass: `dotnet test`
4. Update the `CHANGELOG.md` if applicable
5. Submit a pull request

## Code Style

- Follow the `.editorconfig` rules in the repository
- Use file-scoped namespaces
- Use `var` when the type is apparent
- Keep methods small and focused
- Document public APIs with XML doc comments

## Pull Request Guidelines

- Keep PRs focused on a single change
- Provide a clear description of the change
- Reference any related issues
- Ensure CI passes before requesting review

## Reporting Issues

- Use the GitHub issue templates
- Include .NET version, OS, and steps to reproduce
- Provide a minimal reproduction if possible

## License

By contributing, you agree that your contributions will be licensed under the MIT License.
