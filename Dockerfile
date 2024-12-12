WORKDIR /src

# Копируем решение
COPY Fashion_Company.sln ./

# Копируем проект
COPY Fashion_Company.csproj ./

# Восстанавливаем зависимости
RUN dotnet restore

# Копируем остальные файлы
COPY . .

# Сборка
WORKDIR /src
RUN dotnet publish -c Release -o /app/publish


