FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MassTransitRider.ServiceBusConsumer/MassTransitRider.ServiceBusConsumer.csproj", "MassTransitRider.ServiceBusConsumer/"]
RUN dotnet restore "MassTransitRider.ServiceBusConsumer/MassTransitRider.ServiceBusConsumer.csproj"
COPY . .
WORKDIR "/src/MassTransitRider.ServiceBusConsumer"
RUN dotnet build "MassTransitRider.ServiceBusConsumer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MassTransitRider.ServiceBusConsumer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MassTransitRider.ServiceBusConsumer.dll"]
