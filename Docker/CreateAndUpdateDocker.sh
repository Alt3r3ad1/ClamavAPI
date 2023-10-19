#!/bin/bash

abort()
{
    echo -e "\033[1;31m [!] An error occurred. Exiting... \033[0m"
    exit 1
}

trap 'abort' 0
set -e

imageName="clamavapi"
archivesDir="../bin/Release/net6.0/publish/."

#SYSBOX INSTALL#
dpkg -s sysbox-ce &> /dev/null
if [ $? -ne 0 ]; then
   docker rm -f $(sudo docker ps -a -q)
   wget https://downloads.nestybox.com/sysbox/releases/v0.6.1/sysbox-ce_0.6.1-0.linux_amd64.deb
   apt-get -y install jq && apt-get -y install ./sysbox-ce_0.6.1-0.linux_amd64.deb
   rm -r ./sysbox-ce_0.6.1-0.linux_amd64.deb
fi
systemctl start sysbox

#PROJECT PUBLISH#
dotnet publish ../ -c Release

docker rm -f $imageName >/dev/null 2>&1
docker rmi -f $imageName >/dev/null 2>&1

#IMAGE BUILD#
if docker image inspect $imageName &> /dev/null; then
    echo "Docker image $imageName exists locally."
else
    echo "Docker image $imageName does not exist locally. Creating..."
    docker build -t $imageName ../
fi

#CONTAINER INIT#
if [ $(docker container inspect $imageName 2> /dev/null | grep "Running" | cut -d ":" -f2 | sed "s/ //" | sed "s/,//") ]; then
    echo "Docker container $imageName is already running."
else
    echo "Docker container $imageName does not exist. Creating..."
    if [ ! -d /var/log/$imageName ]; then
     mkdir /var/log/$imageName
    fi

    docker run --name $imageName \
               -d -it \
	       -v /var/log/$imageName/:/home/log/$imageName/ \
	       --runtime=sysbox-runc --rm $imageName

    dpkg -s sshpass &> /dev/null
    if [ $? -ne 0 ]; then
      apt-get install -y sshpass
    fi

    ipDocker=$(docker inspect  -f '{{range.NetworkSettings.Networks}}{{.IPAddress}}{{end}}' $imageName)
    ssh-keygen -f ~/.ssh/known_hosts -R $ipDocker >/dev/null &

    echo "Waiting services..."
    while true; do
     commandSSH="ssh -o StrictHostKeyChecking=accept-new admin@$ipDocker -fN -L 8666:127.0.0.1:5000 >/dev/null"
     sshpass -p admin $commandSSH

     sleep 4
     if [ -n "$(ps aux | grep "$commandSSH" | grep "?")" ]; then
        echo -e "\033[1;32m [!] Success, Docker container $imageName has the IP $(docker inspect -f '{{range.NetworkSettings.Networks}}{{.IPAddress}}{{end}}' $imageName) \033[0m"
        break
     fi
    done
fi

trap : 0
