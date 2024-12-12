# Указываем базовый образ .NET SDK для сборки
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

# Указываем рабочую директорию
WORKDIR /src

# Копируем файл решения и проект
COPY *.sln ./
COPY ./Fashion_Company.csproj ./Fashion_Company/

# Восстанавливаем зависимости
RUN dotnet restore

# Копируем оставшиеся файлы
COPY . .

# Сборка проекта в Release-режиме
RUN dotnet publish Fashion_Company/Fashion_Company.csproj -c Release -o /app/publish

# Указываем базовый образ .NET Runtime для выполнения
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime

# Устанавливаем рабочую директорию
WORKDIR /app

# Копируем собранное приложение из предыдущего этапа
COPY --from=build /app/publish .

# Указываем команду запуска приложения
ENTRYPOINT ["dotnet", "Fashion_Company.dll"]



