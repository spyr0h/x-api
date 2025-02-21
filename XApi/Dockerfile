# Build Stage
FROM bitnami/dotnet-sdk:8-debian-11 AS build
WORKDIR /app
COPY . .
RUN dotnet restore "API/API.csproj"
RUN dotnet publish "API/API.csproj" -c Release -o output

# Serve Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
ARG BDD_CONNECTION_STRING
ARG PUBLIC_API_KEY
ARG PRIVATE_API_KEY
ENV BDD_CONNECTION_STRING $BDD_CONNECTION_STRING
ENV PUBLIC_API_KEY $PUBLIC_API_KEY
ENV PRIVATE_API_KEY $PRIVATE_API_KEY
ENV ASPNETCORE_URLS=http://+:80  
EXPOSE 80
WORKDIR /app
COPY --from=build /app/output .
RUN apt-get update && apt-get -y install libssl-dev && apt-get clean && rm -rf /var/lib/apt/lists/*
CMD [ "dotnet", "API.dll" ]