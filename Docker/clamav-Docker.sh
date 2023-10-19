#!/bin/bash
imageName="clamav"

while true; do
 if [ ! $(docker container inspect $imageName 2> /dev/null | grep "Running" | cut -d ":" -f2 | sed "s/ //" | sed "s/,//") ]; then
  if [ "$(docker inspect -f '{{.State.Health.Status}}' $imageName)" != "healthy" ] && [ "$(docker inspect -f '{{.State.Health.Status}}' $imageName)" != "starting" ]; then
   docker rm -f $imageName >/dev/null 2>&1

   docker run -d -it -p 3310:3310 --name $imageName --rm clamav/clamav
  fi
 else
  docker run -d -it -p 3310:3310 --name $imageName --rm clamav/clamav
 fi

 sleep 10
done
