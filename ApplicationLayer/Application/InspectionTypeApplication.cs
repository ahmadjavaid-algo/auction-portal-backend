// InspectionTypeApplication.cs
using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.InfrastructureLayer.Interfaces;
using AuctionPortal.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuctionPortal.ApplicationLayer.Application
{
    public class InspectionTypeApplication : BaseApplication, IInspectionTypeApplication
    {
        public InspectionTypeApplication(IInspectionTypeInfrastructure InspectionTypeInfrastructure, IConfiguration configuration)
            : base(configuration)
        {
            this.InspectionTypeInfrastructure = InspectionTypeInfrastructure
                ?? throw new ArgumentNullException(nameof(InspectionTypeInfrastructure));
        }

        public IInspectionTypeInfrastructure InspectionTypeInfrastructure { get; }

        #region Queries
        public async Task<InspectionType> Get(InspectionType entity)
        {
            return await InspectionTypeInfrastructure.Get(entity);
        }

       

        public async Task<List<InspectionType>> GetList(InspectionType entity)
        {
            return await InspectionTypeInfrastructure.GetList(entity);
        }
        #endregion

        #region Commands
        public async Task<int> Add(InspectionType entity)
        {
            return await InspectionTypeInfrastructure.Add(entity);
        }

        public async Task<bool> Update(InspectionType entity)
        {
            return await InspectionTypeInfrastructure.Update(entity);
        }

        public async Task<bool> Activate(InspectionType entity)
        {
            return await InspectionTypeInfrastructure.Activate(entity);
        }
        #endregion
    }
}
