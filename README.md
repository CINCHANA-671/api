# WeeklyOS API

A .NET 8 Web API for managing weekly activities and reports.

## Project Structure

```
WeeklyOSApi/
├── CONTROLLERS/           # API Controllers
│   └── WeeklyOSController.cs
├── BUSINESSLOGIC/         # Business Logic Layer
│   └── WeeklyOSManager.cs
├── SERVICES/              # Data Service Layer
│   └── WeeklyOSService.cs
├── DATAACCESS/           # Database Connection
│   └── DbConnectionFactory.cs
├── Models/               # Data Models
│   └── WeeklyOSModels.cs
├── Properties/
│   └── launchSettings.json
├── appsettings.json      # Configuration
└── Program.cs            # Application Entry Point
```

## API Endpoints

### 1. Get Weekly Parameters
- **POST** `/api/WeeklyOS/GetWeeklyParameters`
- Gets weekly parameters by employee ID and master data

### 2. Get Weekly Activities  
- **GET** `/api/WeeklyOS/GetWeeklyActivities`
- Retrieves weekly activities for an employee

### 3. Get New API Data
- **POST** `/api/WeeklyOS/GetNewApiData` 
- Fetches new API data based on request parameters

### 4. Save Weekly Activities
- **POST** `/api/WeeklyOS/SaveWeeklyActivities`
- Saves weekly activity data

### 5. Validate Import Data
- **POST** `/api/WeeklyOS/ValidateImportData`
- Validates data before Excel import

### 6. Import Excel Data
- **POST** `/api/WeeklyOS/ImportExcelData`
- Imports weekly activities from Excel file

## Database Setup

Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=YOUR_DB;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=true;"
  }
}
```

## Running the Application

1. **Build the project:**
   ```bash
   dotnet build
   ```

2. **Run the application:**
   ```bash
   dotnet run
   ```

3. **Access Swagger UI** (in Development):
   - Navigate to `https://localhost:5001/swagger` or `http://localhost:5000/swagger`

## Dependencies

- **Microsoft.Data.SqlClient** - SQL Server data access
- **Dapper** - Micro ORM for database operations  
- **Swashbuckle.AspNetCore** - API documentation (Swagger)

## Architecture

The application follows a layered architecture:

1. **Controllers** - Handle HTTP requests and responses
2. **Business Logic** - Contains business rules and orchestration
3. **Services** - Data access and external service integration
4. **Data Access** - Database connection management
5. **Models** - Data transfer objects and domain models