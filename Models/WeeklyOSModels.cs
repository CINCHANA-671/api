namespace WeeklyOSApi.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public T? Data { get; set; }
    }

    public class WeeklyParametersRequest
    {
        public string Param { get; set; } = "Report";
        public int RMMEmpId { get; set; }
        public long MasterId { get; set; }
        public int InstanceId { get; set; }
    }

    public class WeeklyParametersResult
    {
        public object? MasterTable { get; set; }
        public object? ChildTable { get; set; }
    }

    public class WeeklyActivityRow
    {
        public int EmployeeId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int CurrentMonth { get; set; }
        public string? Activities { get; set; }
    }

    public class WeeklyActivitiesQuery
    {
        public int EmployeeId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int CurrentMonth { get; set; }
    }

    // New API related models
    public class NewApiRequest
    {
        public int EmployeeId { get; set; }
        public int MasterId { get; set; }
        public int InstanceId { get; set; }
    }

    public class NewApiResult
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Value { get; set; } = "";
        public DateTime CreatedDate { get; set; }
    }

    // Save Weekly Activities related models
    public class SaveWeeklyActivitiesRequest
    {
        public int EmployeeId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<WeeklyActivityItem> Activities { get; set; } = new();
    }

    public class WeeklyActivityItem
    {
        public string Activity { get; set; } = "";
        public decimal Hours { get; set; }
        public DateTime Date { get; set; }
    }

    public class SaveWeeklyActivitiesResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public int RowsAffected { get; set; }
    }

    // Import validation related models
    public class ImportValidationRequest
    {
        public int RWMEmpId { get; set; }
        public int MasterId { get; set; }
        public int InstanceId { get; set; }
    }

    public class ImportValidationResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = "";
        public int Month { get; set; }
        public int Year { get; set; }
    }

    // Excel import related models
    public class ExcelImportRequest
    {
        public int RWMEmpId { get; set; }
        public int MasterId { get; set; }
        public int InstanceId { get; set; }
        public IFormFile ExcelFile { get; set; } = null!;
    }

    public class ExcelImportResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public int RowsImported { get; set; }
    }
}