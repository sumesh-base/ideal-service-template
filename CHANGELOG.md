# Changelog

All notable changes to this project will be documented in this file.

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
