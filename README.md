# Countries & Cities Management API (.NET 10)

A robust, enterprise-grade RESTful Web API built with **ASP.NET Core (.NET 10)**, **Entity Framework Core**, **FluentValidation**, and **Scalar OpenAPI Reference**. It provides full CRUD operations, pagination, filtering, sorting, cascading relationships, and an interactive management dashboard.

---

## 🚀 Features

- **Countries Management**:
  - Create Country (with ISO code uniqueness and format validation)
  - Get Country by ID (includes detailed country data and cities overview)
  - Get Countries with pagination, search, and region filtering
  - Update Country (with validation and conflict detection)
  - Delete Country (with automatic cascade deletion of associated cities)
  - Get Cities by Country ID with pagination
- **Cities Management**:
  - Create City (validates country existence and prevents duplicate names per country)
  - Get City by ID (with enriched country metadata)
  - Get Cities with pagination, search, capital filter, and country filter
  - Update City (supports re-assigning country and capital status management)
  - Delete City
- **Interactive UI Dashboard**: Modern Web Dashboard embedded in `wwwroot/index.html` with real-time stats, modals, and tables.
- **Interactive API Documentation**: Modern OpenAPI documentation powered by **Scalar** at `/scalar/v1`.
- **Validation & Error Handling**: Comprehensive FluentValidation rules and RFC 7807 `ProblemDetails` error handling middleware.
- **Automated Tests**: Comprehensive suite of 24 integration tests verifying all endpoints and business rules.

---

## 🛠️ Tech Stack

- **Framework**: .NET 10 (ASP.NET Core Web API)
- **Database**: Entity Framework Core (In-Memory provider for immediate zero-config execution)
- **Validation**: FluentValidation
- **API Documentation**: Microsoft.AspNetCore.OpenApi & Scalar.AspNetCore
- **Testing**: xUnit, Microsoft.AspNetCore.Mvc.Testing

---

## 🏃 Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Git

### Running Locally

1. Clone the repository:
   ```bash
   git clone https://github.com/Mohammed99Ismail/Countries.git
   cd Countries
   ```

2. Run the application:
   ```bash
   dotnet run --launch-profile http
   ```

3. Open in your browser:
   - **Web Dashboard**: [http://localhost:5112](http://localhost:5112)
   - **Scalar API Reference**: [http://localhost:5112/scalar/v1](http://localhost:5112/scalar/v1)
   - **OpenAPI JSON**: [http://localhost:5112/openapi/v1.json](http://localhost:5112/openapi/v1.json)

---

## 🧪 Running Tests

Execute the automated test suite:

```bash
dotnet test
```

Output:
```
Passed! - Failed: 0, Passed: 24, Skipped: 0, Total: 24
```

---

## 📡 API Endpoints Overview

### Countries (`/api/countries`)
| Method | Endpoint | Description |
|:---|:---|:---|
| `GET` | `/api/countries` | List countries with pagination, search & region filtering |
| `GET` | `/api/countries/{id}` | Get country by ID with cities overview |
| `POST` | `/api/countries` | Create a new country |
| `PUT` | `/api/countries/{id}` | Update an existing country |
| `DELETE` | `/api/countries/{id}` | Delete a country (cascades to cities) |
| `GET` | `/api/countries/{id}/cities` | Get cities for a specific country with pagination |

### Cities (`/api/cities`)
| Method | Endpoint | Description |
|:---|:---|:---|
| `GET` | `/api/cities` | List cities with pagination, search, country & capital filters |
| `GET` | `/api/cities/{id}` | Get city by ID |
| `POST` | `/api/cities` | Create a new city |
| `PUT` | `/api/cities/{id}` | Update an existing city |
| `DELETE` | `/api/cities/{id}` | Delete a city |
