# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS base
USER $APP_UID
WORKDIR /app
# Создаем папки для избражений
RUN mkdir -p /app/data/uploads
# Устанавливаем зависимости для QuestPDF
USER root
RUN apk add --no-cache \
    gcompat \
    fontconfig \
    libgcc \
    libstdc++ \
    zlib \
    icu-libs \
    libssl3 \
    krb5-libs \
    ttf-dejavu \
    ttf-liberation \
    tzdata \
    && fc-cache -fv
EXPOSE 8080

# Этот этап используется для сборки проекта службы
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Directory.Build.props", "src/"]
COPY ["src/GoldenBread.Api/GoldenBread.Api.csproj", "src/GoldenBread.Api/"]
COPY ["src/GoldenBread.Application/GoldenBread.Application.csproj", "src/GoldenBread.Application/"]
COPY ["src/GoldenBread.Domain/GoldenBread.Domain.csproj", "src/GoldenBread.Domain/"]
COPY ["src/GoldenBread.Infrastructure/GoldenBread.Infrastructure.csproj", "src/GoldenBread.Infrastructure/"]
RUN dotnet restore "./src/GoldenBread.Api/GoldenBread.Api.csproj"
COPY . .
WORKDIR "/src/src/GoldenBread.Api"
RUN dotnet build "./GoldenBread.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GoldenBread.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Слушаем только http. Прокси для https - отдельная служба
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "GoldenBread.Api.dll"]