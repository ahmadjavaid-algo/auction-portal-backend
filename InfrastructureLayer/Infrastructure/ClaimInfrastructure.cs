// AuctionPortal.InfrastructureLayer.Infrastructure/ClaimInfrastructure.cs
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using AuctionPortal.Common.Infrastructure;
using AuctionPortal.InfrastructureLayer.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using AuctionPortal.Models;

namespace AuctionPortal.InfrastructureLayer.Infrastructure
{
    public class ClaimInfrastructure : BaseInfrastructure, IClaimInfrastructure
    {
        #region Constructor
        public ClaimInfrastructure(IConfiguration configuration, ILogger<ClaimInfrastructure> logger)
            : base(configuration, logger)
        {
        }
        #endregion

        #region Constants

        private const string GetEffectiveStoredProcedureName = "[dbo].[sp_User_GetEffectivePermissions]";
        private const string GetAllCodesStoredProcedureName = "[dbo].[sp_Claim_GetAllClaimCodes]";
        private const string GetByRoleStoredProcedureName = "[dbo].[sp_RoleClaims_GetByRole]";
        private const string SetForRoleStoredProcedureName = "[dbo].[sp_RoleClaims_SetForRole]";
        private const string GetListStoredProcedureName = "[dbo].[sp_Claim_GetList]";

        private const string ClaimIdColumnName = "ClaimId";
        private const string ClaimGroupIdColumnName = "ClaimGroupId";
        private const string EndpointColumnName = "Endpoint";
        private const string DescriptionColumnName = "Description";
        private const string ClaimCodeColumnName = "ClaimCode";
        private const string RoleIdParameterName = "@RoleId";
        private const string ClaimIdParameterName = "@ClaimId";
        private const string ClaimIdsCsvParameterName = "@ClaimIdsCsv";
        private const string UserIdParameterName = "@UserId";

        #endregion

        #region IClaimInfrastructure Implementation

        public async Task<List<string>> GetAllClaimCodes()
        {
            var list = new List<string>();
            var parameters = new List<DbParameter>();

            using (var reader = await ExecuteReader(parameters, GetAllCodesStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        list.Add(reader.GetStringValue(ClaimCodeColumnName));
                    }

                    if (!reader.IsClosed)
                        reader.Close();
                }
            }

            return list;
        }

        public async Task<List<string>> GetEffectiveClaimCodesForUser(int userId)
        {
            var list = new List<string>();
            var parameters = new List<DbParameter>
            {
                GetParameter(UserIdParameterName, userId)
            };

            using (var reader = await ExecuteReader(parameters, GetEffectiveStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        list.Add(reader.GetStringValue(ClaimCodeColumnName));
                    }

                    if (!reader.IsClosed)
                        reader.Close();
                }
            }

            return list;
        }

        public async Task<List<RoleClaims>> GetByRole(RoleClaims request)
        {
            var list = new List<RoleClaims>();
            var parameters = new List<DbParameter>
            {
                GetParameter(RoleIdParameterName, request.RoleId)
            };

            using (var r = await ExecuteReader(parameters, GetByRoleStoredProcedureName, CommandType.StoredProcedure))
            {
                if (r != null)
                {
                    while (r.Read())
                    {
                        list.Add(new RoleClaims
                        {
                            RoleId = r.GetIntegerValue("RoleId"),
                            ClaimId = r.GetIntegerValue("ClaimId"),
                            ClaimCode = r.GetStringValue("ClaimCode"),
                            Endpoint = r.GetStringValue("Endpoint"),
                            Description = r.GetStringValue("Description")
                        });
                    }

                    if (!r.IsClosed)
                        r.Close();
                }
            }

            return list;
        }

        public async Task<bool> SetForRole(RoleClaims request)
        {
            var csv = string.Join(",", (request.ClaimIds ?? new List<int>()).Distinct());
            var parameters = new List<DbParameter>
            {
                GetParameter(RoleIdParameterName, request.RoleId),
                GetParameter(ClaimIdsCsvParameterName, csv)
            };

            var rows = await ExecuteNonQuery(parameters, SetForRoleStoredProcedureName, CommandType.StoredProcedure);
            return rows >= 0; // Some SPs SELECT; ExecuteNonQuery may return 0.
        }

        public async Task<List<RoleClaims>> GetList(RoleClaims request)
        {
            var list = new List<RoleClaims>();
            var parameters = new List<DbParameter>(); // none needed for full list

            using (var r = await ExecuteReader(parameters, GetListStoredProcedureName, CommandType.StoredProcedure))
            {
                if (r != null)
                {
                    while (r.Read())
                    {
                        list.Add(new RoleClaims
                        {

                            ClaimId = r.GetIntegerValue(ClaimIdColumnName),
                            ClaimCode = r.GetStringValue(ClaimCodeColumnName),
                            Endpoint = r.GetStringValue(EndpointColumnName),
                            Description = r.GetStringValue(DescriptionColumnName),

                        });
                    }

                    if (!r.IsClosed)
                        r.Close();
                }
            }

            return list;
        }




        #endregion
    }
}
