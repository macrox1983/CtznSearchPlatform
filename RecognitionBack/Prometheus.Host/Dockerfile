FROM mcr.microsoft.com/dotnet/core/runtime:2.2-stretch-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["Prometheus.Host/Prometheus.Host.csproj", "Prometheus.Host/"]
COPY ["Prometheus.Services/Prometheus.Services.csproj", "Prometheus.Services/"]
COPY ["Prometheus.DAL/Prometheus.DAL.csproj", "Prometheus.DAL/"]
COPY ["Prometheus.Model/Prometheus.Model.csproj", "Prometheus.Model/"]
COPY ["Prometheus.Infrastructure.Model/Prometheus.Infrastructure.Model.csproj", "Prometheus.Infrastructure.Model/"]
COPY ["Prometheus.Infrastructure/Prometheus.Infrastructure.csproj", "Prometheus.Infrastructure/"]
COPY ["Prometheus.Infrastructure.Component/Prometheus.Infrastructure.Component.csproj", "Prometheus.Infrastructure.Component/"]
COPY ["Prometheus.Presentation/Prometheus.Presentation.csproj", "Prometheus.Presentation/"]
COPY ["Prometheus.DbContext/Prometheus.DbContext.csproj", "Prometheus.DbContext/"]
COPY ["Prometheus.ComponentManager/Prometheus.ComponentManager.csproj", "Prometheus.ComponentManager/"]
RUN dotnet restore "Prometheus.Host/Prometheus.Host.csproj"
COPY . .
WORKDIR "/src/Prometheus.Host"
RUN dotnet build "Prometheus.Host.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Prometheus.Host.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Prometheus.Host.dll"]