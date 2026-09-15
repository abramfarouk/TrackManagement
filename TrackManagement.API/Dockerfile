FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY ["TrackManagement.API/TrackManagement.API.csproj", "TrackManagement.API/"]
COPY ["TrackManagement.Application/TrackManagement.Application.csproj", "TrackManagement.Application/"]
COPY ["TrackManagement.Domain/TrackManagement.Domain.csproj", "TrackManagement.Domain/"]
COPY ["TrackManagement.Infrastructure/TrackManagement.Infrastructure.csproj", "TrackManagement.Infrastructure/"]
COPY ["TrackManagement.Persistence/TrackManagement.Persistence.csproj", "TrackManagement.Persistence/"]

RUN dotnet restore "TrackManagement.API/TrackManagement.API.csproj"

COPY . .

WORKDIR "/src/TrackManagement.API"

RUN dotnet build \
    "TrackManagement.API.csproj" \
    -c Release \
    -o /app/build

RUN dotnet publish \
    "TrackManagement.API.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

WORKDIR /app

COPY --from=build /app/publish .
ENV ASPNETCORE_HTTP_PORTS=8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "TrackManagement.API.dll"]