using Microsoft.AspNetCore.Mvc;
using WeeklyOSApi.BUSINESSLOGIC;
using WeeklyOSApi.Models;

namespace WeeklyOSApi.CONTROLLERS
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeeklyOSController : ControllerBase
    {
        private readonly WeeklyOSManager _manager;

        public WeeklyOSController(WeeklyOSManager manager)
        {
            _manager = manager;
        }

        /// <summary>
        /// POST api/WeeklyOS/GetWeeklyParameters
        /// Calls SP WF_OSWeeklyActivities_GetWeeklyParameters_ByOSRPEMid
        /// </summary>
        [HttpPost("GetWeeklyParameters")]
        public async Task<ActionResult<ApiResponse<WeeklyParametersResult>>> GetWeeklyParameters([FromBody] WeeklyParametersRequest request, CancellationToken ct)
        {
            var resp = await _manager.GetWeeklyParametersAsync(request, ct);
            if (!resp.Success) return BadRequest(resp);
            return Ok(resp);
        }

        /// <summary>
        /// GET api/WeeklyOS/GetWeeklyActivities?employeeId=1&fromDate=2025-09-01&toDate=2025-09-07&currentMonth=9
        /// Calls SP WF_OSWeeklyActivities_GetOSEmployeeSelfDeclaration
        /// </summary>
        [HttpGet("GetWeeklyActivities")]
        public async Task<ActionResult<ApiResponse<IEnumerable<WeeklyActivityRow>>>> GetWeeklyActivities(
            [FromQuery] int employeeId,
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate,
            [FromQuery] int currentMonth,
            CancellationToken ct)
        {

            var query = new WeeklyActivitiesQuery
            {
                EmployeeId = employeeId,
                FromDate = fromDate,
                ToDate = toDate,
                CurrentMonth = currentMonth
            };

            var resp = await _manager.GetWeeklyActivitiesAsync(query, ct);
            if (!resp.Success) return BadRequest(resp);
            return Ok(resp);
        }
        /// <summary>
        /// POST api/WeeklyOS/GetNewApiData
        /// Calls SP: WF_OSWeeklyActivities_GetNewApiData
        /// </summary>
        [HttpPost("GetNewApiData")]
        public async Task<ActionResult<ApiResponse<IEnumerable<NewApiResult>>>> GetNewApiData(
            [FromBody] NewApiRequest request,
            CancellationToken ct)
        {
            var resp = await _manager.GetNewApiDataAsync(request, ct);
            if (!resp.Success) return BadRequest(resp);
            return Ok(resp);
        }
        [HttpPost("SaveWeeklyActivities")]
        public async Task<ActionResult<ApiResponse<SaveWeeklyActivitiesResponse>>> SaveWeeklyActivities(
                   [FromBody] SaveWeeklyActivitiesRequest request,
                   CancellationToken ct)
        {
            var resp = await _manager.SaveWeeklyActivitiesAsync(request, ct);
            if (!resp.Success) return BadRequest(resp);
            return Ok(resp);
        }
        [HttpPost("ValidateImportData")]
        public async Task<ActionResult<ApiResponse<ImportValidationResult>>> ValidateImportData(
            [FromBody] ImportValidationRequest request,
            CancellationToken ct)
        {
            var resp = await _manager.ValidateImportDataAsync(request, ct);
            if (!resp.Success) return BadRequest(resp);
            return Ok(resp);
        }

        /// <summary>
        /// POST api/WeeklyOS/ImportExcelData
        /// Calls SP: WF_OSWeeklyActivities_Import_ChildDetails
        /// Imports weekly activities from Excel file
        /// </summary>
        [HttpPost("ImportExcelData")]
        public async Task<ActionResult<ApiResponse<ExcelImportResult>>> ImportExcelData(
            [FromForm] ExcelImportRequest request,
            CancellationToken ct)
        {
            var resp = await _manager.ImportExcelDataAsync(request, ct);
            if (!resp.Success) return BadRequest(resp);
            return Ok(resp);
        }
    }
}
