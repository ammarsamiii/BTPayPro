# ===================== BUILD =====================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# On copie uniquement les .csproj nécessaires
# (le projet WebUI + le projet core BTPayPro dont il dépend)
COPY BTPayPro.WebUI/BTPayPro.WebUI.csproj BTPayPro.WebUI/
COPY BTPayPro/BTPayPro.csproj BTPayPro/

# Restore sur le projet WebUI
RUN dotnet restore BTPayPro.WebUI/BTPayPro.WebUI.csproj

# Maintenant on copie le code des deux projets
COPY BTPayPro.WebUI/ BTPayPro.WebUI/
COPY BTPayPro/ BTPayPro/

# Publish de la WebUI
RUN dotnet publish BTPayPro.WebUI/BTPayPro.WebUI.csproj -c Release -o /app/publish

# ==================== RUNTIME ====================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "BTPayPro.WebUI.dll"]
