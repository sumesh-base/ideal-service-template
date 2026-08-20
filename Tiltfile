docker_build('ghcr.io/sumesh-base/helloapi', 'src/HelloApi')

helm_resource(
    'helloapi',
    'k8s/helloapi',
    port_forwards='8080:80'
)
