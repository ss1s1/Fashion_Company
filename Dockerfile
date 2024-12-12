# Указываем базовый образ для SDK
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

# Рабочая директория
WORKDIR /app

# Копируем только файлы проекта
COPY *.sln ./
COPY Fashion_Company/Fashion_Company.csproj ./Fashion_Company/

# Выполняем восстановление зависимостей
RUN dotnet restore

# Копируем остальные файлы
COPY . .

# Сборка приложения
RUN dotnet publish Fashion_Company/Fashion_Company.csproj -c Release -o /app/publish

# Переходим к образу для выполнения
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime

# Рабочая директория
WORKDIR /app

# Копируем собранное приложение
COPY --from=build /app/publish .

# Запуск приложения
ENTRYPOINT ["dotnet", "Fashion_Company.dll"]




