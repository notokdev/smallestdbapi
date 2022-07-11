# Setup MySql
FROM mysql:debian as databasedotnet

# Set MySql image environment variables
ENV MYSQL_ROOT_PASSWORD=Your_password123
ENV MYSQL_DATABASE=smallestdbapi

# Install aspnetcore-runtime
USER root
RUN apt-get update; \
  apt-get install -y wget
RUN wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
RUN dpkg -i packages-microsoft-prod.deb
RUN rm packages-microsoft-prod.deb
RUN apt-get update; \
  apt-get install -y apt-transport-https && \
  apt-get update && \
  apt-get install -y aspnetcore-runtime-6.0

# Publish API
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /src

COPY /src ./
RUN dotnet restore
RUN dotnet publish -c Release -o out


# Start database and API
FROM databasedotnet

# Copy API
COPY --from=build-env /src/out /usr/work/app

# Copy entrypoint
RUN mkdir -p /usr/work
WORKDIR /usr/work/app
COPY entrypoint.sh .

# Set API environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS="http://+:8080" 

EXPOSE 8080

RUN groupadd -r devsecops && useradd --no-log-init -r -g devsecops devsecops
RUN mkdir /home/devsecops
RUN chown -R devsecops /usr/work/app
RUN chown -R devsecops /home/devsecops

USER devsecops

CMD /bin/bash ./entrypoint.sh