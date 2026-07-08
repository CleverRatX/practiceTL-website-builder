# practiceTL-website-builder

## 📋 Описание проекта

Проект представляет собой систему управления контентом, состоящую из двух частей:
1. Публичная часть — главная страница портала travelline.tech.
2. Админ-панель — отдельная страница, через которую можно управлять
содержимым блоков на публичной странице: менять тексты,
добавлять и удалять элементы, менять порядок.

Ключевая идея: изменения, сделанные в админке, сразу отражаются на публичной
странице без перезапуска сервера.

---

## 🛠 Стек технологий

| Компонент | Технология |
|-----------|-----------|
| Backend | .NET 8.0 |
| База данных | PostgreSQL 16+ |
| Контейнеризация | Docker, Docker Compose |
| Тестирование | xUnit + Moq|
| CI/CD | GitHub Actions |
| Конфигурация | Environment variables, `.env` |

## 📁 Структура проекта

```
practiceTL-website-builder/
├── .github/workflows/       # CI для GitHub Actions
├── src/
│   └── PracticeTL.Api/      # Основной код
│       ├── Controllers/     # API-контроллеры
│       ├── Models/          # Сущности
│       ├── Dtos/            # DTO
│       ├── Services/        # Бизнес-логика
│       ├── Data/            # Работа с БД
│       ├── Templates/       # HTML-шаблоны
│       ├── wwwroot/         # Стили, скрипты и медиа файлы
│       └── Program.cs       # Точка входа
├── tests/
│   └── PracticeTL.Tests/    # Юнит-тесты для блоков и сервисов
├── docker-compose.yml
├── Dockerfile
├── .env.template            # Шаблон переменных окружения
├── .dockerignore
├── practiceTL-website-builder.sln
└── README.md
```
## 🚀 Запуск проекта

### 1.Скопируйте и запоните шаблон:

``` bash
cp .env.template .env
```

Пример:
``` .env
POSTGRES_DB=practicetl
POSTGRES_USER=admin
POSTGRES_PASSWORD=your_secure_password
ADMIN_USERNAME=admin
ADMIN_PASSWORD=your_admin_password
```

### 2. Запуск через Docker Compose

Сборка и запуск: 

``` bash
docker-compose up --build -d
```

Остановка:

``` bash
docker-compose down
```

Публичная страница: http://localhost:8080
Админ-панель: http://localhost:8080/admin.html

## 🧪 Тестирование

``` bash
dotnet test
```
