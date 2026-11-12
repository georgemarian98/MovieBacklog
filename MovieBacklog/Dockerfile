#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
RUN apt-get update \
	&& apt-get install -y --no-install-recommends libfontconfig1 libfreetype6 \
	&& rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
RUN dotnet tool install --global dotnet-ef --version 10.0.0
ENV PATH="$PATH:/root/.dotnet/tools"

WORKDIR /src
COPY ["./MovieBacklog/MovieBacklog.csproj", "App/"]
RUN dotnet restore "App/MovieBacklog.csproj"
COPY . ./App
WORKDIR "/src/App"
RUN dotnet build "./MovieBacklog/MovieBacklog.csproj" -c Release -o /app/build
RUN dotnet ef migrations add InitialCreate -p "./MovieBacklog/MovieBacklog.csproj" -s "./MovieBacklog/MovieBacklog.csproj" -o "./Migrations" --verbose

FROM build AS publish
RUN dotnet publish "./MovieBacklog/MovieBacklog.csproj" \
    -c Release -r linux-x64 --self-contained false \
    -p:UseAppHost=false -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MovieBacklog.dll"]