# Team Collaboration Tool (GalaxyUML)

Desktop application for creating teams, scheduling meetings, and collaboratively drawing UML diagrams.

## Features

- User registration and login
- Creating and joining teams
- Creating and joining meetings
- Real-time UML diagram drawing on a shared board

## Technologies

- C# / .NET 10
- ASP.NET Core & SignalR
- React + Vite + TypeScript (Web)
- Avalonia UI (Cross-platform Desktop) & WPF (Windows Desktop)
- Entity Framework Core & SQL Server

## Getting Started

### 1. Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/)
- [Docker](https://www.docker.com/) (for SQL Server)
- [Node.js](https://nodejs.org/) or [Bun](https://bun.sh/) (for the React web client)

---

### 2. Start the Database (SQL Server)
Start the SQL Server Docker container:
```bash
docker start galaxyuml-sqlserver || docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=GalaxyUMLPass123!" -p 1433:1433 --name galaxyuml-sqlserver -d mcr.microsoft.com/mssql/server:2022-latest
```

Apply database migrations:
```bash
dotnet ef database update --project src/GalaxyUML.Data --startup-project src/GalaxyUML.Api
```

---

### 3. Start the Backend API
```bash
dotnet run --project src/GalaxyUML.Api
```
- **API & Swagger:** `http://localhost:5248`
- **SignalR Hub:** `http://localhost:5248/diagramHub`

---

### 4. Run a Frontend Client

Choose one of the available frontends:

#### Option A: Web Client (React) — *Recommended*
```bash
cd client/GalaxyUML.React
bun install   # or: npm install
bun run dev   # or: npm run dev
```
Open **`http://localhost:5173`** in your browser.

#### Option B: Cross-Platform Desktop (Avalonia — Linux / macOS / Windows)
```bash
dotnet run --project client/GalaxyUML.UI.Avalonia
```

#### Option C: Windows Desktop (WPF)
```bash
dotnet run --project client/GalaxyUML.UI
```

---

## Running Tests
```bash
dotnet test GalaxyUML.sln
```

## Status

Currently in development.

## Team Project

Developed in a team of three developers, collaborating on system design
and implementation.

## What We Learned

This project focuses on designing a complete application architecture,
including backend, frontend, and database components.

We gained experience with design patterns, real-time communication using SignalR,
and building collaborative features for multiple users.
