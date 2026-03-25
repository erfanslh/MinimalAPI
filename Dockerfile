FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MinimalAPIMoviez/MinimalAPIMoviez.csproj", "MinimalAPIMoviez/"]
RUN dotnet restore "MinimalAPIMoviez/MinimalAPIMoviez.csproj"
COPY . .
WORKDIR "/src/MinimalAPIMoviez"
RUN dotnet publish "MinimalAPIMoviez.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MinimalAPIMoviez.dll"]
