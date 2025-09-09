using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.InfrastructureLayer.Interfaces; 
using AuctionPortal.Models;
using Microsoft.Extensions.Configuration;

namespace AuctionPortal.ApplicationLayer.Application
{
    public class EmailsApplication : BaseApplication, IEmailsApplication
    {
        public EmailsApplication(IEmailsInfrastructure emailsInfrastructure, IConfiguration configuration)
            : base(configuration)
        {
            EmailsInfrastructure = emailsInfrastructure ?? throw new ArgumentNullException(nameof(emailsInfrastructure));
        }

        public IEmailsInfrastructure EmailsInfrastructure { get; }

        #region Queries
        public async Task<Email> Get(Email entity)
        {
            return await EmailsInfrastructure.Get(entity);
        }

        public async Task<List<Email>> GetList(Email entity)
        {
            return await EmailsInfrastructure.GetList(entity);
        }
        #endregion

        #region Commands
        public async Task<int> Add(Email entity)
        {
            return await EmailsInfrastructure.Add(entity);
        }

        public async Task<bool> Update(Email entity)
        {
            return await EmailsInfrastructure.Update(entity);
        }

        public async Task<bool> Activate(Email entity)
        {
            return await EmailsInfrastructure.Activate(entity);
        }
        #endregion
    }
}
