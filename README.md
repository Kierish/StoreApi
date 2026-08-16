# 🛒 Store Management System (Full-Stack)

![.NET 9](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet&logoColor=white)
![React 19](https://img.shields.io/badge/React-19.0-61DAFB?logo=react&logoColor=black)
![Vite](https://img.shields.io/badge/Vite-Ready-646CFF?logo=vite&logoColor=white)
![EF Core](https://img.shields.io/badge/EF_Core-9.0-31A8FF?logo=nuget&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL_Server-2022-CC292B?logo=microsoftsqlserver&logoColor=white)
![Redis](https://img.shields.io/badge/Redis-Cache-DC382D?logo=redis&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?logo=docker&logoColor=white)
![CI/CD](https://img.shields.io/badge/CI%2FCD-Automated-2088FF?logo=githubactions&logoColor=white)

A strictly typed, full-stack RESTful Web Application built with **ASP.NET Core 9** and a **React 19 / Vite** frontend. 
This project is engineered to handle complex data relationships, featuring a **custom-built JWT authentication pipeline**, **resilient distributed caching**, and **fully containerized infrastructure**. Designed with a strong focus on architectural modularity, fault tolerance, and type safety.

## ⚙️ Core Features (Implemented)

### Security & Authentication
* **Custom JWT Pipeline:** Built from scratch (bypassing ASP.NET Core Identity) using `BCrypt.Net-Next` for robust password hashing.
* **Refresh Token Rotation:** Secure token generation and validation to maintain persistent user sessions.
* **Role-Based Access Control (RBAC):** Strict endpoint protection using `[Authorize]` policies for specific roles (Customer, Employee, Admin).

### Data & Persistence
* **Relational Modeling:** Configured complex EF Core relationships including One-to-Many (`Category` -> `Product`), Many-to-Many (`Product` <-> `Tag`), and One-to-One (`Product` -> `PageMetaData`).
* **Soft Deletion:** Implemented the `ISoftDelete` interface to safely archive records (Products, Comments) without physically removing them from the database.

### API Architecture
* **DTO Pattern & Validation:** Strict separation between internal domain models and API contracts using C# Records for immutability. Input streams are strictly validated via `FluentValidation` interceptors.
* **Result Pattern & Error Handling:** Business logic utilizes the **Result Pattern** (`Result<T>`) to avoid exception-driven control flow. Domain errors are explicitly handled and translated into standardized JSON `ProblemDetails` via a custom `ApiControllerBase` and a global `IExceptionHandler`.
* **Advanced Pagination:** Implemented efficient offset pagination for data grids. Pagination metadata (current page, total pages, page size, etc.) is seamlessly exposed to the client via a custom `X-Pagination` HTTP header.

### Performance & Observability
* **Resilient Distributed Caching:** Uses **Redis** implementing the **Cache-Aside pattern** to reduce database load for read-heavy operations (products, comments). Incorporates **Polly Circuit Breaker** (`ResilientCacheService`) to fallback seamlessly to SQL if Redis goes offline.
* **Advanced Logging:** Structural async logging with **Serilog** (targeting Console, File, and MSSQL `LogEvents` table) with contextual data injection (e.g., UserId via custom middleware).

### Frontend Architecture
* **Modern Stack:** Built with **React 19** and **Vite** for blazing fast HMR and optimized builds.
* **State Management & Data Fetching:** Utilizes **TanStack React Query (v5)** for caching, background fetching, and mutation state.
* **Seamless Auth Flow:** Custom `apiClient` with an automatic interceptor that rotates expired JWTs seamlessly using Refresh Tokens without interrupting the user experience.

### Testing Strategy
* **Unit Testing Suite:** Dedicated test project (`StoreApi.Tests`) utilizing **xUnit**, **Moq** for dependency mocking, **AutoFixture** for test data generation, and **FluentAssertions** for readable assertions. Validates core logic across Controllers, Services, Mappers, and Validators.

### Infrastructure & CI/CD
* **Dockerized Environment:** Fully containerized setup via `docker-compose`, spinning up the API, React Frontend, Redis, and an isolated SQL Server 2022 instance in a single command.
* **Automated CI/CD Pipeline:** GitHub Actions workflows for continuous integration (build, test, artifact publish) and automated CD deployment on a self-hosted runner with Health Check verification and automated rollback on deployment failures.

## 🚀 Getting Started

### Prerequisites
* [Docker Desktop](https://www.docker.com/products/docker-desktop/) or Docker Engine + Compose.

### Running locally
1. Clone the repository.
2. Create your local environment file:
   ```bash
   cp .env.example .env
   ```
3. Update the `.env` file with your secure credentials (e.g., Database Password, JWT Secret).
4. Start the infrastructure:
   ```bash
   docker-compose up -d --build
   ```
5. **Access the Application:**
   * Frontend SPA: `http://localhost:5173`
   * Backend Swagger UI: `http://localhost:8080/swagger`

## 📡 API Endpoints 

### Products
| Method | Endpoint | Description | Access |
|---|---|---|---|
| `GET` | `/api/product` | Retrieve a paginated/filtered list of products | Public |
| `GET` | `/api/product/{id}` | Retrieve details of a specific product | Public |
| `POST` | `/api/product` | Create a new product with relations (Tags, SEO) | Employee, Admin |
| `PUT` | `/api/product/{id}` | Update an existing product | Employee, Admin |
| `DELETE` | `/api/product/{id}` | Delete (Soft Delete) a product | Employee, Admin |

### Comments
| Method | Endpoint | Description | Access |
|---|---|---|---|
| `GET` | `/api/comment/product/{id}` | Get comments for a specific product | Public |
| `POST` | `/api/comment` | Add a new comment to a product | Authenticated |
| `DELETE` | `/api/comment/{id}` | Delete a comment | Author, Employee, Admin |

### Categories & Tags
| Method | Endpoint | Description | Access |
|---|---|---|---|
| `GET` | `/api/category` | Retrieve a list of all categories | Public |
| `GET` | `/api/tag` | Retrieve a list of all tags | Public |

### Authentication & Users
| Method | Endpoint | Description | Access |
|---|---|---|---|
| `POST` | `/api/auth/register-user` | Register a new user | Public |
| `POST` | `/api/auth/login-user` | Authenticate and receive JWT & Refresh Token | Public |
| `POST` | `/api/auth/refresh` | Refresh an expired JWT | Public |
| `GET` | `/api/user` | Retrieve a list of all registered users | Admin |
| `PUT` | `/api/user/{id}/role` | Update the role of a specific user | Admin |

### System
| Method | Endpoint | Description | Access |
|---|---|---|---|
| `GET` | `/health` | Application health check endpoint | Public |