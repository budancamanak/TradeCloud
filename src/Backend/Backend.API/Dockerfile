# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /tradeCloud
COPY *.sln .
COPY ["src/Backend/Backend.API/Backend.API.csproj", "src/Backend/Backend.API/"]
COPY ["src/Common/Common.Core/Common.Core.csproj", "./src/Common/Common.Core/"]
COPY ["src/Common/Common.Messaging/Common.Messaging.csproj", "./src/Common/Common.Messaging/"]
COPY ["src/Common/Common.Plugin/Common.Plugin.csproj", "./src/Common/Common.Plugin/"]
COPY ["src/Common/Common.Protos/Common.Protos.csproj", "./src/Common/Common.Protos/"]
COPY ["src/Common/Common.Web/Common.Web.csproj", "./src/Common/Common.Web/"]
COPY ["src/Common/Common.Application/Common.Application.csproj", "./src/Common/Common.Application/"]
COPY ["src/Common/Common.Grpc/Common.Grpc.csproj", "./src/Common/Common.Grpc/"]
COPY ["src/Common/Common.Logging/Common.Logging.csproj", "./src/Common/Common.Logging/"]
COPY ["src/Common/Common.RabbitMQ/Common.RabbitMQ.csproj", "./src/Common/Common.RabbitMQ/"]
COPY ["src/Backend/Backend.Domain/Backend.Domain.csproj", "./src/Backend/Backend.Domain/"]
COPY ["src/Backend/Backend.Application/Backend.Application.csproj", "./src/Backend/Backend.Application/"]
COPY ["src/Backend/Backend.Infrastructure/Backend.Infrastructure.csproj", "./src/Backend/Backend.Infrastructure/"]

RUN dotnet restore "src/Backend/Backend.API/Backend.API.csproj"
COPY . .
WORKDIR "/tradeCloud/src/Backend/Backend.API"
RUN dotnet build "Backend.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Backend.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Backend.API.dll"]