#!/bin/bash

apt-get -y update
apt-get -y upgrade

curl -s -X POST "https://api.telegram.org/XXXXXXXXXXXXXXXXXX/sendMessage" -d "chat_id=XXXXXXXXXXXXXXXX" -d "text=Restating Server"

/sbin/reboot
