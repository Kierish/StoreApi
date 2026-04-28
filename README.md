# 🛒 Store Management API

![.NET 9](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet&logoColor=white)
![EF Core](https://img.shields.io/badge/EF_Core-9.0-31A8FF?logo=nuget&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL_Server-2022-CC292B?logo=microsoftsqlserver&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?logo=docker&logoColor=white)
![xUnit](https://img.shields.io/badge/Tested_with-xUnit-512BD4?logo=dotnet&logoColor=white)

A strictly typed RESTful Web API built with **ASP.NET Core 9** and **Entity Framework Core**. 
This project is **engineered to handle complex data relationships**, featuring a **custom-built JWT authentication pipeline** and **fully containerized infrastructure**, designed with a focus on **architectural modularity** and **type safety**.

## ⚙️ Core Features (Implemented)

### Security & Authentication
* **Custom JWT Pipeline:** Built from scratch (bypassing ASP.NET Core Identity) using `BCrypt.Net-Next` for robust password hashing.
* **Refresh Token Rotation:** Secure token generation and validation to maintain persistent user sessions.
* **Role-Based Access Control (RBAC):** Strict endpoint protection using `[Authorize]` policies for specific roles (Customer, Employee, Admin).

### Data & Persistence
* **Relational Modeling:** Configured complex EF Core relationships including One-to-Many (`Category` -> `Product`), Many-to-Many (`Product` <-> `Tag`), and One-to-One (`Product` -> `ProductSeo`).
* **Automated Migrations:** Database schema creation and initial data seeding execute automatically on startup.

### API Architecture
* **DTO Pattern & Validation:** Strict separation between internal domain models and API contracts using C# Records for immutability. Input streams are strictly validated via `FluentValidation` interceptors.
* **Global Exception Handling:** Custom `ExceptionMiddleware` intercepts domain exceptions (`NotFoundException`, `BadRequestException`, `UnauthorizedException`) and standardizes JSON error responses.

### Testing Strategy
* **Unit Testing Suite:** Dedicated test project (`StoreApi.Tests`) utilizing **xUnit**, **Moq** for dependency mocking, **AutoFixture** for test data generation, and **FluentAssertions** for readable assertions. Validates core logic across Controllers, Services, Mappers, and Validators.

### Infrastructure
* **Dockerized Environment:** Fully containerized setup via `docker-compose`, spinning up both the API and an isolated SQL Server 2022 instance in a single command.

## 🚀 Getting Started

### Prerequisites
* [Docker Desktop](https://www.docker.com/products/docker-desktop/) or Docker Engine + Compose.

### Running locally
1. Clone the repository.
2. Create your local environment file:
   ```bash
   cp .env.example .env
   ```
3. Update the `.env` file with your secure credentials (optional for dev).
4. Start the infrastructure:
   ```bash
   docker-compose up -d --build
   ```

## 📡 API Endpoints 

| Method | Endpoint | Description | Access |
|---|---|---|---|
| `GET` | `/api/product` | Retrieve a list of products | Public |
| `GET` | `/api/product/{id}` | Retrieve details of a specific product | Customer, Employee |
| `POST` | `/api/product` | Create a new product with relations (Tags, SEO) | Customer, Employee |
| `PUT` | `/api/product/{id}` | Update an existing product | Customer, Employee |
| `DELETE` | `/api/product/{id}` | Delete a product | Customer, Employee |
| `POST` | `/api/auth/register-user` | Register a new user | Public |
| `POST` | `/api/auth/login-user` | Authenticate and receive JWT & Refresh Token | Public |
| `POST` | `/api/auth/refresh` | Refresh an expired JWT | Public |
