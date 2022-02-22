FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY ["Services/Evaluation/Evaluation.API/Evaluation.API.csproj", "Services/Evaluation/Evaluation.API/"]
COPY ["BuildingBlocks/EventBus/EventBusBase/EventBusBase.csproj", "BuildingBlocks/EventBus/EventBusBase/"]
COPY ["BuildingBlocks/EventBus/EventBusRabbitMQ/EventBusRabbitMQ.csproj", "BuildingBlocks/EventBus/EventBusRabbitMQ/"]
COPY ["BuildingBlocks/WebHostKits/WebHost.Customization/WebHost.Customization.csproj", "BuildingBlocks/WebHostKits/WebHost.Customization/"]

RUN dotnet restore "Services/Evaluation/Evaluation.API/Evaluation.API.csproj"
COPY . .
WORKDIR "/src/Services/Evaluation/Evaluation.API"
RUN dotnet build "Evaluation.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Evaluation.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Evaluation.API.dll"]