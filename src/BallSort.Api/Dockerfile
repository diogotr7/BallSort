FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["BallSort.Api/BallSort.Api.csproj", "BallSort.Api/"]
RUN dotnet restore "BallSort.Api/BallSort.Api.csproj"
COPY . .
WORKDIR "/src/BallSort.Api"
RUN dotnet build "BallSort.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BallSort.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BallSort.Api.dll"]
