namespace AuctionPortal.Common.Middleware
{
    public abstract class BaseMiddleware
    {
        #region Constructor
        /// <summary>
        /// BaseMiddleware initializes class object.
        /// </summary>
        /// <param name="next"></param>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        protected BaseMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            Next = next;
            Configuration = configuration;
        }
        #endregion

        #region Properties and Data Members
        protected RequestDelegate Next { get; }
        public IConfiguration Configuration { get; }
        #endregion

        #region Methods

        #endregion
    }
}
