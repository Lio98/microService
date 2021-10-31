#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["MicroService.UserService/MicroService.UserService.csproj", "MicroService.UserService/"]
COPY ["Servicecomb.Saga.Omega.AspNetCore/Servicecomb.Saga.Omega.AspNetCore.csproj", "Servicecomb.Saga.Omega.AspNetCore/"]
COPY ["Servicecomb.Saga.Omega.Abstractions/Servicecomb.Saga.Omega.Abstractions.csproj", "Servicecomb.Saga.Omega.Abstractions/"]
COPY ["Servicecomb.Saga.Omega.Protocol/Servicecomb.Saga.Omega.Protocol.csproj", "Servicecomb.Saga.Omega.Protocol/"]
COPY ["Servicecomb.Saga.Omega.Core/Servicecomb.Saga.Omega.Core.csproj", "Servicecomb.Saga.Omega.Core/"]
COPY ["MicroService.Core/MicroService.Core.csproj", "MicroService.Core/"]
RUN dotnet restore "MicroService.UserService/MicroService.UserService.csproj"
COPY . .
WORKDIR "/src/MicroService.UserService"
RUN dotnet build "MicroService.UserService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MicroService.UserService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MicroService.UserService.dll"]