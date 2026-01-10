FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 9090

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["HRPlatform.csproj", "./"]
RUN dotnet restore "HRPlatform.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "HRPlatform.csproj" -c Release -o /app/build --no-restore

FROM build AS publish
RUN dotnet publish "HRPlatform.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HRPlatform.dll"]

