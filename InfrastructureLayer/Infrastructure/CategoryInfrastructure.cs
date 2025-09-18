// AuctionPortal.InfrastructureLayer.Infrastructure/CategoryInfrastructure.cs
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
    public class CategoryInfrastructure : BaseInfrastructure, ICategoryInfrastructure
    {
        #region Constructor
        /// <summary>
        /// CategoryInfrastructure initializes class object.
        /// </summary>
        public CategoryInfrastructure(IConfiguration configuration, ILogger<CategoryInfrastructure> logger)
            : base(configuration, logger)
        {
        }
        #endregion

        #region Constants
        private const string AddStoredProcedureName = "[dbo].[sp_Category_Add]";
        private const string ActivateStoredProcedureName = "[dbo].[sp_Category_Activate]";
        private const string GetStoredProcedureName = "[dbo].[sp_Category_Get]";
        private const string GetListStoredProcedureName = "[dbo].[sp_Category_GetAll]";
        private const string UpdateStoredProcedureName = "[dbo].[sp_Category_Update]";

        private const string CategoryIdColumnName = "CategoryId";
        private const string YearIdColumnName = "YearId";
        private const string CategoryNameColumnName = "CategoryName";

        private const string CategoryIdParameterName = "@CategoryId";
        private const string YearIdParameterName = "@YearId";
        private const string CategoryNameParameterName = "@CategoryName";
        private const string CreatedByIdParameterName = "@CreatedById";
        private const string ModifiedByIdParameterName = "@ModifiedById";
        #endregion

        #region ICategoryInfrastructure Implementation

        /// <summary>
        /// Add adds new Category and returns generated CategoryId.
        /// </summary>
        public async Task<int> Add(Category Category)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(YearIdParameterName, Category.YearId),
                base.GetParameter(CategoryNameParameterName, Category.CategoryName),
                base.GetParameter(CreatedByIdParameterName, Category.CreatedById)
            };

            // sp_Category_Add INSERTs and SELECTs the inserted row.
            using (var reader = await base.ExecuteReader(parameters, AddStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null && reader.HasRows && reader.Read())
                {
                    Category.CategoryId = reader.GetIntegerValue(CategoryIdColumnName);
                    Category.YearId = reader.GetIntegerValue(YearIdColumnName);
                    Category.CategoryName = reader.GetStringValue(CategoryNameColumnName);

                    // Audit (optional to read on Add)
                    Category.CreatedById = reader.GetIntegerValueNullable(BaseInfrastructure.CreatedByIdColumnName);
                    Category.CreatedDate = reader.GetDateTimeValueNullable(BaseInfrastructure.CreatedDateColumnName);
                    Category.ModifiedById = reader.GetIntegerValueNullable(BaseInfrastructure.ModifiedByIdColumnName) ?? 0;
                    Category.ModifiedDate = reader.GetDateTimeValueNullable(BaseInfrastructure.ModifiedDateColumnName);
                    Category.Active = reader.GetBooleanValue(BaseInfrastructure.ActiveColumnName);
                }

                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }

            return Category.CategoryId;
        }

        /// <summary>
        /// Activate activate/deactivate provided record and returns true if successful.
        /// </summary>
        public async Task<bool> Activate(Category Category)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(CategoryIdParameterName, Category.CategoryId),
                base.GetParameter(BaseInfrastructure.ActiveParameterName, Category.Active),
                base.GetParameter(ModifiedByIdParameterName, Category.ModifiedById)
            };

            var rows = await base.ExecuteNonQuery(parameters, ActivateStoredProcedureName, CommandType.StoredProcedure);
            return rows > 0;
        }

        /// <summary>
        /// Get fetches and returns a single Category.
        /// </summary>
        public async Task<Category> Get(Category Category)
        {
            Category item = null;

            var parameters = new List<DbParameter>
            {
                base.GetParameter(CategoryIdParameterName, Category.CategoryId)
            };

            using (var reader = await base.ExecuteReader(parameters, GetStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null && reader.HasRows && reader.Read())
                {
                    item = new Category
                    {
                        CategoryId = reader.GetIntegerValue(CategoryIdColumnName),
                        YearId = reader.GetIntegerValue(YearIdColumnName),
                        CategoryName = reader.GetStringValue(CategoryNameColumnName),

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
        /// GetList fetches and returns a list of Categorys (trimmed columns).
        /// </summary>
        public async Task<List<Category>> GetList(Category Category)
        {
            var items = new List<Category>();
            var parameters = new List<DbParameter>(); // none

            using (var reader = await base.ExecuteReader(parameters, GetListStoredProcedureName, CommandType.StoredProcedure))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        var item = new Category
                        {
                            CategoryId = reader.GetIntegerValue(CategoryIdColumnName),
                            YearId = reader.GetIntegerValue(YearIdColumnName),
                            CategoryName = reader.GetStringValue(CategoryNameColumnName),
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
        /// Update updates an existing Category and returns true if successful.
        /// </summary>
        public async Task<bool> Update(Category category)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(CategoryIdParameterName, category.CategoryId),
                base.GetParameter(CategoryNameParameterName, (object?)category.CategoryName ?? DBNull.Value),
                base.GetParameter(YearIdParameterName, category.YearId > 0 ? (object)category.YearId : DBNull.Value),
                base.GetParameter(ModifiedByIdParameterName, category.ModifiedById)
            };

            var rows = await base.ExecuteNonQuery(parameters, UpdateStoredProcedureName, CommandType.StoredProcedure);
            return rows > 0;
        }


        #endregion
    }
}
