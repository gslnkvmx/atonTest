# Инструкция по запуску проекта

## Вариант 1: Запуск всего в Docker

1. Клонируйте репозиторий:

    ```bash
    git clone https://github.com/gslnkvmx/atonTest.git
    cd atonTest
    ```

2. Соберите и запустите контейнеры с помощью Docker Compose:

    ```bash
    docker-compose up --build
    ```

Swagger UI будет доступен по адресу: `http://localhost:8080/index.html`

## Вариант 2: Запуск только базы данных в Docker

1. Клонируйте репозиторий:

    ```bash
    git clone https://github.com/gslnkvmx/atonTest.git
    cd atonTest
    ```

2. Запустите PostgreSQL в Docker:

    ```bash
    docker run --name aton-postgres -e POSTGRES_PASSWORD=your_password -e POSTGRES_DB=aton_db -p 5432:5432 -d postgres:latest
    ```

3. Настройте подключение к базе данных в файле `appsettings.json`:

    ```json
    {
      "ConnectionStrings": {
        "DefaultConnection": "Host=localhost;Port=5432;Database=aton_db;Username=postgres;Password=your_password"
      }
    }
    ```

4. Восстановите зависимости и запустите приложение:

    ```bash
    dotnet restore
    dotnet run
    ```

### Дополнительная информация

- Миграция для создания базы данных будет применена автоматически
- По желанию можно настроить JWT в файле `appsettings.json`
- Для работы с API необходимо сначала получить JWT токен через эндпоинт `/api/auth/login`
- Все запросы к API (кроме логина) требуют авторизации через JWT токен
- Некоторые эндпоинты доступны только для пользователей с ролью "admin"
- Логин администратора: "Admin", пароль: "Admin123"

### Примечания

- Для работы с базой данных в Docker убедитесь, что порт 5432 не занят другими приложениями
