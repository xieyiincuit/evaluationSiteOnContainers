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
