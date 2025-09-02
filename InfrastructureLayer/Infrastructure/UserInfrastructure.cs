using AuctionPortal.Common.Infrastructure;
using AuctionPortal.InfrastructureLayer.Interfaces;
using AuctionPortal.Models;
using System.Data;
using System.Data.Common;
using System.Reflection.PortableExecutable;

namespace AuctionPortal.InfrastructureLayer.Infrastructure
{
    public class UserInfrastructure : BaseInfrastructure, IUserInfrastructure
    {
        #region Constructor
        /// <summary>
        /// UserInfrastructure initializes class object.
        /// </summary>
        public UserInfrastructure(IConfiguration configuration, ILogger<UserInfrastructure> logger)
            : base(configuration, logger)
        {
        }
        #endregion

        #region Constants
        // Stored procedure names
        private const string AddStoredProcedureName = "[dbo].[sp_User_Insert]";
        private const string ActivateStoredProcedureName = "[dbo].[sp_User_Activate]";
        private const string GetStoredProcedureName = "[dbo].[sp_User_GetById]";
        private const string GetListStoredProcedureName = "[dbo].[sp_User_GetAll]";
        private const string UpdateStoredProcedureName = "[dbo].[sp_User_Update]";

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
        #region IUserInfrastructure Implementation

        /// <summary>
        /// Add adds new User and returns generated UserId.
        /// </summary>
        public async Task<int> Add(User user)
        {
            var RoleIdParam = base.GetParameterOut(UserInfrastructure.UserIdParameterName, SqlDbType.Int, user.UserId);

            var parameters = new List<DbParameter>
            {
                RoleIdParam,
                base.GetParameter(UserInfrastructure.UserNameParameterName,             user.UserName),
                base.GetParameter(UserInfrastructure.FirstNameParameterName,            user.FirstName),
                base.GetParameter(UserInfrastructure.LastNameParameterName,             user.LastName),
                base.GetParameter(UserInfrastructure.IdentificationNumberParameterName, user.IdentificationNumber),
                base.GetParameter(UserInfrastructure.Address1ParameterName,             user.Address1),
                base.GetParameter(UserInfrastructure.PostalCodeParameterName,           user.PostalCode),
                base.GetParameter(UserInfrastructure.EmailParameterName,                user.Email),
                base.GetParameter(UserInfrastructure.EmailConfirmedParameterName,       user.EmailConfirmed),
                base.GetParameter(UserInfrastructure.PasswordHashParameterName,         user.PasswordHash),
                base.GetParameter(UserInfrastructure.SecurityStampParameterName,        user.SecurityStamp),
                base.GetParameter(UserInfrastructure.PhoneNumberParameterName,          user.PhoneNumber),
                base.GetParameter(UserInfrastructure.LoginDateParameterName,            user.LoginDate),
                base.GetParameter(UserInfrastructure.CodeParameterName,                 user.Code),
                base.GetParameter(UserInfrastructure.CreatedByIdParameterName,          user.CreatedById),
                base.GetParameter(UserInfrastructure.RoleIdParameterName,               (object?)BuildRoleIdsJson(user.RoleId) ?? DBNull.Value)
            };

            await base.ExecuteNonQuery(parameters, UserInfrastructure.AddStoredProcedureName, CommandType.StoredProcedure);

            user.UserId = Convert.ToInt32(RoleIdParam.Value);
            return user.UserId;
        }

        /// <summary>
        /// Activate activate/deactivate provided record and returns true if successful.
        /// </summary>
        public async Task<bool> Activate(User user)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(UserInfrastructure.UserIdParameterName, user.UserId),
                base.GetParameter(BaseInfrastructure.ActiveParameterName, user.Active),
                base.GetParameter(UserInfrastructure.ModifiedByIdParameterName, user.ModifiedById)
            };

