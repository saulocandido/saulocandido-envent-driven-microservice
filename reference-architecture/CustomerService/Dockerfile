FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER root
RUN apt-get update && apt-get install -y iputils-ping net-tools dnsutils
USER app
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["reference-architecture/CustomerService/CustomerService.csproj", "reference-architecture/CustomerService/"]
COPY ["reference-architecture/Common/Common.csproj", "reference-architecture/Common/"]
COPY ["reference-architecture/ReferenceArchitecture.ServiceDefaults/ReferenceArchitecture.ServiceDefaults.csproj", "reference-architecture/ReferenceArchitecture.ServiceDefaults/"]
RUN dotnet restore "./reference-architecture/CustomerService/CustomerService.csproj"
COPY . .
WORKDIR "/src/reference-architecture/CustomerService"
RUN dotnet build "./CustomerService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CustomerService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CustomerService.dll"]
