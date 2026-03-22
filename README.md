#  E-Commerce API (ASP.NET Core)

##  Overview
This is a scalable e-commerce backend built using ASP.NET Core following Clean Architecture and CQRS principles. It provides secure RESTful APIs for managing products, authentication, orders, and payments.

---

##  Features

- Clean Architecture (Domain, Application, Infrastructure, API)
- CQRS pattern using MediatR
- Repository & Unit of Work patterns
- JWT Authentication with Refresh Tokens
- Role-based Authorization (Admin / User)
- Product filtering, searching, and pagination
- Stripe Payment Integration
- Global Error Handling Middleware
- AutoMapper for object mapping

---

##  Architecture

API Layer
│
├── Application Layer (CQRS, DTOs, Interfaces)
├── Domain Layer (Entities, Enums)
├── Infrastructure Layer (EF Core, Repositories)

---

##  Authentication & Authorization

- JWT-based authentication
- Refresh token implementation
- Role-based and policy-based authorization

---

##  Payment Integration

- Integrated with Stripe for secure checkout
- Handles payment intent creation and confirmation

---

##  Tech Stack

- ASP.NET Core
- Entity Framework Core
- SQL Server
- MediatR
- AutoMapper
- Stripe API

---

##  Getting Started

### 1. Clone the repo

git clone https://github.com/fathigasim/E-CommerceBack
cd ecommerce-api

---

### 2. Configure appsettings.json

Update:
- Connection string
- JWT settings
- Stripe keys

---

### 3. Apply migrations

dotnet ef database update

---

### 4. Run the project

dotnet run

---

##  API Endpoints (Examples)

- GET /api/products
- GET /api/products?pageNumber=1&pageSize=10&q=phone
- POST /api/auth/login
- POST /api/orders
- POST /api/payments

---

## 🧪 Future Improvements

- Add caching (Redis)
- Add unit & integration tests
- Logging (Serilog)

---

##  Author

Mohammed Fathi Abualgasim Mustafa
