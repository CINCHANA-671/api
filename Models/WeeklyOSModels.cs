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
        public int RMEmpId { get; set; }
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
}