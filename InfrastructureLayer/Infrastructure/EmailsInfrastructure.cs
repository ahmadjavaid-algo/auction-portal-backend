// AuctionPortal.InfrastructureLayer.Infrastructure/EmailsInfrastructure.cs
using AuctionPortal.Common.Infrastructure;
using AuctionPortal.InfrastructureLayer.Interfaces;
using AuctionPortal.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Data.Common;

namespace AuctionPortal.InfrastructureLayer.Infrastructure
{
    public class EmailsInfrastructure : BaseInfrastructure, IEmailsInfrastructure
    {
        #region Constructor
        public EmailsInfrastructure(IConfiguration configuration, ILogger<EmailsInfrastructure> logger)
            : base(configuration, logger)
        {
        }
        #endregion

        #region Constants
        private const string AddStoredProcedureName = "[dbo].[EmailsAdd]";
        private const string ActivateStoredProcedureName = "[dbo].[EmailsActivate]";
        private const string GetStoredProcedureName = "[dbo].[EmailsGet]";
        private const string GetListStoredProcedureName = "[dbo].[EmailsGetAll]";
        private const string UpdateStoredProcedureName = "[dbo].[EmailsUpdate]";
        private const string GetByCodeStoredProcedureName = "[dbo].[EmailsGetByCode]";

        private const string EmailIdColumnName = "EmailId";
        private const string EmailCodeColumnName = "EmailCode";
        private const string EmailSubjectColumnName = "EmailSubject";
        private const string EmailBodyColumnName = "EmailBody";
        private const string EmailToColumnName = "EmailTo";
        private const string EmailFromColumnName = "EmailFrom";

        private const string EmailIdParameterName = "@PEmailId";
        private const string EmailCodeParameterName = "@PEmailCode";
        private const string EmailSubjectParameterName = "@PEmailSubject";
        private const string EmailBodyParameterName = "@PEmailBody";
        private const string EmailToParameterName = "@PEmailTo";
        private const string EmailFromParameterName = "@PEmailFrom";
        private const string PCurrentUserIdParameterName = "@PCurrentUserId";
        #endregion

        #region IEmailsInfrastructure Implementation

        public async Task<int> Add(Email email)
        {
            var EmailIdParam = base.GetParameterOut(EmailIdParameterName, SqlDbType.Int, email.EmailId);
            var parameters = new List<DbParameter>
            {
                EmailIdParam,
                base.GetParameter(EmailCodeParameterName,    email.EmailCode),
                base.GetParameter(EmailSubjectParameterName, email.EmailSubject),
                base.GetParameter(EmailBodyParameterName,    email.EmailBody),
                base.GetParameter(EmailToParameterName,      email.EmailTo),
                base.GetParameter(EmailFromParameterName,    email.EmailFrom),
                base.GetParameter(PCurrentUserIdParameterName, email.CreatedById)
            };

            await base.ExecuteNonQuery(parameters, AddStoredProcedureName, CommandType.StoredProcedure);
            email.EmailId = Convert.ToInt32(EmailIdParam.Value);
            return email.EmailId;
        }

        public async Task<bool> Activate(Email email)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(EmailIdParameterName, email.EmailId),
                base.GetParameter(BaseInfrastructure.ActiveParameterName, email.Active),
                base.GetParameter(PCurrentUserIdParameterName, email.ModifiedById)
            };

