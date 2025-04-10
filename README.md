[![Production](https://github.com/ferdikosasih/Agency/actions/workflows/deploy.yml/badge.svg)](https://github.com/ferdikosasih/Agency/actions/workflows/deploy.yml)
[![.NET](https://github.com/ferdikosasih/Agency/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/ferdikosasih/Agency/actions/workflows/dotnet.yml)
# Flabs Agency
## 📌 Overview
It's agency system to manages appointments for Customers / Clients.
## 📖 Features
### 🗓️ **Appointment Management**
- Booking schedule appointments
- Ensure appointments are at least **15 minutes in the future**
- Prevent bookings on public holidays
- Support timezone schedules
### **Holiday Calendars**
- Import holiday calendars (ID)
- View holiday calendars
### **Authentication & Authorization**
- Basic Login Request for Access Token
- Basic RBAC 
---  
## 🚀 Tech Stack & Package Reference
- **ASP.NET Core (FastEndpoints)** - High-performance minimal API framework
- **Entity Framework Core** - ORM for PostgreSQL database operations
- **FluentValidation** - Request validation
- **FastEndpoint.Testing & FluentAssertions** - Unit and integration testing
- **Testcontainers (PostgreSQL)** - Containerized testing environment
- **Serilogs** - Logging Console
- **PostgreSQL** - High-performance relational database
- **Ulid** - for token appointment generation simplicity based on timestamp
---
## 🔧 Setup & Installation
### **1️⃣ Prerequisites**
Make sure you have the following installed:
- .NET 8.0.407 SDK
- PostgreSQL 17 database
- Docker (for Testcontainers)
### **2️⃣ How to use**
- Open Browser/Swagger : https://agency-cdawgyavd2eyaubb.southeastasia-01.azurewebsites.net/swagger/index.html
- for demo purpose account UserId : agent1 
---
## 📖 Technical Design & Considerations
- Vertical Slide Architecture
- Fastendpoint helps to reduce boilerplates and high-performance API
- EntityFrameworkCore for database operations
- Testcontainer for real integrations test to PostgreSQL
- Serilogs for common logging
- Supports timezone capabilities for booking appointment
- Unit test not implemented at this time, only integration tests
---
## 🔒Security
- RSA Asymmetrics SHA256
- Jwt token
- Token expired set to 1 hour
## Deployment
- Github Actions CI to Azure Web App Service (Linux)
- Azure Postgre Flexible Server
---
## 📌Future Improvements
- Introduce unit test when features much more complex with business logics
- Expand Authentications & Security with Duende Identity Servers or Azure B2C or KeyCloack
- For production ready, use EF Migrations tools
- Auto register DI for features development
- Token for queue no can be developed using SmartFormat if needed more human read and sequential.
- Introduce indexing tables when needed
- Introduce environment staging & production in azure
- Introduce CI Integrations Test, soon!
---
## ⚠️Limitation
- Develop & Testing using Rider IDE
---
## ✨ Contributing

Pull requests are welcome! Follow these steps:

1. Fork the repository
2. Create a new feature branch (`git checkout -b feature-name`)
3. Commit changes (`git commit -m "Add new feature"`)
4. Push to the branch (`git push origin feature-name`)
5. Open a pull request
