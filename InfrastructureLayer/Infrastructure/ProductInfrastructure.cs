// AuctionPortal.InfrastructureLayer.Infrastructure/ProductInfrastructure.cs
using AuctionPortal.Common.Infrastructure;
using AuctionPortal.InfrastructureLayer.Interfaces;
using AuctionPortal.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace AuctionPortal.InfrastructureLayer.Infrastructure
{
    public class ProductInfrastructure : BaseInfrastructure, IProductInfrastructure
    {
        #region Constructor
        /// <summary>
        /// ProductInfrastructure initializes class object.
        /// </summary>
        public ProductInfrastructure(IConfiguration configuration, ILogger<ProductInfrastructure> logger)
            : base(configuration, logger)
        {
        }
        #endregion

        #region Constants
        private const string AddStoredProcedureName = "[dbo].[sp_Product_Add]";
        private const string ActivateStoredProcedureName = "[dbo].[sp_Product_Activate]";
        private const string GetStoredProcedureName = "[dbo].[sp_Product_Get]";
        private const string GetListStoredProcedureName = "[dbo].[sp_Product_GetAll]";
        private const string UpdateStoredProcedureName = "[dbo].[sp_Product_Update]";

        private const string ProductIdColumnName = "ProductId";
        private const string MakeIdColumnName = "MakeId";
        private const string YearIdColumnName = "YearId";
        private const string ModelIdColumnName = "ModelId";
        private const string CategoryIdColumnName = "CategoryId";
        private const string DisplayNameColumnName = "DisplayName";

        private const string ProductIdParameterName = "@ProductId";
        private const string MakeIdParameterName = "@MakeId";
        private const string YearIdParameterName = "@YearId";
        private const string ModelIdParameterName = "@ModelId";
        private const string CategoryIdParameterName = "@CategoryId";
        private const string DisplayNameParameterName = "@DisplayName";
        private const string CreatedByIdParameterName = "@CreatedById";
        private const string ModifiedByIdParameterName = "@ModifiedById";
        #endregion

        #region IProductInfrastructure Implementation

        /// <summary>
        /// Add adds new Product and returns generated ProductId.
        /// </summary>
        public async Task<int> Add(Product product)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(MakeIdParameterName, product.MakeId),
                base.GetParameter(YearIdParameterName, product.YearId),
                base.GetParameter(ModelIdParameterName, product.ModelId),
                base.GetParameter(CategoryIdParameterName, product.CategoryId),
                base.GetParameter(DisplayNameParameterName, product.DisplayName),
                base.GetParameter(CreatedByIdParameterName, product.CreatedById)
            };

            // sp_Product_Add INSERTs and SELECTs the inserted row.
            using (var reader = await base.ExecuteReader(parameters, AddStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null && reader.HasRows && reader.Read())
                {
                    product.ProductId = reader.GetIntegerValue(ProductIdColumnName);
                    product.MakeId = reader.GetIntegerValue(MakeIdColumnName);
                    product.YearId = reader.GetIntegerValue(YearIdColumnName);
                    product.ModelId = reader.GetIntegerValue(ModelIdColumnName);
                    product.CategoryId = reader.GetIntegerValue(CategoryIdColumnName);
                    product.DisplayName = reader.GetStringValue(DisplayNameColumnName);

                    // Audit (optional to read on Add)
                    product.CreatedById = reader.GetIntegerValueNullable(BaseInfrastructure.CreatedByIdColumnName);
                    product.CreatedDate = reader.GetDateTimeValueNullable(BaseInfrastructure.CreatedDateColumnName);
                    product.ModifiedById = reader.GetIntegerValueNullable(BaseInfrastructure.ModifiedByIdColumnName) ?? 0;
                    product.ModifiedDate = reader.GetDateTimeValueNullable(BaseInfrastructure.ModifiedDateColumnName);
                    product.Active = reader.GetBooleanValue(BaseInfrastructure.ActiveColumnName);
                }

                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }

            return product.ProductId;
        }

        /// <summary>
        /// Activate activate/deactivate provided record and returns true if successful.
        /// </summary>
        public async Task<bool> Activate(Product product)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(ProductIdParameterName, product.ProductId),
                base.GetParameter(BaseInfrastructure.ActiveParameterName, product.Active),
                base.GetParameter(ModifiedByIdParameterName, product.ModifiedById)
            };

            var rows = await base.ExecuteNonQuery(parameters, ActivateStoredProcedureName, CommandType.StoredProcedure);
            return rows > 0;
        }

        /// <summary>
        /// Get fetches and returns a single Product.
        /// </summary>
        public async Task<Product> Get(Product product)
        {
            Product item = null;

            var parameters = new List<DbParameter>
            {
                base.GetParameter(ProductIdParameterName, product.ProductId)
            };

            using (var reader = await base.ExecuteReader(parameters, GetStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null && reader.HasRows && reader.Read())
                {
                    item = new Product
                    {
                        ProductId = reader.GetIntegerValue(ProductIdColumnName),
                        MakeId = reader.GetIntegerValue(MakeIdColumnName),
                        YearId = reader.GetIntegerValue(YearIdColumnName),
                        ModelId = reader.GetIntegerValue(ModelIdColumnName),
                        CategoryId = reader.GetIntegerValue(CategoryIdColumnName),
                        DisplayName = reader.GetStringValue(DisplayNameColumnName),

                        CreatedById = reader.GetIntegerValueNullable(BaseInfrastructure.CreatedByIdColumnName),
                        CreatedDate = reader.GetDateTimeValueNullable(BaseInfrastructure.CreatedDateColumnName),
                        ModifiedById = reader.GetIntegerValueNullable(BaseInfrastructure.ModifiedByIdColumnName) ?? 0,
                        ModifiedDate = reader.GetDateTimeValueNullable(BaseInfrastructure.ModifiedDateColumnName),
                        Active = reader.GetBooleanValue(BaseInfrastructure.ActiveColumnName)
                    };
                }

                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }

            return item;
        }

        /// <summary>
        /// GetList fetches and returns a list of Products (trimmed columns).
        /// </summary>
        public async Task<List<Product>> GetList(Product product)
        {
            var items = new List<Product>();
            var parameters = new List<DbParameter>(); // none

            using (var reader = await base.ExecuteReader(parameters, GetListStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var item = new Product
                        {
                            ProductId = reader.GetIntegerValue(ProductIdColumnName),
                            MakeId = reader.GetIntegerValue(MakeIdColumnName),
                            YearId = reader.GetIntegerValue(YearIdColumnName),
                            ModelId = reader.GetIntegerValue(ModelIdColumnName),
                            CategoryId = reader.GetIntegerValue(CategoryIdColumnName),
                            DisplayName = reader.GetStringValue(DisplayNameColumnName),
                            Active = reader.GetBooleanValue(BaseInfrastructure.ActiveColumnName)
                        };

                        items.Add(item);
                    }

                    if (!reader.IsClosed)
                        reader.Close();
                }
            }

            return items;
        }

        /// <summary>
        /// Update updates an existing Product and returns true if successful.
        /// </summary>
        public async Task<bool> Update(Product product)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(ProductIdParameterName, product.ProductId),

                base.GetParameter(DisplayNameParameterName, (object?)product.DisplayName ?? DBNull.Value),

                base.GetParameter(MakeIdParameterName,     product.MakeId     > 0 ? (object)product.MakeId     : DBNull.Value),
                base.GetParameter(YearIdParameterName,     product.YearId     > 0 ? (object)product.YearId     : DBNull.Value),
                base.GetParameter(ModelIdParameterName,    product.ModelId    > 0 ? (object)product.ModelId    : DBNull.Value),
                base.GetParameter(CategoryIdParameterName, product.CategoryId > 0 ? (object)product.CategoryId : DBNull.Value),

                base.GetParameter(ModifiedByIdParameterName, product.ModifiedById)
            };

            var rows = await base.ExecuteNonQuery(parameters, UpdateStoredProcedureName, CommandType.StoredProcedure);
            return rows > 0;
        }


        #endregion
    }
}
