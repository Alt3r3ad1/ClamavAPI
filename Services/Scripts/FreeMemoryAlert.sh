#!/bin/bash

if [ $(echo "$(free -m)" | awk '/Mem:/ {print $4}') -lt "250" ]; then
  curl -s -X POST "https://api.telegram.org/XXXXXXXXXXXXXXXXXXX/sendMessage" -d "chat_id=XXXXXXXXXXXX" -d "text=Free Memory below 250MB"
fi
