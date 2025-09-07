using WeeklyOSApi.DATAACCESS;
using WeeklyOSApi.Models;
using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;

namespace WeeklyOSApi.SERVICES
{
    public class WeeklyOSService
    {
        private readonly DbConnectionFactory _factory;

        public WeeklyOSService(DbConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task<WeeklyParametersResult> GetWeeklyParametersAsync(WeeklyParametersRequest req, CancellationToken ct = default)
        {
            using var conn = (SqlConnection)_factory.CreateConnection();
            
            // CORRECTED SP NAME: WF_OSWeeklyActivities_GetWeeklyParameters_ByOSRPEMId
            using var cmd = new SqlCommand("WF_OSWeeklyActivities_GetWeeklyParameters_ByOSRPEMId", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            // CORRECT ORDER as per SP: @RMMEmpId, @MasterId, @Param, @InstanceId
            cmd.Parameters.AddWithValue("@RMMEmpId", req.RMMEmpId);
            cmd.Parameters.AddWithValue("@MasterId", req.MasterId);
            cmd.Parameters.AddWithValue("@Param", req.Param);
            cmd.Parameters.AddWithValue("@InstanceId", req.InstanceId);

            await conn.OpenAsync(ct);

            var ds = new DataSet();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            // Name the tables as expected
            if (ds.Tables.Count > 0) ds.Tables[0].TableName = "MasterTable";
            if (ds.Tables.Count > 1) ds.Tables[1].TableName = "ChildTable";

            return new WeeklyParametersResult
            {
                MasterTable = ds.Tables.Count > 0 ? ds.Tables[0] : null,
                ChildTable = ds.Tables.Count > 1 ? ds.Tables[1] : null
            };
        }

        public async Task<IEnumerable<WeeklyActivityRow>> GetWeeklyActivitiesAsync(WeeklyActivitiesQuery q, CancellationToken ct = default)
        {
            using var conn = _factory.CreateConnection();
            
            var result = await conn.QueryAsync<WeeklyActivityRow>(
                "WF_OSWeeklyActivities_GetOSEmployeeSelfDeclaration",
                new
                {
                    EmployeeId = q.EmployeeId,
                    FromDate = q.FromDate,
                    ToDate = q.ToDate,
                    CurrentMonth = q.CurrentMonth
                },
                commandType: CommandType.StoredProcedure
            );
            
            return result;
        }

        public async Task<IEnumerable<NewApiResult>> GetNewApiDataAsync(NewApiRequest req, CancellationToken ct = default)
        {
            using var conn = _factory.CreateConnection();
            
            var result = await conn.QueryAsync<NewApiResult>(
                "WF_OSWeeklyActivities_GetNewApiData",
                new
                {
                    EmployeeId = req.EmployeeId,
                    MasterId = req.MasterId,
                    InstanceId = req.InstanceId
                },
                commandType: CommandType.StoredProcedure
            );
            
            return result;
        }

        public async Task<int> SaveWeeklyActivitiesAsync(SaveWeeklyActivitiesRequest req, CancellationToken ct = default)
        {
            using var conn = _factory.CreateConnection();
            
            var result = await conn.ExecuteAsync(
                "WF_OSWeeklyActivities_Save",
                new
                {
                    EmployeeId = req.EmployeeId,
                    FromDate = req.FromDate,
                    ToDate = req.ToDate,
                    Activities = string.Join(",", req.Activities.Select(a => a.Activity))
                },
                commandType: CommandType.StoredProcedure
            );
            
            return result;
        }

        public async Task<ImportValidationResult> ValidateImportMonthYearAsync(ImportValidationRequest req, CancellationToken ct = default)
        {
            using var conn = _factory.CreateConnection();
            
            var result = await conn.QueryFirstOrDefaultAsync<ImportValidationResult>(
                "WF_OSWeeklyActivities_ValidateImportMonthYear",
                new
                {
                    RWMEmpId = req.RWMEmpId,
                    MasterId = req.MasterId,
                    InstanceId = req.InstanceId
                },
                commandType: CommandType.StoredProcedure
            );
            
            return result ?? new ImportValidationResult { IsValid = false, Message = "Validation failed" };
        }

        public DataTable ReadExcelFile(IFormFile excelFile)
        {
            // Placeholder implementation for Excel reading
            // In a real implementation, you would use a library like EPPlus or ClosedXML
            var dataTable = new DataTable();
            dataTable.Columns.Add("NoData");
            
            if (excelFile == null || excelFile.Length == 0)
            {
                var row = dataTable.NewRow();
                row["NoData"] = "NO data available for respective WN.";
                dataTable.Rows.Add(row);
            }
            
            return dataTable;
        }

        public bool ValidateLeaveUtilized(DataTable excelData)
        {
            // Placeholder validation for Leave Utilized values
            // In a real implementation, you would validate the actual Excel data
            if (excelData.Columns.Contains("LeaveUtilized"))
            {
                foreach (DataRow row in excelData.Rows)
                {
                    if (row["LeaveUtilized"] != null && 
                        decimal.TryParse(row["LeaveUtilized"].ToString(), out decimal value))
                    {
                        if (value < 1 || value > 5)
                            return false;
                    }
                }
            }
            return true;
        }

        public async Task<int> ImportExcelDataAsync(DataTable excelData, CancellationToken ct = default)
        {
            using var conn = _factory.CreateConnection();
            
            // Placeholder implementation for Excel data import
            var result = await conn.ExecuteAsync(
                "WF_OSWeeklyActivities_Import_ChildDetails",
                new
                {
                    ExcelData = excelData.Rows.Count
                },
                commandType: CommandType.StoredProcedure
            );
            
            return result;
        }
    }
}