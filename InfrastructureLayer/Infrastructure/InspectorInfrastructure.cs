using AuctionPortal.Common.Infrastructure;
using AuctionPortal.InfrastructureLayer.Interfaces;
using AuctionPortal.Models;
using System.Data;
using System.Data.Common;
using System.Reflection.PortableExecutable;

namespace AuctionPortal.InfrastructureLayer.Infrastructure
{
    public class InspectorInfrastructure : BaseInfrastructure, IInspectorInfrastructure
    {
        #region Constructor
        /// <summary>
        /// InspectorInfrastructure initializes class object.
        /// </summary>
        public InspectorInfrastructure(IConfiguration configuration, ILogger<InspectorInfrastructure> logger)
            : base(configuration, logger)
        {
        }
        #endregion

        #region Constants
        // Stored procedure names
        private const string AddStoredProcedureName = "[dbo].[sp_Inspector_Insert]";
        private const string ActivateStoredProcedureName = "[dbo].[sp_Inspector_Activate]";
        private const string GetStoredProcedureName = "[dbo].[sp_Inspector_GetById]";
        private const string GetListStoredProcedureName = "[dbo].[sp_Inspector_GetAll]";
        private const string UpdateStoredProcedureName = "[dbo].[sp_Inspector_Update]";
        private const string GetStatsStoredProcedureName = "[dbo].[sp_Inspector_GetStats]";
        // Column names
        private const string UserIdColumnName = "UserId";
        private const string UserNameColumnName = "UserName";
        private const string FirstNameColumnName = "FirstName";
        private const string LastNameColumnName = "LastName";
        private const string IdentificationNumberColumnName = "IdentificationNumber";
        private const string Address1ColumnName = "Address1";
        private const string PostalCodeColumnName = "PostalCode";
        private const string EmailColumnName = "Email";
        private const string EmailConfirmedColumnName = "EmailConfirmed";
        private const string PasswordHashColumnName = "PasswordHash";
        private const string SecurityStampColumnName = "SecurityStamp";
        private const string PhoneNumberColumnName = "PhoneNumber";
        private const string LoginDateColumnName = "LoginDate";
        private const string CodeColumnName = "Code";
        private const string RoleIdColumnName = "RoleId";

        // Parameter names (match your SP params)
        private const string UserIdParameterName = "@UserId";
        private const string UserNameParameterName = "@UserName";
        private const string FirstNameParameterName = "@FirstName";
        private const string LastNameParameterName = "@LastName";
        private const string IdentificationNumberParameterName = "@IdentificationNumber";
        private const string Address1ParameterName = "@Address1";
        private const string PostalCodeParameterName = "@PostalCode";
        private const string EmailParameterName = "@Email";
        private const string EmailConfirmedParameterName = "@EmailConfirmed";
        private const string PasswordHashParameterName = "@PasswordHash";
        private const string SecurityStampParameterName = "@SecurityStamp";
        private const string PhoneNumberParameterName = "@PhoneNumber";
        private const string LoginDateParameterName = "@LoginDate";   
        private const string CodeParameterName = "@Code";
        private const string CreatedByIdParameterName = "@CreatedById";
        private const string ModifiedByIdParameterName = "@ModifiedById";
        private const string RoleIdParameterName = "@RoleId";          
        private const string ReplaceRolesParameterName = "@ReplaceRoles";
        #endregion
        #region IInspectorInfrastructure Implementation

