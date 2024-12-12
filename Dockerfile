# Устанавливаем SDK .NET
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env

# Устанавливаем рабочую директорию
WORKDIR /app

# Копируем файлы
COPY . .

# Восстанавливаем зависимости
RUN dotnet restore "./Fashion_Company.sln"

# Сборка
RUN dotnet publish "Fashion_Company/Fashion_Company.csproj" -c Release -o /app/publish

# Используем минимальный runtime-образ
FROM mcr.microsoft.com/dotnet/aspnet:6.0

WORKDIR /app
COPY --from=build-env /app/publish .

ENTRYPOINT ["dotnet", "Fashion_Company.dll"]

