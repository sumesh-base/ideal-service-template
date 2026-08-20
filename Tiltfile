docker_build('ghcr.io/sumesh-base/ideal-service', 'src/IdealService')

# Pull the Helm chart from GHCR OCI registry
local('gh auth token | helm registry login ghcr.io -u sumesh-base --password-stdin')
local('rm -rf .tiltbuild/ideal-service && helm pull oci://ghcr.io/sumesh-base/charts/ideal-service --version 0.1.0 --untar --untardir .tiltbuild')

k8s_yaml(helm('.tiltbuild/ideal-service', name='ideal-service'))

k8s_resource('ideal-service', port_forwards='8080:8080', labels=['API'])

