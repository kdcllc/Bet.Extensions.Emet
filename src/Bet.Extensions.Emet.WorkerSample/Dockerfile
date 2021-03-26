#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["bet.extensions.emet.workersample/bet.extensions.emet.workersample.csproj", "bet.extensions.emet.workersample/"]
RUN dotnet restore "bet.extensions.emet.workersample/bet.extensions.emet.workersample.csproj"
COPY . .
WORKDIR "/src/bet.extensions.emet.workersample"
RUN dotnet build "bet.extensions.emet.workersample.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "bet.extensions.emet.workersample.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "bet.extensions.emet.workersample.dll"]