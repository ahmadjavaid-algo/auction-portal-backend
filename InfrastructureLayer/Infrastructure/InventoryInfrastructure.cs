// AuctionPortal.InfrastructureLayer.Infrastructure/InventoryInfrastructure.cs
using AuctionPortal.Common.Infrastructure;
using AuctionPortal.InfrastructureLayer.Interfaces;
using AuctionPortal.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace AuctionPortal.InfrastructureLayer.Infrastructure
{
    public class InventoryInfrastructure : BaseInfrastructure, IInventoryInfrastructure
    {
        #region Constructor
        public InventoryInfrastructure(IConfiguration configuration, ILogger<InventoryInfrastructure> logger)
            : base(configuration, logger) { }
        #endregion

        #region Constants
        private const string AddStoredProcedureName = "[dbo].[sp_Inventory_Add]";
        private const string ActivateStoredProcedureName = "[dbo].[sp_Inventory_Activate]";
        private const string GetStoredProcedureName = "[dbo].[sp_Inventory_Get]";
        private const string GetListStoredProcedureName = "[dbo].[sp_Inventory_GetAll]";
        private const string UpdateStoredProcedureName = "[dbo].[sp_Inventory_Update]";

        // Columns
        private const string InventoryIdColumnName = "InventoryId";
        private const string ProductIdColumnName = "ProductId";
        private const string ProductJSONColumnName = "ProductJSON";
        private const string DescriptionColumnName = "Description";
        private const string DisplayNameColumnName = "DisplayName";
        private const string ChassisNoColumnName = "ChassisNo";
        private const string RegistrationNoColumnName = "RegistrationNo";

        // Parameters
        private const string InventoryIdParameterName = "@InventoryId";
        private const string ProductIdParameterName = "@ProductId";
        private const string DescriptionParameterName = "@Description";
        private const string ChassisNoParameterName = "@ChassisNo";
        private const string RegistrationNoParameterName = "@RegistrationNo";
        private const string CreatedByIdParameterName = "@CreatedById";
        private const string ModifiedByIdParameterName = "@ModifiedById";
        #endregion

        #region IInventoryInfrastructure Implementation

        public async Task<int> Add(Inventory inventory)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(ProductIdParameterName,       inventory.ProductId),
                base.GetParameter(DescriptionParameterName,     (object?)inventory.Description ?? DBNull.Value),
                base.GetParameter(ChassisNoParameterName,       (object?)inventory.ChassisNo ?? DBNull.Value),
                base.GetParameter(RegistrationNoParameterName,  (object?)inventory.RegistrationNo ?? DBNull.Value),
                base.GetParameter(CreatedByIdParameterName,     inventory.CreatedById)
            };

            using (var reader = await base.ExecuteReader(parameters, AddStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null && reader.HasRows && reader.Read())
                {
                    inventory.InventoryId = reader.GetIntegerValue(InventoryIdColumnName);
                    inventory.ProductId = reader.GetIntegerValue(ProductIdColumnName);
                    inventory.ProductJSON = reader.GetStringValue(ProductJSONColumnName);
                    inventory.Description = reader.GetStringValue(DescriptionColumnName);
                    inventory.ChassisNo = reader.GetStringValue(ChassisNoColumnName);
                    inventory.RegistrationNo = reader.GetStringValue(RegistrationNoColumnName);

                    // Audit
                    inventory.CreatedById = reader.GetIntegerValueNullable(BaseInfrastructure.CreatedByIdColumnName);
                    inventory.CreatedDate = reader.GetDateTimeValueNullable(BaseInfrastructure.CreatedDateColumnName);
                    inventory.ModifiedById = reader.GetIntegerValueNullable(BaseInfrastructure.ModifiedByIdColumnName) ?? 0;
                    inventory.ModifiedDate = reader.GetDateTimeValueNullable(BaseInfrastructure.ModifiedDateColumnName);
                    inventory.Active = reader.GetBooleanValue(BaseInfrastructure.ActiveColumnName);
                }

                if (reader != null && !reader.IsClosed) reader.Close();
            }

            return inventory.InventoryId;
        }

        public async Task<bool> Activate(Inventory inventory)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(InventoryIdParameterName, inventory.InventoryId),
                base.GetParameter(BaseInfrastructure.ActiveParameterName, inventory.Active),
                base.GetParameter(ModifiedByIdParameterName, inventory.ModifiedById)
            };

            var rows = await base.ExecuteNonQuery(parameters, ActivateStoredProcedureName, CommandType.StoredProcedure);
            return rows > 0;
        }

        public async Task<Inventory> Get(Inventory inventory)
        {
            Inventory item = null;

            var parameters = new List<DbParameter>
            {
                base.GetParameter(InventoryIdParameterName, inventory.InventoryId)
            };

            using (var reader = await base.ExecuteReader(parameters, GetStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null && reader.HasRows && reader.Read())
                {
                    item = new Inventory
                    {
                        InventoryId = reader.GetIntegerValue(InventoryIdColumnName),
                        ProductId = reader.GetIntegerValue(ProductIdColumnName),
                        ProductJSON = reader.GetStringValue(ProductJSONColumnName),
                        Description = reader.GetStringValue(DescriptionColumnName),
                        ChassisNo = reader.GetStringValue(ChassisNoColumnName),
                        RegistrationNo = reader.GetStringValue(RegistrationNoColumnName),

                        CreatedById = reader.GetIntegerValueNullable(BaseInfrastructure.CreatedByIdColumnName),
                        CreatedDate = reader.GetDateTimeValueNullable(BaseInfrastructure.CreatedDateColumnName),
                        ModifiedById = reader.GetIntegerValueNullable(BaseInfrastructure.ModifiedByIdColumnName) ?? 0,
                        ModifiedDate = reader.GetDateTimeValueNullable(BaseInfrastructure.ModifiedDateColumnName),
                        Active = reader.GetBooleanValue(BaseInfrastructure.ActiveColumnName)
                    };
                }

                if (reader != null && !reader.IsClosed) reader.Close();
            }

            return item;
        }

        public async Task<List<Inventory>> GetList(Inventory inventory)
        {
            var items = new List<Inventory>();
            var parameters = new List<DbParameter>(); // none

            using (var reader = await base.ExecuteReader(parameters, GetListStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var item = new Inventory
                        {
                            InventoryId = reader.GetIntegerValue(InventoryIdColumnName),
                            ProductId = reader.GetIntegerValue(ProductIdColumnName),
                            DisplayName = reader.GetStringValue(DisplayNameColumnName),
                            ProductJSON = reader.GetStringValue(ProductJSONColumnName),
                            Description = reader.GetStringValue(DescriptionColumnName),
                            ChassisNo = reader.GetStringValue(ChassisNoColumnName),
                            RegistrationNo = reader.GetStringValue(RegistrationNoColumnName),
                            Active = reader.GetBooleanValue(BaseInfrastructure.ActiveColumnName)
                        };

                        items.Add(item);
                    }

                    if (!reader.IsClosed) reader.Close();
                }
            }

            return items;
        }

        public async Task<bool> Update(Inventory inventory)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(InventoryIdParameterName, inventory.InventoryId),

                // Nullable updates (SP rebuilds ProductJSON when ProductId changes)
                base.GetParameter(ProductIdParameterName,      inventory.ProductId > 0 ? (object)inventory.ProductId : DBNull.Value),
                base.GetParameter(DescriptionParameterName,    (object?)inventory.Description ?? DBNull.Value),
                base.GetParameter(ChassisNoParameterName,      (object?)inventory.ChassisNo ?? DBNull.Value),
                base.GetParameter(RegistrationNoParameterName, (object?)inventory.RegistrationNo ?? DBNull.Value),

                base.GetParameter(ModifiedByIdParameterName,   inventory.ModifiedById)
            };

            var rows = await base.ExecuteNonQuery(parameters, UpdateStoredProcedureName, CommandType.StoredProcedure);
            return rows > 0;
        }

        #endregion
    }
}
