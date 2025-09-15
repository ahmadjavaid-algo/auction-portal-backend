using AuctionPortal.Common.Infrastructure;
using AuctionPortal.InfrastructureLayer.Interfaces;
using AuctionPortal.Models;
using System.Data;
using System.Data.Common;
using System.Reflection.PortableExecutable;

namespace AuctionPortal.InfrastructureLayer.Infrastructure
{
    public class BidderInfrastructure : BaseInfrastructure, IBidderInfrastructure
    {
        #region Constructor
        /// <summary>
        /// BidderInfrastructure initializes class object.
        /// </summary>
        public BidderInfrastructure(IConfiguration configuration, ILogger<BidderInfrastructure> logger)
            : base(configuration, logger)
        {
        }
        #endregion

        #region Constants
        // Stored procedure names
        private const string AddStoredProcedureName = "[dbo].[sp_Bidder_Insert]";
        private const string ActivateStoredProcedureName = "[dbo].[sp_Bidder_Activate]";
        private const string GetStoredProcedureName = "[dbo].[sp_Bidder_GetById]";
        private const string GetListStoredProcedureName = "[dbo].[sp_Bidder_GetAll]";
        private const string UpdateStoredProcedureName = "[dbo].[sp_Bidder_Update]";
        private const string GetStatsStoredProcedureName = "[dbo].[sp_Bidder_GetStats]";
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
        #region IBidderInfrastructure Implementation

        /// <summary>
        /// Add adds new Bidder and returns generated UserId.
        /// </summary>
        public async Task<int> Add(Bidder Bidder)
        {
            var BidderIdParam = base.GetParameterOut(BidderInfrastructure.UserIdParameterName, SqlDbType.Int, Bidder.UserId);

            var parameters = new List<DbParameter>
            {
                BidderIdParam,
                base.GetParameter(BidderInfrastructure.UserNameParameterName,             Bidder.UserName),
                base.GetParameter(BidderInfrastructure.FirstNameParameterName,            Bidder.FirstName),
                base.GetParameter(BidderInfrastructure.LastNameParameterName,             Bidder.LastName),
                base.GetParameter(BidderInfrastructure.IdentificationNumberParameterName, Bidder.IdentificationNumber),
                base.GetParameter(BidderInfrastructure.Address1ParameterName,             Bidder.Address1),
                base.GetParameter(BidderInfrastructure.PostalCodeParameterName,           Bidder.PostalCode),
                base.GetParameter(BidderInfrastructure.EmailParameterName,                Bidder.Email),
                base.GetParameter(BidderInfrastructure.EmailConfirmedParameterName,       Bidder.EmailConfirmed),
                base.GetParameter(BidderInfrastructure.PasswordHashParameterName,         Bidder.PasswordHash),
                base.GetParameter(BidderInfrastructure.SecurityStampParameterName,        Bidder.SecurityStamp),
                base.GetParameter(BidderInfrastructure.PhoneNumberParameterName,          Bidder.PhoneNumber),
                base.GetParameter(BidderInfrastructure.LoginDateParameterName,            Bidder.LoginDate),
                base.GetParameter(BidderInfrastructure.CodeParameterName,                 Bidder.Code),
                base.GetParameter(BidderInfrastructure.CreatedByIdParameterName,          Bidder.CreatedById)
            };

            await base.ExecuteNonQuery(parameters, BidderInfrastructure.AddStoredProcedureName, CommandType.StoredProcedure);

            Bidder.UserId = Convert.ToInt32(BidderIdParam.Value);
            return Bidder.UserId;
        }

        /// <summary>
        /// Activate activate/deactivate provided record and returns true if successful.
        /// </summary>
        public async Task<bool> Activate(Bidder Bidder)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(BidderInfrastructure.UserIdParameterName, Bidder.UserId),
                base.GetParameter(BaseInfrastructure.ActiveParameterName, Bidder.Active),
                base.GetParameter(BidderInfrastructure.ModifiedByIdParameterName, Bidder.ModifiedById)
            };

