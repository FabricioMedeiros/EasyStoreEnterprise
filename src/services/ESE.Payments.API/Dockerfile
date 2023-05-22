#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["ESE.Payments.API/ESE.Payments.API.csproj", "ESE.Payments.API/"]
RUN dotnet restore "ESE.Payments.API/ESE.Payments.API.csproj"
COPY . .
WORKDIR "/src/ESE.Payments.API"
RUN dotnet build "ESE.Payments.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ESE.Payments.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ESE.Payments.API.dll"]