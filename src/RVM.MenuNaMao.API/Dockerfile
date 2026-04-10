FROM node:20-alpine AS frontend
WORKDIR /app
COPY src/menunomao-menu/package*.json ./
RUN npm ci
COPY src/menunomao-menu/ ./
RUN npm run build

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/RVM.MenuNaMao.API/RVM.MenuNaMao.API.csproj", "src/RVM.MenuNaMao.API/"]
COPY ["src/RVM.MenuNaMao.Application/RVM.MenuNaMao.Application.csproj", "src/RVM.MenuNaMao.Application/"]
COPY ["src/RVM.MenuNaMao.Domain/RVM.MenuNaMao.Domain.csproj", "src/RVM.MenuNaMao.Domain/"]
COPY ["src/RVM.MenuNaMao.Infrastructure/RVM.MenuNaMao.Infrastructure.csproj", "src/RVM.MenuNaMao.Infrastructure/"]
RUN dotnet restore "./src/RVM.MenuNaMao.API/RVM.MenuNaMao.API.csproj"
COPY . .
WORKDIR "/src/src/RVM.MenuNaMao.API"
RUN dotnet build "./RVM.MenuNaMao.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./RVM.MenuNaMao.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=frontend /app/dist ./clientapp
ENTRYPOINT ["dotnet", "RVM.MenuNaMao.API.dll"]
