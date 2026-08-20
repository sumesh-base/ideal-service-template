docker_build('ghcr.io/sumesh-base/helloapi', 'src/HelloApi')

# Pull the Helm chart from GHCR OCI registry
local('gh auth token | helm registry login ghcr.io -u sumesh-base --password-stdin')
local('rm -rf .tiltbuild/helloapi && helm pull oci://ghcr.io/sumesh-base/charts/helloapi --version 0.1.0 --untar --untardir .tiltbuild')

k8s_yaml(helm('.tiltbuild/helloapi', name='helloapi'))

k8s_resource('helloapi', port_forwards='8080:80', labels=['API'])

