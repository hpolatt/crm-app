FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["PktApp.API/PktApp.API.csproj", "PktApp.API/"]
COPY ["PktApp.Application/PktApp.Application.csproj", "PktApp.Application/"]
COPY ["PktApp.Core/PktApp.Core.csproj", "PktApp.Core/"]
COPY ["PktApp.Domain/PktApp.Domain.csproj", "PktApp.Domain/"]
COPY ["PktApp.Infrastructure/PktApp.Infrastructure.csproj", "PktApp.Infrastructure/"]

RUN dotnet restore "PktApp.API/PktApp.API.csproj"

COPY . .
WORKDIR "/src/PktApp.API"
RUN dotnet build "PktApp.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PktApp.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "PktApp.API.dll"]
