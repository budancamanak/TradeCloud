# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5162
EXPOSE 5163

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /tradeCloud
COPY *.sln .
COPY ["src/Worker/Worker.API/Worker.API.csproj", "src/Worker/Worker.API/"]
COPY ["src/Common/Common.Core/Common.Core.csproj", "./src/Common/Common.Core/"]
COPY ["src/Common/Common.Messaging/Common.Messaging.csproj", "./src/Common/Common.Messaging/"]
COPY ["src/Common/Common.Plugin/Common.Plugin.csproj", "./src/Common/Common.Plugin/"]
COPY ["src/Common/Common.Protos/Common.Protos.csproj", "./src/Common/Common.Protos/"]
COPY ["src/Common/Common.Web/Common.Web.csproj", "./src/Common/Common.Web/"]
COPY ["src/Common/Common.Application/Common.Application.csproj", "./src/Common/Common.Application/"]
COPY ["src/Common/Common.Grpc/Common.Grpc.csproj", "./src/Common/Common.Grpc/"]
COPY ["src/Common/Common.RabbitMQ/Common.RabbitMQ.csproj", "./src/Common/Common.RabbitMQ/"]
COPY ["src/Worker/Worker.Application/Worker.Application.csproj", "./src/Worker/Worker.Application/"]
COPY ["src/Worker/Worker.Infrastructure/Worker.Infrastructure.csproj", "./src/Worker/Worker.Infrastructure/"]
COPY ["src/Worker/Worker.Plugins/Worker.Plugins.csproj", "./src/Worker/Worker.Plugins/"]

RUN dotnet restore "src/Worker/Worker.API/Worker.API.csproj"
COPY . .
WORKDIR "/tradeCloud/src/Worker/Worker.API"
RUN dotnet build "Worker.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

WORKDIR "/tradeCloud/src/Worker/Worker.Plugins"
RUN dotnet build "Worker.Plugins.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR "/tradeCloud/src/Worker/Worker.API"
RUN dotnet publish "Worker.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
WORKDIR "/tradeCloud/src/Worker/Worker.Plugins"
RUN dotnet publish "Worker.Plugins.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Worker.API.dll"]
