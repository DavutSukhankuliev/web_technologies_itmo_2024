FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8090

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["web_technologies_itmo_2024/web_technologies_itmo_2024.csproj", "web_technologies_itmo_2024/"]
RUN dotnet restore "web_technologies_itmo_2024/web_technologies_itmo_2024.csproj"
COPY . .
WORKDIR "/src/web_technologies_itmo_2024"
RUN dotnet build "web_technologies_itmo_2024.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "web_technologies_itmo_2024.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "web_technologies_itmo_2024.dll"]
