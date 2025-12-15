# Fundo - Loan Management System

Full-stack loan management system built with **.NET Core 6.0** backend and **Angular 19** frontend, implementing clean architecture and industry best practices.

## 1. Backend - REST API (.NET Core 6.0)

### Directory Structure

```
backend/src/
├── Fundo.Applications.WebApi/          # PRESENTATION LAYER
│   ├── Controllers/                    # REST controllers
│   ├── Program.cs & Startup.cs         # App configuration
│
├── Fundo.Core/                         # BUSINESS LAYER
│   ├── Interfaces/                     # Service contracts
│   ├── Models/                         # Domain models & DTOs
│   └── Services/                       # Business logic implementation
│
├── Fundo.DAL/                          # DATA ACCESS LAYER
│   ├── Entities/                       # Database entities (Loan, Payment)
│   ├── Enums/                          # Enumerations (LoanStatus)
│   └── Context/                        # Entity Framework DbContext
│
├── Fundo.Services.Tests/               # TESTS
│   └── Integration/                    # Integration tests
│
├── Dockerfile                          # Docker configuration
├── docker-compose.yml                  # Orchestration
├── .env & .env.example                 # Environment variables
├── README.md                           # Development guide
└── DEPLOYMENT.md                       # Render.com deployment guide
```

### Three-Layer Architecture

```
┌─────────────────────────────────────────┐
│   PRESENTATION (WebApi)                 │
│   Controllers → HTTP Request/Response   │
└─────────────────┬───────────────────────┘
                  ↓
┌─────────────────────────────────────────┐
│   BUSINESS (Core)                       │
│   Services → Business Logic             │
└─────────────────┬───────────────────────┘
                  ↓
┌─────────────────────────────────────────┐
│   DATA ACCESS (DAL)                     │
│   Entities → Database Operations        │
└─────────────────────────────────────────┘
```

### API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/loans` | List loans with filters and pagination |
| `GET` | `/loans/{id}` | Get loan details with payment history |
| `POST` | `/loans` | Create a new loan |
| `POST` | `/loans/{id}/payment` | Register a payment |

**Available filters:** `pageNumber`, `pageSize`, `applicantName`, `startDate`, `endDate`

### Deployment

#### Local Development with Docker

```bash
cd backend/src

# Configure environment
cp .env.example .env

# Start services (API + SQL Server)
docker-compose up -d

# API available at http://localhost:5000
```

#### Production Deployment on Render.com

1. **Push to Git**
   ```bash
   git push origin main
   ```

2. **Create Web Service on Render.com**
   - Root Directory: `backend/src`
   - Environment: `Docker`
   - Instance Type: `Free`

3. **Configure Environment Variables**
   - `ASPNETCORE_ENVIRONMENT` = `Production`
   - `ConnectionStrings__DefaultConnection` = Your Azure SQL connection string

4. **Deploy** - Wait 3-5 minutes for deployment

### Environment Configuration

#### `.env` File (Local Development)

```env
# Connection String
# Option 1: Local SQL Server (Docker)
DATABASE_CONNECTION_STRING=Server=sqlserver,1433;Initial Catalog=FundoDB;User ID=sa;Password=YourStrongPassword123!;...

# Option 2: Azure SQL (Production)
# DATABASE_CONNECTION_STRING=Server=your-server.database.windows.net,1433;Initial Catalog=your-db;User ID=user;Password=pass;...

ASPNETCORE_ENVIRONMENT=Development
```

#### Render.com Variables (Production)

| Variable | Value |
|----------|-------|
| `ASPNETCORE_ENVIRONMENT` | `Production` |
| `ASPNETCORE_URLS` | `http://0.0.0.0:80` |
| `ConnectionStrings__DefaultConnection` | Your Azure SQL connection string |

---

### 1.2. Pending Features in API

#### 1. Authentication & Authorization
#### 2. Swagger/OpenAPI Documentation
#### 3. Error Handling & Logging Middleware
#### 4. Soft Delete & Audit Fields

---

## 2. Frontend - Web Application (Angular 19)

