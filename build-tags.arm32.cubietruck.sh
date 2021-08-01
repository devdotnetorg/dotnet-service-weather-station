#!/bin/bash

# $ chmod +x build-tags.arm32.cubietruck.sh
# $ ./build-tags.arm32.cubietruck.sh

set -e

echo "Start"

#WS.Sensors
#:armhf Debian 10 - buster 
docker build -f Dockerfile.WS.Sensors.arm32.buster -t devdotnetorg/dotnet-ws-sensors:v1-armhf .
docker build -f Dockerfile.WS.Sensors.arm32.buster -t devdotnetorg/dotnet-ws-sensors:v1-buster-armhf .
docker build -f Dockerfile.WS.Sensors.arm32.buster -t devdotnetorg/dotnet-ws-sensors:armhf .
docker build -f Dockerfile.WS.Sensors.arm32.buster -t devdotnetorg/dotnet-ws-sensors:buster-armhf .
#WS.Panel
#Avalonia with Xfce4
#:armhf Debian 10 - buster 
docker build -f Dockerfile.WS.Panel.AvaloniaX11WithXfce4.buster -t devdotnetorg/dotnet-ws-panel:avalonia-xfce4-buster-armhf .
docker build -f Dockerfile.WS.Panel.AvaloniaX11WithXfce4.buster -t devdotnetorg/dotnet-ws-panel:avalonia-xfce4-armhf .
docker build -f Dockerfile.WS.Panel.AvaloniaX11WithXfce4.buster -t devdotnetorg/dotnet-ws-panel:armhf .
#WS.Panel
#Avalonia only Xorg
#:armhf Debian 10 - buster 
docker build -f Dockerfile.WS.Panel.AvaloniaX11.buster -t devdotnetorg/dotnet-ws-panel:avalonia-xorg-buster-armhf .
docker build -f Dockerfile.WS.Panel.AvaloniaX11.buster -t devdotnetorg/dotnet-ws-panel:avalonia-xorg-armhf .

#Push
docker push devdotnetorg/dotnet-ws-sensors:v1-armhf
docker push devdotnetorg/dotnet-ws-sensors:v1-buster-armhf
docker push devdotnetorg/dotnet-ws-sensors:armhf
docker push devdotnetorg/dotnet-ws-sensors:buster-armhf
docker push devdotnetorg/dotnet-ws-panel:avalonia-xfce4-buster-armhf
docker push devdotnetorg/dotnet-ws-panel:avalonia-xfce4-armhf
docker push devdotnetorg/dotnet-ws-panel:armhf
docker push devdotnetorg/dotnet-ws-panel:avalonia-xorg-buster-armhf
docker push devdotnetorg/dotnet-ws-panel:avalonia-xorg-armhf

echo "END"
