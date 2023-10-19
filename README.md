# ClamavAPI

The main objective of this project is to detect malware in files, verify the type of content they contain and validate their actual extensions. All this is achieved through an ASP.NET C# and Razor application.

The project runs in an isolated environment, using [Nestybox](https://github.com/nestybox/sysbox) to manage two different [Docker](https://www.docker.com/) containers. The first container contains the essential dependencies for running the API, while the second container, housed in the first, is dedicated to initializing the [ClamAV](https://www.clamav.net/) malware analyzer.

Communication between the containers is established via the [nClam](https://github.com/tekmaven/nClam) library, ensuring that malware analysis is carried out effectively. In addition, the project includes an authenticity check for files based on their content, using the [MimeDetective](https://github.com/MediatedCommunications/Mime-Detective) library.

With this project, you can safely and efficiently scan files for malware and validate their integrity and content.

## Table of Contents
- [Usage](#usage)
- [Docker Setup](#docker-configuration)
- [License](#license)

## Usage

You can use this API to check files for viruses and verify their contents. Make HTTP requests to the "/scan" endpoint on port "8666" of "localhost", this port will be created from port forwarding using ssh, or access this same address via the web for visual interaction with the API.

## Docker configuration

This project is containerized using Docker and Nestybox to facilitate deployment and isolation. On the Debian distribution and its derivatives, you can use the "CreateAndUpdateDocker.sh" script to install and create the necessary dependencies and run it.

1. Run the "CreateAndUpdateDocker.sh" script in the "Docker" directory, wait for the image to be generated.
2. Wait for the script to run and validate that the API is available at the "localhost:8666" address.

## License

This project is licensed under the MIT License - see the file [LICENSE.md](LICENSE.md) for details.
