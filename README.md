# IdealService

A minimal, production-ready ASP.NET Core (.NET 8) Web API built with an enterprise-grade CI/CD and GitOps lifecycle.

## Endpoints

| Method | Path      | Description                                  |
|--------|-----------|-----------------------------------------------|
| GET    | `/`       | Returns a beautiful HTML system information UI |
| GET    | `/api/v1/info` | Returns raw JSON system and deployment info |
| GET    | `/health` | ASP.NET Core health check, returns `Healthy`  |

## Enterprise Production-Ready Features

This repository implements a full enterprise-grade CI/CD pipeline and GitOps workflow:

1. **Automated Testing**: Runs xUnit tests on every Pull Request to ensure code quality before merging.
2. **Container Security**: Uses Trivy to scan the built Docker images for OS and library vulnerabilities (CRITICAL/HIGH) before deploying.
3. **Multi-Architecture Builds**: Automatically cross-compiles immutable Docker images for both `linux/amd64` (Intel) and `linux/arm64` (Apple Silicon / AWS Graviton) via QEMU.
4. **Auto-Versioning**: Integrated with **Google Release Please**. Merging conventional commits automatically bumps `version.yaml`, updates `CHANGELOG.md`, and creates GitHub Tags and Releases.
5. **GitOps CD**: Uses an App-of-Apps pattern with **ArgoCD**. Automatically deploys distinct `dev`, `staging`, and `prod` environments isolated by namespaces.
6. **CodeQL SAST**: Natively scans C# source code for logical bugs and SQL injections inside Pull Requests.
7. **Automated Dependency Updates**: Uses Dependabot to auto-update NuGet packages, Docker base images, and GitHub Actions.
8. **Observability**: The .NET 8 API is instrumented with **OpenTelemetry** to export Metrics and Distributed Traces via OTLP.
9. **Secrets Management**: Integrated **HashiCorp Vault** into the Helm chart to allow safe commitment of encrypted secrets to the repository.
10. **Test & Security Visibility**: Test results (via Dorny Test Reporter) and Trivy vulnerability scans (via SARIF) are natively annotated and visualized directly on the PR timeline and GitHub Security tab.

## Developer Workflow (Commits & Versioning)

This repository enforces **Conventional Commits** and strict branching rules. All versions and releases are entirely automated based on your Pull Request titles!

### Branch Naming
All branches **must** contain a Jira ticket number (e.g., `PROJ-123`).
Examples of valid branches:
- `feat/PROJ-123-add-login`
- `fix/PROJ-456-resolve-deadlock`
- `PROJ-789-hotfix`

### How Versioning Works
When your Pull Request is merged into `main`, the bot reads the PR title to determine the semantic version bump:
- **Minor Bump** (`1.5.0` -> `1.6.0`): Use `feat:` (e.g., `feat: new payment gateway`)
- **Patch Bump** (`1.5.0` -> `1.5.1`): Use `fix:` (e.g., `fix: resolve crash loop`)
- **No Bump**: Use `docs:`, `chore:`, `refactor:`, or `test:`

### Emergency Hotfix Workflow
If you need to patch an older version in `staging` or `prod` *without* deploying the newest features from `main`, use the strict hotfix workflow:
1. **Checkout the stable tag:** `git checkout v1.5.0 -b PROJ-999-hotfix-1.5`
2. **Make the fix & update version:** Fix the bug, then manually bump `version.yaml` to `1.5.0-hotfix.1`.
3. **Tag & Push:**
   ```bash
   git commit -am "fix: urgent patch for production"
   git tag v1.5.0-hotfix.1
   git push origin PROJ-999-hotfix-1.5
   git push origin v1.5.0-hotfix.1
   ```
4. **Deploy via GitOps:** Because you pushed a valid semver tag, the CI pipeline will automatically build and publish the `1.5.0-hotfix.1` Docker image. Once the CI finishes, switch back to `main` and update `manifests/prod/values.yaml` to point to `1.5.0-hotfix.1`.

## Project Layout

```
.
├── src/
│   ├── IdealService/             # Minimal API application
│   └── IdealService.Tests/       # xUnit integration tests
├── k8s/
│   └── ideal-service/            # Helm Chart (Templates & defaults)
├── manifests/                    # GitOps Environment Overrides
│   ├── dev/                      # Development values.yaml
│   ├── staging/                  # Staging values.yaml
│   ├── prod/                     # Production values.yaml
│   └── argocd-apps.yaml          # ArgoCD App-of-Apps definition
├── .github/workflows/
│   ├── ci.yml                    # Build, Test, Scan, and Publish
│   ├── pr-validation.yml         # PR validation (Tests)
│   └── release-please.yml        # Automated semantic versioning
└── version.yaml                  # Single source of truth for versions
```

## Local Development

Requires the .NET 8 SDK.

```bash
cd src/IdealService
dotnet run
```

Run tests:
```bash
dotnet test src/IdealService.Tests/IdealService.Tests.csproj
```

## Docker Image

The `Dockerfile` uses a multi-stage build resulting in a highly secure, non-root, chiseled container:

1. **Build stage** — `mcr.microsoft.com/dotnet/sdk:8.0-noble`
2. **Runtime stage** — `mcr.microsoft.com/dotnet/aspnet:8.0-noble-chiseled` (Minimal attack surface, no shell, ~121 MB)

The CI pipeline pushes the resulting multi-arch image and packaged Helm chart to the GitHub Container Registry (GHCR).

## GitOps with ArgoCD

This repository follows the GitOps **App-of-Apps** pattern. The base Helm chart is defined in `k8s/ideal-service`, while environment-specific deployments are controlled via the `manifests/` directory.

To bootstrap the entire cluster locally in `kind`:

### Installed Cluster Operators (PoC)
To run the full GitOps and Secrets management workflow locally, the `kind` cluster is provisioned with the following operators:
1. **ArgoCD**: Continuously syncs the cluster state with the `main` branch.
2. **HashiCorp Vault**: Dynamically generates PostgreSQL database credentials and injects them into the `.NET` pods via a Sidecar.

### Bootstrapping the Cluster
```bash
# 1. Install ArgoCD
kubectl create namespace argocd
kubectl apply -n argocd -f https://raw.githubusercontent.com/argoproj/argo-cd/stable/manifests/install.yaml

# 2. Install and Configure HashiCorp Vault (Database Secrets Engine)
chmod +x scripts/setup-vault.sh
./scripts/setup-vault.sh

# 3. Apply the App-of-Apps manifest
kubectl apply -f manifests/argocd-apps.yaml
```

ArgoCD will automatically spawn the `ideal-service-dev`, `ideal-service-staging`, and `ideal-service-prod` applications and keep them synchronized with the `main` branch.

To force an immediate sync across all environments:
```bash
kubectl -n argocd annotate application ideal-service-dev argocd.argoproj.io/refresh=hard --overwrite
kubectl -n argocd annotate application ideal-service-staging argocd.argoproj.io/refresh=hard --overwrite
kubectl -n argocd annotate application ideal-service-prod argocd.argoproj.io/refresh=hard --overwrite
```
