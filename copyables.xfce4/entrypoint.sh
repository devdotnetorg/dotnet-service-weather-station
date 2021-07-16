#!/bin/sh
set -e

echo "Start entrypoint.sh"

echo "Set LANG"

export LANGUAGE=ru_RU.utf8
export LANG=ru_RU.utf8
export LC_ALL=ru_RU.utf8

echo "Set timezone"

if [ -e /etc/timezone ]; then
	rm /etc/timezone  
fi

echo ${TZ} >  /etc/timezone

echo "Run app"

exec "$@"