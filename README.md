# Small Bank App Reference Solution

This package contains a reference ASP.NET Core / C# REST API, Bootstrap frontend, and SQL Server 2022 database scripts for a small-bank digital banking application.

## Stack
- Frontend: Bootstrap, HTML, CSS, JavaScript hosted from ASP.NET Core wwwroot
- Backend: C#, ASP.NET Core REST API, Identity, JWT Bearer auth
- Database: SQL Server 2022
- Security: MFA-ready login, biometric/passkey-ready frontend, encrypted transport, audit logs, AML screening stub

## Run locally
1. Install .NET 8 SDK and SQL Server 2022.
2. Create the database using `database/SmallBankAppDb.sql` or use EF Core migrations.
3. Update `src/SmallBankApp.Api/appsettings.json` with a secure connection string and JWT key.
4. From `src/SmallBankApp.Api`, run:
   ```bash
   dotnet restore
   dotnet run
   ```
5. Open `https://localhost:5001` for the Bootstrap demo and `/swagger` for APIs.

## Production hardening checklist
- Store secrets in Azure Key Vault or equivalent.
- Replace simulated e-KYC, AML, alerts, and payment services with approved vendors.
- Enable WebAuthn/passkeys or native mobile biometric APIs.
- Add rate limiting, device binding, fraud scoring, token rotation, and mTLS for partner APIs.
- Complete PCI DSS scoping before handling cardholder data.
- Perform threat modeling, SAST/DAST, penetration testing, and accessibility testing before release.

## UI and Technology Stack Update

### Frontend
- ASP.NET Core static web assets hosted from `wwwroot`.
- Bootstrap 5 responsive layout with cards, tables, forms, navigation, and toast notifications.
- HTML5 semantic sections for Accounts, Transfers, Alerts, Digital Onboarding/e-KYC, and AML Review.
- Custom CSS in `wwwroot/css/site.css` for secure-banking branding, responsive layout, focus states, and high-contrast accessibility mode.
- JavaScript in `wwwroot/js/app.js` for demo account rendering, transfer submission simulation, alert notifications, low-balance threshold UI, biometric/passkey demo messaging, and AML review routing.

### Backend
- C# ASP.NET Core Web API.
- REST endpoints under `/api/auth`, `/api/accounts`, `/api/transfers`, `/api/onboarding`, `/api/alerts`, and `/api/external-accounts`.
- SQL Server 2022 compatible connection string and schema scripts under `database/`.
- Entity Framework Core with Identity, JWT authentication, audit logging, e-KYC service abstraction, AML screening, and payment gateway service abstraction.

### Running Locally
1. Install .NET 8 SDK and SQL Server 2022.
2. Create the database using `database/SmallBankAppDb.sql` or configure EF migrations.
3. Update `src/SmallBankApp.Api/appsettings.json` with your SQL Server credentials and a secure JWT signing key.
4. From the solution root, run:
   ```bash
   dotnet restore
   dotnet build
   dotnet run --project src/SmallBankApp.Api
   ```
5. Open the app at the local ASP.NET URL and open Swagger at `/swagger`.
