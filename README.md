# TradeNexus - Brokerage Infrastructure Management System

TradeNexus is a multi-tenant, B2B Brokerage Infrastructure System. It is designed for master brokerage firms to manage multiple sub-brokers, clients, and trades with real-time risk analysis.

## 🚀 Overview

The system allows a master broker to:
- **Onboard Sub-Brokers**: Manage multiple brokerage companies under one platform.
- **Client Management**: Sub-brokers can create and manage their own clients.
- **Risk Management**: A hybrid C# and Python engine analyzes trade data to detect margin breaches and risk levels.
- **RESTful APIs**: Complete API endpoints with role-based access control for integration.
- **Enterprise Ready**: Designed to be hosted on IIS with MSSQL for robust data storage.

## 🛠 Tech Stack

- **Backend**: ASP.NET Core MVC (C#) + .NET 10.0
- **Database**: Microsoft SQL Server (MSSQL)
- **Quant/Risk Engine**: Python 3
- **ORM**: Entity Framework Core 10.0
- **Frontend**: Razor Views + Bootstrap 4
- **API**: RESTful APIs with Basic Authentication
- **Security**: Role-Based Access Control (RBAC)

## 📂 Project Structure

```bash
TradeNexus/
├── TradeNexus.sln             # Visual Studio Solution
├── Program.cs                 # App Entry Point
├── Startup.cs                 # App Configuration & Pipeline
├── appsettings.json           # DB Connection Strings & Logging
└── TradeNexus.Web/            # Main Web Project
    ├── Controllers/           # MVC Controllers (TradeController)
    ├── Data/                  # EF Core Context & Migrations
    ├── Models/                # Domain Entities (SubBroker, Client, Trade)
    ├── Services/              # Business Logic (Python Risk Service)
    ├── PythonEngine/          # Quant Logic (risk_engine.py)
    └── Views/                 # Razor Templates & UI Logic
```

## 📋 Prerequisites

Before running the project, ensure you have:
1. **Visual Studio 2019** or later (with .NET Core cross-platform development workload).
2. **.NET Core 3.1 SDK** installed.
3. **Microsoft SQL Server** (LocalDB or Express).
4. **Python 3.x** installed and added to your system `PATH`.

## ⚙️ Setup & Installation

### 1. Database Configuration
Update the `DefaultConnection` in `appsettings.json` to point to your SQL Server instance:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=TradeNexusDB;User Id=...;Password=...;"
}
```

### 2. Open in Visual Studio
- Open `TradeNexus.sln`.
- Right-click **TradeNexus.Web** in the Solution Explorer and select **Set as Startup Project**.

### 3. Initialize Database
Open the **Package Manager Console** (Service -> NuGet Package Manager) and run:
```powershell
Add-Migration InitialCreate
Update-Database
```

### 4. Run the Application
- Press **F5** to start the application.
- The browser will open to the **Client List** dashboard.
- Clicking **"Calculate Risk"** will trigger the Python script to analyze client positions.

## 🧠 Risk Engine Logic

The Python engine (`risk_engine.py`) uses the following logic:
- **Total Exposure**: Sum of all open positions (Price × Quantity).
- **Required Margin**: 20% of Total Exposure.
- **Risk Levels**:
    - 🟢 **Low**: Margin usage < 50%
    - 🟡 **Medium**: Margin usage 50% - 80%
    - 🔴 **High**: Margin usage > 80% (or Breach)

---
Developed for Master Brokerage Infrastructure Management.
