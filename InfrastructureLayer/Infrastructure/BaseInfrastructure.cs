using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using AuctionPortal.Common.Infrastructure;
using AuctionPortal.InfrastructureLayer.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AuctionPortal.InfrastructureLayer.Infrastructure
{
    /// <summary>
    /// Template-style base infrastructure that exposes ADO.NET helpers used by
    /// your UserInfrastructure (ExecuteReader/ExecuteNonQuery/ExecuteScalar, GetParameter*).
    /// </summary>
    public abstract class BaseInfrastructure
    {
        #region Constructor
        protected BaseInfrastructure(IConfiguration configuration, ILogger logger)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        #endregion

        #region Common column/param names
        protected const string CreatedByIdColumnName = "CreatedById";
        protected const string CreatedByNameColumnName = "CreatedByName";
        protected const string CreatedDateColumnName = "CreatedDate";
        protected const string ModifiedByIdColumnName = "ModifiedById";
        protected const string ModifiedByNameColumnName = "ModifiedByName";
        protected const string ModifiedDateColumnName = "ModifiedDate";
        protected const string ActiveColumnName = "Active";
        protected const string IsDeletedColumnName = "IsDeleted";
        protected const string ActiveForCustomerColumnName = "ActiveForCustomer";
        protected const string UnReadCountColumnName = "UnReadCount";

        protected const string CurrentUserIdParameterName = "PCurrentUserId";
        protected const string AgentIdParameterName = "PAgentId";
        protected const string IsActiveParameterName = "PActive";
        protected const string OffsetParameterName = "POffset";
        protected const string PageSizeParameterName = "PPageSize";
        protected const string TotalRecordParameterName = "PTotalRecord";
        protected const string SortColumnParameterName = "PSortColumn";
        protected const string SortAscendingParameterName = "PSortAscending";
        protected const string FilterColumnIdParameterName = "PFilterColumnId";
        protected const string ActiveParameterName = "PActive";
        protected const string ActiveForCustomerParameterName = "PActiveForCustomer";
        protected const string NotificationCodeParamaterName = "PNotificationCode";
        protected const string SearchTextParameterName = "PSearchText";
        protected const string CheckSFCFlag = "PCheckSFCFlag";
        protected const string EmailAddress = "Email";
        #endregion

        /// <summary>Connection string value.</summary>
        public string ConnectionString { get; set; } = string.Empty;

        /// <summary>App configuration.</summary>
        protected IConfiguration Configuration { get; }

        /// <summary>Logger (optional usage).</summary>
        protected ILogger Logger { get; }

        #region Private helpers
        private DbConnection GetConnection()
        {
            ConnectionString = Configuration.GetConnectionString("DefaultConnection")
                               ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            var connection = new SqlConnection(ConnectionString);
            if (connection.State != ConnectionState.Open)
                connection.Open();

            return connection;
        }

        private static DbCommand GetCommand(DbConnection connection, string commandText, CommandType commandType, List<DbParameter> parameters)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = commandText;
            cmd.CommandType = commandType;

            if (parameters is { Count: > 0 })
                cmd.Parameters.AddRange(parameters.ToArray());

            return cmd;
        }

        private DatabaseException GetException(string className, string methodName, Exception ex, List<DbParameter> parameters, string commandText, CommandType commandType)
        {
            var message = $"Failed in {className}.{methodName}. {ex.Message}";
            return new DatabaseException(message, ex, parameters, commandText, commandType);
        }
        #endregion

        #region Parameter builders
        protected DbParameter GetParameter(string name, object? value)
        {
            var p = new SqlParameter(name, value ?? DBNull.Value) { Direction = ParameterDirection.Input };
            return p;
        }

        protected DbParameter GetParameterOut(string name, SqlDbType type, object? value = null, ParameterDirection direction = ParameterDirection.InputOutput)
        {
            var p = new SqlParameter(name, type)
            {
                Direction = direction,
                Value = value ?? DBNull.Value
            };

            // Handle size for string-like types to avoid truncation.
            if (type is SqlDbType.NVarChar or SqlDbType.VarChar or SqlDbType.NText or SqlDbType.Text)
                p.Size = -1;

            return p;
        }
        #endregion

        #region Execute APIs (template-compatible)
        protected async Task<int> ExecuteNonQuery(List<DbParameter> parameters, string commandText, CommandType commandType = CommandType.StoredProcedure)
        {
            var returnValue = -1;
            try
            {
                using var connection = GetConnection();
                using var cmd = GetCommand(connection, commandText, commandType, parameters);
                cmd.CommandTimeout = 0;
                returnValue = await cmd.ExecuteNonQueryAsync();
                if (connection.State == ConnectionState.Open) connection.Close();
            }
            catch (Exception ex)
            {
                var dbEx = GetException(GetType().FullName!, nameof(ExecuteNonQuery), ex, parameters, commandText, commandType);
                throw dbEx;
            }
            return returnValue;
        }

        protected async Task<object?> ExecuteScalar(List<DbParameter> parameters, string commandText, CommandType commandType = CommandType.StoredProcedure)
        {
            object? returnValue = null;
            try
            {
                using var connection = GetConnection();
                using var cmd = GetCommand(connection, commandText, commandType, parameters);
                returnValue = await cmd.ExecuteScalarAsync();
                if (connection.State == ConnectionState.Open) connection.Close();
            }
            catch (Exception ex)
            {
                var dbEx = GetException(GetType().FullName!, nameof(ExecuteScalar), ex, parameters, commandText, commandType);
                throw dbEx;
            }
            return returnValue;
        }

        protected async Task<DbDataReader> ExecuteReader(List<DbParameter> parameters, string commandText, CommandType commandType = CommandType.StoredProcedure)
        {
            try
            {
                var connection = GetConnection(); // intentionally not disposed until reader is closed
                var cmd = GetCommand(connection, commandText, commandType, parameters);
                cmd.CommandTimeout = 0;
                return await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                var dbEx = GetException(GetType().FullName!, nameof(ExecuteReader), ex, parameters, commandText, commandType);
                throw dbEx;
            }
        }
        #endregion
    }
}
