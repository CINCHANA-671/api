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

        public async Task<WeeklyParametersResult> GetWeeklyParametersAsync(WeeklyParametersRequest req)
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

            await conn.OpenAsync();

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

        public async Task<IEnumerable<WeeklyActivityRow>> GetWeeklyActivitiesAsync(WeeklyActivitiesQuery q)
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
    }
}