using System.Data;
using Microsoft.Data.SqlClient;

namespace WeeklyOSApi.DATAACCESS
{
    public class DbConnectionFactory
    {
        private readonly IConfiguration _config;

        public DbConnectionFactory(IConfiguration config)
        {
            _config = config;
        }

        public IDbConnection CreateConnection() 
            => new SqlConnection(_config.GetConnectionString("DefaultConnection"));
    }
}