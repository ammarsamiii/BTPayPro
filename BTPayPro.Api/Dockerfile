# ===== BUILD =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copier uniquement les .csproj de l’API et des projets dépendants
COPY BTPayPro.Api/BTPayPro.Api.csproj BTPayPro.Api/
COPY BTPayPro/BTPayPro.csproj BTPayPro/

RUN dotnet restore BTPayPro.Api/BTPayPro.Api.csproj

# Copier uniquement le code API + dépendances
COPY BTPayPro.Api/ BTPayPro.Api/
COPY BTPayPro/ BTPayPro/

RUN dotnet publish BTPayPro.Api/BTPayPro.Api.csproj -c Release -o /app/publish

# ===== RUNTIME =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "BTPayPro.Api.dll"]
