#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app

EXPOSE 5432

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["./MovieBacklog/MovieBacklog.csproj", "App/"]
RUN dotnet restore "App/MovieBacklog.csproj"
COPY . ./App
WORKDIR "/src/App"
RUN dotnet build "./MovieBacklog/MovieBacklog.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./MovieBacklog/MovieBacklog.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MovieBacklog.dll"]