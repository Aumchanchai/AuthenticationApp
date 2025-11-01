ได้เลยครับ! ผมจัดให้แบบ **สรุปชัดเจน เหมาะกับ GitHub** พร้อมโครงสร้าง project และเทคโนโลยีที่ใช้ รวมทั้งสคริปต์ Angular และสภาพแวดล้อมของ .NET

---

```markdown
# Authentication Project

This repository contains a **Layered Monolith / Modular Monolith** application built with **.NET 7.0**, **Angular 13**, and **Microsoft SQL Server 2022**. It implements a complete authentication system with backend APIs, a test project, and a frontend SPA (Single Page Application).

---

## Project Structure

```

Authentication/          # .NET API Project
Authentication.Tests/    # Unit & Integration Tests for Authentication
DAL/                     # Class Library for EF Core and Domain Logic
Authentication-FE/       # Angular 13 Frontend Application

````

### Details

1. **Authentication (API Project)**
   - Backend implemented in **.NET 7.0**
   - Exposes endpoints for authentication, user management, etc.
   - References `DAL` class library for database access and domain logic

2. **Authentication.Tests**
   - Test project for backend
   - Uses xUnit/NUnit (depending on your setup) for unit and integration tests

3. **DAL**
   - Class library containing **Entity Framework Core** DbContext and domain logic
   - Encapsulates database operations for modularity

4. **Authentication-FE (Angular 13)**
   - SPA frontend for authentication system
   - Uses:
     - `@angular/core`, `@angular/router`, `@angular/forms`
     - `rxjs`, `zone.js`
     - `jwt-decode` for JWT handling
   - Scripts available in `package.json`:
     ```json
     "start": "ng serve",
     "build": "ng build",
     "watch": "ng build --watch --configuration development",
     "test": "ng test"
     ```

---

## Technologies

- **Backend:** .NET 7.0, C#
- **Frontend:** Angular 13, TypeScript
- **Database:** Microsoft SQL Server 2022 (RTM) - 16.0.1000.6 (X64)
- **ORM:** Entity Framework Core
- **Authentication:** JWT-based
- **Testing:** xUnit/NUnit (backend)

---

## Setup Instructions

### Backend (.NET)

1. Open solution in Visual Studio 2022 / VS Code
2. Restore NuGet packages:
   ```bash
   dotnet restore
````

3. Update connection string in `appsettings.json` for SQL Server
4. Apply EF Core migrations:

   ```bash
   dotnet ef database update --project DAL
   ```
5. Run the API:

   ```bash
   dotnet run --project Authentication
   ```

### Frontend (Angular)

1. Navigate to frontend folder:

   ```bash
   cd Authentication-FE
   ```
2. Install dependencies:

   ```bash
   npm install
   ```
3. Run Angular development server:

   ```bash
   npm start
   ```
4. Access the application at `http://localhost:4200`

---

## Notes

* `.gitignore` is configured to exclude build artifacts (`bin/`, `obj/`), Node dependencies (`node_modules/`), and Angular build output (`dist/`)
* This project follows a **modular monolith pattern**:

  * Backend logic is separated into layers: API, DAL, Tests
  * Frontend is a standalone Angular SPA consuming API endpoints

---
```
