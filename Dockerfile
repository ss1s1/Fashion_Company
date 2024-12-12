# Используем образ SDK .NET для сборки
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env

# Устанавливаем рабочую директорию
WORKDIR /app

# Копируем только необходимые файлы для восстановления зависимостей
COPY *.sln ./
COPY Fashion_Company/*.csproj ./Fashion_Company/

# Восстанавливаем зависимости
RUN dotnet restore "./Fashion_Company.sln"

# Копируем оставшиеся файлы проекта
COPY . .

# Сборка и публикация
RUN dotnet publish "Fashion_Company/Fashion_Company.csproj" -c Release -o /app/publish

# Используем минимальный runtime-образ
FROM mcr.microsoft.com/dotnet/aspnet:6.0

WORKDIR /app
COPY --from=build-env /app/publish .

ENTRYPOINT ["dotnet", "Fashion_Company.dll"]


