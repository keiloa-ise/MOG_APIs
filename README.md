# MOJ_Solution 'User Management API Documentation'

### Repoditory
https://github.com/keiloa-ise/MOG_APIs

### Prerequisites
- Modern API Design with CQRS
- .NET 9.0 SDK or newer
- SQL Server 

### Verify Installation:
1. Clone the repo
     ```
    git clone https://github.com/your-repo/MOG_Solution.git
    cd MOG_Solution
    ```
2. Update connection strings in `appsettings.json`
3. Migrations (Package Manager Console):
    * cd APIs
    * dotnet restore
    * dotnet build
    * Add-Migration MigrationName -OutputDir Data\Migrations -Context ApplicationDbContext
    * Update-Database

## Content
| Project  | Discription | Type |
| ------------- |:-------------:|:-------------:|
| APIs | Main Project | Web API |
| UserManagments | User Managment Logic | Class Library |

# Markdown syntax guide

## 📋 Table of Contents

-   Overview
    
-   Base URL
    
-   Authentication
    
-   API Endpoints
    
    -   Authentication Endpoints
        
    -   User Management Endpoints
      
    -   User Departments Endpoints
        
    -   Role Management Endpoints
        
    -   Health Check Endpoints
        
    -   Password Management Utilities
        
-   Error Responses
    
-   Role Hierarchy & Permissions
    
-   Configuration
    
-   Testing APIs
    
-   Rate Limiting
    
-   Database Schema
    
-   Migration Commands
    
-   Security Features
    
-   Support & Contact
    
-   License
    
-   Changelog
    
-   Quick Start Guide
    

## 📋 Overview

