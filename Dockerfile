FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["QuickOrder.Api/QuickOrder.Api.csproj", "QuickOrder.Api/"]
COPY ["QuickOrder.Application/QuickOrder.Application.csproj", "QuickOrder.Application/"]
COPY ["QuickOrder.Domain/QuickOrder.Domain.csproj", "QuickOrder.Domain/"]
COPY ["QuickOrder.Infrastructure/QuickOrder.Infrastructure.csproj", "QuickOrder.Infrastructure/"]
RUN dotnet restore "QuickOrder.Api/QuickOrder.Api.csproj"
COPY . .
WORKDIR "/src/QuickOrder.Api"
RUN dotnet publish "QuickOrder.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "QuickOrder.Api.dll"]
