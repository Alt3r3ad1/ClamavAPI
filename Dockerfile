FROM debian:latest

RUN apt-get -y update && \
    apt-get -y upgrade

RUN apt-get -y update && \
    apt-get -y install --no-install-recommends -y \
    apt-transport-https \
    ca-certificates \
    curl \
    gnupg2 \
    software-properties-common \
    openssh-server \
    cron

RUN curl -fsSL https://download.docker.com/linux/debian/gpg | apt-key add -
RUN apt-key fingerprint 0EBFCD88
RUN add-apt-repository \
       "deb [arch=amd64] https://download.docker.com/linux/debian \
       $(lsb_release -cs) \
       stable"

RUN apt-get -y update && \
     apt-get -y install --no-install-recommends -y docker.io

RUN apt-get -y update && \
    apt-get install -y wget && \
    wget https://packages.microsoft.com/config/debian/11/packages-microsoft-prod.deb -O packages-microsoft-prod.deb && \
    dpkg -i packages-microsoft-prod.deb && \
    rm packages-microsoft-prod.deb && \
    apt-get -y update && \
    apt-get install -y aspnetcore-runtime-6.0

RUN apt-get install -y systemctl

RUN apt-get purge -y wget && \
    apt-get purge -y curl

RUN apt-get clean -y && \
    rm -rf \
       /var/lib/apt/lists/* \
       /var/log/* \
       /tmp/* \
       /var/tmp/* \
       /usr/share/doc/* \
       /usr/share/man/* \
       /usr/share/local/*

RUN apt-get -y update && \
    apt-get autoremove -y

ADD ./bin/Release/net6.0/publish/. /API/
ADD ./Docker/kestrel-API.service /etc/systemd/system/
ADD ./Docker/clamav-Docker.sh /usr/local/bin/
ADD ./Docker/clamav-Docker.service /etc/systemd/system/
RUN systemctl daemon-reload

ADD ./Docker/initServices.sh /usr/local/bin/
RUN chmod +x /usr/local/bin/initServices.sh

RUN mkdir /home/log
RUN mkdir /home/log/clamavAPI

RUN useradd --create-home --shell /bin/bash admin && echo "admin:admin" | chpasswd
RUN mkdir /home/admin/.ssh && \
    chown admin:admin /home/admin/.ssh
RUN usermod -a -G docker admin

EXPOSE 22

ENTRYPOINT ["bash", "/usr/local/bin/initServices.sh"]