### Directory Structure

```
frontend/src/app/
├── core/                               # BUSINESS LOGIC
│   ├── config/                         # Configuration (API URLs)
│   ├── models/                         # Interfaces & models
│   ├── repositories/                   # Repository interfaces
│   └── use-cases/                      # Application use cases
│
├── infraestructure/                    # INFRASTRUCTURE
│   ├── repositories/                   # Repository implementations
│   └── services/                       # API services (HttpClient)
│
├── presentation/                       # UI COMPONENTS
│   └── loan/
│       ├── loan-list/                  # Loan list component
│       ├── loan-details-dialog/        # Details modal
│       ├── create-loan-dialog/         # Create loan modal (pending)
│       └── create-payment-dialog/      # Payment modal (pending)
│
└── environments/                       # ENVIRONMENT CONFIG
    ├── environment.ts                  # Production
    └── environment.development.ts      # Development
```

### Clean Architecture (Frontend)

```
┌───────────────────────────────────────────┐
│   PRESENTATION (Components)               │
│   UI Logic & Templates                    │
└───────────────┬───────────────────────────┘
                ↓
┌───────────────────────────────────────────┐
│   USE CASES (Application Logic)           │
│   Orchestration & Business Rules          │
└───────────────┬───────────────────────────┘
                ↓
┌───────────────────────────────────────────┐
│   REPOSITORIES (Interfaces)               │
│   Data Access Abstractions                │
└───────────────┬───────────────────────────┘
                ↓
┌───────────────────────────────────────────┐
│   INFRASTRUCTURE (Implementations)        │
│   API Services & HTTP Calls               │
└───────────────────────────────────────────┘
```

**Benefits:** Testable, maintainable, scalable, framework-independent

### Implemented Components

1. **LoanListComponent** - Paginated loan list with filters
2. **LoanDetailsDialogComponent** - Loan details with payment history

### Environment Configuration

```typescript
// environment.ts (Production)
export const environment = {
  production: true,
  apiUrl: 'https://your-api.onrender.com'
};

// environment.development.ts (Development)
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000'
};
```

### Run Frontend

#### Local Development

```bash
cd frontend

# Install dependencies
npm install

# Development server (port 4200)
npm start

# Production build
npm run build
```

#### Deploy to Vercel

1. **Push to Git**
   ```bash
   git push origin main
   ```

2. **Import Project on Vercel**
   - Go to [vercel.com](https://vercel.com)
   - Click "Add New Project"
   - Import your Git repository
   - Vercel auto-detects Angular configuration

3. **Configure Environment Variables**
   - Add `VITE_API_URL` or configure in `environment.ts`
   - Set API URL to your Render backend

4. **Deploy** - Vercel automatically builds and deploys

Your app will be live at: `https://your-app.vercel.app`

---

### 2.2. Pending Features in Frontend

#### 1. Global Error Handling (Interceptors)
#### 2. Loan & Payment Management Improvements (validations)

## Quick Start

### Backend (API)

```bash
cd backend/src
cp .env.example .env
docker-compose up -d
curl http://localhost:5000/loans
```

### Frontend (Web)

```bash
cd frontend
npm install
npm start
# Open http://localhost:4200
```

---

## Tech Stack

### Backend
- .NET Core 6.0 - Web framework
- Entity Framework Core - ORM
- SQL Server - Database
- xUnit - Testing
- Docker - Containerization

### Frontend
- Angular 19 - SPA framework
- Angular Material - UI components
- RxJS - Reactive programming
- TypeScript - Type-safe JavaScript
- SCSS - Styling

### DevOps
- Docker & Docker Compose - Backend containerization
- Render.com - Backend hosting
- Vercel - Frontend hosting & deployment

---

## Architecture Decisions

1. **Three-Layer Architecture (Backend)** - Clear separation: presentation, business, data
2. **Clean Architecture (Frontend)** - Framework-independent, testable
3. **Repository Pattern** - Data access abstraction
4. **Use Case Pattern** - Application logic encapsulation
5. **DTO Pattern** - Separation between domain and transfer models
