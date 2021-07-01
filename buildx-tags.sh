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

# $ chmod +x buildx-tags.sh
# $ ./buildx-tags.sh

set -e

echo "Start BUILDX"

#:amd64
docker buildx build --platform linux/amd64 -f Dockerfile.alpine -t devdotnetorg/dotnet-gpioset:amd64 . --push
#:aarch64
docker buildx build --platform linux/arm64 -f Dockerfile.alpine -t devdotnetorg/dotnet-gpioset:aarch64 . --push
#:latest all platform
docker buildx build --platform linux/arm64,linux/amd64 -f Dockerfile.alpine -t devdotnetorg/dotnet-gpioset:latest . --push

echo "BUILDX END"