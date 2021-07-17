#!/bin/bash

# Install buildx
# $ export DOCKER_BUILDKIT=1
# $ docker build --platform=local -o . git://github.com/docker/buildx
# $ mkdir -p ~/.docker/cli-plugins
# $ mv buildx ~/.docker/cli-plugins/docker-buildx

# Execute each time before building with buildx
# $ export DOCKER_BUILDKIT=1
# $ docker run --rm --privileged docker/binfmt:a7996909642ee92942dcd6cff44b9b95f08dad64
# $ cat /proc/sys/fs/binfmt_misc/qemu-aarch64

# $ chmod +x buildx-tags.test.sh
# $ ./buildx-tags.test.sh

set -e

echo "Start BUILDX"

#Dockerfile.WS.Sensors
#Error
docker buildx build --platform linux/arm -f Dockerfile.WS.Sensors.test.arm32.alpine -t devdotnetorg/dotnet-ws-sensors:armhf . --load

#Dockerfile.WS.Panel
#Error
#docker buildx build --platform linux/arm -f Dockerfile.WS.Panel.AvaloniaX11WithXfce4.alpine -t devdotnetorg/dotnet-ws-panel:armhf . --load

echo "BUILDX END"
