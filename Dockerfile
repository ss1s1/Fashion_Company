# Указываем базовый образ
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /app

# Копируем файлы проекта
COPY . ./

# Восстанавливаем зависимости
RUN dotnet restore "./Fashion_Company.sln"

# Сборка приложения
RUN dotnet publish "./Fashion_Company.sln" -c Release -o /out

# Стартовый образ для выполнения
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build-env /out .
ENTRYPOINT ["dotnet", "Fashion_Company.dll"]




