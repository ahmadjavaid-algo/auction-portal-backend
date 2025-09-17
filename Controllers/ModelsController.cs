using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.Common.Controllers;
using AuctionPortal.Common.Core;
using AuctionPortal.Common.Models;
using AuctionPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionPortal.Controllers
{
    public class ModelsController : APIBaseController
    {
        #region Constructor
        /// <summary>
        /// ModelsController initializes class object.
        /// </summary>
        public ModelsController(IModelApplication ModelApplication, IHeaderValue headerValue, IConfiguration configuration)
            : base(headerValue, configuration)
        {
            this.ModelApplication = ModelApplication;
        }
        #endregion

        #region Properties and Data Members
        public IModelApplication ModelApplication { get; }
        #endregion

        [HttpPost("add")]
        public async Task<int> Add([FromBody] Model Model)
        {
            var ModelId = await this.ModelApplication.Add(Model);
            return ModelId;
        }

        [HttpPut("update")]
        public async Task<bool> Update([FromBody] Model Model)
        {
            var response = await ModelApplication.Update(Model);
            return response;
        }

        [HttpPut("activate")]
        public async Task<bool> Activate([FromBody] Model Model)
        {
            var response = await ModelApplication.Activate(Model);
            return response;
        }

        [HttpGet("get")]
        public async Task<Model> Get([FromQuery] Model request)
        {
            Model response = await this.ModelApplication.Get(request);
            return response;
        }

        [HttpGet("getlist")]
        public async Task<List<Model>> GetList([FromQuery] Model request)
        {
            List<Model> response = await this.ModelApplication.GetList(request);
            return response;
        }
    }
}
