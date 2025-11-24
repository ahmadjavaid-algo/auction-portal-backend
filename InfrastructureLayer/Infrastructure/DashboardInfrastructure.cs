// AuctionPortal.InfrastructureLayer.Infrastructure/DashboardInfrastructure.cs
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using AuctionPortal.Common.Infrastructure;
using AuctionPortal.InfrastructureLayer.Interfaces;
using AuctionPortal.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AuctionPortal.InfrastructureLayer.Infrastructure
{
    public class DashboardInfrastructure : BaseInfrastructure, IDashboardInfrastructure
    {
        #region Constructor

        public DashboardInfrastructure(
            IConfiguration configuration,
            ILogger<DashboardInfrastructure> logger)
            : base(configuration, logger)
        {
        }

        #endregion

        #region Constants

        private const string AddStoredProcedureName = "[dbo].[sp_Dashboardstats_Add]";
        private const string ActivateStoredProcedureName = "[dbo].[sp_Dashboardstats_Activate]";
        private const string GetStoredProcedureName = "[dbo].[sp_Dashboardstats_Get]";
        private const string GetListStoredProcedureName = "[dbo].[sp_Dashboardstats_GetAll]";
        private const string UpdateStoredProcedureName = "[dbo].[sp_Dashboardstats_Update]";

        // 🔹 New: refresh proc with all the logic you just built
        private const string RefreshStoredProcedureName = "[dbo].[sp_Dashboardstats_RefreshLiveAuction]";

        private const string DashboardstatsIdColumnName = "DashboardstatsId";
        private const string DashboardstatsNameColumnName = "DashboardstatsName";
        private const string DashboardnumberColumnName = "Dashboardnumber";

        private const string DashboardstatsIdParameterName = "@DashboardstatsId";
        private const string DashboardstatsNameParameterName = "@DashboardstatsName";
        private const string DashboardnumberParameterName = "@Dashboardnumber";

        #endregion

        #region IDashboardInfrastructure Implementation

        public Task<int> Add(Dashboard entity)
        {
            throw new NotImplementedException("Add for Dashboard is not implemented.");
        }

        public Task<bool> Activate(Dashboard entity)
        {
            throw new NotImplementedException("Activate for Dashboard is not implemented.");
        }

        /// <summary>
        /// Get a single Dashboard record by Id (sp_Dashboardstats_Get).
        /// NOTE: Does NOT trigger refresh; use GetList for full refreshed dashboard.
        /// </summary>
        public async Task<Dashboard> Get(Dashboard entity)
        {
            Dashboard item = null;

            var parameters = new List<DbParameter>
            {
                base.GetParameter(DashboardstatsIdParameterName, entity.DashboardstatsId)
            };

            using (var reader = await base.ExecuteReader(parameters, GetStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null && reader.HasRows && reader.Read())
                {
                    item = new Dashboard
                    {
                        DashboardstatsId = reader.GetIntegerValue(DashboardstatsIdColumnName),
                        DashboardstatsName = reader.GetStringValue(DashboardstatsNameColumnName),
                        Dashboardnumber = reader.GetIntegerValueNullable(DashboardnumberColumnName)
                    };
                }

                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
            }

            return item;
        }

        /// <summary>
        /// Refreshes dashboard stats via sp_Dashboardstats_RefreshLiveAuction
        /// and then returns all rows via sp_Dashboardstats_GetAll.
        /// This is what your dashboard page should call.
        /// </summary>
        public async Task<List<Dashboard>> GetList(Dashboard entity)
        {
            var items = new List<Dashboard>();

            
            var refreshParameters = new List<DbParameter>(); 
            await base.ExecuteNonQuery(
                refreshParameters,
                RefreshStoredProcedureName,
                CommandType.StoredProcedure);

           
            var parameters = new List<DbParameter>(); 

            using (var reader = await base.ExecuteReader(parameters, GetListStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var item = new Dashboard
                        {
                            DashboardstatsId = reader.GetIntegerValue(DashboardstatsIdColumnName),
                            DashboardstatsName = reader.GetStringValue(DashboardstatsNameColumnName),
                            Dashboardnumber = reader.GetIntegerValueNullable(DashboardnumberColumnName)
                        };

                        items.Add(item);
                    }

                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
            }

            return items;
        }

        public Task<bool> Update(Dashboard entity)
        {
            throw new NotImplementedException("Update for Dashboard is not implemented.");
        }

        #endregion
    }
}
