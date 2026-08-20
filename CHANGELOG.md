# Changelog

All notable changes to this project will be documented in this file.

## [1.4.0](https://github.com/sumesh-base/ideal-service-template/compare/v1.3.0...v1.4.0) (2026-08-20)


### Features

* add opentelemetry, dependabot, codeql, and sealed secrets ([56f70ec](https://github.com/sumesh-base/ideal-service-template/commit/56f70ec059cd4af1938ab97deb026cf680f4f187))
* implement advanced enterprise features ([860431e](https://github.com/sumesh-base/ideal-service-template/commit/860431e980ff09ca324bed02d244f55fe607f2a7))

## [1.3.0](https://github.com/sumesh-base/ideal-service-template/compare/v1.2.1...v1.3.0) (2026-08-20)


### Features

* Add Helm chart and update CI/CD to push chart to GHCR ([358f520](https://github.com/sumesh-base/ideal-service-template/commit/358f5204e3d9aaf8528dbde7a25f53dccb90a585))
* add PR validation and strictly enforce branch protections ([f5ccdce](https://github.com/sumesh-base/ideal-service-template/commit/f5ccdce639a5e2218d0bae6d5a7981ca2c2e45dc))
* Add Tiltfile for local development ([447b99e](https://github.com/sumesh-base/ideal-service-template/commit/447b99e3e0f36d19120070b56c67aa120f3230b0))
* Automate GHCR login with PAT from .env and ignore .env file ([fd02cff](https://github.com/sumesh-base/ideal-service-template/commit/fd02cfff87cdf82dcee0ab180d42dd9fd5ad7b63))
* enforce version.yaml and CHANGELOG.md updates on push to main ([c741244](https://github.com/sumesh-base/ideal-service-template/commit/c74124427f835e92225ecb0b0c5eed51ff550f5a))
* implement testing, security scanning, and automated versioning via release-please ([0d3dd20](https://github.com/sumesh-base/ideal-service-template/commit/0d3dd20369e720ed5785a6479cb8ed89fea29a13))
* implement testing, security scanning, and automated versioning via release-please ([75b2e54](https://github.com/sumesh-base/ideal-service-template/commit/75b2e544581b1e7fafe19e057c92e1820b0ea06b))
* Pull Helm chart from GHCR inside Tiltfile ([9b97ead](https://github.com/sumesh-base/ideal-service-template/commit/9b97ead11cda6cb3b444d52e826a888271bb374a))
* rename project to ideal-service and add beautiful system info UI ([721d189](https://github.com/sumesh-base/ideal-service-template/commit/721d189ca06e05fa7331e79f6276baf40c065e06))
* setup full GitOps environment structures ([018bbc4](https://github.com/sumesh-base/ideal-service-template/commit/018bbc46815d7a1f78c44b44a77a3fc860aaf405))
* setup full GitOps environment structures ([d16fa3d](https://github.com/sumesh-base/ideal-service-template/commit/d16fa3d98fb83dd1fc50407075020245f9636f3c))
* setup proper dynamic SHA tagging for Docker images and Helm charts ([81f5b62](https://github.com/sumesh-base/ideal-service-template/commit/81f5b62a9e889faab4a29ac890a53e5d74e37c30))
* update UI to white theme and inject Helm/K8s info via env vars ([6a51311](https://github.com/sumesh-base/ideal-service-template/commit/6a51311590ea7c4ad96eeb126df253553666ffe4))
* Use gh auth token in Tiltfile instead of .env ([db68692](https://github.com/sumesh-base/ideal-service-template/commit/db686920a2cb0390f1427ae8298bbdac7b417630))
* use version.yaml as single source of truth for Docker and Helm tagging ([d2dcc92](https://github.com/sumesh-base/ideal-service-template/commit/d2dcc92d6764f6f0245886204928f322f4e862cb))


### Bug Fixes

* Add .tiltbuild to .tiltignore to prevent infinite loop ([953b0cf](https://github.com/sumesh-base/ideal-service-template/commit/953b0cf7127621613073c22d9a98dfb707a3c2c4))
* build multi-arch images via qemu for arm64 support ([e6a7cec](https://github.com/sumesh-base/ideal-service-template/commit/e6a7cec3095d151bceb5999d5292c4f5b7dc45d5))
* build multi-arch images via qemu for arm64 support ([54cc6a2](https://github.com/sumesh-base/ideal-service-template/commit/54cc6a2603f0fc79b32ab8e0ca7b07cec5f61dc0))
* Correct C# raw string interpolation syntax ([dfe9e90](https://github.com/sumesh-base/ideal-service-template/commit/dfe9e90a0f48a2f80a533308a2f8ce38f84cb0f7))
* Correct port forwarding in Tiltfile to point to container port 8080 ([135c19c](https://github.com/sumesh-base/ideal-service-template/commit/135c19c24eeb373278bc907c3e288a2a6cb962b6))
* refine GitHub Actions triggers and redundant checks for monorepo ([f223901](https://github.com/sumesh-base/ideal-service-template/commit/f2239014157b1ff88e04862e21c16427e70458f1))
* Remove existing helm chart directory before pull in Tiltfile ([ee53984](https://github.com/sumesh-base/ideal-service-template/commit/ee5398430dbc5e747dfd8c30439f0f6daf10fdb6))
* Remove non-existent test connection k8s_resource from Tiltfile ([e6fac97](https://github.com/sumesh-base/ideal-service-template/commit/e6fac97501c7340a11756e334d49bec99e94a33a))
* Remove unsupported tilt_resource from Tiltfile ([cad326f](https://github.com/sumesh-base/ideal-service-template/commit/cad326fae1e4c03087c46be2adf77b3d669ad4ed))
* skip Trivy image scan on PRs since image is not pushed ([6540a07](https://github.com/sumesh-base/ideal-service-template/commit/6540a07ec86d5c1472e685712b1ad9b9b7fcd79b))
* Update Tiltfile for Helm integration ([121a057](https://github.com/sumesh-base/ideal-service-template/commit/121a05786daaffb8b709b54700b82611dfd46b37))
* use triple-dot syntax in git diff for PR validation ([e182c33](https://github.com/sumesh-base/ideal-service-template/commit/e182c33f3c1fcd6d987b1d74e93048c679b0376a))

## [1.2.1] - 2026-08-20
### Fixed
- Configured GitHub Actions to build Docker images for both `linux/amd64` and `linux/arm64` via QEMU

## [1.2.0] - 2026-08-20
### Added
- Created `manifests/` directory for full multi-environment GitOps with ArgoCD (dev, staging, prod)
- Set up `argocd-apps.yaml` to define environments

## [1.1.1] - 2026-08-20
### Fixed
- Updated README.md to document the new UI endpoints correctly

## [1.1.0] - 2026-08-20
### Added
- PR Validation GitHub Action workflow to strictly enforce Changelog/Version checks
- Branch protection rules for main branch

## [1.0.0] - 2026-08-20
### Added
- Initial release of ideal-service
- Added beautiful system info UI
- Configured GHCR Docker and Helm deployments
