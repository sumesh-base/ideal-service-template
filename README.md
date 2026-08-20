# IdealService

A minimal ASP.NET Core (.NET 8) Web API with a `hello` endpoint and a health check,
containerized with a minimal multi-stage Docker build, and deployed to a local
`kind` Kubernetes cluster via ArgoCD (GitOps).

## Endpoints

| Method | Path      | Description                                  |
|--------|-----------|-----------------------------------------------|
| GET    | `/`       | Returns a beautiful HTML system information UI |
| GET    | `/api/v1/info` | Returns raw JSON system and deployment info |
| GET    | `/health` | ASP.NET Core health check, returns `Healthy`  |

## Project layout

```
.
├── src/IdealService/
│   ├── Program.cs           # Minimal API with /hello and /health
│   ├── IdealService.csproj      # .NET 8 project file
│   ├── Dockerfile           # Multi-stage build, minimal chiseled runtime image
│   ├── .dockerignore
│   └── ...                  # appsettings, launchSettings, IdealService.http
└── k8s/
    └── deployment.yaml      # Deployment + Service for Kubernetes
```

## Running locally

Requires the .NET 8 SDK.

```bash
cd src/IdealService
dotnet run
curl http://localhost:<port>/hello
curl http://localhost:<port>/health
```

## Docker image

The `Dockerfile` uses a multi-stage build:

1. **Build stage** — `mcr.microsoft.com/dotnet/sdk:8.0-noble`, restores and publishes the app.
2. **Runtime stage** — `mcr.microsoft.com/dotnet/aspnet:8.0-noble-chiseled`, Microsoft's
   minimal "chiseled" runtime image: no shell, no package manager, runs as the
   built-in non-root `app` user by default. Final image size: **~121 MB**.

```bash
cd src/IdealService
docker build -t ghcr.io/sumesh-base/ideal-service:latest .
docker run -d -p 8080:8080 ghcr.io/sumesh-base/ideal-service:latest
curl http://localhost:8080/hello
curl http://localhost:8080/health
```

## Image registry

The image is published to GitHub Container Registry (GHCR):

```
ghcr.io/sumesh-base/ideal-service:latest
```

```bash
docker login ghcr.io
docker push ghcr.io/sumesh-base/ideal-service:latest
```

## Kubernetes deployment

`k8s/deployment.yaml` defines:

- A `Deployment` (3 replicas) running `ghcr.io/sumesh-base/ideal-service:latest`,
  with readiness/liveness probes against `/health`.
- A `ClusterIP` `Service` named `ideal-service` exposing port `80` → container port `8080`.

Apply directly with kubectl:

```bash
kubectl apply -f k8s/deployment.yaml
kubectl get pods -l app=ideal-service
```

## GitOps with ArgoCD

This repo is tracked by an ArgoCD `Application` (`ideal-service`) with automated
sync, self-heal, and pruning enabled. Any change pushed to `k8s/deployment.yaml`
on `main` is automatically applied to the cluster — no manual `kubectl apply`
needed.

ArgoCD Application spec (applied separately, not stored in this repo):

```yaml
apiVersion: argoproj.io/v1alpha1
kind: Application
metadata:
  name: ideal-service
  namespace: argocd
spec:
  project: default
  source:
    repoURL: https://github.com/sumesh-base/dotnetactions.git
    targetRevision: main
    path: k8s
    directory:
      include: "deployment.yaml"
  destination:
    server: https://kubernetes.default.svc
    namespace: default
  syncPolicy:
    automated:
      prune: true
      selfHeal: true
    syncOptions:
      - CreateNamespace=true
```

To force an immediate sync instead of waiting for ArgoCD's polling interval:

```bash
kubectl -n argocd annotate application ideal-service argocd.argoproj.io/refresh=hard --overwrite
```

## Verifying the deployment

```bash
kubectl run curltest --image=curlimages/curl:8.10.1 --rm -i --restart=Never -- \
  sh -c "curl -s http://ideal-service/hello; echo; curl -s http://ideal-service/health"
```
