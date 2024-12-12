# Используем .NET SDK для сборки
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Копируем файлы проекта и восстанавливаем зависимости
COPY *.sln ./
COPY Fashion_Company_s/*.csproj ./Fashion_Company/
RUN dotnet restore

# Копируем весь проект и собираем
COPY . .
WORKDIR /src/Fashion_Company
RUN dotnet publish -c Release -o /app/publish

# Используем .NET Runtime для запуска
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Fashion_Company.dll"]


