using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using WeeklyOSApi.DATAACCESS;
using WeeklyOSApi.Models;
using System.Data.OleDb;

namespace WeeklyOSApi.SERVICES
{
    public class WeeklyOSService
    {
        private readonly DbConnectionFactory _factory;
        private readonly IConfiguration _config;

        public WeeklyOSService(DbConnectionFactory factory, IConfiguration config)
        {
            _factory = factory;
            _config = config;
        }

        /// <summary>
        /// Calls SP: WF_OSWeeklyActivities_GetWeeklyParameters_ByOSRPEMid
        /// Returns two result sets → MasterTable, ChildTable (as DataTables).
        /// </summary>
        public async Task<WeeklyParametersResult> GetWeeklyParametersAsync(WeeklyParametersRequest req, CancellationToken ct = default)
        {
            using var conn = (SqlConnection)_factory.Create();
            using var cmd = new SqlCommand("WF_OSWeeklyActivities_GetWeeklyParameters_ByOSRPEMid", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Param", req.Param);
            cmd.Parameters.AddWithValue("@RMEMpId", req.RMEmpId);
            cmd.Parameters.AddWithValue("@MasterId", req.MasterId);
            cmd.Parameters.AddWithValue("@InstanceId", req.InstanceId);

            await conn.OpenAsync(ct);

            // Fill a DataSet to preserve both tables without defining schema
            var ds = new DataSet();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            if (ds.Tables.Count > 0) ds.Tables[0].TableName = "MasterTable";
            if (ds.Tables.Count > 1) ds.Tables[1].TableName = "ChildTable";

            return new WeeklyParametersResult
            {
                MasterTable = ds.Tables.Count > 0 ? ds.Tables[0] : null,
                ChildTable = ds.Tables.Count > 1 ? ds.Tables[1] : null
            };
        }

        /// <summary>
        /// Example extra SP from your earlier notes:
        /// WF_OSWeeklyActivities_GetOSEmployeeSelfDeclaration
        /// </summary>
        public async Task<IEnumerable<WeeklyActivityRow>> GetWeeklyActivitiesAsync(WeeklyActivitiesQuery q, CancellationToken ct = default)
        {
            using var conn = _factory.Create();
            var result = await conn.QueryAsync<WeeklyActivityRow>(
                "WF_OSWeeklyActivities_GetOSEmployeeSelfDeclaration",
                new
                {
                    FromDate = q.FromDate,
                    ToDate = q.ToDate,
                    EmployeeId = q.EmployeeId,
                    CurrentMonth = q.CurrentMonth
                },
                commandType: CommandType.StoredProcedure);
            return result;
        }
        public async Task<IEnumerable<NewApiResult>> GetNewApiDataAsync(NewApiRequest req, CancellationToken ct = default)
        {
            using var conn = _factory.Create();
            var result = await conn.QueryAsync<NewApiResult>(
                "WF_OSWeeklyActivities_GetNewApiData", // Replace with your actual SP name
                new
                {
                    EmployeeId = req.EmployeeId,
                    FromDate = req.FromDate,
                    ToDate = req.ToDate,
                    Status = req.Status
                },
                commandType: CommandType.StoredProcedure);

            return result;
        }
        public async Task<int> SaveWeeklyActivitiesAsync(SaveWeeklyActivitiesRequest request, CancellationToken ct = default)
        {
            int totalRowsAffected = 0;

            using var conn = (SqlConnection)_factory.Create();
            await conn.OpenAsync(ct);

            using var transaction = conn.BeginTransaction();

            try
            {
                foreach (var activity in request.Activities)
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@Param", request.Param);
                    parameters.Add("@CID", activity.CID);
                    parameters.Add("@OSMID", activity.OSMID);
                    parameters.Add("@OSMEMPId", activity.OSMEMPId);
                    parameters.Add("@TaskDescription", activity.TaskDescription ?? (object)DBNull.Value);
                    parameters.Add("@InstanceId", request.InstanceId);
                    parameters.Add("@wStatus", activity.wStatus);
                    parameters.Add("@Continue", activity.Continue ?? (object)DBNull.Value);
                    parameters.Add("@LeaveUtilized", activity.LeaveUtilized);
                    parameters.Add("@ExecutSize", activity.ExecutSize ?? (object)DBNull.Value);
                    parameters.Add("@Remarks", activity.Remarks ?? (object)DBNull.Value);

                    var rowsAffected = await conn.ExecuteAsync(
                        "Wr_OSWeeklyActivities_Update_ChildDetails",
                        parameters,
                        transaction: transaction,
                        commandType: CommandType.StoredProcedure
                    );

                    totalRowsAffected += rowsAffected;
                }

                transaction.Commit();
                return totalRowsAffected;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        
        public async Task<ImportValidationResult> ValidateImportMonthYearAsync(ImportValidationRequest request, CancellationToken ct = default)
        {
            using var conn = _factory.Create();
            var result = await conn.QueryFirstOrDefaultAsync<ImportValidationResult>(
                "WF_OSWeeklyActivities_ImportCheckMonthOsEmployes",
                new
                {
                    RWMEmpId = request.RWMEmpId,
                    MasterId = request.MasterId,
                    InstanceId = request.InstanceId
                },
                commandType: CommandType.StoredProcedure);

            return result ?? new ImportValidationResult { IsValid = false };
        }

        /// <summary>
        /// Calls SP: WF_OSWeeklyActivities_Import_ChildDetails
        /// Imports data from Excel DataTable
        /// </summary>
        public async Task<int> ImportExcelDataAsync(DataTable excelData, CancellationToken ct = default)
        {
            using var conn = (SqlConnection)_factory.Create();
            await conn.OpenAsync(ct);

            var parameters = new DynamicParameters();
            parameters.Add("@UserDefineTable", excelData.AsTableValuedParameter("dbo.WeeklyActivitiesImportType"));

            var rowsAffected = await conn.ExecuteAsync(
                "WF_OSWeeklyActivities_Import_ChildDetails",
                parameters,
                commandType: CommandType.StoredProcedure);

            return rowsAffected;
        }

        /// <summary>
        /// Reads Excel file and converts to DataTable
        /// </summary>
        public DataTable ReadExcelFile(IFormFile excelFile)
        {
            var dtImportExcel = new DataTable();
            var extension = Path.GetExtension(excelFile.FileName).ToLower();
            var conStr = "";

            switch (extension)
            {
                case ".xls": // Excel 97-03
                    conStr = _config.GetConnectionString("Excel03ConString") ?? 
                        "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR={1}'";
                    break;
                case ".xlsx": // Excel 07+
                    conStr = _config.GetConnectionString("Excel07ConString") ?? 
                        "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 12.0;HDR={1}'";
                    break;
                default:
                    throw new ArgumentException("Invalid file format. Only .xls and .xlsx are supported.");
            }

            // Create temp file
            var tempFilePath = Path.GetTempFileName() + extension;
            using (var stream = new FileStream(tempFilePath, FileMode.Create))
            {
                excelFile.CopyTo(stream);
            }

            try
            {
                conStr = string.Format(conStr, tempFilePath, "YES");
                
                using (var connExcel = new OleDbConnection(conStr))
                using (var cmdExcel = new OleDbCommand())
                using (var oda = new OleDbDataAdapter())
                {
                    cmdExcel.Connection = connExcel;
                    connExcel.Open();

                    var dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    if (dtExcelSchema == null || dtExcelSchema.Rows.Count == 0)
                        throw new Exception("No sheets found in Excel file.");

                    var sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                    cmdExcel.CommandText = "SELECT * FROM [" + sheetName + "]";
                    
                    oda.SelectCommand = cmdExcel;
                    oda.Fill(dtImportExcel);
                }

                return dtImportExcel;
            }
            finally
            {
                // Clean up temp file
                if (File.Exists(tempFilePath))
                    File.Delete(tempFilePath);
            }
        }

        /// <summary>
        /// Validates LeaveUtilized values (1-5)
        /// </summary>
        public bool ValidateLeaveUtilized(DataTable excelData)
        {
            foreach (DataRow row in excelData.Rows)
            {
                if (row["LeaveUtilized"] != DBNull.Value && !string.IsNullOrEmpty(row["LeaveUtilized"].ToString()))
                {
                    var leaveUtilizationVal = row["LeaveUtilized"].ToString();
                    if (!int.TryParse(leaveUtilizationVal, out int leaveValue) || leaveValue < 1 || leaveValue > 5)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }


    
}
