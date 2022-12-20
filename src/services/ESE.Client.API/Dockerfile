#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["src/services/ESE.Client.API/ESE.Client.API.csproj", "src/services/ESE.Client.API/"]
RUN dotnet restore "src/services/ESE.Client.API/ESE.Client.API.csproj"
COPY . .
WORKDIR "/src/src/services/ESE.Client.API"
RUN dotnet build "ESE.Client.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ESE.Client.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ESE.Client.API.dll"]