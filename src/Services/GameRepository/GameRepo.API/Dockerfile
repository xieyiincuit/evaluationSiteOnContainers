FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

#设置字符集为UTF-8
ENV LANG en_US.UTF-8
#设置时间为中国上海
ENV TZ=Asia/Shanghai
ENV DEBIAN_FRONTEND noninteractive

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY ["Services/GameRepository/GameRepo.API/GameRepo.API.csproj", "Services/GameRepository/GameRepo.API/"]
COPY ["BuildingBlocks/EventBus/EventBusBase/EventBusBase.csproj", "BuildingBlocks/EventBus/EventBusBase/"]
COPY ["BuildingBlocks/EventBus/IntegrationEventLogEF/IntegrationEventLogEF.csproj", "BuildingBlocks/EventBus/IntegrationEventLogEF/"]
COPY ["BuildingBlocks/DistributedCache/RedisRepository/RedisRepository.csproj", "BuildingBlocks/DistributedCache/RedisRepository/"]
COPY ["BuildingBlocks/EventBus/EventBusRabbitMQ/EventBusRabbitMQ.csproj", "BuildingBlocks/EventBus/EventBusRabbitMQ/"]
COPY ["BuildingBlocks/WebHostKits/WebHost.Customization/WebHost.Customization.csproj", "BuildingBlocks/WebHostKits/WebHost.Customization/"]

RUN dotnet restore "Services/GameRepository/GameRepo.API/GameRepo.API.csproj"
COPY . .
WORKDIR "/src/Services/GameRepository/GameRepo.API"
RUN dotnet build "GameRepo.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GameRepo.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GameRepo.API.dll"]