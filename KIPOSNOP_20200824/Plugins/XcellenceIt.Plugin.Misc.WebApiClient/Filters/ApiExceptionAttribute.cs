
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace XcellenceIt.Plugin.Misc.WebApiClient.Filters
{
    public class ApiExceptionAttribute : TypeFilterAttribute
    {
        #region Ctor

        /// <summary>
        /// Create instance of the filter attribute
        /// </summary>
        public ApiExceptionAttribute() : base(typeof(ApiExceptionFilter))
        {
        }

        #endregion

        public class ApiExceptionFilter : ExceptionFilterAttribute
        {
            public override void OnException(ExceptionContext filterContext)
            {
                filterContext.ExceptionHandled = true;

                var Data = new { Error = filterContext.Exception.Message };
                var jsonResult = new JsonResult(Data);
                filterContext.Result = jsonResult;
            }
        }
    }
}
