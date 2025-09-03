using AuctionPortal.Common.Infrastructure;
using AuctionPortal.InfrastructureLayer.Interfaces;
using AuctionPortal.Models;
using System.Data;
using System.Data.Common;

namespace AuctionPortal.InfrastructureLayer.Infrastructure
{
    public class RoleInfrastructure : BaseInfrastructure, IRoleInfrastructure
    {
        #region Constructor
        /// <summary>
        /// RoleInfrastructure initializes class object.
        /// </summary>
        public RoleInfrastructure(IConfiguration configuration, ILogger<RoleInfrastructure> logger)
            : base(configuration, logger)
        {
        }
        #endregion

        #region Constants
        
        private const string AddStoredProcedureName = "[dbo].[sp_Role_Insert]";
        private const string ActivateStoredProcedureName = "[dbo].[sp_Role_Activate]";
        private const string GetStoredProcedureName = "[dbo].[sp_Role_GetById]";
        private const string GetListStoredProcedureName = "[dbo].[sp_Role_GetAll]";
        private const string UpdateStoredProcedureName = "[dbo].[sp_Role_Update]";

        
        private const string RoleIdColumnName = "RoleId";
        private const string RoleNameColumnName = "RoleName";
        private const string RoleCodeColumnName = "RoleCode";
        private const string DescriptionColumnName = "Description";

       
        private const string RoleIdParameterName = "@RoleId";
        private const string RoleNameParameterName = "@RoleName";
        private const string RoleCodeParameterName = "@RoleCode";
        private const string DescriptionParameterName = "@Description";
        private const string CreatedByIdParameterName = "@CreatedById";
        private const string ModifiedByIdParameterName = "@ModifiedById";
        private const string NewIdParameterName = "@NewId";
        #endregion

        #region IRoleInfrastructure Implementation

        /// <summary>
        /// Add adds new Role and returns generated RoleId.
        /// </summary>
        public async Task<int> Add(Role role)
        {
            var RoleIdParam = base.GetParameterOut(RoleInfrastructure.RoleIdParameterName, SqlDbType.Int, role.RoleId);
            var parameters = new List<DbParameter>
            {
                RoleIdParam,
                base.GetParameter(RoleInfrastructure.RoleNameParameterName, role.RoleName),
                base.GetParameter(RoleInfrastructure.RoleCodeParameterName, role.RoleCode),
                base.GetParameter(RoleInfrastructure.DescriptionParameterName, role.Description),
                base.GetParameter(RoleInfrastructure.CreatedByIdParameterName, role.CreatedById)
            };

            await base.ExecuteNonQuery(parameters, RoleInfrastructure.AddStoredProcedureName, CommandType.StoredProcedure);

            role.RoleId = Convert.ToInt32(RoleIdParam.Value);
            return role.RoleId;
        }

        /// <summary>
        /// Activate activate/deactivate provided record and returns true if successful.
        /// </summary>
        public async Task<bool> Activate(Role role)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(RoleInfrastructure.RoleIdParameterName, role.RoleId),
                base.GetParameter(BaseInfrastructure.ActiveParameterName, role.Active),
                base.GetParameter(RoleInfrastructure.ModifiedByIdParameterName, role.ModifiedById)
            };

            var rows = await base.ExecuteNonQuery(parameters, RoleInfrastructure.ActivateStoredProcedureName, CommandType.StoredProcedure);
            return rows > 0;
        }

        /// <summary>
        /// Get fetches and returns a single Role.
        /// </summary>
        public async Task<Role> Get(Role role)
        {
            Role roleItem = null;
            var parameters = new List<DbParameter>
            {
                base.GetParameter(RoleIdParameterName, role.RoleId)
            };

            using (var dataReader = await base.ExecuteReader(parameters, RoleInfrastructure.GetStoredProcedureName, CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    if (dataReader.Read())
                    {
                        roleItem = new Role
                        {
                            RoleId = dataReader.GetIntegerValue(RoleInfrastructure.RoleIdColumnName),
                            RoleName = dataReader.GetStringValue(RoleInfrastructure.RoleNameColumnName),
                            RoleCode = dataReader.GetStringValue(RoleInfrastructure.RoleCodeColumnName),
                            Description = dataReader.GetStringValue(RoleInfrastructure.DescriptionColumnName),

                            
                            CreatedById = dataReader.GetIntegerValueNullable(BaseInfrastructure.CreatedByIdColumnName),
                            CreatedDate = dataReader.GetDateTimeValueNullable(BaseInfrastructure.CreatedDateColumnName),
                            ModifiedById = dataReader.GetIntegerValueNullable(BaseInfrastructure.ModifiedByIdColumnName) ?? 0,
                            ModifiedDate = dataReader.GetDateTimeValueNullable(BaseInfrastructure.ModifiedDateColumnName),
                            Active = dataReader.GetBooleanValue(BaseInfrastructure.ActiveColumnName)
                        };
                    }

                    if (!dataReader.IsClosed)
                        dataReader.Close();
                }
            }

            return roleItem;
        }

        /// <summary>
        /// GetList fetches and returns a list of Roles (trimmed columns).
        /// </summary>
        public async Task<List<Role>> GetList(Role role)
        {
            var roles = new List<Role>();
            var parameters = new List<DbParameter>(); 

            using (var dataReader = await base.ExecuteReader(parameters, RoleInfrastructure.GetListStoredProcedureName, CommandType.StoredProcedure))
            {
                if (dataReader != null)
                {
                    while (dataReader.Read())
                    {
                        var Roleitem = new Role
                        {
                            RoleId = dataReader.GetIntegerValue(RoleInfrastructure.RoleIdColumnName),
                            RoleName = dataReader.GetStringValue(RoleInfrastructure.RoleNameColumnName),
                            RoleCode = dataReader.GetStringValue(RoleInfrastructure.RoleCodeColumnName),
                            Active = dataReader.GetBooleanValue(BaseInfrastructure.ActiveColumnName),
                            Description = dataReader.GetStringValue(RoleInfrastructure.DescriptionColumnName)
                        };

                        roles.Add(Roleitem);
                    }

                    if (!dataReader.IsClosed)
                        dataReader.Close();
                }
            }

            return roles;
        }

        /// <summary>
        /// Update updates an existing Role and returns true if successful.
        /// </summary>
        public async Task<bool> Update(Role role)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(RoleInfrastructure.RoleNameParameterName, role.RoleName),
                base.GetParameter(RoleInfrastructure.RoleIdParameterName, role.RoleId),
                base.GetParameter(RoleInfrastructure.DescriptionParameterName, role.Description),
                base.GetParameter(RoleInfrastructure.RoleCodeParameterName, role.RoleCode),
                base.GetParameter(RoleInfrastructure.ModifiedByIdParameterName, role.ModifiedById)
            };

            var rows = await base.ExecuteNonQuery(parameters, RoleInfrastructure.UpdateStoredProcedureName, CommandType.StoredProcedure);
            return rows > 0;
        }

        #endregion
    }
}
