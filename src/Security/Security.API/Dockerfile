FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Security/Security.API/Security.API.csproj", "src/Security/Security.API/"]
COPY ["src/Common/Common.Protos/Common.Protos.csproj", "src/Common/Common.Protos/"]
COPY ["src/Common/Common.Grpc/Common.Grpc.csproj", "src/Common/Common.Grpc/"]
COPY ["src/Common/Common.Web/Common.Web.csproj", "src/Common/Common.Web/"]
COPY ["src/Common/Common.Core/Common.Core.csproj", "src/Common/Common.Core/"]
COPY ["src/Security/Security.Infrastructure/Security.Infrastructure.csproj", "src/Security/Security.Infrastructure/"]
COPY ["src/Security/Security.Domain/Security.Domain.csproj", "src/Security/Security.Domain/"]
COPY ["src/Common/Common.Security/Common.Security.csproj", "src/Common/Common.Security/"]
COPY ["src/Security/Security.Application/Security.Application.csproj", "src/Security/Security.Application/"]
COPY ["src/Common/Common.Application/Common.Application.csproj", "src/Common/Common.Application/"]
COPY ["src/Common/Common.Logging/Common.Logging.csproj", "src/Common/Common.Logging/"]
RUN dotnet restore "src/Security/Security.API/Security.API.csproj"
COPY . .
WORKDIR "/src/src/Security/Security.API"
RUN dotnet build "Security.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Security.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Security.API.dll"]
