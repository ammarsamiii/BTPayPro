# ====== BUILD ======
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# On copie uniquement les .csproj nécessaires
COPY BTPayPro/BTPayPro.csproj BTPayPro/
COPY BTPayPro.Api/BTPayPro.Api.csproj BTPayPro.Api/
COPY BTPayPro.WebUI/BTPayPro.WebUI.csproj BTPayPro.WebUI/

# Restore sur le projet API
RUN dotnet restore BTPayPro.Api/BTPayPro.Api.csproj

# Maintenant on copie tout le code
COPY . .

# Publish de l'API
RUN dotnet publish BTPayPro.Api/BTPayPro.Api.csproj -c Release -o /app/publish

# ====== RUNTIME ======
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "BTPayPro.Api.dll"]
