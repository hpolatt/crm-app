FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["CrmApp.API/CrmApp.API.csproj", "CrmApp.API/"]
COPY ["CrmApp.Application/CrmApp.Application.csproj", "CrmApp.Application/"]
COPY ["CrmApp.Core/CrmApp.Core.csproj", "CrmApp.Core/"]
COPY ["CrmApp.Domain/CrmApp.Domain.csproj", "CrmApp.Domain/"]
COPY ["CrmApp.Infrastructure/CrmApp.Infrastructure.csproj", "CrmApp.Infrastructure/"]

RUN dotnet restore "CrmApp.API/CrmApp.API.csproj"

COPY . .
WORKDIR "/src/CrmApp.API"
RUN dotnet build "CrmApp.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CrmApp.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "CrmApp.API.dll"]
