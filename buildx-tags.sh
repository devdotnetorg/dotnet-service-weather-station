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

#Dockerfile.WS.Sensors
#:aarch64
docker buildx build --platform linux/arm64 -f Dockerfile.WS.Sensors.alpine -t devdotnetorg/dotnet-ws-sensors:v1-aarch64 . --push
docker buildx build --platform linux/arm64 -f Dockerfile.WS.Sensors.alpine -t devdotnetorg/dotnet-ws-sensors:aarch64 . --push
#:amd64
docker buildx build --platform linux/amd64 -f Dockerfile.WS.Sensors.alpine -t devdotnetorg/dotnet-ws-sensors:v1-amd64 . --push
docker buildx build --platform linux/amd64 -f Dockerfile.WS.Sensors.alpine -t devdotnetorg/dotnet-ws-sensors:amd64 . --push
#:latest all platform
docker buildx build --platform linux/arm64,linux/amd64 -f Dockerfile.WS.Sensors.alpine -t devdotnetorg/dotnet-ws-sensors:latest . --push

#Dockerfile.WS.Panel V1
#:aarch64
docker buildx build --platform linux/arm64 -f Dockerfile.WS.Panel.AvaloniaX11WithXfce4.alpine -t devdotnetorg/dotnet-ws-panel:avaloniax11v1-aarch64 . --push
docker buildx build --platform linux/arm64 -f Dockerfile.WS.Panel.AvaloniaX11WithXfce4.alpine -t devdotnetorg/dotnet-ws-panel:avaloniax11-aarch64 . --push
docker buildx build --platform linux/arm64 -f Dockerfile.WS.Panel.AvaloniaX11WithXfce4.alpine -t devdotnetorg/dotnet-ws-panel:avaloniax11 . --push
#:amd64
docker buildx build --platform linux/amd64 -f Dockerfile.WS.Panel.AvaloniaX11WithXfce4.alpine -t devdotnetorg/dotnet-ws-panel:avaloniax11v1-amd64 . --push
docker buildx build --platform linux/amd64 -f Dockerfile.WS.Panel.AvaloniaX11WithXfce4.alpine -t devdotnetorg/dotnet-ws-panel:avaloniax11-amd64 . --push
docker buildx build --platform linux/arm64,linux/amd64 -f Dockerfile.WS.Panel.AvaloniaX11WithXfce4.alpine -t devdotnetorg/dotnet-ws-panel:avaloniax11 . --push
#:latest all platform
docker buildx build --platform linux/arm64,linux/amd64 -f Dockerfile.WS.Panel.AvaloniaX11WithXfce4.alpine -t devdotnetorg/dotnet-ws-panel:latest . --push

#Dockerfile.WS.Panel V2
#:aarch64
#docker buildx build --platform linux/arm64 -f Dockerfile.WS.Panel.AvaloniaX11.alpine -t devdotnetorg/dotnet-ws-panel:avaloniax11v2-aarch64 . --push
#docker buildx build --platform linux/arm64 -f Dockerfile.WS.Panel.AvaloniaX11.alpine -t devdotnetorg/dotnet-ws-panel:avaloniax11-aarch64 . --push
#docker buildx build --platform linux/arm64 -f Dockerfile.WS.Panel.AvaloniaX11.alpine -t devdotnetorg/dotnet-ws-panel:avaloniax11 . --push
#:latest all platform
#docker buildx build --platform linux/arm64 -f Dockerfile.WS.Panel.AvaloniaX11.alpine -t devdotnetorg/dotnet-ws-panel:latest . --push

echo "BUILDX END"
