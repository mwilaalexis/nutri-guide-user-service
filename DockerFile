FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["UserProfileServiceProject/UserProfileServiceProject.csproj", "UserProfileServiceProject/"]
COPY ["UserProject.core/UserProject.core.csproj", "UserProject.core/"]
COPY ["UserProject.DataAccess/UserProject.DataAccess.csproj", "UserProject.DataAccess/"]
RUN dotnet restore "./UserProfileServiceProject/UserProfileServiceProject.csproj"
COPY . .
WORKDIR "/src/UserProfileServiceProject"
RUN dotnet build "./UserProfileServiceProject.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./UserProfileServiceProject.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserProfileServiceProject.dll"]
