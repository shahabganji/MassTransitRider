FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MassTransitRider.Api/MassTransitRider.Api.csproj", "MassTransitRider.Api/"]
RUN dotnet restore "MassTransitRider.Api/MassTransitRider.Api.csproj"
COPY . .
WORKDIR "/src/MassTransitRider.Api"
RUN dotnet build "MassTransitRider.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MassTransitRider.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MassTransitRider.Api.dll"]
