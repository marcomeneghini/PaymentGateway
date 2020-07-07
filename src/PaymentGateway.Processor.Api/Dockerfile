#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["PaymentGateway.Processor.Api/PaymentGateway.Processor.Api.csproj", "PaymentGateway.Processor.Api/"]
COPY ["PaymentGateway.SharedLib/PaymentGateway.SharedLib.csproj", "PaymentGateway.SharedLib/"]
RUN dotnet restore "PaymentGateway.Processor.Api/PaymentGateway.Processor.Api.csproj"
COPY . .
WORKDIR "/src/PaymentGateway.Processor.Api"
RUN dotnet build "PaymentGateway.Processor.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PaymentGateway.Processor.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PaymentGateway.Processor.Api.dll"]
