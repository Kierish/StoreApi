
# 🛒 Store Management API

![.NET 9](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet&logoColor=white)
![EF Core](https://img.shields.io/badge/EF_Core-9.0-31A8FF?logo=nuget&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL_Server-2022-CC292B?logo=microsoftsqlserver&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?logo=docker&logoColor=white)

A strictly typed RESTful Web API built with **ASP.NET Core 9** and **Entity Framework Core**. 
This project is **engineered to handle complex data relationships**, featuring a **custom-built JWT authentication pipeline** and **fully containerized infrastructure**, designed with a focus on **architectural modularity** and **type safety**.

## 🚧 Active Development: Authentication Layer
> **Note:** The authentication system is currently a Work In Progress. 
> I am building a custom JWT-based authentication flow from scratch (bypassing ASP.NET Core Identity) using `BCrypt.Net-Next` for password hashing to fully understand token generation, claims, and payload security.

## ⚙️ Core Features (Implemented)

### Data & Persistence
* **Relational Modeling:** Configured complex EF Core relationships including One-to-Many (`Category` -> `Product`), Many-to-Many (`Product` <-> `Tag`), and One-to-One (`Product` -> `ProductSeo`).
* **Automated Migrations:** Database schema creation and initial data seeding execute automatically on startup.

### API Architecture
* **DTO Pattern:** Strict separation between internal domain models and API contracts using C# Records for immutability.
* **Global Exception Handling:** Custom `ExceptionMiddleware` intercepts domain exceptions (`NotFoundException`, `BadRequestException`) and standardizes JSON error responses.

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
*(This table is actively updated as new features are integrated)*

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/product` | Retrieve a list of products |
| `GET` | `/api/product/{id}` | Retrieve details of a specific product |
| `POST` | `/api/product` | Create a new product with relations (Tags, SEO) |
| `PUT` | `/api/product/{id}` | Update an existing product |
| `DELETE` | `/api/product/{id}` | Delete a product |
| `POST` | `/api/auth/reg-user` | *(WIP)* Register a new user |
| `POST` | `/api/auth/login-user` | *(WIP)* Authenticate and receive JWT |

