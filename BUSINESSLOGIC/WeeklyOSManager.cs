using WeeklyOSApi.Models;
using WeeklyOSApi.SERVICES;

namespace WeeklyOSApi.BUSINESSLOGIC
{
    public class WeeklyOSManager
    {
        private readonly WeeklyOSService _service;

        public WeeklyOSManager(WeeklyOSService service)
        {
            _service = service;
        }

        public async Task<ApiResponse<WeeklyParametersResult>> GetWeeklyParametersAsync(WeeklyParametersRequest req, CancellationToken ct = default)
        {
            try
            {
                var data = await _service.GetWeeklyParametersAsync(req, ct);
                return new ApiResponse<WeeklyParametersResult>
                {
                    Success = true,
                    Message = "Parameters fetched.",
                    Data = data
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<WeeklyParametersResult>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponse<IEnumerable<WeeklyActivityRow>>> GetWeeklyActivitiesAsync(WeeklyActivitiesQuery q, CancellationToken ct = default)
        {
            try
            {
                var data = await _service.GetWeeklyActivitiesAsync(q, ct);
                return new ApiResponse<IEnumerable<WeeklyActivityRow>>
                {
                    Success = true,
                    Message = "Activities fetched.",
                    Data = data
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<WeeklyActivityRow>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
        public async Task<ApiResponse<IEnumerable<NewApiResult>>> GetNewApiDataAsync(NewApiRequest req, CancellationToken ct = default)
        {
            try
            {
                var data = await _service.GetNewApiDataAsync(req, ct);
                return new ApiResponse<IEnumerable<NewApiResult>>
                {
                    Success = true,
                    Message = "Data fetched successfully.",
                    Data = data
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<NewApiResult>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
public async Task<ApiResponse<SaveWeeklyActivitiesResponse>> SaveWeeklyActivitiesAsync(SaveWeeklyActivitiesRequest req, CancellationToken ct = default)
        {
            try
            {
                var rowsAffected = await _service.SaveWeeklyActivitiesAsync(req, ct);
                return new ApiResponse<SaveWeeklyActivitiesResponse>
                {
                    Success = true,
                    Message = "Weekly activities saved successfully.",
                    Data = new SaveWeeklyActivitiesResponse
                    {
                        Success = true,
                        Message = "Save operation completed",
                        RowsAffected = rowsAffected
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<SaveWeeklyActivitiesResponse>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = new SaveWeeklyActivitiesResponse
                    {
                        Success = false,
                        Message = ex.Message,
                        RowsAffected = 0
                    }
                };
            }
        }

         public async Task<ApiResponse<ImportValidationResult>> ValidateImportDataAsync(ImportValidationRequest req, CancellationToken ct = default)
        {
            try
            {
                var result = await _service.ValidateImportMonthYearAsync(req, ct);
                return new ApiResponse<ImportValidationResult>
                {
                    Success = true,
                    Message = "Validation completed.",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<ImportValidationResult>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponse<ExcelImportResult>> ImportExcelDataAsync(ExcelImportRequest req, CancellationToken ct = default)
        {
            try
            {
                // First validate month/year
                var validationRequest = new ImportValidationRequest
                {
                    RWMEmpId = req.RWMEmpId,
                    MasterId = req.MasterId,
                    InstanceId = req.InstanceId
                };

                var validationResult = await _service.ValidateImportMonthYearAsync(validationRequest, ct);
                
                if (!validationResult.IsValid)
                {
                    return new ApiResponse<ExcelImportResult>
                    {
                        Success = false,
                        Message = "Month/Year validation failed.",
                        Data = new ExcelImportResult { Success = false, Message = "Invalid month/year in database." }
                    };
                }

                // Read Excel file
                var excelData = _service.ReadExcelFile(req.ExcelFile);

                // Check if Excel has data
                if (excelData.Rows.Count == 0 || 
                    (excelData.Rows.Count > 0 && excelData.Rows[0]["NoData"].ToString() == "NO data available for respective WN."))
                {
                    return new ApiResponse<ExcelImportResult>
                    {
                        Success = false,
                        Message = "No data available in Excel file.",
                        Data = new ExcelImportResult { Success = false, Message = "No data available in Excel file." }
                    };
                }

                // Validate LeaveUtilized values
                if (!_service.ValidateLeaveUtilized(excelData))
                {
                    return new ApiResponse<ExcelImportResult>
                    {
                        Success = false,
                        Message = "Leave Utilized values should be numeric and between 1 to 5.",
                        Data = new ExcelImportResult { Success = false, Message = "Invalid Leave Utilized values." }
                    };
                }

                // Import data
                var rowsImported = await _service.ImportExcelDataAsync(excelData, ct);

                return new ApiResponse<ExcelImportResult>
                {
                    Success = true,
                    Message = "Data imported successfully.",
                    Data = new ExcelImportResult 
                    { 
                        Success = true, 
                        Message = "Data imported successfully.", 
                        RowsImported = rowsImported 
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<ExcelImportResult>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = new ExcelImportResult { Success = false, Message = ex.Message }
                };
            }
        }
    }
}
