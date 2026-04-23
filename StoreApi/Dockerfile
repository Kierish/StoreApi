FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# 1. Install the dotnet-ef tool globally in the build stage
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

COPY ["StoreApi/StoreApi.csproj", "StoreApi/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
RUN dotnet restore "StoreApi/StoreApi.csproj"
COPY . .
WORKDIR "/src/StoreApi"
RUN dotnet build "./StoreApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./StoreApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# 2. Create the EF Core Migration Bundle
# We specify StoreApi as the startup project and Infrastructure as the target project
WORKDIR "/src"
RUN dotnet ef migrations bundle \
    --startup-project StoreApi/StoreApi.csproj \
    --project Infrastructure/Infrastructure.csproj \
    --configuration $BUILD_CONFIGURATION \
    -o /app/publish/efbundle \
    -r linux-x64 \
    --self-contained

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# 3. Give execution permissions to the bundle (Requires temporary root access)
USER root
RUN chmod +x ./efbundle
USER $APP_UID

# 4. Modify Entrypoint to run the bundle BEFORE starting the application
# The assembly name is "app" as defined in StoreApi.csproj (<AssemblyName>app</AssemblyName>)
ENTRYPOINT ["/bin/sh", "-c", "./efbundle && dotnet app.dll"]