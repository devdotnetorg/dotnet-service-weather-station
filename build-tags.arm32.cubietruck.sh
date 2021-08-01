#!/bin/bash

# $ chmod +x build-tags.arm32.cubietruck.sh
# $ ./build-tags.arm32.cubietruck.sh

set -e

echo "Start"

#Dockerfile.WS.Sensors.arm32.buster
#:armhf

docker build -f Dockerfile.WS.Sensors.arm32.buster -t devdotnetorg/dotnet-ws-sensors:v1-armhf .
docker build -f Dockerfile.WS.Sensors.arm32.buster -t devdotnetorg/dotnet-ws-sensors:armhf .

#WS.Panel V1
#Dockerfile.WS.Panel.AvaloniaX11WithXfce4.buster
#:armhf

docker build -f Dockerfile.WS.Panel.AvaloniaX11WithXfce4.buster -t devdotnetorg/dotnet-ws-panel:avaloniax11v1-armhf .
docker build -f Dockerfile.WS.Panel.AvaloniaX11WithXfce4.buster -t devdotnetorg/dotnet-ws-panel:avaloniax11-armhf .

#WS.Panel V2

docker build -f Dockerfile.WS.Panel.AvaloniaX11.buster -t devdotnetorg/dotnet-ws-panel:avaloniax11v2-armhf .

#Push
docker push devdotnetorg/dotnet-ws-sensors:v1-armhf
docker push devdotnetorg/dotnet-ws-sensors:armhf
docker push devdotnetorg/dotnet-ws-panel:avaloniax11v1-armhf
docker push devdotnetorg/dotnet-ws-panel:avaloniax11-armhf
docker push devdotnetorg/dotnet-ws-panel:avaloniax11v2-armhf

echo "END"
