#!/bin/bash
imageName="clamavapi"
if [ ! $(docker container inspect $imageName 2> /dev/null | grep "Running" | cut -d ":" -f2 | sed "s/ //" | sed "s/,//") ]; then
 ##SYSBOX INSTALL##
 dpkg -s sysbox-ce &> /dev/null
 if [ $? -ne 0 ]; then
  docker rm -f $(sudo docker ps -a -q)
  wget https://downloads.nestybox.com/sysbox/releases/v0.6.1/sysbox-ce_0.6.1-0.linux_amd64.deb
  apt-get -y install jq && apt-get -y install ./sysbox-ce_0.6.1-0.linux_amd64.deb
  rm -r ./sysbox-ce_0.6.1-0.linux_amd64.deb
 fi
 systemctl start sysbox

 if [ ! -d /var/log/clamavapi ]; then
  mkdir /var/log/clamavapi
 fi
 docker run -m 3000m --name $imageName \
        -d -it \
        -v /var/log/clamavapi/:/home/log/clamavapi/ \
        --runtime=sysbox-runc --rm $imageName

 sleep 5
fi

ipDocker=$(docker inspect  -f '{{range.NetworkSettings.Networks}}{{.IPAddress}}{{end}}' $imageName)
commandSSH="ssh -o StrictHostKeyChecking=accept-new admin@$ipDocker -fN -L 8666:127.0.0.1:5000"

ssh-keygen -f ~/.ssh/known_hosts -R $ipDocker

##SSHPASS##
dpkg -s sshpass &> /dev/null
if [ $? -ne 0 ]; then
 apt-get install -y sshpass
fi

##NET-TOOLS##
dpkg -s net-tools &> /dev/null
if [ $? -ne 0 ]; then
 apt-get install -y net-tools
fi

while true; do
 sshpass -p admin $commandSSH

 sleep 2
 if [ -n "$(netstat -nlpt | grep "8666" | grep "ssh")" ]; then
  systemctl start nginx
  break
 fi
done

while true; do
 echo 1 > /proc/sys/vm/drop_caches

 curl -s -X POST "https://api.telegram.org/XXXXXXXXXXXXXXX/sendMessage" -d "chat_id=XXXXXXXXXXXXX" -d "text=Free Cache Memory"

 sleep 2h
done
