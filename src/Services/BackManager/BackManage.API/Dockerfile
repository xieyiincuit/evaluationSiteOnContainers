#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Services/BackManager/BackManage.API/BackManage.API.csproj", "Services/BackManager/BackManage.API/"]
COPY ["BuildingBlocks/WebHostKits/WebHost.Customization/WebHost.Customization.csproj", "BuildingBlocks/WebHostKits/WebHost.Customization/"]
RUN dotnet restore "Services/BackManager/BackManage.API/BackManage.API.csproj"
COPY . .
WORKDIR "/src/Services/BackManager/BackManage.API"
RUN dotnet build "BackManage.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BackManage.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BackManage.API.dll"]
