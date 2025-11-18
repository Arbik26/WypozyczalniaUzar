FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["WypozyczalniaUzar/WypozyczalniaUzar.csproj", "WypozyczalniaUzar/"]
RUN dotnet restore "WypozyczalniaUzar/WypozyczalniaUzar.csproj"

COPY . .
WORKDIR "/src/WypozyczalniaUzar"
RUN dotnet build "WypozyczalniaUzar.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WypozyczalniaUzar.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 8080

ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "WypozyczalniaUzar.dll"]
