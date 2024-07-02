#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/TeamManagement.Api/TeamManagement.Api.csproj", "src/TeamManagement.Api/"]
COPY ["src/TeamManagement.Application/TeamManagement.Application.csproj", "src/TeamManagement.Application/"]
COPY ["src/TeamManagement.Core/TeamManagement.Core.csproj", "src/TeamManagement.Core/"]
COPY ["src/TeamManagement.Infra/TeamManagement.Infra.csproj", "src/TeamManagement.Infra/"]
RUN dotnet restore "./src/TeamManagement.Api/TeamManagement.Api.csproj"
COPY . .
WORKDIR "/src/src/TeamManagement.Api"
RUN dotnet build "./TeamManagement.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TeamManagement.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TeamManagement.Api.dll"]