#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/Common/Common.csproj", "Common/"]
COPY ["src/Consumer/Producer.csproj", "Consumer/"]
RUN dotnet restore "Consumer/Consumer.csproj"
COPY ["src/Consumer/*", "Consumer/"]
WORKDIR "/Consumer"
RUN dotnet build "Consumer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Consumer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["Consumer.exe"]
