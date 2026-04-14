# TaskFlow API

## 1. Overview

TaskFlow API is a backend service for managing tasks and projects with user authentication.

### Features:

* User registration & login (JWT-based authentication)
* Project management
* Task management (CRUD operations)
* Secure API endpoints

### Tech Stack:

* ASP.NET Core Web API (.NET 8)
* Entity Framework Core
* PostgreSQL
* Docker & Docker Compose
* JWT Authentication
* FluentValidation

---

## 2. Architecture Decisions

### Structure:

The project follows a layered architecture:

* **Controllers** → Handle HTTP requests
* **Services** → Business logic
* **Repositories** → Data access using EF Core
* **Models** → Entity definitions
* **DTOs** → Data transfer objects

### Why this approach?

* Separation of concerns
* Easier to maintain and scale
* Clean and testable code

### Tradeoffs:

* Added complexity compared to simple CRUD apps
* More files and boilerplate

### What was intentionally left out:

* Advanced caching (e.g., Redis)
* Role-based authorization
* Unit/integration tests (time constraint)

---

## 3. Running Locally

### Prerequisites:

* Docker installed

### Steps:

```bash
git clone https://github.com/AnubPrasad/taskflow-api.git
cd taskflow-api

# create environment file
cp .env.example .env

# run application
docker compose up --build
```

### Access:

* API: http://localhost:5000
* Swagger UI: http://localhost:5000/swagger

---

## 4. Running Migrations

If migrations are not applied automatically:

```bash
docker exec -it taskflow-api dotnet ef database update
```

Or locally:

```bash
dotnet ef database update
```

---

## 5. Test Credentials

Use the following credentials to log in:

```
Email:    test@example.com
Password: password123
```

---

## 6. API Reference

### Authentication

#### Register

POST /api/auth/register

Request:

```json
{
  "email": "test@example.com",
  "password": "password123"
}
```

#### Login

POST /api/auth/login

Response:

```json
{
  "token": "jwt_token_here"
}
```

---

### Tasks

#### Get Tasks

GET /api/tasks

#### Create Task

POST /api/tasks

Request:

```json
{
  "title": "Sample Task",
  "description": "Task description"
}
```

---

### Projects

#### Get Projects

GET /api/projects

#### Create Project

POST /api/projects

---

## 7. What I'd Do With More Time

* Add unit and integration tests
* Implement role-based authorization (Admin/User)
* Improve validation and error handling
* Add pagination and filtering
* Introduce caching (Redis) for performance
* Improve logging and monitoring
* Add CI/CD pipeline for automated deployment

---

## Notes

* Ensure `.env` file is configured before running
* Docker handles PostgreSQL setup and API deployment
* Do not commit `.env` file (contains sensitive data)