This is a comprehensive RESTful API for User Management System built with [ASP.NET](https://asp.net/) Core, CQRS, MediatR, and Entity Framework Core. The API provides complete user management functionality including authentication, authorization, role management, and password management.

* * *

## 🚀 Base URL
```
text

https://localhost:7289/api
```
Development Environment: `https://localhost:7289/api`  
Production Environment: `https://aa.a.a/api`

* * *

## 🔐 Authentication

All endpoints (except public ones) require JWT Bearer Token authentication.

### Authentication Header:

http

Authorization: Bearer {jwt\_token}

### Token Structure:
```
json

{
  "access\_token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refresh\_token": "abc123...",
  "expires\_at": "2024-01-15T11:30:00Z",
  "token\_type": "Bearer"
}
```
* * *

## 📊 API Endpoints

## 1\. 🔐 Authentication Endpoints

### 1.1 Signup - Register New User
```
`POST /api/users/signup`
```
Public Endpoint - No authentication required

Request Body:
```
json

{
  "username": "john\_doe",
  "email": "john@example.com",
  "password": "Password123!",
  "confirmPassword": "Password123!",
  "fullName": "John Doe",
  "phoneNumber": "+971501234567",
  "roleId": 3
}
```
Required Fields:

-   `username` (string, 3-50 chars)
    
-   `email` (string, valid email format)
    
-   `password` (string, min 8 chars with uppercase, lowercase, number, special char)
    
-   `confirmPassword` (string, must match password)
    
-   `roleId` (integer, default: 3 for "User")
    

Response (201 Created):
```
json

{
  "status": "success",
  "message": "User registered successfully",
  "data": {
    "userId": "1",
    "username": "john\_doe",
    "email": "john@example.com",
    "fullName": "John Doe",
    "roleName": "User",
    "roleId": 3,
    "createdAt": "2024-01-15T10:30:00Z",
    "message": "User registered successfully"
  },
  "errors": null
}
```
* * *

### 1.2 Signin - User Login
```
`POST /api/auth/signin`
```
Public Endpoint - No authentication required

Request Body:
```
json

{
  "usernameOrEmail": "john\_doe",
  "password": "Password123!",
  "rememberMe": true
}
```
Response (200 OK):
```
json

{
  "status": "success",
  "message": "Login successful",
  "data": {
    "userId": "1",
    "username": "john\_doe",
    "email": "john@example.com",
    "fullName": "John Doe",
    "roleName": "User",
    "roleId": 3,
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "abc123...",
    "tokenExpiry": "2024-01-15T11:30:00Z",
    "lastLogin": "2024-01-15T10:30:00Z",
    "message": "Login successful"
  },
  "errors": null
}
```
* * *

### 1.3 Refresh Token
```
`POST /api/auth/refresh`
```
Public Endpoint - Requires expired access token

Request Body:
```
json

{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "abc123..."
}
```
Response (200 OK):
```
json

{
  "status": "success",
  "message": "Token refreshed successfully",
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "xyz789...",
    "expiresAt": "2024-01-15T12:30:00Z",
    "tokenType": "Bearer"
  },
  "errors": null
}
```
* * *

### 1.4 Get User Profile
```
`GET /api/auth/profile`
```
Protected Endpoint - Requires valid JWT token

Headers:
```
http

Authorization: Bearer {access\_token}
```
Response (200 OK):
```
json

{
  "status": "success",
  "message": "Profile retrieved successfully",
  "data": {
    "userId": "1",
    "username": "john\_doe",
    "email": "john@example.com",
    "fullName": "John Doe",
    "phoneNumber": "+971501234567",
    "roleName": "User",
    "roleId": 3,
    "createdAt": "2024-01-15T10:30:00Z",
    "lastLogin": "2024-01-15T10:30:00Z",
    "isActive": true
  },
  "errors": null
}
```
* * *

### 1.5 Change Password
```
`POST /api/auth/change-password`
```
Protected Endpoint - Requires valid JWT token

Headers:
```
http

Authorization: Bearer {access\_token}
```
Request Body:
```
json

{
  "currentPassword": "OldPassword123!",
  "newPassword": "NewPassword456@",
  "confirmPassword": "NewPassword456@"
}
```
Password Requirements:

-   Minimum 8 characters
    
-   At least one uppercase letter
    
-   At least one lowercase letter
    
-   At least one number
    
-   At least one special character (!@#$%^&\* etc.)
    

Response (200 OK):
```
json

{
  "status": "success",
  "message": "Password changed successfully",
  "data": {
    "userId": "1",
    "username": "john\_doe",
    "email": "john@example.com",
    "changedAt": "2024-01-15T14:30:00Z",
    "message": "Password changed successfully"
  },
  "errors": null
}
```
* * *

### 1.6 Signout
```
`POST /api/auth/signout`
```
Protected Endpoint - Requires valid JWT token

Headers:
```
http

Authorization: Bearer {access\_token}
```
Response (200 OK):
```
json

{
  "status": "success",
  "message": "Logged out successfully"
}
```
* * *

## 2\. 👥 User Management Endpoints

### 2.1 Check Username/Email Availability
```
`GET /api/users/check-availability`
```
Public Endpoint - No authentication required

Query Parameters:

-   `email` (optional): Email to check
    
-   `username` (optional): Username to check
    

Example Request:
```
text

GET /api/users/check-availability?email=john@example.com&username=john\_doe
```
Response (200 OK):
```
json

{
  "status": "success",
  "message": "Availability checked successfully",
  "data": {
    "available": false,
    "email": "john@example.com",
    "username": "john\_doe",
    "message": "Already taken"
  },
  "errors": null
}
```
* * *

### 2.2 Change User Role
```
`POST /api/users/change-role`
```
Protected Endpoint - Requires roles: `SuperAdmin, Admin, Manager`

Headers:
```
http

Authorization: Bearer {access\_token}
```
Request Body:
```
json

{
  "userId": 10,
  "newRoleId": 4,
  "reason": "Promoted to Manager due to performance"
}
```
Response (200 OK):
```
json

{
  "status": "success",
  "message": "User role changed successfully",
  "data": {
    "userId": "10",
    "username": "john\_doe",
    "previousRoleName": "User",
    "previousRoleId": 3,
    "newRoleName": "Manager",
    "newRoleId": 4,
    "changedBy": "admin\_user",
    "changedAt": "2024-01-15T14:30:00Z",
    "message": "User role changed successfully"
  },
  "errors": null
}
```
* * *

### 2.3 Get User Role History
```
`GET /api/users/{userId}/role-history`
```
Protected Endpoint - Requires roles: `SuperAdmin, Admin, Manager`

Headers:

http

Authorization: Bearer {access\_token}

Path Parameters:

-   `userId` (integer): User ID
    

Query Parameters:

-   `fromDate` (optional): Start date (YYYY-MM-DD)
    
-   `toDate` (optional): End date (YYYY-MM-DD)
    

Example Request:
```
text

GET /api/users/10/role-history?fromDate=2024-01-01&toDate=2024-01-31
```
Response (200 OK):
```
json

{
  "status": "success",
  "message": "Role history retrieved successfully",
  "data": \[
    {
      "id": 1,
      "userId": 10,
      "username": "john\_doe",
      "previousRoleName": "User",
      "newRoleName": "Manager",
      "changedByUsername": "admin\_user",
      "reason": "Promoted to Manager",
      "changedAt": "2024-01-15T14:30:00Z"
    }
  \],
  "errors": null
}
```
* * *

### 2.4 Assign Role (Alternative Endpoint)
```
`POST /api/users/{userId}/assign-role`
```
Protected Endpoint - Requires roles: `SuperAdmin, Admin, Manager`

Headers:

http

Authorization: Bearer {access\_token}

Path Parameters:

-   `userId` (integer): User ID
    

Request Body:
```
json

{
  "roleId": 5,
  "reason": "Assigned as Editor for content management"
}
```
Response: Same as Change User Role endpoint

* * *
## 2\. 👥 User Departments Endpoints

|Method	|Endpoint	|Description|
| ------------- |:-------------:|:-------------:|
GET | /api/users/{userId}/departments | Get user's departments
POST | /api/users/{userId}/departments/assign | Assign new departments (replace)
PUT | /api/users/{userId}/departments/update | Update departments (add/delete)
DELETE | /api/users/{userId}/departments/clear | Delete all departments
PATCH | /api/users/{userId}/departments/set-primary/{departmentId} | Set a primary department
GET | /api/users/{userId}/departments/check/{departmentId} | Check if a user is in a department
GET | /api/users/{userId}/departments/stats | Department statistics
* * *

## 3\. 🏛️ Role Management Endpoints

### 3.1 Get All Roles
```
`GET /api/roles`
```
Protected Endpoint - Requires roles: `SuperAdmin, Admin`

Headers:

http

Authorization: Bearer {access\_token}

Query Parameters:

-   `isActive` (optional): Filter by active status (true/false)
    

Example Request:
```
text

GET /api/roles?isActive=true
```
Response (200 OK):
```
json

{
  "status": "success",
  "message": "Roles retrieved successfully",
  "data": \[
    {
      "id": 1,
      "name": "SuperAdmin",
      "description": "System Super Administrator",
      "isActive": true,
      "createdAt": "2024-01-15T10:30:00Z",
      "userCount": 1
    },
    {
      "id": 2,
      "name": "Admin",
      "description": "System Administrator",
      "isActive": true,
      "createdAt": "2024-01-15T10:30:00Z",
      "userCount": 3
    },
    {
      "id": 3,
      "name": "User",
      "description": "Regular User",
      "isActive": true,
      "createdAt": "2024-01-15T10:30:00Z",
      "userCount": 100
    }
  \],
  "errors": null
}
```
* * *

### 3.2 Get Available Roles (Public)
```
`GET /api/roles/available`
```
Public Endpoint - No authentication required

Response (200 OK):
```
json

{
  "status": "success",
  "message": "Roles retrieved successfully",
  "data": \[
    {
      "id": 3,
      "name": "User",
      "description": "Regular User",
      "isActive": true,
      "createdAt": "2024-01-15T10:30:00Z",
      "userCount": 100
    },
    {
      "id": 4,
      "name": "Manager",
      "description": "Department Manager",
      "isActive": true,
      "createdAt": "2024-01-15T10:30:00Z",
      "userCount": 10
    }
  \],
  "errors": null
}
```
* * *

## 4\. 🩺 Health Check Endpoints

### 4.1 API Health Check
```
`GET /health`
```
Public Endpoint - No authentication required

Response (200 OK):
```
json

{
  "status": "Healthy",
  "totalDuration": "00:00:00.1234567",
  "entries": \[
    {
      "name": "self",
      "status": "Healthy",
      "duration": "00:00:00.0012345"
    }
  \]
}
```
* * *

### 4.2 Database Health Check
```
`GET /health/database`
```
Public Endpoint - No authentication required

Response (200 OK):
```
json

{
  "status": "Healthy",
  "totalDuration": "00:00:00.2345678",
  "entries": \[
    {
      "name": "database",
      "status": "Healthy",
      "duration": "00:00:00.1234567",
      "description": "Database connection is healthy"
    }
  \]
}
```
* * *

### 4.3 Health Controller Endpoint
```
`GET /api/health`
```
Public Endpoint - No authentication required

Response (200 OK):
```
json

{
  "status": "Healthy",
  "totalDuration": "00:00:00.1234567",
  "timestamp": "2024-01-15T10:30:00Z",
  "version": "1.0.0",
  "environment": "Development",
  "entries": \[
    {
      "name": "self",
      "status": "Healthy",
      "duration": 1.2345,
      "description": null,
      "exception": null,
      "data": {}
    }
  \]
}
```
* * *

### 4.4 Ping Endpoint
```
`GET /api/health/ping`
```
Public Endpoint - No authentication required

Response (200 OK):
```
json

{
  "message": "API is running",
  "timestamp": "2024-01-15T10:30:00Z",
  "version": "1.0.0"
}
```
* * *

## 5\. 📝 Password Management Utilities

### 5.1 Validate Password Strength
```
`POST /api/auth/validate-password`
```
Public Endpoint - No authentication required

Request Body:
```
json

{
  "password": "Test123!"
}
```
Response (200 OK):
```
json

{
  "isValid": false,
  "errors": \[
    "Password must be at least 8 characters long",
    "Password must contain at least one special character"
  \]
}
```
* * *

### 5.2 Generate Strong Password
```
`GET /api/auth/generate-password`
```
Protected Endpoint - Requires valid JWT token

Headers:

http

Authorization: Bearer {access\_token}

Response (200 OK):
```
json

{
  "password": "A1b@C2d#E3f$",
  "strength": "strong",
  "length": 12
}
```
* * *

## 📋 Error Responses

### Common Error Status Codes:

| Status Code | Description |
| --- | --- |
| 400 | Bad Request - Invalid input data |
| 401 | Unauthorized - Missing or invalid token |
| 403 | Forbidden - Insufficient permissions |
| 404 | Not Found - Resource not found |
| 409 | Conflict - Resource already exists |
| 429 | Too Many Requests - Rate limit exceeded |
| 500 | Internal Server Error - Server error |

### Error Response Format:
```
json

{
  "status": "error",
  "message": "Error description",
  "errors": \[
    "Error detail 1",
    "Error detail 2"
  \],
  "detail": "Technical details (only in development)"
}
```
### Example Error Responses:

400 - Validation Error:
```
json

{
  "status": "error",
  "message": "Validation failed",
  "errors": \[
    "Username must be between 3 and 50 characters",
    "Invalid email address"
  \]
}
```
401 - Unauthorized:
```
json

{
  "status": "error",
  "message": "Invalid authentication token"
}
```
403 - Forbidden:
```
json

{
  "status": "error",
  "message": "You do not have permission to perform this action"
}
```
409 - Conflict:
```
json

{
  "status": "error",
  "message": "Email already exists"
}
```
* * *

## 🔒 Role Hierarchy & Permissions

### Role Levels:

1.  SuperAdmin (Level 1) - Full system access
    
2.  Admin (Level 2) - User and role management
    
3.  Manager (Level 3) - Team management
    
4.  Editor (Level 4) - Content editing
    
5.  User (Level 5) - Regular user access
    
6.  Viewer (Level 6) - Read-only access
    

### Permission Rules:

-   Users can only modify roles lower than their own level
    
-   SuperAdmin can modify any role (including other SuperAdmins)
    
-   Admin cannot modify SuperAdmin roles
    
-   Users cannot change their own role
    
-   Password changes require current password verification
    

### Default Roles:

| Role ID | Name | Description | Default Users |
| --- | --- | --- | --- |
| 1 | SuperAdmin | System Super Administrator | 1 |
| 2 | Admin | System Administrator | 3 |
| 3 | User | Regular User | 100+ |
| 4 | Manager | Department Manager | 10 |
| 5 | Editor | Content Editor | 15 |
| 6 | Viewer | Content Viewer | 20 |

* * *

## ⚙️ Configuration

### Environment Variables:

bash

\# Database Configuration
```
ConnectionStrings\_\_DefaultConnection\=Server\=localhost;Database\=MOJ\_Users;Trusted\_Connection\=True;MultipleActiveResultSets\=true;TrustServerCertificate\=True;
```
\# JWT Configuration
```
Jwt\_\_Key\=YourSuperSecretKeyHereAtLeast32CharactersLong12345!
Jwt\_\_Issuer\=MOJ\_API
Jwt\_\_Audience\=MOJ\_Client
Jwt\_\_ExpireMinutes\=60
Jwt\_\_RefreshTokenExpireDays\=7
```
\# Redis Configuration (Optional)
```
APP\_REDIS\_HOST\=localhost
APP\_REDIS\_PORT\=6379
APP\_REDIS\_PASSWORD\=
APP\_REDIS\_DB\=0
APP\_REDIS\_SSL\=0
```
\# Background Services
```
BackgroundService\_ProcessIntervalMinutes\=1.0
BackgroundService\_RetryDelayMinutes\=0.5
```
\# Application Settings
```
AllowedHosts\=\*
LogLevel\_\_Default\=Information
LogLevel\_\_Microsoft.AspNetCore\=Warning
```
### appsettings.json Structure:
```
json

{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "\*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\\\MSSQLLocalDB;Database=MOJ\_Users;Trusted\_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "YourSuperSecretKeyHereAtLeast32CharactersLong12345!",
    "Issuer": "MOJ\_API",
    "Audience": "MOJ\_Client",
    "ExpireMinutes": 60,
    "RefreshTokenExpireDays": 7
  }
}
```
* * *

## 🧪 Testing APIs

### Using cURL:

Signup Example:
```
bash

curl \-X POST "https://localhost:7289/api/users/signup" \\
  \-H "Content-Type: application/json" \\
  \-H "accept: application/json" \\
  \-d '{"username":"testuser","email":"test@example.com","password":"Password123!","confirmPassword":"Password123!","roleId":3}'
```
Signin Example:
```
bash

curl \-X POST "https://localhost:7289/api/auth/signin" \\
  \-H "Content-Type: application/json" \\
  \-H "accept: application/json" \\
  \-d '{"usernameOrEmail":"testuser","password":"Password123!","rememberMe":true}'
```
Protected Endpoint Example:
```
bash

curl \-X GET "https://localhost:7289/api/auth/profile" \\
  \-H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." \\
  \-H "accept: application/json"
```
### Using Swagger UI:

Access interactive API documentation at:
```
text

https://localhost:7289/swagger
```
Swagger Features:

-   Interactive API testing
    
-   Request/Response examples
    
-   Authentication setup
    
-   Schema documentation
    
-   Try-it-out functionality
    

### Using Postman:

Postman Collection Structure:
```
text

MOJ User Management API
├── Authentication
│   ├── Signup
│   ├── Signin
│   ├── Refresh Token
│   ├── Get Profile
│   ├── Change Password
│   └── Signout
├── User Management
│   ├── Check Availability
│   ├── Change User Role
│   └── Get Role History
├── Role Management
│   ├── Get All Roles
│   └── Get Available Roles
└── Health Checks
    ├── API Health
    ├── Database Health
    └── Ping
```
Postman Environment Variables:
```
javascript

{
  "base\_url": "https://localhost:7289/api",
  "access\_token": "{{jwt\_token}}",
  "user\_id": "{{current\_user\_id}}"
}
```
* * *

## 🗃️ Database Schema

### Tables Overview:

| Table | Description | Key Columns |
| --- | --- | --- |
| AppUsers | User accounts | Id, Username, Email, RoleId |
| Roles | System roles | Id, Name, IsActive |
| UserRoleChangeLogs | Role change history | Id, UserId, PreviousRoleId, NewRoleId |
| PasswordChangeLogs | Password change audit | Id, UserId, ChangeType, PasswordHash |
| PasswordHistories | Password history | Id, UserId, PasswordHash, CreatedAt |

### Entity Relationships:

text
```
AppUsers (1) ──── (1) Roles
    │
    ├─── (1..\*) UserRoleChangeLogs
    ├─── (1..\*) PasswordChangeLogs
    └─── (1..\*) PasswordHistories
```
### Indexes:

-   `IX_AppUsers_Email` (Unique)
    
-   `IX_AppUsers_Username` (Unique)
    
-   `IX_Roles_Name` (Unique)
    
-   `IX_UserRoleChangeLogs_UserId`
    
-   `IX_PasswordChangeLogs_UserId`
    
-   `IX_PasswordHistories_UserId`
    

* * *

## 🔄 Migration Commands

### Using Package Manager Console:

powershell

\# Add new migration
```
Add-Migration MigrationName -OutputDir Data\Migrations -Context ApplicationDbContext
```
\# Update database
```
Update-Database \-Context ApplicationDbContext
```
\# Remove last migration
```
Remove-Migration \-Context ApplicationDbContext
```
\# Generate SQL script
```
Script-Migration \-From PreviousMigration \-To NewMigration \-Context ApplicationDbContext
```
### Using .NET CLI:

bash

\# Install EF Core tools
```
dotnet tool install \--global dotnet-ef
```
\# Add new migration
```
dotnet ef migrations add MigrationName \--project Infrastructure --startup-project API \--context ApplicationDbContext
```
\# Update database
```
dotnet ef database update \--project Infrastructure --startup-project API \--context ApplicationDbContext
```
\# Remove migration
```
dotnet ef migrations remove \--project Infrastructure --startup-project API \--context ApplicationDbContext
```
### Common Migration Examples:

bash

\# Initial database setup
```
dotnet ef migrations add InitialCreate
dotnet ef database update
```
\# Add new tables
```
dotnet ef migrations add AddPasswordTables
dotnet ef database update
```
\# Modify existing tables
```
dotnet ef migrations add UpdateUserTable
dotnet ef database update
```
### Database Reset (Development Only):

bash

\# Drop and recreate database
```
dotnet ef database drop \--force
dotnet ef database update
```
* * *

## 🚨 Security Features

### 1. Authentication & Authorization

-   JWT Bearer Token authentication
    
-   Role-based access control (RBAC)
    
-   Token expiration and refresh mechanism
    
-   Secure password hashing with BCrypt
    

### 2. Data Protection

-   SQL injection protection via Entity Framework
    
-   XSS prevention through input validation
    
-   HTTPS enforcement
    
-   CORS policy configuration
    
-   CSRF protection
    

### 3. Password Security

-   BCrypt hashing with work factor 12
    
-   Password history tracking (last 5 passwords)
    
-   Password strength validation
    
-   Password reuse prevention
    
-   Account lockout after failed attempts
    

### 4. Audit & Logging

-   Complete audit trails for sensitive operations
    
-   Password change logging
    
-   Role change history
    
-   IP address tracking
    
-   User agent logging
    

### 5. Rate Limiting & Throttling

-   Request rate limiting
    
-   Brute force protection
    
-   DDoS mitigation
    
-   API key rotation (if implemented)
    

### 6. Input Validation

-   Request model validation
    
-   SQL injection prevention
    
-   XSS prevention
    
-   File upload validation
    
-   Data sanitization
    

### 7. Error Handling

-   Secure error messages
    
-   No sensitive data in error responses
    
-   Structured error logging
    
-   Exception filtering
    

* * *

## 📞 Support & Contact

### Technical Support:

-   Email: `api-support@moj.gov`
    
-   Phone: `+971 2 123 4567`
    
-   Hours: Sunday-Thursday, 8:00 AM - 4:00 PM (GST)
    

### Documentation:

-   API Documentation: `https://docs.moj.gov/api`
    
-   Swagger UI: `https://api.moj.gov/swagger`
    
-   GitHub Repository: `https://github.com/moj-uae/user-management-api`
    

### Status & Monitoring:

-   Status Page: `https://status.moj.gov`
    
-   API Health: `https://api.moj.gov/health`
    
-   Uptime: 99.9% SLA
    

### Issue Reporting:

1.  GitHub Issues: For bug reports and feature requests
    
2.  Email Support: For production issues
    
3.  Slack Channel: `#api-support` for quick questions
    

### Response Times:

| Priority | Initial Response | Resolution Target |
| --- | --- | --- |
| Critical | 1 hour | 4 hours |
| High | 4 hours | 24 hours |
| Medium | 24 hours | 3 days |
| Low | 3 days | 7 days |

* * *

## 📄 License

### Proprietary License

© 2024 Ministry of Justice, UAE. All rights reserved.

Terms of Use:

1.  This software is proprietary and confidential
    
2.  Unauthorized copying, modification, or distribution is prohibited
    
3.  Use is restricted to authorized personnel only
    
4.  All modifications must be documented and approved
    

Commercial Use:

-   Internal use within MOJ: Permitted
    
-   External distribution: Prohibited
    
-   Integration with third-party systems: Requires written approval
    

Copyright Notice:

text

Copyright (c) 2024 Ministry of Justice, UAE.
This software is protected by copyright law and international treaties.
Unauthorized reproduction or distribution may result in civil and criminal penalties.

* * *

## 🔄 Changelog

### Version 1.0.0 (January 15, 2024)

#### 🎉 Initial Release

Features:

-   Complete user management system
    
-   JWT-based authentication
    
-   Role-based authorization
    
-   Password management with policy enforcement
    
-   Audit logging for all sensitive operations
    
-   Health monitoring endpoints
    
-   Swagger documentation
    

Endpoints Added:

-   User registration and authentication
    
-   Profile management
    
-   Role management
    
-   Password change and reset
    
-   Health checks
    
-   API monitoring
    

Security Features:

-   BCrypt password hashing
    
-   JWT token refresh mechanism
    
-   Rate limiting
    
-   Input validation
    
-   SQL injection prevention
    

* * *

## 🎯 Quick Start Guide

### Step 1: Prerequisites

bash

\# Required Software
```
- .NET 8.0 SDK
- SQL Server 2019\+ (or SQL Server Express)
- Visual Studio 2022\+ or VS Code
- Git
```
\# Verify Installation
```
dotnet \--version
sqlcmd -?
git \--version
```
### Step 2: Clone and Setup

bash

\# Clone repository
```
git clone https://github.com/moj-uae/user-management-api.git
cd user-management-api
```
\# Restore packages
```
dotnet restore
```
\# Configure appsettings
```
cp appsettings.Development.json appsettings.json
\# Edit appsettings.json with your database connection
```
### Step 3: Database Setup

bash

\# Create database migration
```
dotnet ef migrations add InitialCreate
```
\# Apply migrations
```
dotnet ef database update
```
\# Verify database
```
sqlcmd \-S localhost \-d MOJ\_Users \-Q "SELECT name FROM sys.tables"
```
### Step 4: Run the Application

bash

\# Development mode
```
dotnet run
```
\# Or using Visual Studio
\# 1. Open MOJ\_Solution.sln
\# 2. Set APIs project as Startup Project
\# 3. Press F5

### Step 5: Initial Configuration

bash

\# 1. Access Swagger UI: https://localhost:7289/swagger
\# 2. Create first SuperAdmin user:
```
curl \-X POST "https://localhost:7289/api/users/signup" \\
  \-H "Content-Type: application/json" \\
  \-d '{"username":"superadmin","email":"admin@moj.gov","password":"Admin123!","confirmPassword":"Admin123!","roleId":1}'
```
\# 3. Login to get JWT token
```
curl \-X POST "https://localhost:7289/api/auth/signin" \\
  \-H "Content-Type: application/json" \\
  \-d '{"usernameOrEmail":"superadmin","password":"Admin123!"}'
```
### Step 6: Testing

bash

\# Test health endpoint
```
curl https://localhost:7289/health
```
\# Test API with token
```
curl \-H "Authorization: Bearer {token}" https://localhost:7289/api/auth/profile
```
### Step 7: Development Workflow

bash

\# Create new feature
```
git checkout \-b feature/new-endpoint
```
\# Add migration for database changes
```
dotnet ef migrations add AddNewFeature
```
\# Test changes
```
dotnet test
```
\# Commit and push
```
git add .
git commit \-m "Add new endpoint"
git push origin feature/new-endpoint
```
### Step 8: Deployment

bash

\# Build for production
```
dotnet publish \-c Release \-o ./publish
```
\# Deploy to IIS
\# 1. Create application in IIS
\# 2. Set physical path to publish folder
\# 3. Configure app pool for No Managed Code
\# 4. Set environment variables

\# Or deploy to Docker
```
docker build \-t moj-api .
docker run \-p 8080:80 \-e ASPNETCORE\_ENVIRONMENT\=Production moj-api
```
### Troubleshooting:

bash

\# Common issues and solutions:

\# 1. Database connection failed
\#    - Check SQL Server is running
\#    - Verify connection string
\#    - Check firewall settings

\# 2. Migration errors
```
dotnet ef database drop \--force
dotnet ef database update
```
\# 3. Port already in use
```
netstat \-ano | findstr :7289
taskkill /PID <PID\> /F
```
\# 4. JWT token issues
\#    - Check JWT configuration in appsettings.json
\#    - Verify token expiration
\#    - Check issuer and audience settings

* * *

Last Updated: January 15, 2024  
API Version: 1.0.0  
Author: MOJ Development Team  
Status: Production Ready

* * *

## 📱 API Clients

### JavaScript/TypeScript:

javascript

// Using Axios
```
import axios from 'axios';

const api \= axios.create({
  baseURL: 'https://api.moj.gov/api',
  headers: {
    'Content-Type': 'application/json'
  }
});
```
// Add token to requests
```
api.interceptors.request.use(config \=> {
  const token \= localStorage.getItem('access\_token');
  if (token) {
    config.headers.Authorization \= \`Bearer ${token}\`;
  }
  return config;
});
```
### C# .NET Client:
```
csharp

public class MojApiClient
{
    private readonly HttpClient \_httpClient;
    
    public MojApiClient(string baseUrl, string accessToken \= null)
    {
        \_httpClient \= new HttpClient
        {
            BaseAddress \= new Uri(baseUrl)
        };
        
        if (!string.IsNullOrEmpty(accessToken))
        {
            \_httpClient.DefaultRequestHeaders.Authorization \= 
                new AuthenticationHeaderValue("Bearer", accessToken);
        }
    }
    
    public async Task<UserProfile\> GetProfileAsync()
    {
        var response \= await \_httpClient.GetAsync("/api/auth/profile");
        response.EnsureSuccessStatusCode();
        
        var content \= await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ApiResponse<UserProfile\>\>(content);
    }
}
```
### Python Client:
```
python

import requests

class MojApiClient:
    def \_\_init\_\_(self, base\_url, access\_token\=None):
        self.base\_url \= base\_url
        self.session \= requests.Session()
        
        if access\_token:
            self.session.headers.update({
                'Authorization': f'Bearer {access\_token}'
            })
    
    def get\_profile(self):
        response \= self.session.get(f'{self.base\_url}/api/auth/profile')
        response.raise\_for\_status()
        return response.json()
```
* * *

## 🔍 Monitoring & Analytics

### Health Dashboard:

-   API Status: `GET /health`
    
-   Database Status: `GET /health/database`
    
-   System Metrics: `GET /api/metrics` (Future)
    

### Logging Levels:
```
json

{
  "LogLevel": {
    "Default": "Information",
    "Microsoft.AspNetCore": "Warning",
    "MOJ.Modules.UserManagments": "Information"
  }
}
```
### Performance Metrics:

-   Response time tracking
    
-   Error rate monitoring
    
-   Request volume analytics
    
-   Database query performance
    

* * *

## 🚀 Production Checklist

-   Update JWT secret key
    
-   Configure production database
    
-   Set up HTTPS certificates