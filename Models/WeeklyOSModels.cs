namespace WeeklyOSApi.Models
{
    // === Request DTOs ===
    public sealed class WeeklyParametersRequest
    {
        // "Report" or "RMApprove"
        public string Param { get; set; } = "Report";
        public int RMEmpId { get; set; }
        public long MasterId { get; set; }
        public int InstanceId { get; set; }
    }

    public sealed class WeeklyActivitiesQuery
    {
        public int EmployeeId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int CurrentMonth { get; set; }
    }

    // === Shape used by Dapper for GetWeeklyActivities (if you need it) ===
    public sealed class WeeklyActivityRow
    {
        public int EmployeeId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int CurrentMonth { get; set; }
        public string? Activities { get; set; }
    }

    // === Generic response ===
    public sealed class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public T? Data { get; set; }
    }

    // A POCO that carries the two SP result tables
    public sealed class WeeklyParametersResult
    {
        // Use object to keep DataTable payloads without a hard schema
        public object? MasterTable { get; set; }
        public object? ChildTable { get; set; }
    }
    // === Request DTO ===
    public sealed class NewApiRequest
    {
        public int EmployeeId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int Status { get; set; }  // Example extra filter if needed
    }

    // === Response DTO ===
    public sealed class NewApiResult
    {
        public int EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? Department { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedOn { get; set; }
    }
    public sealed class SaveWeeklyActivitiesRequest
    {
        public int Param { get; set; } = 1; // Default to 1 for "Save as Draft"
        public List<WeeklyActivityItem> Activities { get; set; } = new List<WeeklyActivityItem>();
        public int InstanceId { get; set; }
    }

    public sealed class WeeklyActivityItem
    {
        public int CID { get; set; } // Row ID from DB
        public int OSMID { get; set; } // OS Week ID
        public int OSMEMPId { get; set; } // OS MEMPID
        public string? TaskDescription { get; set; }
        public int wStatus { get; set; } = 1; // Default to 1
        public string? Continue { get; set; }
        public int LeaveUtilized { get; set; } = 0; // Default to 0
        public string? ExecutSize { get; set; }
        public string? Remarks { get; set; }
    }

    // === Response DTO for save operation ===
    public sealed class SaveWeeklyActivitiesResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public int RowsAffected { get; set; }
    }
    public sealed class ImportValidationRequest
    {
        public int RWMEmpId { get; set; }
        public long MasterId { get; set; }
        public int InstanceId { get; set; }
    }

    public sealed class ImportValidationResult
    {
        public string MonthValue { get; set; } = "";
        public string YearValue { get; set; } = "";
        public bool IsValid { get; set; }
    }

    // === New Request DTO for Excel import ===
    public sealed class ExcelImportRequest
    {
        public int RWMEmpId { get; set; }
        public long MasterId { get; set; }
        public int InstanceId { get; set; }
        public IFormFile ExcelFile { get; set; } = null!;
    }

    public sealed class ExcelImportResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public int RowsImported { get; set; }
    }

    // === DTO for Excel data row ===
    public sealed class ExcelDataRow
    {
        public string? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? Week { get; set; }
        public string? TaskDescription { get; set; }
        public string? LeaveUtilized { get; set; }
        public string? Remarks { get; set; }
        public string? MonthValue { get; set; }
        public string? YearValue { get; set; }
    }

}
