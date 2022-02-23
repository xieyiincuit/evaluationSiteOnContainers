FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

#设置字符集为UTF-8
ENV LANG C.UTF-8
#设置时间为中国上海
ENV TZ=Asia/Shanghai
ENV DEBIAN_FRONTEND noninteractive

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Web/WebStatus/WebStatus.csproj", "Web/WebStatus/"]
RUN dotnet restore "Web/WebStatus/WebStatus.csproj"
COPY . .
WORKDIR "/src/Web/WebStatus"
RUN dotnet build "WebStatus.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebStatus.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebStatus.dll"]