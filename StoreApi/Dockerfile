FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

COPY ["StoreApi.csproj", "./"]
RUN dotnet restore "StoreApi.csproj"

COPY . .
RUN dotnet publish "StoreApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
WORKDIR /app

RUN apk add --no-cache icu-libs

COPY --from=build --chown=$APP_UID:$APP_UID /app/publish .

USER $APP_UID

ENTRYPOINT ["dotnet", "StoreApi.dll"]
