#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TMA Warehouse solution.csproj", "."]
RUN dotnet restore "./TMA Warehouse solution.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "TMA Warehouse solution.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TMA Warehouse solution.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TMA Warehouse solution.dll"]