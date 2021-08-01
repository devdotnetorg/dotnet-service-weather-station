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

#WS.Sensors
#:aarch64 Alpine 3.13
docker buildx build --platform linux/arm64 -f Dockerfile.WS.Sensors.alpine -t devdotnetorg/dotnet-ws-sensors:v1-aarch64 . --push
docker buildx build --platform linux/arm64 -f Dockerfile.WS.Sensors.alpine -t devdotnetorg/dotnet-ws-sensors:v1-alpine-aarch64 . --push
docker buildx build --platform linux/arm64 -f Dockerfile.WS.Sensors.alpine -t devdotnetorg/dotnet-ws-sensors:aarch64 . --push
docker buildx build --platform linux/arm64 -f Dockerfile.WS.Sensors.alpine -t devdotnetorg/dotnet-ws-sensors:alpine-aarch64 . --push
#:amd64 Alpine 3.13
docker buildx build --platform linux/amd64 -f Dockerfile.WS.Sensors.alpine -t devdotnetorg/dotnet-ws-sensors:v1-amd64 . --push
docker buildx build --platform linux/amd64 -f Dockerfile.WS.Sensors.alpine -t devdotnetorg/dotnet-ws-sensors:v1-alpine-amd64 . --push
docker buildx build --platform linux/amd64 -f Dockerfile.WS.Sensors.alpine -t devdotnetorg/dotnet-ws-sensors:amd64 . --push
docker buildx build --platform linux/amd64 -f Dockerfile.WS.Sensors.alpine -t devdotnetorg/dotnet-ws-sensors:alpine-amd64 . --push
#:latest all platform Alpine 3.13
docker buildx build --platform linux/arm64,linux/amd64 -f Dockerfile.WS.Sensors.alpine -t devdotnetorg/dotnet-ws-sensors:v1 . --push
docker buildx build --platform linux/arm64,linux/amd64 -f Dockerfile.WS.Sensors.alpine -t devdotnetorg/dotnet-ws-sensors:v1-alpine . --push
docker buildx build --platform linux/arm64,linux/amd64 -f Dockerfile.WS.Sensors.alpine -t devdotnetorg/dotnet-ws-sensors:latest . --push

#WS.Panel
#Avalonia with Xfce4
#for compatibility
docker buildx build --platform linux/arm64 -f Dockerfile.WS.Panel.AvaloniaX11WithXfce4.alpine -t devdotnetorg/dotnet-ws-panel:avaloniax11 . --push
#:aarch64 Alpine 3.13
docker buildx build --platform linux/arm64 -f Dockerfile.WS.Panel.AvaloniaX11WithXfce4.alpine -t devdotnetorg/dotnet-ws-panel:avalonia-xfce4-alpine-aarch64 . --push
docker buildx build --platform linux/arm64 -f Dockerfile.WS.Panel.AvaloniaX11WithXfce4.alpine -t devdotnetorg/dotnet-ws-panel:avalonia-xfce4-aarch64 . --push
docker buildx build --platform linux/arm64 -f Dockerfile.WS.Panel.AvaloniaX11WithXfce4.alpine -t devdotnetorg/dotnet-ws-panel:aarch64 . --push
#:amd64 Alpine 3.13
docker buildx build --platform linux/amd64 -f Dockerfile.WS.Panel.AvaloniaX11WithXfce4.alpine -t devdotnetorg/dotnet-ws-panel:avalonia-xfce4-alpine-amd64 . --push
docker buildx build --platform linux/amd64 -f Dockerfile.WS.Panel.AvaloniaX11WithXfce4.alpine -t devdotnetorg/dotnet-ws-panel:avalonia-xfce4-amd64 . --push
docker buildx build --platform linux/amd64 -f Dockerfile.WS.Panel.AvaloniaX11WithXfce4.alpine -t devdotnetorg/dotnet-ws-panel:amd64 . --push
#:latest all platform Alpine 3.13
docker buildx build --platform linux/arm64,linux/amd64 -f Dockerfile.WS.Panel.AvaloniaX11WithXfce4.alpine -t devdotnetorg/dotnet-ws-panel:avalonia-xfce4 . --push
docker buildx build --platform linux/arm64,linux/amd64 -f Dockerfile.WS.Panel.AvaloniaX11WithXfce4.alpine -t devdotnetorg/dotnet-ws-panel:latest . --push

#WS.Panel
#Avalonia only Xorg
#:aarch64 Alpine 3.13
docker buildx build --platform linux/arm64 -f Dockerfile.WS.Panel.AvaloniaX11.alpine -t devdotnetorg/dotnet-ws-panel:avalonia-xorg-alpine-aarch64 . --push
docker buildx build --platform linux/arm64 -f Dockerfile.WS.Panel.AvaloniaX11.alpine -t devdotnetorg/dotnet-ws-panel:avalonia-xorg-aarch64 . --push
#:amd64 Alpine 3.13
docker buildx build --platform linux/amd64 -f Dockerfile.WS.Panel.AvaloniaX11.alpine -t devdotnetorg/dotnet-ws-panel:avalonia-xorg-alpine-amd64 . --push
docker buildx build --platform linux/amd64 -f Dockerfile.WS.Panel.AvaloniaX11.alpine -t devdotnetorg/dotnet-ws-panel:avalonia-xorg-amd64 . --push
#:latest all platform Alpine 3.13
docker buildx build --platform linux/arm64,linux/amd64 -f Dockerfile.WS.Panel.AvaloniaX11.alpine -t devdotnetorg/dotnet-ws-panel:avalonia-xorg . --push
echo "BUILDX END"
