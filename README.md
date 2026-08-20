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

```bash
# 1. Install ArgoCD
kubectl create namespace argocd
kubectl apply -n argocd -f https://raw.githubusercontent.com/argoproj/argo-cd/stable/manifests/install.yaml

# 2. Apply the App-of-Apps manifest
kubectl apply -f manifests/argocd-apps.yaml
```

ArgoCD will automatically spawn the `ideal-service-dev`, `ideal-service-staging`, and `ideal-service-prod` applications and keep them synchronized with the `main` branch.

To force an immediate sync across all environments:
```bash
kubectl -n argocd annotate application ideal-service-dev argocd.argoproj.io/refresh=hard --overwrite
kubectl -n argocd annotate application ideal-service-staging argocd.argoproj.io/refresh=hard --overwrite
kubectl -n argocd annotate application ideal-service-prod argocd.argoproj.io/refresh=hard --overwrite
```
