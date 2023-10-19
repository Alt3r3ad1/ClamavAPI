service ssh start &>/dev/null
service docker start &> /dev/null

systemctl start kestrel-API
systemctl enable kestrel-API

systemctl start clamav-Docker
systemctl enable clamav-Docker

/bin/bash
