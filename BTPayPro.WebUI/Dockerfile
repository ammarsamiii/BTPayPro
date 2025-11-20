# ====== BUILD ======
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY BTPayPro/BTPayPro.csproj BTPayPro/
COPY BTPayPro.Api/BTPayPro.Api.csproj BTPayPro.Api/
COPY BTPayPro.WebUI/BTPayPro.WebUI.csproj BTPayPro.WebUI/

RUN dotnet restore BTPayPro.WebUI/BTPayPro.WebUI.csproj

COPY . .

RUN dotnet publish BTPayPro.WebUI/BTPayPro.WebUI.csproj -c Release -o /app/publish

# ====== RUNTIME ======
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "BTPayPro.WebUI.dll"]

