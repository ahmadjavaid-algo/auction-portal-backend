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
    public class BidderOperationInfrastructure : BaseInfrastructure, IBidderOperationInfrastructure
    {
        public BidderOperationInfrastructure(
            IConfiguration configuration,
            ILogger<BidderOperationInfrastructure> logger,
            IClaimInfrastructure claimInfrastructure,   // kept for parity with existing DI
            ITokenService tokenService)                 // kept for parity with existing DI
            : base(configuration, logger)
        {
            ClaimInfrastructure = claimInfrastructure;
            TokenService = tokenService;
        }

        private IClaimInfrastructure ClaimInfrastructure { get; }
        private ITokenService TokenService { get; }

        private const string SpLogin = "[dbo].[sp_Bidder_Login]";
        private const string SpLogout = "[dbo].[sp_Bidder_Logout]";
        private const string SpChangePassword = "[dbo].[sp_Bidder_ChangePassword]";
        private const string SpForgotPassword = "[dbo].[sp_Bidder_ForgotPassword]";
        private const string SpResetPassword = "[dbo].[sp_Bidder_ResetPassword]";

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
        public async Task<BidderOperation?> Login(BidderOperation request)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(PUserName,     request.UserName),
                base.GetParameter(PPasswordHash, request.PasswordHash)
            };

            BidderOperation? result = null;

            using (var reader = await base.ExecuteReader(parameters, SpLogin, CommandType.StoredProcedure))
            {
                if (reader != null && reader.HasRows && reader.Read())
                {
                    result = new BidderOperation
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
                result = new BidderOperation
                {
                    Success = false,
                    Message = "Invalid credentials."
                };
            }

            return result;
        }

        public async Task<bool> Logout(BidderOperation request)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(PUserId, request.UserId)
            };

            var rows = await base.ExecuteNonQuery(parameters, SpLogout, CommandType.StoredProcedure);
            return rows > 0;
        }

        public async Task<bool> ChangePassword(BidderOperation request)
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
                ForgotPassword(BidderOperation request)
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

           
            await base.ExecuteNonQuery(parameters, SpForgotPassword, CommandType.StoredProcedure);

            string? token = tokenOut.Value == DBNull.Value ? null : Convert.ToString(tokenOut.Value);
            DateTime? exp = expiresOut.Value == DBNull.Value ? null : (DateTime?)expiresOut.Value;
            string? first = firstOut.Value == DBNull.Value ? null : Convert.ToString(firstOut.Value);

            return (true, request.Email, token, exp, first);
        }

        public async Task<bool> ResetPassword(BidderOperation request)
        {
            var parameters = new List<DbParameter>
        {
            base.GetParameter(PEmail,       request.Email),
            base.GetParameter(PNewPassword, request.NewPasswordHash),
            base.GetParameter(PResetCode,   request.ResetCode)
        };

            var rows = await base.ExecuteNonQuery(parameters, SpResetPassword, CommandType.StoredProcedure);
            return rows > 0; 
        }


        public async Task<int> Add(BidderOperation entity) => 0;

        public async Task<bool> Update(BidderOperation entity) => true;

        public async Task<bool> Activate(BidderOperation entity) => true;

        public async Task<BidderOperation> Get(BidderOperation entity)
        {
            await Task.CompletedTask;
            return new BidderOperation();
        }

        public async Task<List<BidderOperation>> GetList(BidderOperation entity)
        {
            await Task.CompletedTask;
            return new List<BidderOperation>();
        }
    }
}