            var rows = await base.ExecuteNonQuery(parameters, ActivateStoredProcedureName, CommandType.StoredProcedure);
            return rows > 0;
        }

        public async Task<Email> Get(Email email)
        {
            Email item = null;
            var parameters = new List<DbParameter>
            {
                base.GetParameter(EmailIdParameterName, email.EmailId)
            };

            using (var dataReader = await base.ExecuteReader(parameters, GetStoredProcedureName, CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    if (dataReader.Read())
                    {
                        item = new Email
                        {
                            EmailId = dataReader.GetIntegerValue(EmailIdColumnName),
                            EmailCode = dataReader.GetStringValue(EmailCodeColumnName),
                            EmailSubject = dataReader.GetStringValue(EmailSubjectColumnName),
                            EmailBody = dataReader.GetStringValue(EmailBodyColumnName),
                            EmailTo = dataReader.GetStringValue(EmailToColumnName),
                            EmailFrom = dataReader.GetStringValue(EmailFromColumnName),

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

            return item;
        }

        public async Task<List<Email>> GetList(Email email)
        {
            var list = new List<Email>();
            var parameters = new List<DbParameter>(); // EmailsGetAll has no inputs

            using (var dataReader = await base.ExecuteReader(parameters, GetListStoredProcedureName, CommandType.StoredProcedure))
            {
                if (dataReader != null)
                {
                    while (dataReader.Read())
                    {
                        list.Add(new Email
                        {
                            EmailId = dataReader.GetIntegerValue(EmailIdColumnName),
                            EmailCode = dataReader.GetStringValue(EmailCodeColumnName),
                            EmailSubject = dataReader.GetStringValue(EmailSubjectColumnName),
                            EmailBody = dataReader.GetStringValue(EmailBodyColumnName),
                            EmailTo = dataReader.GetStringValue(EmailToColumnName),
                            EmailFrom = dataReader.GetStringValue(EmailFromColumnName),
                            CreatedById = dataReader.GetIntegerValueNullable(BaseInfrastructure.CreatedByIdColumnName),
                            CreatedDate = dataReader.GetDateTimeValueNullable(BaseInfrastructure.CreatedDateColumnName),
                            ModifiedById = dataReader.GetIntegerValueNullable(BaseInfrastructure.ModifiedByIdColumnName) ?? 0,
                            ModifiedDate = dataReader.GetDateTimeValueNullable(BaseInfrastructure.ModifiedDateColumnName),
                            Active = dataReader.GetBooleanValue(BaseInfrastructure.ActiveColumnName)
                        });
                    }

                    if (!dataReader.IsClosed)
                        dataReader.Close();
                }
            }

            return list;
        }

        public async Task<bool> Update(Email email)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(EmailIdParameterName,      email.EmailId),
                base.GetParameter(EmailCodeParameterName,    email.EmailCode),
                base.GetParameter(EmailSubjectParameterName, email.EmailSubject),
                base.GetParameter(EmailBodyParameterName,    email.EmailBody),
                base.GetParameter(EmailToParameterName,      email.EmailTo),
                base.GetParameter(EmailFromParameterName,    email.EmailFrom),
                base.GetParameter(PCurrentUserIdParameterName, email.ModifiedById)
            };

            var rows = await base.ExecuteNonQuery(parameters, UpdateStoredProcedureName, CommandType.StoredProcedure);
            return rows > 0;
        }
        public async Task<Email?> GetByCode(string code)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(EmailCodeParameterName, code)
            };

            using var dr = await base.ExecuteReader(parameters, GetByCodeStoredProcedureName, CommandType.StoredProcedure);
            Email? item = null;
            if (dr != null && dr.Read())
            {
                item = new Email
                {
                    EmailId = dr.GetIntegerValue("EmailId"),
                    EmailCode = dr.GetStringValue("EmailCode"),
                    EmailSubject = dr.GetStringValue("EmailSubject"),
                    EmailBody = dr.GetStringValue("EmailBody"),
                    EmailTo = dr.GetStringValue("EmailTo"),
                    EmailFrom = dr.GetStringValue("EmailFrom"),
                    Active = dr.GetBooleanValue(BaseInfrastructure.ActiveColumnName),
                    CreatedById = dr.GetIntegerValueNullable(BaseInfrastructure.CreatedByIdColumnName),
                    CreatedDate = dr.GetDateTimeValueNullable(BaseInfrastructure.CreatedDateColumnName),
                    ModifiedById = dr.GetIntegerValueNullable(BaseInfrastructure.ModifiedByIdColumnName) ?? 0,
                    ModifiedDate = dr.GetDateTimeValueNullable(BaseInfrastructure.ModifiedDateColumnName)
                };
            }
            if (dr != null && !dr.IsClosed) dr.Close();
            return item;
        }
        #endregion
    }
}