        /// <summary>
        /// Add adds new Inspector and returns generated UserId.
        /// </summary>
        public async Task<int> Add(Inspector Inspector)
        {
            var InspectorIdParam = base.GetParameterOut(InspectorInfrastructure.UserIdParameterName, SqlDbType.Int, Inspector.UserId);

            var parameters = new List<DbParameter>
            {
                InspectorIdParam,
                base.GetParameter(InspectorInfrastructure.UserNameParameterName,             Inspector.UserName),
                base.GetParameter(InspectorInfrastructure.FirstNameParameterName,            Inspector.FirstName),
                base.GetParameter(InspectorInfrastructure.LastNameParameterName,             Inspector.LastName),
                base.GetParameter(InspectorInfrastructure.IdentificationNumberParameterName, Inspector.IdentificationNumber),
                base.GetParameter(InspectorInfrastructure.Address1ParameterName,             Inspector.Address1),
                base.GetParameter(InspectorInfrastructure.PostalCodeParameterName,           Inspector.PostalCode),
                base.GetParameter(InspectorInfrastructure.EmailParameterName,                Inspector.Email),
                base.GetParameter(InspectorInfrastructure.EmailConfirmedParameterName,       Inspector.EmailConfirmed),
                base.GetParameter(InspectorInfrastructure.PasswordHashParameterName,         Inspector.PasswordHash),
                base.GetParameter(InspectorInfrastructure.SecurityStampParameterName,        Inspector.SecurityStamp),
                base.GetParameter(InspectorInfrastructure.PhoneNumberParameterName,          Inspector.PhoneNumber),
                base.GetParameter(InspectorInfrastructure.LoginDateParameterName,            Inspector.LoginDate),
                base.GetParameter(InspectorInfrastructure.CodeParameterName,                 Inspector.Code),
                base.GetParameter(InspectorInfrastructure.CreatedByIdParameterName,          Inspector.CreatedById)
            };

            await base.ExecuteNonQuery(parameters, InspectorInfrastructure.AddStoredProcedureName, CommandType.StoredProcedure);

            Inspector.UserId = Convert.ToInt32(InspectorIdParam.Value);
            return Inspector.UserId;
        }

        /// <summary>
        /// Activate activate/deactivate provided record and returns true if successful.
        /// </summary>
        public async Task<bool> Activate(Inspector Inspector)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(InspectorInfrastructure.UserIdParameterName, Inspector.UserId),
                base.GetParameter(BaseInfrastructure.ActiveParameterName, Inspector.Active),
                base.GetParameter(InspectorInfrastructure.ModifiedByIdParameterName, Inspector.ModifiedById)
            };

