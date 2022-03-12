FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

ENV LANG C.UTF-8
ENV TZ=Asia/Shanghai
ENV DEBIAN_FRONTEND noninteractive

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY "evaluationSiteOnContainers.sln" "evaluationSiteOnContainers.sln"
COPY "ApiGateways/Ocelot/OcelotGateway/OcelotGateway.csproj" "ApiGateways/Ocelot/OcelotGateway/OcelotGateway.csproj"
COPY "BuildingBlocks/DistributedCache/RedisRepository/RedisRepository.csproj" "BuildingBlocks/DistributedCache/RedisRepository/RedisRepository.csproj"
COPY "BuildingBlocks/EventBus/EventBusBase/EventBusBase.csproj" "BuildingBlocks/EventBus/EventBusBase/EventBusBase.csproj"
COPY "BuildingBlocks/EventBus/EventBusRabbitMQ/EventBusRabbitMQ.csproj" "BuildingBlocks/EventBus/EventBusRabbitMQ/EventBusRabbitMQ.csproj"
COPY "BuildingBlocks/EventBus/IntegrationEventLogEF/IntegrationEventLogEF.csproj" "BuildingBlocks/EventBus/IntegrationEventLogEF/IntegrationEventLogEF.csproj"
COPY "BuildingBlocks/WebHostKits/WebHost.Customization/WebHost.Customization.csproj" "BuildingBlocks/WebHostKits/WebHost.Customization/WebHost.Customization.csproj"
COPY "Services/BackManager/BackManage.API/BackManage.API.csproj" "Services/BackManager/BackManage.API/BackManage.API.csproj"
COPY "Services/BackManager/BackManage.FunctionalTests/BackManage.FunctionalTests.csproj" "Services/BackManager/BackManage.FunctionalTests/BackManage.FunctionalTests.csproj"
COPY "Services/BackManager/BackManage.UnitTests/BackManage.UnitTests.csproj" "Services/BackManager/BackManage.UnitTests/BackManage.UnitTests.csproj"
COPY "Services/Evaluation/Evaluation.API/Evaluation.API.csproj" "Services/Evaluation/Evaluation.API/Evaluation.API.csproj"
COPY "Services/GameRepository/GameRepo.API/GameRepo.API.csproj" "Services/GameRepository/GameRepo.API/GameRepo.API.csproj"
COPY "Services/IdentityServer/Identity.API/Identity.API.csproj" "Services/IdentityServer/Identity.API/Identity.API.csproj"
COPY "Services/Ordering/Ordering.API/Ordering.API.csproj" "Services/Ordering/Ordering.API/Ordering.API.csproj"
COPY "Web/WebStatus/WebStatus.csproj" "Web/WebStatus/WebStatus.csproj"

COPY "docker-compose.dcproj" "docker-compose.dcproj"
RUN dotnet restore "evaluationSiteOnContainers.sln"

COPY . .
WORKDIR "/src/Web/WebStatus"
RUN dotnet build "WebStatus.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebStatus.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebStatus.dll"]