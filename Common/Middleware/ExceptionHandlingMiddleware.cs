using AuctionPortal.Common.Core;
using AuctionPortal.Common.Infrastructure;
using AuctionPortal.Common.Middleware;
using Newtonsoft.Json;
using System.Net;
using System.Reflection.Metadata;

namespace AuctionPortal.Common.Middleware
{
    public class ExceptionHandlingMiddleware : BaseMiddleware
    {
        #region Constructor
        /// <summary>
        /// ErrorHandlingMiddleware initializes class object.
        /// </summary>
        /// <param name="next"></param>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public ExceptionHandlingMiddleware(RequestDelegate next, IConfiguration configuration) : base(next, configuration)
        {
        }
        #endregion

        #region Properties and Data Members

        #endregion

        #region Methods
        /// <summary>
        /// Invoke method is called when middleware has been called.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="headerValue"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context, IHeaderValue headerValue)
        {
            try
            {
                await this.Next(context);
            }
            catch (Exception ex)
            {
                await this.HandleExceptionAsync(context, ex, headerValue);
            }
        }

        /// <summary>
        /// HandleExceptionAsync creates response in case of exception.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        /// <param name="headerValue"></param>
        /// <returns></returns>
        private Task HandleExceptionAsync(HttpContext context, Exception exception, IHeaderValue headerValue)
        {
            var code = HttpStatusCode.InternalServerError; // 500 if unexpected

            if (exception is EntryPointNotFoundException)
            {
                code = HttpStatusCode.NotFound;
            }
            else if (exception is AccessViolationException)
            {
                code = HttpStatusCode.Forbidden;
            }
            else if (exception is UnauthorizedAccessException)
            {
                code = HttpStatusCode.Unauthorized;
            }
            else if (exception is DatabaseException)
            {
                code = HttpStatusCode.BadRequest;
            }

            var result = JsonConvert.SerializeObject(new { error = exception.Message });

            context.Response.ContentType = Constants.JsonContentType;
            context.Response.StatusCode = (int)code;


            return context.Response.WriteAsync(result);
        }
        #endregion
    }
}
