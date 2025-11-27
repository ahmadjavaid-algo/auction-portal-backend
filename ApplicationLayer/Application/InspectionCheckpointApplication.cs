// InspectionCheckpointApplication.cs
using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.InfrastructureLayer.Interfaces;
using AuctionPortal.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuctionPortal.ApplicationLayer.Application
{
    public class InspectionCheckpointApplication : BaseApplication, IInspectionCheckpointApplication
    {
        public InspectionCheckpointApplication(IInspectionCheckpointInfrastructure InspectionCheckpointInfrastructure, IConfiguration configuration)
            : base(configuration)
        {
            this.InspectionCheckpointInfrastructure = InspectionCheckpointInfrastructure
                ?? throw new ArgumentNullException(nameof(InspectionCheckpointInfrastructure));
        }

        public IInspectionCheckpointInfrastructure InspectionCheckpointInfrastructure { get; }

        #region Queries
        public async Task<InspectionCheckpoint> Get(InspectionCheckpoint entity)
        {
            return await InspectionCheckpointInfrastructure.Get(entity);
        }

       

        public async Task<List<InspectionCheckpoint>> GetList(InspectionCheckpoint entity)
        {
            return await InspectionCheckpointInfrastructure.GetList(entity);
        }
        #endregion

        #region Commands
        public async Task<int> Add(InspectionCheckpoint entity)
        {
            return await InspectionCheckpointInfrastructure.Add(entity);
        }

        public async Task<bool> Update(InspectionCheckpoint entity)
        {
            return await InspectionCheckpointInfrastructure.Update(entity);
        }

        public async Task<bool> Activate(InspectionCheckpoint entity)
        {
            return await InspectionCheckpointInfrastructure.Activate(entity);
        }
        #endregion
    }
}
