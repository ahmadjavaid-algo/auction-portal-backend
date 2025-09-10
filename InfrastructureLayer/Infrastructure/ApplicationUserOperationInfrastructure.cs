using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using AuctionPortal.Common.Infrastructure;
using AuctionPortal.Common.Services;
using AuctionPortal.InfrastructureLayer.Interfaces;
using AuctionPortal.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AuctionPortal.InfrastructureLayer.Infrastructure
{
    public class ApplicationUserOperationInfrastructure : BaseInfrastructure, IApplicationUserOperationInfrastructure
    {
        public ApplicationUserOperationInfrastructure(
            IConfiguration configuration,
            ILogger<ApplicationUserOperationInfrastructure> logger,
            IClaimInfrastructure claimInfrastructure,   // kept for parity with existing DI
            ITokenService tokenService)                 // kept for parity with existing DI
            : base(configuration, logger)
        {
            ClaimInfrastructure = claimInfrastructure;
            TokenService = tokenService;
        }

        private IClaimInfrastructure ClaimInfrastructure { get; }
        private ITokenService TokenService { get; }

        private const string SpLogin = "[dbo].[sp_User_Login]";
        private const string SpLogout = "[dbo].[sp_User_Logout]";
        private const string SpChangePassword = "[dbo].[sp_User_ChangePassword]";
        private const string SpForgotPassword = "[dbo].[sp_User_ForgotPassword]";
        private const string SpResetPassword = "[dbo].[sp_User_ResetPassword]";

        private const string PUserId = "@UserId";
        private const string PUserName = "@UserName";
        private const string PPasswordHash = "@PasswordHash";
        private const string POldPassword = "@OldPasswordHash";
        private const string PNewPassword = "@NewPasswordHash";
        private const string PEmail = "@Email";
        private const string PResetCode = "@ResetCode";

        private const string ColUserId = "UserId";
        private const string ColUserName = "UserName";
        private const string ColFirst = "FirstName";
        private const string ColEmail = "Email";

        private const string PResetToken = "@PResetToken";
        private const string PExpiresAt = "@PExpiresAt";
        private const string PFirstNameOut = "@PFirstName";

        // Returns a single-DTO result (Success/Message and user/profile fields set if OK).
        public async Task<ApplicationUserOperation?> Login(ApplicationUserOperation request)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(PUserName,     request.UserName),
                base.GetParameter(PPasswordHash, request.PasswordHash)
            };

            ApplicationUserOperation? result = null;

            using (var reader = await base.ExecuteReader(parameters, SpLogin, CommandType.StoredProcedure))
            {
                if (reader != null && reader.HasRows && reader.Read())
                {
                    result = new ApplicationUserOperation
                    {
                        Success = true,
                        Message = "OK",
                        UserId = reader.GetIntegerValue(ColUserId),
                        UserName = reader.GetStringValue(ColUserName),
                        FirstName = reader.GetStringValue(ColFirst),
                        Email = reader.GetStringValue(ColEmail)
                    };
                }

                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }

            
            if (result == null)
            {
                result = new ApplicationUserOperation
                {
                    Success = false,
                    Message = "Invalid credentials."
                };
            }

            return result;
        }

        public async Task<bool> Logout(ApplicationUserOperation request)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(PUserId, request.UserId)
            };

            var rows = await base.ExecuteNonQuery(parameters, SpLogout, CommandType.StoredProcedure);
            return rows > 0;
        }

        public async Task<bool> ChangePassword(ApplicationUserOperation request)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(PUserId,      request.UserId),
                base.GetParameter(POldPassword, request.OldPasswordHash),
                base.GetParameter(PNewPassword, request.NewPasswordHash)
            };

            var rows = await base.ExecuteNonQuery(parameters, SpChangePassword, CommandType.StoredProcedure);
            return rows > 0;
        }

        public async Task<(bool Success, string Email, string? Token, DateTime? ExpiresAt, string? FirstName)>
                ForgotPassword(ApplicationUserOperation request)
        {
            var tokenOut = base.GetParameterOut(PResetToken, SqlDbType.NVarChar, value: DBNull.Value, direction: ParameterDirection.InputOutput);
            var expiresOut = base.GetParameterOut(PExpiresAt, SqlDbType.DateTime2, value: DBNull.Value, direction: ParameterDirection.InputOutput);
            var firstOut = base.GetParameterOut(PFirstNameOut, SqlDbType.NVarChar, value: DBNull.Value, direction: ParameterDirection.InputOutput);

            var parameters = new List<DbParameter>
        {
            base.GetParameter(PEmail, request.Email),
            tokenOut,
            expiresOut,
            firstOut
        };

            // Insert of token counts as 1 row when user exists; still "success" either way.
            await base.ExecuteNonQuery(parameters, SpForgotPassword, CommandType.StoredProcedure);

            string? token = tokenOut.Value == DBNull.Value ? null : Convert.ToString(tokenOut.Value);
            DateTime? exp = expiresOut.Value == DBNull.Value ? null : (DateTime?)expiresOut.Value;
            string? first = firstOut.Value == DBNull.Value ? null : Convert.ToString(firstOut.Value);

            return (true, request.Email, token, exp, first);
        }

        public async Task<bool> ResetPassword(ApplicationUserOperation request)
        {
            var parameters = new List<DbParameter>
        {
            base.GetParameter(PEmail,       request.Email),
            base.GetParameter(PNewPassword, request.NewPasswordHash),
            base.GetParameter(PResetCode,   request.ResetCode)
        };

            var rows = await base.ExecuteNonQuery(parameters, SpResetPassword, CommandType.StoredProcedure);
            return rows > 0; // will be 0 if token invalid/expired/not found
        }


        public async Task<int> Add(ApplicationUserOperation entity) => 0;

        public async Task<bool> Update(ApplicationUserOperation entity) => true;

        public async Task<bool> Activate(ApplicationUserOperation entity) => true;

        public async Task<ApplicationUserOperation> Get(ApplicationUserOperation entity)
        {
            await Task.CompletedTask;
            return new ApplicationUserOperation();
        }

        public async Task<List<ApplicationUserOperation>> GetList(ApplicationUserOperation entity)
        {
            await Task.CompletedTask;
            return new List<ApplicationUserOperation>();
        }
    }
}