            var rows = await base.ExecuteNonQuery(parameters, BidderInfrastructure.ActivateStoredProcedureName, CommandType.StoredProcedure);
            return rows > 0;
        }

        /// <summary>
        /// Get fetches and returns a single Bidder.
        /// </summary>
        public async Task<Bidder> Get(Bidder Bidder)
        {
            Bidder BidderItem = null;
            var parameters = new List<DbParameter>
    {
        base.GetParameter(UserIdParameterName, Bidder.UserId)
    };

            using (var dr = await base.ExecuteReader(parameters, GetStoredProcedureName, CommandType.StoredProcedure))
            {
                if (dr != null)
                {
                    // first result set: Bidder
                    if (dr.Read())
                    {
                        BidderItem = new Bidder
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

            return BidderItem;
        }


        /// <summary>
        /// GetList fetches and returns a list of Bidders (trimmed columns).
        /// </summary>
        public async Task<List<Bidder>> GetList(Bidder Bidder)
        {
            var Bidders = new List<Bidder>();
            var parameters = new List<DbParameter>(); // add filters from 'Bidder' if needed

            using (var dataReader = await base.ExecuteReader(parameters, BidderInfrastructure.GetListStoredProcedureName, CommandType.StoredProcedure))
            {
                if (dataReader != null)
                {
                    while (dataReader.Read())
                    {
                        var item = new Bidder
                        {
                            UserId = dataReader.GetIntegerValue(BidderInfrastructure.UserIdColumnName),
                            UserName = dataReader.GetStringValue(BidderInfrastructure.UserNameColumnName),
                            FirstName = dataReader.GetStringValue(BidderInfrastructure.FirstNameColumnName),
                            Email = dataReader.GetStringValue(BidderInfrastructure.EmailColumnName),
                            EmailConfirmed = dataReader.GetBooleanValue(BidderInfrastructure.EmailConfirmedColumnName), // NEW
                            LoginDate = dataReader.GetDateTimeValueNullable(BidderInfrastructure.LoginDateColumnName), // NEW
                            Active = dataReader.GetBooleanValue(BaseInfrastructure.ActiveColumnName),         // NEW
                            CreatedDate = dataReader.GetDateTimeValueNullable(BaseInfrastructure.CreatedDateColumnName) // optional
                        };

                        Bidders.Add(item);
                    }

                    if (!dataReader.IsClosed)
                        dataReader.Close();
                }
            }

            return Bidders;
        }

        /// <summary>
        /// Update updates an existing Bidder and returns true if successful.
        /// </summary>
        public async Task<bool> Update(Bidder Bidder)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(BidderInfrastructure.UserIdParameterName,               Bidder.UserId),
                base.GetParameter(BidderInfrastructure.UserNameParameterName,             Bidder.UserName),
                base.GetParameter(BidderInfrastructure.FirstNameParameterName,            Bidder.FirstName),
                base.GetParameter(BidderInfrastructure.LastNameParameterName,             Bidder.LastName),
                base.GetParameter(BidderInfrastructure.IdentificationNumberParameterName, Bidder.IdentificationNumber),
                base.GetParameter(BidderInfrastructure.Address1ParameterName,             Bidder.Address1),
                base.GetParameter(BidderInfrastructure.PostalCodeParameterName,           Bidder.PostalCode),
                base.GetParameter(BidderInfrastructure.EmailParameterName,                Bidder.Email),
                base.GetParameter(BidderInfrastructure.EmailConfirmedParameterName,       Bidder.EmailConfirmed),
                base.GetParameter(BidderInfrastructure.PasswordHashParameterName,         Bidder.PasswordHash),
                base.GetParameter(BidderInfrastructure.SecurityStampParameterName,        Bidder.SecurityStamp),
                base.GetParameter(BidderInfrastructure.PhoneNumberParameterName,          Bidder.PhoneNumber),
                base.GetParameter(BidderInfrastructure.LoginDateParameterName,            Bidder.LoginDate),
                base.GetParameter(BidderInfrastructure.CodeParameterName,                 Bidder.Code),
                base.GetParameter(BidderInfrastructure.ModifiedByIdParameterName,         Bidder.ModifiedById)
            };

            var rows = await base.ExecuteNonQuery(parameters, BidderInfrastructure.UpdateStoredProcedureName, CommandType.StoredProcedure);
            return rows > 0;
        }
        /// <summary>
        /// Returns Total/Active/Inactive Bidder counts.
        /// </summary>
        public async Task<Bidder> GetStats()
        {
            var stats = new Bidder();
            var parameters = new List<DbParameter>(); // none

            using (var dr = await base.ExecuteReader(parameters, BidderInfrastructure.GetStatsStoredProcedureName, CommandType.StoredProcedure))
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
