# 🔗 Linkly - URL Shortener Platform

**Linkly** is a high-performance, secure, and modern URL shortening service built with .NET 10, React, and PostgreSQL. Designed as a Technical Task for INFORCE, it features Clean Architecture, Role-Based Access Control, and Server-Side Rendered (SSR) components.

---

## 🚀 Quick Start (Docker)

The fastest way to run the entire stack (Database, Backend API, Frontend UI) is using **Docker Compose**:

```bash
# Clone the repository and navigate to the root
docker-compose up --build
```

- **Frontend**: `http://localhost:5173`
- **Backend API**: `http://localhost:5121/api`
- **Swagger/Scalar Docs**: `http://localhost:5121/scalar/v1`
- **Algorithm Info (About Page)**: `http://localhost:5121/About`

---

## 🛠 Tech Stack

### Backend (.NET 10)
- **Framework**: ASP.NET Core 10 Web API
- **Database**: PostgreSQL with EF Core
- **Auth**: ASP.NET Core Identity + JWT (HttpOnly Cookies)
- **Architecture**: Clean Architecture (Domain, Application, Infrastructure, Web)
- **Testing**: xUnit + Moq + FluentAssertions
- **Pattern**: Result Pattern (Functional style error handling)

### Frontend (React)
- **Framework**: Vite + React
- **Styling**: Tailwind CSS (Premium Dark Theme)
- **Context**: Global state management via AuthContext and UrlContext

---

## ✨ Features

- **Unique Short Codes**: 6-character Base62 algorithm (up to 56 billion combinations).
- **Role-Based Access**:
    - **Admin**: Can create/delete any link and edit the "About" page content.
    - **User**: Can create/delete their own links.
    - **Anonymous**: Can view the public link table and use redirects.
- **Server-Side Rendered "About" Page**: Built with Razor Pages for algorithm description, editable only by admins.
- **Security**:
    - **HttpOnly JWT**: Tokens are stored in secure cookies to prevent XSS.
    - **Privacy**: User lists are filtered to show only personal links.
- **Performance**: In-memory caching (`IMemoryCache`) for lightning-fast redirection.

---

## 🔐 Default Admin Credentials

For testing purposes, the system automatically seeds an admin user:
- **Email**: `admin@inforce.com`
- **Password**: `Admin123!`

---

## 📂 Project Structure

```text
├── backend
│   ├── UrlShortener.Domain         # Entities & Domain Logic
│   ├── UrlShortener.Application    # Interfaces & Services
│   ├── UrlShortener.Infrastructure # DB context & Repositories
│   ├── UrlShortener.Web            # Controllers, Razor Pages & Middlewares
│   └── UrlShortener.Tests          # xUnit Testing Suite
├── frontend
│   ├── src/context                # Global State (Auth & URL)
│   ├── src/pages                  # React Pages (Home, Auth, Info)
│   └── src/components             # Reusable UI Components
└── docker-compose.yml             # Full-stack orchestrator
```

---

## 🧪 Running Tests

To run the backend test suite (13+ tests covering validation and permissions):

```bash
cd backend/UrlShortener.Tests
dotnet test
```

---

## 📞 Contact

Developed by **[emil720a]** for the INFORCE URL Shortener Development task.
