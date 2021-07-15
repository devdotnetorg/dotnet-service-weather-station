#!/bin/sh
set -e

echo "Start entrypoint.sh"

echo "Set timezone"

if [ -e /etc/timezone ]; then
	rm /etc/timezone  
fi

echo ${TZ} >  /etc/timezone

echo "Run app"

exec "$@"