            var rows = await base.ExecuteNonQuery(parameters, UserInfrastructure.ActivateStoredProcedureName, CommandType.StoredProcedure);
            return rows > 0;
        }

        /// <summary>
        /// Get fetches and returns a single User.
        /// </summary>
        public async Task<User> Get(User user)
        {
            User userItem = null;
            var parameters = new List<DbParameter>
    {
        base.GetParameter(UserIdParameterName, user.UserId)
    };

            using (var dr = await base.ExecuteReader(parameters, GetStoredProcedureName, CommandType.StoredProcedure))
            {
                if (dr != null)
                {
                    // first result set: user
                    if (dr.Read())
                    {
                        userItem = new User
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
                            Active = dr.GetBooleanValue(BaseInfrastructure.ActiveColumnName),
                            RoleId = new List<int>()
                        };
                    }

                    // second result set: roles
                    if (userItem != null && dr.NextResult())
                    {
                        while (dr.Read())
                        {
                            var rid = dr.GetIntegerValue(RoleIdColumnName);
                            (userItem.RoleId ??= new List<int>()).Add(rid);
                        }
                    }

                    // Fallback: if SP wasn't updated and we only had a single result set with a joined RoleId column
                    if (userItem != null && (userItem.RoleId == null || userItem.RoleId.Count == 0))
                    {
                        try
                        {
                            // Try to iterate more rows of the same result set (join case)
                            while (dr.Read())
                            {
                                var rid = dr.GetIntegerValue(RoleIdColumnName);
                                (userItem.RoleId ??= new List<int>()).Add(rid);
                            }
                        }
                        catch { /* ignore if RoleId not present */ }
                    }

                    if (!dr.IsClosed) dr.Close();
                }
            }

            return userItem;
        }


        /// <summary>
        /// GetList fetches and returns a list of Users (trimmed columns).
        /// </summary>
        public async Task<List<User>> GetList(User user)
        {
            var users = new List<User>();
            var parameters = new List<DbParameter>(); // add filters from 'user' if needed

            using (var dataReader = await base.ExecuteReader(parameters, UserInfrastructure.GetListStoredProcedureName, CommandType.StoredProcedure))
            {
                if (dataReader != null)
                {
                    while (dataReader.Read())
                    {
                        var item = new User
                        {
                            UserId = dataReader.GetIntegerValue(UserInfrastructure.UserIdColumnName),
                            UserName = dataReader.GetStringValue(UserInfrastructure.UserNameColumnName),
                            FirstName = dataReader.GetStringValue(UserInfrastructure.FirstNameColumnName),
                            Email = dataReader.GetStringValue(UserInfrastructure.EmailColumnName),
                            EmailConfirmed = dataReader.GetBooleanValue(UserInfrastructure.EmailConfirmedColumnName), // NEW
                            LoginDate = dataReader.GetDateTimeValueNullable(UserInfrastructure.LoginDateColumnName), // NEW
                            Active = dataReader.GetBooleanValue(BaseInfrastructure.ActiveColumnName),         // NEW
                            CreatedDate = dataReader.GetDateTimeValueNullable(BaseInfrastructure.CreatedDateColumnName) // optional
                        };

                        users.Add(item);
                    }

                    if (!dataReader.IsClosed)
                        dataReader.Close();
                }
            }

            return users;
        }

        /// <summary>
        /// Update updates an existing User and returns true if successful.
        /// </summary>
        public async Task<bool> Update(User user)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(UserInfrastructure.UserIdParameterName,               user.UserId),
                base.GetParameter(UserInfrastructure.UserNameParameterName,             user.UserName),
                base.GetParameter(UserInfrastructure.FirstNameParameterName,            user.FirstName),
                base.GetParameter(UserInfrastructure.LastNameParameterName,             user.LastName),
                base.GetParameter(UserInfrastructure.IdentificationNumberParameterName, user.IdentificationNumber),
                base.GetParameter(UserInfrastructure.Address1ParameterName,             user.Address1),
                base.GetParameter(UserInfrastructure.PostalCodeParameterName,           user.PostalCode),
                base.GetParameter(UserInfrastructure.EmailParameterName,                user.Email),
                base.GetParameter(UserInfrastructure.EmailConfirmedParameterName,       user.EmailConfirmed),
                base.GetParameter(UserInfrastructure.PasswordHashParameterName,         user.PasswordHash),
                base.GetParameter(UserInfrastructure.SecurityStampParameterName,        user.SecurityStamp),
                base.GetParameter(UserInfrastructure.PhoneNumberParameterName,          user.PhoneNumber),
                base.GetParameter(UserInfrastructure.LoginDateParameterName,            user.LoginDate),
                base.GetParameter(UserInfrastructure.CodeParameterName,                 user.Code),
                base.GetParameter(UserInfrastructure.ModifiedByIdParameterName,         user.ModifiedById),
                base.GetParameter(UserInfrastructure.RoleIdParameterName,               (object?)BuildRoleIdsJson(user.RoleId) ?? DBNull.Value)
            };

            var rows = await base.ExecuteNonQuery(parameters, UserInfrastructure.UpdateStoredProcedureName, CommandType.StoredProcedure);
            return rows > 0;
        }
        private static string? BuildRoleIdsJson(IEnumerable<int>? ids)
        {
            if (ids == null) return null;               // means: don't touch roles
            var arr = ids.Distinct().ToArray();
            return "[" + string.Join(",", arr) + "]";   // "[]" allowed = clear all
        }

        #endregion
    }
}