            var rows = await base.ExecuteNonQuery(parameters, InspectorInfrastructure.ActivateStoredProcedureName, CommandType.StoredProcedure);
            return rows > 0;
        }

        /// <summary>
        /// Get fetches and returns a single Inspector.
        /// </summary>
        public async Task<Inspector> Get(Inspector Inspector)
        {
            Inspector InspectorItem = null;
            var parameters = new List<DbParameter>
    {
        base.GetParameter(UserIdParameterName, Inspector.UserId)
    };

            using (var dr = await base.ExecuteReader(parameters, GetStoredProcedureName, CommandType.StoredProcedure))
            {
                if (dr != null)
                {
                    // first result set: Inspector
                    if (dr.Read())
                    {
                        InspectorItem = new Inspector
                        {
                            UserId = dr.GetIntegerValue(UserIdColumnName),
                            UserName = dr.GetStringValue(UserNameColumnName),
                            FirstName = dr.GetStringValue(FirstNameColumnName),
                            LastName = dr.GetStringValue(LastNameColumnName),
                            IdentificationNumber = dr.GetStringValue(IdentificationNumberColumnName),
                            Address1 = dr.GetStringValue(Address1ColumnName),
                            PostalCode = dr.GetStringValue(PostalCodeColumnName),
                            Email = dr.GetStringValue(EmailColumnName),
                            EmailConfirmed = dr.GetBooleanValue(EmailConfirmedColumnName),
                            PasswordHash = dr.GetStringValue(PasswordHashColumnName),
                            SecurityStamp = dr.GetStringValue(SecurityStampColumnName),
                            PhoneNumber = dr.GetStringValue(PhoneNumberColumnName),
                            LoginDate = dr.GetDateTimeValueNullable(LoginDateColumnName),
                            Code = dr.GetStringValue(CodeColumnName),
                            CreatedById = dr.GetIntegerValueNullable(BaseInfrastructure.CreatedByIdColumnName),
                            CreatedDate = dr.GetDateTimeValueNullable(BaseInfrastructure.CreatedDateColumnName),
                            ModifiedById = dr.GetIntegerValueNullable(BaseInfrastructure.ModifiedByIdColumnName) ?? 0,
                            ModifiedDate = dr.GetDateTimeValueNullable(BaseInfrastructure.ModifiedDateColumnName),
                            Active = dr.GetBooleanValue(BaseInfrastructure.ActiveColumnName)
                        };
                    }

                    if (!dr.IsClosed) dr.Close();
                }
            }

            return InspectorItem;
        }


        /// <summary>
        /// GetList fetches and returns a list of Inspectors (trimmed columns).
        /// </summary>
        public async Task<List<Inspector>> GetList(Inspector Inspector)
        {
            var Inspectors = new List<Inspector>();
            var parameters = new List<DbParameter>(); // add filters from 'Inspector' if needed

            using (var dataReader = await base.ExecuteReader(parameters, InspectorInfrastructure.GetListStoredProcedureName, CommandType.StoredProcedure))
            {
                if (dataReader != null)
                {
                    while (dataReader.Read())
                    {
                        var item = new Inspector
                        {
                            UserId = dataReader.GetIntegerValue(InspectorInfrastructure.UserIdColumnName),
                            UserName = dataReader.GetStringValue(InspectorInfrastructure.UserNameColumnName),
                            FirstName = dataReader.GetStringValue(InspectorInfrastructure.FirstNameColumnName),
                            Email = dataReader.GetStringValue(InspectorInfrastructure.EmailColumnName),
                            EmailConfirmed = dataReader.GetBooleanValue(InspectorInfrastructure.EmailConfirmedColumnName), // NEW
                            LoginDate = dataReader.GetDateTimeValueNullable(InspectorInfrastructure.LoginDateColumnName), // NEW
                            Active = dataReader.GetBooleanValue(BaseInfrastructure.ActiveColumnName),         // NEW
                            CreatedDate = dataReader.GetDateTimeValueNullable(BaseInfrastructure.CreatedDateColumnName) // optional
                        };

                        Inspectors.Add(item);
                    }

                    if (!dataReader.IsClosed)
                        dataReader.Close();
                }
            }

            return Inspectors;
        }

        /// <summary>
        /// Update updates an existing Inspector and returns true if successful.
        /// </summary>
        public async Task<bool> Update(Inspector Inspector)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(InspectorInfrastructure.UserIdParameterName,               Inspector.UserId),
                base.GetParameter(InspectorInfrastructure.UserNameParameterName,             Inspector.UserName),
                base.GetParameter(InspectorInfrastructure.FirstNameParameterName,            Inspector.FirstName),
                base.GetParameter(InspectorInfrastructure.LastNameParameterName,             Inspector.LastName),
                base.GetParameter(InspectorInfrastructure.IdentificationNumberParameterName, Inspector.IdentificationNumber),
                base.GetParameter(InspectorInfrastructure.Address1ParameterName,             Inspector.Address1),
                base.GetParameter(InspectorInfrastructure.PostalCodeParameterName,           Inspector.PostalCode),
                base.GetParameter(InspectorInfrastructure.EmailParameterName,                Inspector.Email),
                base.GetParameter(InspectorInfrastructure.EmailConfirmedParameterName,       Inspector.EmailConfirmed),
                base.GetParameter(InspectorInfrastructure.PasswordHashParameterName,         Inspector.PasswordHash),
                base.GetParameter(InspectorInfrastructure.SecurityStampParameterName,        Inspector.SecurityStamp),
                base.GetParameter(InspectorInfrastructure.PhoneNumberParameterName,          Inspector.PhoneNumber),
                base.GetParameter(InspectorInfrastructure.LoginDateParameterName,            Inspector.LoginDate),
                base.GetParameter(InspectorInfrastructure.CodeParameterName,                 Inspector.Code),
                base.GetParameter(InspectorInfrastructure.ModifiedByIdParameterName,         Inspector.ModifiedById)
            };

            var rows = await base.ExecuteNonQuery(parameters, InspectorInfrastructure.UpdateStoredProcedureName, CommandType.StoredProcedure);
            return rows > 0;
        }
        /// <summary>
        /// Returns Total/Active/Inactive Inspector counts.
        /// </summary>
        public async Task<Inspector> GetStats()
        {
            var stats = new Inspector();
            var parameters = new List<DbParameter>(); // none

            using (var dr = await base.ExecuteReader(parameters, InspectorInfrastructure.GetStatsStoredProcedureName, CommandType.StoredProcedure))
            {
                if (dr != null && dr.Read())
                {
                    stats.TotalUsers = dr.GetIntegerValue("TotalUsers");
                    stats.ActiveUsers = dr.GetIntegerValue("ActiveUsers");
                    stats.InactiveUsers = dr.GetIntegerValue("InactiveUsers");
                }

                if (dr != null && !dr.IsClosed) dr.Close();
            }

            return stats;
        }


        #endregion
    }
}
