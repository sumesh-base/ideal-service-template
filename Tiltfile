docker_build('ghcr.io/sumesh-base/helloapi', 'src/HelloApi')

k8s_yaml(helm('k8s/helloapi', name='helloapi'))

k8s_resource('helloapi', port_forwards='8080:80')
