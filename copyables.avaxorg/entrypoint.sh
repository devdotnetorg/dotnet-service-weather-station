#!/bin/sh
set -e

echo "Start entrypoint.sh"

echo "Set LANG"

export LANGUAGE=ru_RU.utf8
export LANG=ru_RU.utf8
export LC_ALL=ru_RU.utf8

echo "Set timezone"

if [ ! -e /etc/localtime ]; then
	cp /usr/share/zoneinfo/${TZ} /etc/localtime	
fi

if [ ! -e /etc/timezone ]; then
	echo ${TZ} >  /etc/timezone		
fi

echo "Run app"

exec "$@"