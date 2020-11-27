FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["AJobBoard/AJobBoard.csproj", "AJobBoard/"]
RUN dotnet restore "AJobBoard/AJobBoard.csproj"
COPY . .
WORKDIR "/src/AJobBoard"
RUN dotnet build "AJobBoard.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "AJobBoard.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Jobtransparency.dll"]
